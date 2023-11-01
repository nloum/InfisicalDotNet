using Microsoft.Extensions.Configuration;

namespace InfisicalDotNet;

public static class InfisicalConfigurationExtensions
{
    public static IConfigurationBuilder AddInfisical(
        this IConfigurationBuilder builder, 
        string? infisicalToken,
        string apiUrl = "https://app.infisical.com")
    {
        return builder.Add(new InfisicalConfigurationSource(apiUrl, infisicalToken));
    }
}