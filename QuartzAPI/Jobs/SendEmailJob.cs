using QuartzAPI.Models;
using QuartzAPI.Services;
using Quartz;
using System.Text.Json;

namespace QuartzAPI.Jobs;

public class SendEmailJob : IJob
{
    private readonly IEmailService _emailService;
    private readonly IJobStorageService _jobStorageService;
    private readonly ILogger<SendEmailJob> _logger;

    public SendEmailJob(IEmailService emailService, IJobStorageService jobStorageService, ILogger<SendEmailJob> logger)
    {
        _emailService = emailService;
        _jobStorageService = jobStorageService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation($"Executing SendEmailJob: {context.JobDetail.Key.Name}");

            var jobDataMap = context.JobDetail.JobDataMap;
            var jobId = jobDataMap.GetString("JobId");
            var toAddress = jobDataMap.GetString("ToAddress");
            var subject = jobDataMap.GetString("Subject");
            var body = jobDataMap.GetString("Body");
            var isHtml = jobDataMap.GetBoolean("IsHtml");

            await _emailService.SendEmailAsync(toAddress, subject, body, isHtml);

            if (!string.IsNullOrEmpty(jobId))
            {
                var job = await _jobStorageService.GetJobAsync(jobId);
                if (job != null)
                {
                    job.LastExecutedAt = DateTime.UtcNow;
                    await _jobStorageService.UpdateJobAsync(job);
                }
            }

            _logger.LogInformation($"SendEmailJob completed successfully: {context.JobDetail.Key.Name}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in SendEmailJob: {ex.Message}");
            throw;
        }
    }
}
