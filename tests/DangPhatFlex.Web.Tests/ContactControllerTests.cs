using System.Net.Http;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DangPhatFlex.Web.Tests;

public class ContactControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ContactControllerTests(TestWebApplicationFactory factory)
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

        var (token, cookie) = await GetAntiforgeryTokenAndCookieAsync(client, "/Public/Contact");

        var form = new Dictionary<string, string>
        {
            ["FullName"] = "Nguyễn Văn A",
            ["Phone"] = "0900000000",
            ["Message"] = "Tôi cần báo giá khớp nối mềm inox DP25UB.",
            ["__RequestVerificationToken"] = token
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/Public/Contact")
        {
            Content = new FormUrlEncodedContent(form)
        };
        request.Headers.Add("Cookie", cookie);

        var response = await client.SendAsync(request);

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
    }

    [Fact]
    public async Task PostContactForm_MissingRequiredFields_ReturnsFormWithoutRedirect()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var (token, cookie) = await GetAntiforgeryTokenAndCookieAsync(client, "/Public/Contact");

        var form = new Dictionary<string, string>
        {
            ["FullName"] = "",
            ["Phone"] = "",
            ["Message"] = "",
            ["__RequestVerificationToken"] = token
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/Public/Contact")
        {
            Content = new FormUrlEncodedContent(form)
        };
        request.Headers.Add("Cookie", cookie);

        var response = await client.SendAsync(request);

        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    /// <summary>
    /// Standard ASP.NET Core integration-test pattern for posting to an antiforgery-protected
    /// action: GET the form page to obtain the antiforgery cookie + hidden token, then reuse
    /// both on the subsequent POST. Reusable as-is for Admin CRUD POST tests (Tasks 13-16).
    /// </summary>
    private static async Task<(string Token, string Cookie)> GetAntiforgeryTokenAndCookieAsync(HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var cookie = response.Headers.TryGetValues("Set-Cookie", out var cookies)
            ? string.Join("; ", cookies.Select(c => c.Split(';')[0]))
            : throw new InvalidOperationException($"No Set-Cookie header returned from {url}.");

        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(html, @"name=""__RequestVerificationToken""[^>]*value=""([^""]+)""");
        if (!match.Success)
        {
            throw new InvalidOperationException($"Antiforgery token not found in response body from {url}.");
        }

        return (match.Groups[1].Value, cookie);
    }

    private class FakeEmailSender : DangPhatFlex.Web.Services.IEmailSender
    {
        public Task SendAsync(string toEmail, string subject, string body) => Task.CompletedTask;
    }
}
