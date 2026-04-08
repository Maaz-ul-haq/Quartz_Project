namespace QuartzAPI.Models
{
    public class APICallHistoryDto
    {
        public int Id { get; set; }
        // Foreign Key
        public string JobId { get; set; }
        public string ResponseBody { get; set; }
        public int? StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public DateTime ExecutedAt { get; set; }

        public JobDto? Job { get; set; }
    }
}
