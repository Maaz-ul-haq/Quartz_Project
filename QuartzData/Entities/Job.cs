using QuartzData.Entities;

namespace QuartzData;


public class Job
{
    public string Id { get; set; } = string.Empty;
    public string JobName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int JobType { get; set; }
    public string CronExpression { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastExecutedAt { get; set; }
    public string? JobData { get; set; }
   
    public ICollection<APICallHistory> ApiCallHistories { get; set; }
}
