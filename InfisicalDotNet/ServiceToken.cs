using System.Text.Json;
using System.Text.Json.Serialization;

namespace InfisicalDotNet;

public class ServiceToken
{
    [JsonPropertyName("_id")]
    public string Id { get; set; }
    public string Name { get; set; }
    public string Workspace { get; set; }
    public List<ServiceTokenScope> Scopes { get; set; }
    public ServiceTokenUser User { get; set; }
    public string LastUsed { get; set; }
    public string ExpiresAt { get; set; }
    public string EncryptedKey { get; set; }
    public string Iv { get; set; }
    public string Tag { get; set; }
    public List<string> Permissions { get; set; }
    public string CreatedAt { get; set; }
    public string UpdatedAt { get; set; }
    [JsonPropertyName("__v")]
    public int V { get; set; }

    public static ServiceToken Deserialize(string content)
    {
        var result = JsonSerializer.Deserialize<ServiceToken>(content, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
        
        return result;
    }
}

public class ServiceTokenScope
{
    public string environment { get; set; }
    public string secretPath { get; set; }
    public string _id { get; set; }
}

public class ServiceTokenUser
{
    public string _id { get; set; }
    public List<string> authMethods { get; set; }
    public string email { get; set; }
    public bool isMfaEnabled { get; set; }
    public List<object> mfaMethods { get; set; }
    public string createdAt { get; set; }
    public string updatedAt { get; set; }
    public int __v { get; set; }
    public string firstName { get; set; }
    public string lastName { get; set; }
}