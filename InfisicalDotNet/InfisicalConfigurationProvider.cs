using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace InfisicalDotNet;

public class InfisicalConfigurationProvider : ConfigurationProvider
{
    private readonly string _apiUrl;
    private readonly HttpClient _httpClient;
    private Dictionary<string, string> _secretsCache = new();
    private readonly string? _infisicalServiceToken;
    private readonly bool _includeImports;
    private readonly string _prefix;
    private static readonly Regex _tokenRegex = new(@"(st\.[a-f0-9]+\.[a-f0-9]+)\.([a-f0-9]+)");

    public InfisicalConfigurationProvider(string apiUrl, string? infisicalServiceToken = null, bool includeImports = true, string prefix = "")
    {
        _apiUrl = apiUrl;
        _infisicalServiceToken = infisicalServiceToken ?? Environment.GetEnvironmentVariable("INFISICAL_SERVICE_TOKEN");
        _includeImports = includeImports;
        _prefix = prefix;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _infisicalServiceToken);
        _httpClient.BaseAddress = new Uri(_apiUrl);
    }

    public override void Load()
    {
        LoadAsync().GetAwaiter().GetResult();
    }

    private async Task LoadAsync()
    {
        try
        {
            var tokenDataUrl = $"{_apiUrl}/api/v2/service-token";
            var tokenDataResponse = await _httpClient.GetAsync(tokenDataUrl);
            tokenDataResponse.EnsureSuccessStatusCode();
            var tokenDataContent = await tokenDataResponse.Content.ReadAsStringAsync();
            var serviceTokenObj = ServiceToken.Deserialize(tokenDataContent);
            if (serviceTokenObj is null)
            {
                throw new InvalidOperationException("Failed to access service token details");
            }
            var environment = serviceTokenObj.Scopes[0].environment;
            var secretPath = serviceTokenObj.Scopes[0].secretPath;
            var workspace = serviceTokenObj.Workspace;

            var tokenMatch = _tokenRegex.Match(_infisicalServiceToken);
            var serviceTokenKey = tokenMatch.Groups[2].Value;
            
            var url = $"{_apiUrl}/api/v3/secrets/?environment={environment}&workspaceId={workspace}&secretPath={secretPath}&include_imports={_includeImports.ToString().ToLower()}";
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            var secrets = SecretsList.Deserialize(content);

            var workspaceKey = DecryptSymmetric128BitHexKeyUtf8(serviceTokenKey, serviceTokenObj.EncryptedKey,
                serviceTokenObj.Tag, serviceTokenObj.Iv);
            
            _secretsCache = secrets.Secrets.ToDictionary(
                secret => DecryptSymmetric128BitHexKeyUtf8(
                    key: workspaceKey,
                    ciphertext: secret.SecretKeyCiphertext,
                    tag: secret.SecretKeyTag,
                    iv: secret.SecretKeyIV), 
                secret => DecryptSymmetric128BitHexKeyUtf8(
                    key: workspaceKey,
                    ciphertext: secret.SecretValueCiphertext,
                    tag: secret.SecretValueTag,
                    iv: secret.SecretValueIV)
                );

            foreach (var secret in _secretsCache)
            {
                var key = _prefix + secret.Key.Replace("__", ":");
                Data.Add(key, secret.Value);
            }
        }
        catch
        {
            foreach (var secret in _secretsCache)
            {
                Data.Add(secret.Key, secret.Value);
            }
        }
    }
    
    public string DecryptSymmetric128BitHexKeyUtf8(string key, string ciphertext, string tag, string iv)
    {
        try
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] ivBytes = Convert.FromBase64String(iv);
            byte[] tagBytes = Convert.FromBase64String(tag);
            byte[] cipherTextBytes = Convert.FromBase64String(ciphertext);

            byte[] combinedCipherText = new byte[cipherTextBytes.Length + tagBytes.Length];
            Buffer.BlockCopy(cipherTextBytes, 0, combinedCipherText, 0, cipherTextBytes.Length);
            Buffer.BlockCopy(tagBytes, 0, combinedCipherText, cipherTextBytes.Length, tagBytes.Length);

            KeyParameter keyParam = ParameterUtilities.CreateKeyParameter("AES", keyBytes);
            AeadParameters parameters = new AeadParameters(keyParam, 128, ivBytes);

            GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
            cipher.Init(false, parameters);

            byte[] plainBytes = new byte[cipher.GetOutputSize(combinedCipherText.Length)];
            int len = cipher.ProcessBytes(combinedCipherText, 0, combinedCipherText.Length, plainBytes, 0);
            cipher.DoFinal(plainBytes, len);

            string plaintext = Encoding.UTF8.GetString(plainBytes);
            return plaintext;
        }
        catch
        {
            throw new ArgumentException("Decryption failed");
        }
    }
}
