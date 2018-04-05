﻿using System;
using Axoom.MyService.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Axoom.MyService.Services
{
    public class DatabaseFactsBase : IDisposable
    {
        protected readonly MyServiceDbContext Context;

        protected DatabaseFactsBase() => Context = new MyServiceDbContext(new DbContextOptionsBuilder()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use GUID so every test has its own DB
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .EnableSensitiveDataLogging()
            .Options);

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }
}