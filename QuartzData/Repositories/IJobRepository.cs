using QuartzData.Entities;

namespace QuartzData.Repositories;

public interface IJobRepository
{
    Task SaveJobAsync(Job job);
    Task<Job?> GetJobAsync(string jobId);
    Task<IEnumerable<Job>> GetAllJobsAsync();
    Task UpdateJobAsync(Job job);
    Task DeleteJobAsync(string jobId);
    Task<bool> JobExistsAsync(string jobId);
    Task SaveApiCallResponseAsync(APICallHistory apiCallHistory);
    Task<IEnumerable<APICallHistory>> GetAllApiCallHistoriesAsync();
}
