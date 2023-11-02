using Microsoft.Extensions.Configuration;

namespace InfisicalDotNet;

public static class InfisicalConfigurationExtensions
{
    public static IConfigurationBuilder AddInfisical(
        this IConfigurationBuilder builder, 
        string? infisicalServiceToken = null,
        string apiUrl = "https://app.infisical.com",
        string secretPath = "/",
        bool includeImports = true,
        string prefix="")
    {
        return builder.Add(new InfisicalConfigurationSource(apiUrl, infisicalServiceToken, secretPath, includeImports, prefix));
    }
}
