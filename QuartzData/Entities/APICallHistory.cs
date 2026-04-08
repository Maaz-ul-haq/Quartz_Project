using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzData.Entities
{
    public class APICallHistory
    {
      
        public int Id { get; set; }
        // Foreign Key
        public string JobId { get; set; }
        public string ResponseBody { get; set; }
        public int? StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public DateTime ExecutedAt { get; set; }

        // Navigation Property
        public Job Job { get; set; }
        
    }
}
