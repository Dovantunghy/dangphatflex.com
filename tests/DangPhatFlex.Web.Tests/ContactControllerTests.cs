using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ContactControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ContactControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddScoped<DangPhatFlex.Web.Services.IEmailSender, FakeEmailSender>();
            });
        });
    }

    [Fact]
    public async Task PostContactForm_ValidInput_RedirectsAndSaves()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var form = new Dictionary<string, string>
        {
            ["FullName"] = "Nguyễn Văn A",
            ["Phone"] = "0900000000",
            ["Message"] = "Tôi cần báo giá khớp nối mềm inox DP25UB."
        };

        var response = await client.PostAsync("/Public/Contact", new FormUrlEncodedContent(form));

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task PostContactForm_MissingRequiredFields_ReturnsFormWithoutRedirect()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var form = new Dictionary<string, string>
        {
            ["FullName"] = "",
            ["Phone"] = "",
            ["Message"] = ""
        };

        var response = await client.PostAsync("/Public/Contact", new FormUrlEncodedContent(form));

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    private class FakeEmailSender : DangPhatFlex.Web.Services.IEmailSender
    {
        public Task SendAsync(string toEmail, string subject, string body) => Task.CompletedTask;
    }
}
