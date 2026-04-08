using QuartzAPI.Models;

namespace QuartzAPI.Services;

public interface IHttpApiService
{
    Task<HttpResponseMessage> CallApiAsync(string url, string httpMethod, string? headers = null, string? requestBody = null);
}

public class HttpApiService : IHttpApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpApiService> _logger;

    public HttpApiService(HttpClient httpClient, ILogger<HttpApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> CallApiAsync(string url, string httpMethod, string? headers = null, string? requestBody = null)
    {
        try
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
            request.Method = new HttpMethod(httpMethod.ToUpper());

            if (!string.IsNullOrEmpty(headers))
            {
                var headerLines = headers.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (var headerLine in headerLines)
                {
                    if (string.IsNullOrWhiteSpace(headerLine)) continue;
                    var parts = headerLine.Split(new[] { ':' }, 2, StringSplitOptions.TrimEntries);
                    if (parts.Length == 2)
                    {
                        request.Headers.Add(parts[0], parts[1]);
                    }
                }
            }

            if (!string.IsNullOrEmpty(requestBody))
            {
                request.Content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"API call to {url} completed with status {response.StatusCode}");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error calling API: {ex.Message}");
            throw;
        }
    }
}
