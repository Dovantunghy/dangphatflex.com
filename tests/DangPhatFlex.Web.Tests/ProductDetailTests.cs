using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ProductDetailTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductDetailTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProductDetail_ReturnsOk_AndContainsVariantCodeAndJsonLd()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Public/Products/khop-noi-mem-inox/dang-phat-flex-dp25");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("DP25UB-15-700", content);

        const string startTag = "<script type=\"application/ld+json\">";
        var startIndex = content.IndexOf(startTag, StringComparison.Ordinal);
        Assert.True(startIndex >= 0, "Expected an application/ld+json script tag in the response.");
        startIndex += startTag.Length;
        var endIndex = content.IndexOf("</script>", startIndex, StringComparison.Ordinal);
        Assert.True(endIndex >= 0, "Expected a closing </script> tag for the JSON-LD block.");
        var jsonLd = content[startIndex..endIndex];

        using var document = JsonDocument.Parse(jsonLd);
        Assert.Equal("Product", document.RootElement.GetProperty("@type").GetString());
    }

    [Fact]
    public async Task ProductDetail_UnknownSlug_ReturnsNotFound()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Public/Products/khop-noi-mem-inox/khong-ton-tai");

        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
