using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace InfisicalDotNet.Tests;

[TestClass]
public class SystemTests
{
    [TestMethod]
    public void ShouldGetSecretsFromTestAccount()
    {
        var configuration = new ConfigurationBuilder()
            .AddInfisical()
            .Build();
        
        configuration["TEST_SECRET"].Should().Be("I'm used by CI tests to verify that InfisicalDotNet works");
        configuration.GetConnectionString("DefaultConnection").Should().Be("This is a connection string");
    }
}