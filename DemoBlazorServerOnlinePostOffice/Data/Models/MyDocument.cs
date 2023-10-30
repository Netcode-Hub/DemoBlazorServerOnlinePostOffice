namespace DemoBlazorServerOnlinePostOffice.Data.Models
{
    public class MyDocument
    {
        public int FileId { get; set; }
        public string? SenderName { get; set; }
        public string? FileName { get; set; }
        public DateTime? DateSent { get; set; }
    }
}
