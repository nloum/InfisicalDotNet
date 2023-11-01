using System.Text.Json;

namespace InfisicalDotNet;

public class SecretsList
{
    public List<Secret> Secrets { get; set; }

    public static SecretsList Deserialize(string content)
    {
        var result = JsonSerializer.Deserialize<SecretsList>(content, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
        return result;
    }
}

public class Secret
{
    public string SecretKeyCiphertext { get; set; }
    public string SecretKeyIV { get; set; }
    public string SecretKeyTag { get; set; }
    public string SecretValueCiphertext { get; set; }
    public string SecretValueIV { get; set; }
    public string SecretValueTag { get; set; }
}
