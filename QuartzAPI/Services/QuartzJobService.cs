using QuartzAPI.Models;
using QuartzAPI.Jobs;
using Quartz;
using System.Text.Json;

namespace QuartzAPI.Services;

public interface IQuartzJobService
{
    Task CreateJobAsync(JobDto job);
    Task UpdateJobAsync(JobDto job);
    Task DeleteJobAsync(string jobId);
    Task<JobDto?> GetJobAsync(string jobId);
    Task<IEnumerable<JobDto>> GetAllJobsAsync();
    Task PauseJobAsync(string jobId);
    Task ResumeJobAsync(string jobId);
    Task<IEnumerable<APICallHistoryDto>> GetApiCallHistoriesAsync();
}

public class QuartzJobService : IQuartzJobService
{
    private readonly ISchedulerFactory _schedulerFactory;
    private readonly IJobStorageService _jobStorageService;
    private readonly ILogger<QuartzJobService> _logger;
    private IScheduler? _scheduler;

    public QuartzJobService(ISchedulerFactory schedulerFactory, IJobStorageService jobStorageService, ILogger<QuartzJobService> logger)
    {
        _schedulerFactory = schedulerFactory;
        _jobStorageService = jobStorageService;
        _logger = logger;
    }

    private async Task<IScheduler> GetSchedulerAsync()
    {
        if (_scheduler == null)
        {
            _scheduler = await _schedulerFactory.GetScheduler();
        }
        return _scheduler;
    }

    public async Task CreateJobAsync(JobDto job)
    {
        try
        {
            var scheduler = await GetSchedulerAsync();

            IJobDetail jobDetail;
            ITrigger trigger;

            if (job.JobType == JobType.SendEmail)
            {
                jobDetail = JobBuilder.Create<SendEmailJob>()
                    .WithIdentity(job.Id, "email-jobs")
                    .UsingJobData("JobId", job.Id)
                    .UsingJobData("ToAddress", JsonSerializer.Deserialize<EmailJobData>(job.JobData ?? "{}")?.ToAddress ?? "")
                    .UsingJobData("Subject", JsonSerializer.Deserialize<EmailJobData>(job.JobData ?? "{}")?.Subject ?? "")
                    .UsingJobData("Body", JsonSerializer.Deserialize<EmailJobData>(job.JobData ?? "{}")?.Body ?? "")
                    .UsingJobData("IsHtml", JsonSerializer.Deserialize<EmailJobData>(job.JobData ?? "{}")?.IsHtml ?? true)
                    .Build();
            }
            else
            {
                jobDetail = JobBuilder.Create<CallHttpApiJob>()
                    .WithIdentity(job.Id, "api-jobs")
                    .UsingJobData("JobId", job.Id)
                    .UsingJobData("Url", JsonSerializer.Deserialize<HttpApiJobData>(job.JobData ?? "{}")?.Url ?? "")
                    .UsingJobData("HttpMethod", JsonSerializer.Deserialize<HttpApiJobData>(job.JobData ?? "{}")?.HttpMethod ?? "GET")
                    .UsingJobData("Headers", JsonSerializer.Deserialize<HttpApiJobData>(job.JobData ?? "{}")?.Headers ?? "")
                    .UsingJobData("RequestBody", JsonSerializer.Deserialize<HttpApiJobData>(job.JobData ?? "{}")?.RequestBody ?? "")
                    .Build();
            }

            trigger = TriggerBuilder.Create()
                .WithIdentity($"{job.Id}-trigger", job.JobType == JobType.SendEmail ? "email-triggers" : "api-triggers")
                .WithCronSchedule(job.CronExpression)
                .Build();

            await scheduler.ScheduleJob(jobDetail, trigger);

            var jobExt = await _jobStorageService.GetJobAsync(job.Id);
            
            if(jobExt is null)
            {
                await _jobStorageService.SaveJobAsync(job);
            }
           
            _logger.LogInformation($"Job created: {job.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error creating job: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateJobAsync(JobDto job)
    {
        try
        {
            var scheduler = await GetSchedulerAsync();
            await DeleteJobAsync(job.Id);
            await CreateJobAsync(job);

            _logger.LogInformation($"Job updated: {job.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating job: {ex.Message}");
            throw;
        }
    }

    public async Task DeleteJobAsync(string jobId)
    {
        try
        {
            var scheduler = await GetSchedulerAsync();
            var job = await _jobStorageService.GetJobAsync(jobId);

            if (job != null)
            {
                var jobKey = new JobKey(jobId, job.JobType == JobType.SendEmail ? "email-jobs" : "api-jobs");
                await scheduler.DeleteJob(jobKey);
            }

            await _jobStorageService.DeleteJobAsync(jobId);

            _logger.LogInformation($"Job deleted: {jobId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting job: {ex.Message}");
            throw;
        }
    }

    public async Task<JobDto?> GetJobAsync(string jobId)
    {
        return await _jobStorageService.GetJobAsync(jobId);
    }

    public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
    {
        return await _jobStorageService.GetAllJobsAsync();
    }

    public async Task PauseJobAsync(string jobId)
    {
        try
        {
            var scheduler = await GetSchedulerAsync();
            var job = await _jobStorageService.GetJobAsync(jobId);

            if (job != null)
            {
                var jobKey = new JobKey(jobId, job.JobType == JobType.SendEmail ? "email-jobs" : "api-jobs");
                await scheduler.PauseJob(jobKey);
                job.IsActive = false;
                await _jobStorageService.UpdateJobAsync(job);

                _logger.LogInformation($"Job paused: {jobId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error pausing job: {ex.Message}");
            throw;
        }
    }

    public async Task ResumeJobAsync(string jobId)
    {
        try
        {
            var scheduler = await GetSchedulerAsync();
            var job = await _jobStorageService.GetJobAsync(jobId);

            if (job != null)
            {
                var jobKey = new JobKey(jobId, job.JobType == JobType.SendEmail ? "email-jobs" : "api-jobs");
                await scheduler.ResumeJob(jobKey);
                job.IsActive = true;
                await _jobStorageService.UpdateJobAsync(job);

                _logger.LogInformation($"Job resumed: {jobId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error resuming job: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<APICallHistoryDto>> GetApiCallHistoriesAsync()
    {
        return await _jobStorageService.GetApiCallHistoriesAsync();
    }
}
