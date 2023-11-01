using Microsoft.Extensions.Configuration;

namespace InfisicalDotNet;

public class InfisicalConfigurationSource : IConfigurationSource
{
    private readonly string _apiUrl;
    private readonly string? _infisicalToken;

    public InfisicalConfigurationSource(string apiUrl, string? infisicalToken)
    {
        _apiUrl = apiUrl;
        _infisicalToken = infisicalToken;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new InfisicalConfigurationProvider(_apiUrl, _infisicalToken);
    }
}