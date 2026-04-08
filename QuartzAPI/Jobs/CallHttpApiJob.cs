using Quartz;
using QuartzAPI.Models;
using QuartzAPI.Services;
using QuartzAPI.Utilities;
using QuartzData.Entities;

namespace QuartzAPI.Jobs;

public class CallHttpApiJob : IJob
{
    private readonly IHttpApiService _httpApiService;
    private readonly IJobStorageService _jobStorageService;
    private readonly ILogger<CallHttpApiJob> _logger;

    public CallHttpApiJob(IHttpApiService httpApiService, IJobStorageService jobStorageService, ILogger<CallHttpApiJob> logger)
    {
        _httpApiService = httpApiService;
        _jobStorageService = jobStorageService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation($"Executing CallHttpApiJob: {context.JobDetail.Key.Name}");

            var jobDataMap = context.JobDetail.JobDataMap;
            var jobId = jobDataMap.GetString("JobId");
            var url = jobDataMap.GetString("Url");
            var httpMethod = jobDataMap.GetString("HttpMethod") ?? "GET";
            var headers = jobDataMap.GetString("Headers");
            var requestBody = jobDataMap.GetString("RequestBody");

            var response = await _httpApiService.CallApiAsync(url, httpMethod, headers, requestBody);
           
            var responseBody = await response.Content.ReadAsStringAsync();

            var apiCallHistory = new APICallHistoryDto
            {
                JobId = jobId,
                ResponseBody = responseBody,
                StatusCode = (int)response.StatusCode,
                IsSuccess = response.IsSuccessStatusCode,
                ErrorMessage = response.IsSuccessStatusCode ? null : responseBody,
                ExecutedAt = DateTime.UtcNow
            };

            await _jobStorageService.SaveApiCallResponseAsync(apiCallHistory);

            if (!string.IsNullOrEmpty(jobId))
            {
                var job = await _jobStorageService.GetJobAsync(jobId);
                if (job != null)
                {
                    job.LastExecutedAt = DateTime.UtcNow;
                    await _jobStorageService.UpdateJobAsync(job);
                }
            }

            _logger.LogInformation($"CallHttpApiJob completed successfully: {context.JobDetail.Key.Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in CallHttpApiJob: {ex.Message}");
            throw;
        }
    }
}
