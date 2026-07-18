using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class AboutControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AboutControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AboutPage_ReturnsOk_AndContainsCompanyLegalName()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Public/About");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT", content);
    }
}
