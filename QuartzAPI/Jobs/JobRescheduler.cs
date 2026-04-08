using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using QuartzAPI.Models;
using QuartzAPI.Services;
using QuartzData;
using QuartzData.Context;

namespace QuartzAPI.Jobs
{
    public class JobRescheduler : IJob
    {
        private readonly IServiceProvider _services;
        private readonly IQuartzJobService _quartzServices;
        private readonly ILogger<JobRescheduler> _logger;

        public JobRescheduler(IServiceProvider services, ILogger<JobRescheduler> logger, IQuartzJobService quartzServices)
        {
            _services = services;
            _quartzServices = quartzServices;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {

            _logger.LogInformation("Bootstrapping jobs from database...");

            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<QuartzDbContext>();
            var scheduler = context.Scheduler;

            var jobs = await db.Jobs
                      .Where(x => x.IsActive)
                      .Select(x => new JobDto
                      {
                          Id = x.Id,
                          JobName = x.JobName,
                          Description = x.Description,
                          JobType = x.JobType == 0 ? JobType.SendEmail : JobType.CallHttpApi,
                          CronExpression = x.CronExpression,
                          IsActive = x.IsActive,
                          CreatedAt = x.CreatedAt,
                          LastExecutedAt = x.LastExecutedAt,
                          JobData = x.JobData,
                      })
                      .ToListAsync();

            foreach (var job in jobs)
            {
                await _quartzServices.CreateJobAsync(job);
            }

           _logger.LogInformation("All jobs loaded into memory.");
        }
    }
}