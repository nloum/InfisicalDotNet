using Microsoft.Extensions.Configuration;

namespace InfisicalDotNet;

public class InfisicalConfigurationSource : IConfigurationSource
{
    private readonly string _apiUrl;
    private readonly string? _infisicalServiceToken;
    private readonly string _secretPath;
    private readonly bool _includeImports;
    private readonly string _prefix;

    public InfisicalConfigurationSource(string apiUrl, string? infisicalServiceToken = null, string secretPath = "/", bool includeImports = true, string prefix = "")
    {
        _apiUrl = apiUrl;
        _infisicalServiceToken = infisicalServiceToken;
        _secretPath = secretPath;
        _includeImports = includeImports;
        _prefix = prefix;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new InfisicalConfigurationProvider(_apiUrl, _infisicalServiceToken, _secretPath, _includeImports, _prefix);
    }
}