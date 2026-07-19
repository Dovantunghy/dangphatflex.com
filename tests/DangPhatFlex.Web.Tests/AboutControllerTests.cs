using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class AboutControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AboutControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AboutPage_ReturnsOk_AndContainsCompanyLegalName()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/gioi-thieu");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("CÔNG TY TNHH CƠ ĐIỆN ĐĂNG PHÁT", content);
    }
}
