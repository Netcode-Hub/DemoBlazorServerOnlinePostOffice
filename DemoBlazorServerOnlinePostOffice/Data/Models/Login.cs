using System.ComponentModel.DataAnnotations;

namespace DemoBlazorServerOnlinePostOffice.Data.Models
{
    public class Login
    {
        [Required, EmailAddress, DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Password), Required]
        public string? Password { get; set; }
    }
}
