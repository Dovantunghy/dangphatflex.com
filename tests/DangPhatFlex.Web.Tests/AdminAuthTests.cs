using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class AdminAuthTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AdminAuthTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AdminDashboard_WithoutLogin_RedirectsToLogin()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/Admin/Dashboard");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }

    [Fact]
    public async Task LoginPage_Get_ReturnsSuccessWithLoginForm()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Identity/Account/Login");

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Input.Email", body);
        Assert.Contains("Input.Password", body);
    }
}
