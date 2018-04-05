﻿using System;
using System.Diagnostics;
using System.Threading;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Axoom.MyService.Database
{
    [UsedImplicitly]
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<MyServiceDbContext>
    {
        private const string ContainerName = "myservice_postgres_temp";
        private const int ContainerPort = 15432;

        public MyServiceDbContext CreateDbContext(string[] args)
        {
            RemoveTempPostgres(); // Remove leftovers from previous runs
            StartTempPostgres();
            var context = new MyServiceDbContext(new DbContextOptionsBuilder()
                .UseNpgsql($"Host=localhost;Port={ContainerPort};User ID=postgres;Password=postgres;Database=postgres")
                .Options);
            context.Disposed += RemoveTempPostgres;
            return context;
        }

        private static void StartTempPostgres()
        {
            if (Run("docker", $"run -d --name {ContainerName} -p {ContainerPort}:5432 postgres:9.6") != 0)
                throw new InvalidOperationException("Failed to start Postgres container.");
            Thread.Sleep(5000); // Wait for DB to finish startup
        }

        private static void RemoveTempPostgres() => Run("docker", $"rm -f {ContainerName}");

        private static int Run(string  fileName, string arguments)
        {
            var process = Process.Start(fileName, arguments);
            process?.WaitForExit();
            return process?.ExitCode ?? 0;
        }
    }
}