using DemoBlazorServerOnlinePostOffice.Data.Models;

namespace DemoBlazorServerOnlinePostOffice.Services
{
    public interface IAccountService
    {
        Task<(bool flag, string message)> RegisterAsync(UserAccountModel model);
        Task<string> LoginAsync(Login model);
    }
}
