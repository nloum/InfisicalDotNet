# InfisicalDotNet

This is a .NET library that makes it easy to use the [.NET configuration system](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0) with [Infisical](https://infisical.com/).

![Nuget](https://img.shields.io/nuget/dt/InfisicalDotNet) | ![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/nloum/InfisicalDotNet/continuous)

## How do I use this?

Install the package:

```shell
dotnet add package InfisicalDotNet
```

Set up the configuration provider:

```csharp
using InfisicalDotNet;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddInfisical(Environment.GetEnvironmentVariable("INFISICAL_TOKEN"));

// Add services to the container.
```

## Notes

Currently this library only supports authentication using service tokens. Infisical service tokens are only allowed to access a specific workspace and environment. That is how this library knows which workspace and environment to use.
