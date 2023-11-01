using FluentAssertions;

namespace InfisicalDotNet.Tests;

[TestClass]
public class ParseTests
{
    [TestMethod]
    public void ShouldParseServiceTokenCorrectly()
    {
      var content = $$"""
                        {
                          "_id": "83f893jofjiji3o",
                          "name": "Development",
                          "workspace": "f8j983j9soi3foj3flk",
                          "scopes": [
                            {
                              "environment": "dev",
                              "secretPath": "/",
                              "_id": "3fj398fj98foijs"
                            }
                          ],
                          "user": {
                            "_id": "jf93wj9r8h38rho3sls",
                            "authMethods": [
                              "email"
                            ],
                            "email": "test@example.com",
                            "isMfaEnabled": false,
                            "mfaMethods": [],
                            "createdAt": "2023-10-22T22:24:46.179Z",
                            "updatedAt": "2023-10-22T22:54:59.831Z",
                            "__v": 2,
                            "firstName": "Jane",
                            "lastName": "Doe"
                          },
                          "lastUsed": "2023-10-27T12:56:16.784Z",
                          "expiresAt": "2024-10-19T12:13:13.183Z",
                          "encryptedKey": "4uwflkw4jfklawjkflajwrklj4w\u003d",
                          "iv": "4fh9a84hfoawflawjk\u003d\u003d",
                          "tag": "awfh9a4hf98ahfoiawlfjalwe\u003d\u003d",
                          "permissions": [
                            "read"
                          ],
                          "createdAt": "2023-10-25T12:13:13.185Z",
                          "updatedAt": "2023-10-27T12:56:16.784Z",
                          "__v": 0
                        }
                        """;

        var serviceToken = ServiceToken.Deserialize(content);
        serviceToken.Should().NotBeNull();

        serviceToken.Id.Should().Be("83f893jofjiji3o");
        serviceToken.Name.Should().Be("Development");
        serviceToken.Workspace.Should().Be("f8j983j9soi3foj3flk");
        serviceToken.Scopes.Should().HaveCount(1);
        serviceToken.Scopes[0].environment.Should().Be("dev");
        serviceToken.Scopes[0].secretPath.Should().Be("/");
        serviceToken.Scopes[0]._id.Should().Be("3fj398fj98foijs");
        serviceToken.User.Should().NotBeNull();
        serviceToken.User._id.Should().Be("jf93wj9r8h38rho3sls");
        serviceToken.User.authMethods.Should().Contain("email");
        serviceToken.User.email.Should().Be("test@example.com");
        serviceToken.User.isMfaEnabled.Should().BeFalse();
        serviceToken.User.mfaMethods.Should().BeEmpty();
        serviceToken.User.createdAt.Should().Be("2023-10-22T22:24:46.179Z");
        serviceToken.User.updatedAt.Should().Be("2023-10-22T22:54:59.831Z");
        serviceToken.User.__v.Should().Be(2);
        serviceToken.User.firstName.Should().Be("Jane");
        serviceToken.User.lastName.Should().Be("Doe");
        serviceToken.LastUsed.Should().Be("2023-10-27T12:56:16.784Z");
        serviceToken.ExpiresAt.Should().Be("2024-10-19T12:13:13.183Z");
        serviceToken.EncryptedKey.Should().Be("4uwflkw4jfklawjkflajwrklj4w\u003d");
        serviceToken.Iv.Should().Be("4fh9a84hfoawflawjk\u003d\u003d");
        serviceToken.Tag.Should().Be("awfh9a4hf98ahfoiawlfjalwe\u003d\u003d");
        serviceToken.Permissions.Should().Contain("read");
        serviceToken.CreatedAt.Should().Be("2023-10-25T12:13:13.185Z");
        serviceToken.UpdatedAt.Should().Be("2023-10-27T12:56:16.784Z");
        serviceToken.V.Should().Be(0);
    }

    [TestMethod]
    public void ShouldParseSecretsCorrectly()
    {
      var content = $$"""
                      {
                        "secrets": [
                          {
                            "_id": "83d9q3j8f9jf89jf93jfj30",
                            "version": 1,
                            "workspace": "w3fj3w0jf903jf093j9f0",
                            "type": "shared",
                            "tags": [],
                            "environment": "dev",
                            "secretKeyCiphertext": "(*ja0djO#PKpfkfpkLK:KKFf\u003d",
                            "secretKeyIV": "(*ja0djO#PKpfkfpkLK:KKFf\u003d",
                            "secretKeyTag": "(*ja0djO#PKpfkfpkLK:KKFf\u003d",
                            "secretValueCiphertext": "(*ja0djO#PKpfkfpkLK:KKFf\u003d(*ja0djO#PKpfkfpkLK:KKFf\u003d(*ja0djO#PKpfkfpkLK:KKFf\u003d",
                            "secretValueIV": "(*ja0djO#PKpfkfpkLK:KKFf\u003d",
                            "secretValueTag": "(*ja0djO#PKpfkfpkLK:KKFf\u003d",
                            "secretCommentCiphertext": "",
                            "secretCommentIV": "(*ja0djO#PKpfkfpkLK:KKFf\u003d",
                            "secretCommentTag": "(*ja0djO#PKpfkfpkLK:KKFf\u003d",
                            "algorithm": "aes-256-gcm",
                            "keyEncoding": "utf8",
                            "folder": "root",
                            "createdAt": "2023-10-22T23:08:06.897Z",
                            "updatedAt": "2023-10-22T23:08:06.897Z",
                            "__v": 0
                          }
                        ],
                        "imports": []
                      }
                      """;
        
        var secrets = SecretsList.Deserialize(content);
        secrets.Should().NotBeNull();

        // Assuming that there is only one secret in the list
        var secret = secrets.Secrets.First();

        secret.SecretKeyCiphertext.Should().Be("(*ja0djO#PKpfkfpkLK:KKFf\u003d");
        secret.SecretKeyIV.Should().Be("(*ja0djO#PKpfkfpkLK:KKFf\u003d");
        secret.SecretKeyTag.Should().Be("(*ja0djO#PKpfkfpkLK:KKFf\u003d");
        secret.SecretValueCiphertext.Should().Be("(*ja0djO#PKpfkfpkLK:KKFf\u003d(*ja0djO#PKpfkfpkLK:KKFf\u003d(*ja0djO#PKpfkfpkLK:KKFf\u003d");
        secret.SecretValueIV.Should().Be("(*ja0djO#PKpfkfpkLK:KKFf\u003d");
        secret.SecretValueTag.Should().Be("(*ja0djO#PKpfkfpkLK:KKFf\u003d");
    }
}
