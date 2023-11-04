using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace InfisicalDotNet.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        var configuration = new ConfigurationBuilder()
            .AddInfisical()
            .Build();
        
        configuration["TEST_SECRET"].Should().Be("I'm used by CI tests to verify that InfisicalDotNet works");
    }
}