using System.ComponentModel.DataAnnotations;

namespace DemoBlazorServerOnlinePostOffice.Data.Models
{
    
        public class FileSentModel
        {
            public int Id { get; set; }
            public int SenderId { get; set; }
            public List<string>? FileNames { get; set; }
            [Required]
            public int ReceiverBoxNumber { get; set; }
            public DateTime? DateSent { get; set; } = DateTime.Now;
        }
    
}
