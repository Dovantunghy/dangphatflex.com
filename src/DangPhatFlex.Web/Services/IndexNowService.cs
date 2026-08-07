using System.Net.Http.Json;

namespace DangPhatFlex.Web.Services;

public interface IIndexNowService
{
    Task NotifyUrlAsync(string absoluteUrl);
}

// IndexNow (indexnow.org) is a free protocol Bing and Yandex support officially for "tell me about
// this URL right now" pings — unlike Google, which has no public API for requesting indexing of
// ordinary content (its Indexing API is contractually restricted to JobPosting/BroadcastEvent pages).
// This only shortens Bing/Yandex discovery time; Google still crawls on its own schedule regardless.
public class IndexNowService : IIndexNowService
{
    // Public by design — IndexNow keys are meant to be discoverable via the /{key}.txt file below,
    // not a secret. Generated once for this domain; keep stable so past submissions stay valid.
    public const string Key = "7f3ac2df2b9d4d0f8f1e6a54c9c3d21e";
    private const string Host = "dangphatflex.com";

    private readonly HttpClient _httpClient;
    private readonly ILogger<IndexNowService> _logger;

    public IndexNowService(HttpClient httpClient, ILogger<IndexNowService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task NotifyUrlAsync(string absoluteUrl)
    {
        try
        {
            var payload = new
            {
                host = Host,
                key = Key,
                keyLocation = $"https://{Host}/{Key}.txt",
                urlList = new[] { absoluteUrl }
            };

            using var response = await _httpClient.PostAsJsonAsync("https://api.indexnow.org/indexnow", payload);
            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("IndexNow submission for {Url} returned {StatusCode}", absoluteUrl, response.StatusCode);
        }
        catch (Exception ex)
        {
            // Best-effort ping to a third-party service — never let it fail the actual save.
            _logger.LogWarning(ex, "IndexNow submission failed for {Url}", absoluteUrl);
        }
    }
}
