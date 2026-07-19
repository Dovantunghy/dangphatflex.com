using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ProductsControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductsControllerTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProductsIndex_ReturnsOk_AndListsSeededProduct()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/san-pham");

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Đăng Phát Flex DP25", content);
    }
}
