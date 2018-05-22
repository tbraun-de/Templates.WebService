# AXOOM Web Service Template

This template helps you create web services for the [AXOOM](http://www.axoom.com/) Platform. In addition to its role as a template this repository also serves as a reference implementation and living documentation for infrastructure concerns such as configuration, logging and monitoring.

The template creates a C# [ASP.NET Core](https://docs.microsoft.com/aspnet/core/) project packaged to run in a [Docker](https://www.docker.com/) container. The AXOOM Platform uses common web technologies for communication, so services created with this template can easily be used in other environments as well. You can also use any other containerized language to create services for the AXOOM Platform. In this case you can use the following documentation to learn about our infrastructure requirements and best practices.

This template focuses on creating web services that provide RESTful APIs. If this is not your goal one of our other templates may be a better match:
- [AXOOM Library Template](https://github.com/AXOOM/Axoom.Templates.Library)
- [AXOOM Service Template](https://github.com/AXOOM/Axoom.Templates.Service)
- [AXOOM Portal App Template](https://github.com/AXOOM/Axoom.Templates.PortalApp)


# Using the template

Download and install the [.NET Core SDK](https://www.microsoft.com/net/download) on your machine. You can then install the template by running the following:

    dotnet new --install Axoom.Templates.WebService::*

To use the template to create a new project:

    mkdir MyVendor.MyService
    cd MyVendor.MyService
    dotnet new axoom-webservice --serviceName myvendor-myservice --friendlyName "My Service" --vendorDomain example.com --description "my description"

In the commands above replace
- `MyVendor.MyService` with the .NET namespace you wish to use,
- `myvendor-myservice` with the name of your company and service using only lowercase letters and hyphens,
- `My Service` with the full name of your service,
- `example.com` with the public domain of your company and
- `my description` with a brief (single sentence) description of the service.

You can now open the generated project using your favorite IDE. We recommend [Visual Studio](https://www.visualstudio.com/downloads/), [Visual Studio Code](https://code.visualstudio.com/Download) or [Rider](https://www.jetbrains.com/rider/).


# Walkthrough

The following is a detailed walkthrough of the project structure generated by the template. To follow along you can either use the output of `dotnet new` or this repository.

The [`content/`](content/) directory in this repository contains the payload that is instantiated by the template. The file [`content/.template.config/template.json`](content/.template.config/template.json) instructs the templating engine which placeholders to replace. The following descriptions all refer to files and directories below this directory.

## Formatting and encoding

The [`.gitignore`](content/.gitignore) file prevents build artifacts, per-user settings and IDE-specific temp files from being checked into version control.

The [`.gitattributes`](content/.gitattributes) file disable's Git's automatic End of Line (EOL) conversion between Windows and Linux. This ensures that files are binary-identical on all platforms.

We use [EditorConfig](http://editorconfig.org/) to define and maintain consistent coding styles between different editors and IDEs. Our [`.editorconfig`](content/.editorconfig) file also ensures a consistent EOL style and charset encoding based on file type.

## Versioning

To make a CI/CD pipeline successful it's extremely relevant, to get the most information out of your versioning.
Using [SemVer](https://semver.org/), you create the level of predictability of upgrade impact you need to enable continous delivery.
Furthermore it enables looser coupling between diverse assets if you can make sure, a certain update will not break the API consumed by other services.

So in very little words, a semantic version consists of three parts which have to be bumped on certain changes:
- _Major_: Breaking changes
- _Minor_: Features added
- _Patch_: Bugs have been fixed

__Special versions:__
- `1.0.0`: Your asset is mature enough to be production ready
- `0.1.0`: The initial version of each asset

There is a exception in how to handle versions before first production readyness, where we decided to use __Minor for breaking changes__ and __Patch for feature additions and bug fixes as well__, since in the ongoing development phase you are not bug aware anyway.

We decided to use [GitVersion](http://gitversion.readthedocs.io/) to extract version information from our git commits. 
Hence, we make use of git tags to create a release whereas all other commits are pre-releases by default. The source code itself contains no version numbers.
The [`GitVersion.yml`](content/GitVersion.yml) file configures GitVersion to extract a version number like the following from an untagged commit:
```
0.1.1-pre0045-20180404094115
```
| Version Part     | Description                                                                                               |
| ---------------- | --------------------------------------------------------------------------------------------------------- |
| `0.1.1`          | The latest tag with a bumped patch version                                                                |
| `pre0045`        | Indicates, that it's a prerelease (of `0.1.1`) and 45 commits have been made since the last tag (`0.1.0`) |
| `20180404094115` | The Timestamp _04/04/2018 09:41:15_                                                                       |

Whereas `0.1.1` is extracted from a commit with tag `0.1.1`.

## Project structure

[`src/`](content/src/) contains the C# project structure.

- [`Service`](content/src/Service/): The web service project.
- [`Client`](content/src/Client/): A client library for consuming the service's REST API.
- [`Dto`](content/src/Dto/): DTOs shared between the service and the client.
- [`UnitTests`](content/src/UnitTests/): Unit tests for the service and the client.

__Note:__ All projects share the same namespace.

## Building

TODO: Build scripts

`artifacts/` is created during build and contains artifacts for publishing (e.g. NuGet packages).

[`release/`](content/release/)

[`deploy/`](content/deploy/)

## Configuration

After a `git clone` an asset should be pre-configured and ready to use with defaults for local development and debugging including any dependencies.

When packaged as a release however, assets should ship with defaults ready for production. Only business parameters should be set at deployment-time. Avoid exposing connection strings and other technical details here if possible.

Assets contain some configuration bundled at compile-time. This can be overriden at run-time using environment variables.

The [`appsettings.yml`](content/src/Service/appsettings.yml) file is bundled into the Docker Image during build and sets development defaults.
We decided to use YAML files for configuration purposes.
JSON is for sure a great format for serialization and is as well human-readable, but YAML is furthermore human-writable and supports comments, what you actually want from a configuration format.

The [`docker-compose.override.yml`](content/src/docker-compose.override.yml) file is used to set dummy values and wire up dependencies for local development.
proper volumes optional

The [`asset.yml`](content/release/asset.yml) file is used to generate releases, sets production defaults and maps external environment variables to service-specific environment variables.
proper volumes mandatory
Only business parameters should be set at deployment-time (avoid exposing connection strings, etc.)

## Layers vs Slices

We for ourselves found, it's a good practice to architect services into slices instead of only thinking in layers.
In times of _micro services_ services tend to become more and more stateless and isolated. 
This increases availability, operability and simplifies technical ownership.
In the process of architecting and implementing, we learn a lot about the problem domain we are working in. In particular, we get to know the diverse actors in domain processes, the necessary/desired cardinalities and such.
So, we mostly don't start building a micro service but a monolith---simply because we don't know what we actually need.
Thinking in slices makes it extremely easy to reign our monolith's (internal) dependencies in an isolated way, so that we can extract projects or even a whole new service, implementing a single slice of our domain.

__So what is a slice?__ Actually it's a vertical cut through your application representing a single part of the domain or an use case.
Having a vertical cut does not mean you have to drop your thoughts about layers, if your dependencies tend to get more and more complex, layers will still improve the design of your slice. The actual difference is then that you don't think of __the__ persistence layer anymore but about a _customers persistence layer_ and a _products persistence layer_.

__Further reading:__
- https://www.thoughtworks.com/insights/blog/slicing-your-development-work-multi-layer-cake
- http://deviq.com/vertical-slices/

The template also sticks to slices and with that, you will find the slices going across the projects.
Therefore, each project comes with sub directories named by a part of the domain (`Contacts` in the included example) encapsulating the domain's concerns.

To make all these concerns pluggable---which is possible due to the encapsualtion---you will also find `Startup.cs` files, containing extension methods for `IServiceCollection` to simply plug the slice into the central dependency injection.

Also, we made use of the `partial class` concept to isolate domain concerns working on the `DbContext` ([Base](content/src/Service/DbContext.cs) and [Contacts Slice](content/src/Service/Contacts/DbContext.cs)).
This means, every slice enriches the main `DbContext` with its domain specific concerns.
So we have one single class maintaining connection and caching concerns whereas the actual domain logic is split over the diverse slices.
Same holds true for the `Client` ([Base](content/src/Client/Client.cs) vs [Contacts Slice](content/src/Client/Contacts/Client.cs)).

## Infrastructure pseudo-slice

There is one slice that actually seems not to handle actual business concerns: The `Infrastructure` slice.
In times of micro services reigning the software world, infrastructure becomes more and more important, this is why we handle this also as a seperate part of our domain.

What the slice actually does is handling mostly boilerplate for setting up our non-functional concerns like WebAPI, logging, monitoring, etc..
If there are no special requirements, there is no need to touch the classes inside.
One class that might be of special interest is the `ApiExceptionFilterAttribute.cs` where you can map Exceptions thrown while processing a request to HTTP status codes.

Staying in our slice pattern, the infrastructure slices also comes with its own [`Startup.cs`](content/src/Service/Infrastructure/Startup.cs).
The high-level config is meant to be happening in the top-level [`Startup.cs`](content/src/Service/Startup.cs).

### Logging

Print to console instead of file and let Docker handle the collection

Print as JSON in production (handle multi-line messages, filter by log-level, etc.)

Trace
- Expected to cause severe performance degradation
- May log every request
- May duplicate entire request payload

Debug (default for Dev stage)
- Expected to cause slight performance degradation
- Do *NOT* log every request
- Do *NOT* duplicate entire request payload

Information (default for Test, Staging and Live stages)
- May be somewhat verbose during startup (e.g. dumping entire config)
- Do *NOT* log for frequently expected events

Warning
- Indicates degraded customer experience
- Indicates issues that may be fixable via config change
- Should be collected for later triage, but system continues to function

Error
- Indicates blocking issues for customers
- Indicates issues that may require code changes to be fixed
- Should be surfaced in monitoring and addressed soon

Fatal
- System should be considered offline/crashed
- Requires immediate attention

Do not catch, log and rethrow unless you REALLY have context to add

### Metrics

Exposed on separate port to allow easy firewalling

Avoid parameterizing by customer data, which effectively creates a memory leak (reason why we don't use Nexogen's default HTTP metrics)

## "Contacts" sample slice

Guideline for getting started, easy to remove

Demonstrates "collection", "element" and "action" REST/TypedRest patterns

Demonstrates DTO/Entity asymetry (Contact + Note vs Contact + Poke)

No custom Repository or UnitOfWork classes, because Entity Framework's Context already implements those patterns

Uses SQLite for simplicity. Consider PostgreSQL as an alternative, but only if you actually need scalability.

Reusable CollectionController and ICrudService provided outside of slice

### Client library

TypedRest: builds clients at a higher-level (like collections instead of idividual HTTP routes) and implicitly guides to cleaner/more consistent API design

### Unit testing

Test controllers via client (avoid fragility, avoid testing of Framework itself) using 

Inject in-memory DB contexts into services (avoid building complex mocks yourself)
