using System.ComponentModel.DataAnnotations;

namespace DemoBlazorServerOnlinePostOffice.Data.Models
{
    public class UserAccountModel
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required, EmailAddress, DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [Required]
        public int BoxNumberAssigned { get; set; }
        [Required, DataType(DataType.MultilineText)]
        public string? Location { get; set; }
        [Required, DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required, Compare("Password"), DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
