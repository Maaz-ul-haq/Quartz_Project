using Mapster;
using QuartzAPI.Models;
using QuartzData;
using QuartzData.Entities;
using QuartzData.Repositories;
//using static Quartz.Logging.OperationName;

namespace QuartzAPI.Services;

public interface IJobStorageService
{
    Task SaveJobAsync(JobDto job);
    Task<JobDto?> GetJobAsync(string jobId);
    Task<IEnumerable<JobDto>> GetAllJobsAsync();
    Task UpdateJobAsync(JobDto job);
    Task DeleteJobAsync(string jobId);
    Task<bool> JobExistsAsync(string jobId);
    Task SaveApiCallResponseAsync(APICallHistoryDto aPICallHistory);
    Task<IEnumerable<APICallHistoryDto>> GetApiCallHistoriesAsync();
}

public class DatabaseJobStorageService : IJobStorageService
{
    private readonly IJobRepository _jobRepository;
    private readonly ILogger<DatabaseJobStorageService> _logger;

    public DatabaseJobStorageService(IJobRepository jobRepository, ILogger<DatabaseJobStorageService> logger)
    {
        _jobRepository = jobRepository;
        _logger = logger;
    }

    private JobDto MapToDto(QuartzData.Job dbJob)
    {
        return new JobDto
        {
            Id = dbJob.Id,
            JobName = dbJob.JobName,
            Description = dbJob.Description,
            JobType = (JobType)dbJob.JobType,
            CronExpression = dbJob.CronExpression,
            IsActive = dbJob.IsActive,
            CreatedAt = dbJob.CreatedAt,
            LastExecutedAt = dbJob.LastExecutedAt,
            JobData = dbJob.JobData
        };
    }

    public async Task SaveJobAsync(JobDto dto)
    {
        try
        {
            var dbJob = dto.Adapt<Job>();
            await _jobRepository.SaveJobAsync(dbJob);
            _logger.LogInformation($"Job {dto.Id} saved to database");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving job: {ex.Message}");
            throw;
        }
    }

    public async Task SaveApiCallResponseAsync(APICallHistoryDto aPICallHistory)
    {
        try
        {
            var dbRes = aPICallHistory.Adapt<APICallHistory>();
            await _jobRepository.SaveApiCallResponseAsync(dbRes);
            _logger.LogInformation($"API Response History saved to database");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving API Response History: {ex.Message}");
            throw;
        }
    }

    public async Task<JobDto?> GetJobAsync(string jobId)
    {
        try
        {
            var dbJob = await _jobRepository.GetJobAsync(jobId);
            return dbJob != null ? dbJob.Adapt<JobDto>() : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting job: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<JobDto>> GetAllJobsAsync()
    {
        try
        {
            var dbJobs = await _jobRepository.GetAllJobsAsync();
            return dbJobs.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all jobs: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateJobAsync(JobDto job)
    {
        try
        {
            var dbJob = job.Adapt<Job>();
            await _jobRepository.UpdateJobAsync(dbJob);
            _logger.LogInformation($"Job {job.Id} updated");
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
            await _jobRepository.DeleteJobAsync(jobId);
            _logger.LogInformation($"Job {jobId} deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error deleting job: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> JobExistsAsync(string jobId)
    {
        try
        {
            return await _jobRepository.JobExistsAsync(jobId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking job existence: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<APICallHistoryDto>> GetApiCallHistoriesAsync()
    {
        try
        {
            var dbHistories = await _jobRepository.GetAllApiCallHistoriesAsync();
            return dbHistories.Select(h => new APICallHistoryDto
            {
                Id = h.Id,
                JobId = h.JobId,
                ResponseBody = h.ResponseBody,
                StatusCode = h.StatusCode,
                IsSuccess = h.IsSuccess,
                ErrorMessage = h.ErrorMessage,
                ExecutedAt = h.ExecutedAt,
                Job = h.Job != null ? new JobDto
                {
                    Id = h.Job.Id,
                    JobName = h.Job.JobName,
                    Description = h.Job.Description,
                    JobType = (JobType)h.Job.JobType,
                    CronExpression = h.Job.CronExpression,
                    IsActive = h.Job.IsActive,
                    CreatedAt = h.Job.CreatedAt,
                    LastExecutedAt = h.Job.LastExecutedAt,
                    JobData = h.Job.JobData
                } : null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting API call histories: {ex.Message}");
            throw;
        }
    }
}

#region In Memory Code

//public class InMemoryJobStorageService : IJobStorageService
//{
//    private readonly Dictionary<string, JobDto> _jobs = new();
//    private readonly ILogger<InMemoryJobStorageService> _logger;

//    public InMemoryJobStorageService(ILogger<InMemoryJobStorageService> logger)
//    {
//        _logger = logger;
//    }

//    public Task SaveJobAsync(JobDto job)
//    {
//        _jobs[job.Id] = job;
//        _logger.LogInformation($"Job {job.Id} saved (in-memory)");
//        return Task.CompletedTask;
//    }

//    public Task<JobDto?> GetJobAsync(string jobId)
//    {
//        _jobs.TryGetValue(jobId, out var job);
//        return Task.FromResult(job);
//    }

//    public Task<IEnumerable<JobDto>> GetAllJobsAsync()
//    {
//        return Task.FromResult(_jobs.Values.AsEnumerable());
//    }

//    public Task UpdateJobAsync(JobDto job)
//    {
//        if (_jobs.ContainsKey(job.Id))
//        {
//            _jobs[job.Id] = job;
//            _logger.LogInformation($"Job {job.Id} updated (in-memory)");
//        }
//        return Task.CompletedTask;
//    }

//    public Task DeleteJobAsync(string jobId)
//    {
//        _jobs.Remove(jobId);
//        _logger.LogInformation($"Job {jobId} deleted (in-memory)");
//        return Task.CompletedTask;
//    }

//    public Task<bool> JobExistsAsync(string jobId)
//    {
//        return Task.FromResult(_jobs.ContainsKey(jobId));
//    }
//}

#endregion
