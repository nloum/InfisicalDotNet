# InfisicalDotNet

This is a .NET library that makes it easy to use the [.NET configuration system](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0) with [Infisical](https://infisical.com/).

[![Nuget](https://img.shields.io/nuget/dt/InfisicalDotNet)](https://www.nuget.org/packages/InfisicalDotNet)

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
    .AddInfisical(Environment.GetEnvironmentVariable("INFISICAL_SERVICE_TOKEN"));

// Add services to the container.
```

Or you can avoid specifying the service token, in which case it uses the `INFISICAL_SERVICE_TOKEN` environment variable behind the scenes:

```csharp
using InfisicalDotNet;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddInfisical();

// Add services to the container.
```

## How do I format secret keys?

Secret keys should be formatted like environment variables. For example, consider this `appsettings.json` file:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=db.sqlite3"
  }
}
```

The equivalent of this JSON would be a secret in Infisical with the key `CONNNECTIONSTRINGS__DEFAULTCONNECTION`. Note the double underscore. More information on this is available in [ASP.NET Core's environment variable naming documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-7.0#naming-of-environment-variables).

## Why can't I specify the secret path, workspace or environment?

When you create a service token it is scoped to a specific workspace, environment and secret path, so this configuration provider uses those.
