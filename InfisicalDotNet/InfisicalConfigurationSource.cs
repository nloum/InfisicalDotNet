using Microsoft.Extensions.Configuration;

namespace InfisicalDotNet;

public class InfisicalConfigurationSource : IConfigurationSource
{
    private readonly string _apiUrl;
    private readonly string? _infisicalServiceToken;
    private readonly bool _includeImports;
    private readonly string _prefix;

    public InfisicalConfigurationSource(string apiUrl, string? infisicalServiceToken = null, bool includeImports = true, string prefix = "")
    {
        _apiUrl = apiUrl;
        _infisicalServiceToken = infisicalServiceToken;
        _includeImports = includeImports;
        _prefix = prefix;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new InfisicalConfigurationProvider(_apiUrl, _infisicalServiceToken, _includeImports, _prefix);
    }
}