namespace DemoBlazorServerOnlinePostOffice.Data.Models
{
    public class UserAccount
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int BoxNumberAssigned { get; set; }
        public string? BoxKey { get; set; }
        public string? Password { get; set; }
        public string? Location { get; set; }
    }

}
