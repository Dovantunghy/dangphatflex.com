using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ContactSubmissionsAdminTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ContactSubmissionsAdminTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Index_WithoutLogin_RedirectsToLogin()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var response = await client.GetAsync("/Admin/ContactSubmissions");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location!.ToString());
    }
}
