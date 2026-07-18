using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class SeoControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;

    public SeoControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Sitemap_ReturnsXml_ContainingProductUrl()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/sitemap.xml");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/xml", response.Content.Headers.ContentType?.MediaType);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("dang-phat-flex-dp25", content);
    }

    [Fact]
    public async Task RobotsTxt_ReturnsOk()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/robots.txt");

        response.EnsureSuccessStatusCode();
    }
}
