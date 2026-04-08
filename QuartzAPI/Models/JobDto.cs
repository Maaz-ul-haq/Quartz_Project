namespace QuartzAPI.Models;

public class JobDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string JobName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public JobType JobType { get; set; }
    public string CronExpression { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastExecutedAt { get; set; }
    public string? JobData { get; set; }
}

public enum JobType
{
    SendEmail = 0,
    CallHttpApi = 1
}

public class EmailJobData
{
    public string ToAddress { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsHtml { get; set; } = true;
}

public class HttpApiJobData
{
    public string Url { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = "GET";
    public string? Headers { get; set; }
    public string? RequestBody { get; set; }
}
