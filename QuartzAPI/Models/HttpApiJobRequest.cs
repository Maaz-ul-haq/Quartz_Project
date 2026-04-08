namespace QuartzAPI.Models
{
    public class HttpApiJobRequest
    {
        public string JobName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CronExpression { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = "GET";
        public string? Headers { get; set; }
        public string? RequestBody { get; set; }
    }
}
