using QuartzAPI.Models;

namespace QuartzAPI.Utilities
{
    public static class JobFactory
    {
        public static JobDto CreateJob<T>(string name, string description, JobType type, string cron, T data)
        {
            return new JobDto
            {
                JobName = name,
                Description = description,
                JobType = type,
                CronExpression = cron,
                IsActive = true,
                // Serialize the generic data object
                JobData = System.Text.Json.JsonSerializer.Serialize(data)
            };
        }
    }
}
