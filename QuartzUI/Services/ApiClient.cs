using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuartzUI.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "https://localhost:7062/api";

    public ApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // Email Job Endpoints
    public async Task<ApiResponse<JobDto>> CreateEmailJobAsync(EmailJobRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_apiUrl}/jobs/create-email-job", content);
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<JobDto>>(responseJson) ?? new ApiResponse<JobDto>();
    }

    // HTTP API Job Endpoints
    public async Task<ApiResponse<JobDto>> CreateHttpApiJobAsync(HttpApiJobRequest request)
    {
        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_apiUrl}/jobs/create-http-api-job", content);
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<JobDto>>(responseJson) ?? new ApiResponse<JobDto>();
    }

    // General Job Endpoints
    public async Task<ApiResponse<JobDto>> GetJobAsync(string jobId)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/jobs/{jobId}");
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<JobDto>>(responseJson) ?? new ApiResponse<JobDto>();
    }

    public async Task<ApiResponse<IEnumerable<JobDto>>> GetAllJobsAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/jobs/all");
        var responseJson = await response.Content.ReadAsStringAsync();
        var jobObj = JsonSerializer.Deserialize<ApiResponse<IEnumerable<JobDto>>>(responseJson) ?? new ApiResponse<IEnumerable<JobDto>>();
        return jobObj;
    }

    public async Task<ApiResponse<string>> UpdateJobAsync(JobDto job)
    {
        var json = JsonSerializer.Serialize(job);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{_apiUrl}/jobs/update", content);
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<string>>(responseJson) ?? new ApiResponse<string>();
    }

    public async Task<ApiResponse<string>> DeleteJobAsync(string jobId)
    {
        var response = await _httpClient.DeleteAsync($"{_apiUrl}/jobs/{jobId}");
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<string>>(responseJson) ?? new ApiResponse<string>();
    }

    public async Task<ApiResponse<string>> PauseJobAsync(string jobId)
    {
        var response = await _httpClient.PostAsync($"{_apiUrl}/jobs/{jobId}/pause", null);
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<string>>(responseJson) ?? new ApiResponse<string>();
    }

    public async Task<ApiResponse<string>> ResumeJobAsync(string jobId)
    {
        var response = await _httpClient.PostAsync($"{_apiUrl}/jobs/{jobId}/resume", null);
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ApiResponse<string>>(responseJson) ?? new ApiResponse<string>();
    }

    // Generic GET method
    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/{endpoint}");
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(responseJson) ?? default;
    }

    // API Call History Endpoints
    public async Task<List<APICallHistoryDto>> GetApiCallHistoryAsync()
    {
        var response = await _httpClient.GetAsync($"{_apiUrl}/jobs/api-call-history");
        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<APICallHistoryDto>>(responseJson) ?? new List<APICallHistoryDto>();
    }
}

public class ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
    [JsonPropertyName("data")]
    public T? Data { get; set; }
}

public class JobDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonPropertyName("jobName")]
    public string JobName { get; set; } = string.Empty;
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    [JsonPropertyName("jobType")]
    public JobType JobType { get; set; }
    [JsonPropertyName("cronExpression")]
    public string CronExpression { get; set; } = string.Empty;
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [JsonPropertyName("lastExecutedAt")]
    public DateTime? LastExecutedAt { get; set; }
    [JsonPropertyName("jobData")]
    public string? JobData { get; set; }
}

public enum JobType
{
    SendEmail = 0,
    CallHttpApi = 1
}

public class EmailJobRequest
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [Required(ErrorMessage = "Job Name is required")]
    public string JobName { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Cron Expression is required")]
    public string CronExpression { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string ToAddress { get; set; }

    [Required(ErrorMessage = "Subject is required")]
    public string Subject { get; set; }

    [Required(ErrorMessage = "Body is required")]
    public string Body { get; set; }

    public bool IsHtml { get; set; }
}

public class HttpApiJobRequest
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    [Required(ErrorMessage = "Job Name is required")]
    public string JobName { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Cron Expression is required")]
    public string CronExpression { get; set; }

    [Required(ErrorMessage = "API URL is required")]
    [Url(ErrorMessage = "Invalid URL format")]
    public string Url { get; set; }

    [Required(ErrorMessage = "HTTP Method is required")]
    public string HttpMethod { get; set; }

    public string Headers { get; set; }

    public string RequestBody { get; set; }
}

public class APICallHistoryDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("jobId")]
    public string JobId { get; set; }

    [JsonPropertyName("responseBody")]
    public string ResponseBody { get; set; }

    [JsonPropertyName("statusCode")]
    public int? StatusCode { get; set; }

    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; }

    [JsonPropertyName("executedAt")]
    public DateTime ExecutedAt { get; set; }

    [JsonPropertyName("job")]
    public JobDto Job { get; set; }
}
