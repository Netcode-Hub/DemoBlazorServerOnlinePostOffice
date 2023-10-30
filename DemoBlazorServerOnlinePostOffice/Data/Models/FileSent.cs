namespace DemoBlazorServerOnlinePostOffice.Data.Models
{
    public class FileSent
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string? FileName { get; set; }
        public int ReceiverId { get; set; }
        public DateTime? DateSent { get; set; }
    }
}
