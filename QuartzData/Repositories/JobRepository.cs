using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuartzData.Context;
using QuartzData.Entities;

namespace QuartzData.Repositories;

public class JobRepository : IJobRepository
{
    private readonly QuartzDbContext _context;
    private readonly ILogger<JobRepository> _logger;

    public JobRepository(QuartzDbContext context, ILogger<JobRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SaveJobAsync(Job job)
    {
        try
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Job {job.Id} saved to database");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving job: {ex.Message}");
            throw;
        }
    }

    public async Task SaveApiCallResponseAsync(APICallHistory apiCallHistory)
    {
        try
        {
            _context.ApiCallHistories.Add(apiCallHistory);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Api Response History saved to database");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving Api Response History: {ex.Message}");
            throw;
        }
    }

    public async Task<Job?> GetJobAsync(string jobId)
    {
        try
        {
            return await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting job: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<Job>> GetAllJobsAsync()
    {
        try
        {
            return await _context.Jobs.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting all jobs: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateJobAsync(Job job)
    {
        try
        {
            var existingJob = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == job.Id);
            
            if (existingJob != null)
            {
                existingJob.JobName = job.JobName;
                existingJob.Description = job.Description;
                existingJob.JobType = job.JobType;
                existingJob.CronExpression = job.CronExpression;
                existingJob.IsActive = job.IsActive;
                existingJob.LastExecutedAt = job.LastExecutedAt;
                existingJob.JobData = job.JobData;

                _context.Jobs.Update(existingJob);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Job {job.Id} updated");
            }
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
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
            
            if (job != null)
            {
                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Job {jobId} deleted");
            }
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
            return await _context.Jobs.AnyAsync(j => j.Id == jobId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error checking job existence: {ex.Message}");
            throw;
        }
    }

    public async Task<IEnumerable<APICallHistory>> GetAllApiCallHistoriesAsync()
    {
        try
        {
            return await _context.ApiCallHistories
                .Include(h => h.Job)
                .OrderByDescending(h => h.ExecutedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting API call histories: {ex.Message}");
            throw;
        }
    }
}
