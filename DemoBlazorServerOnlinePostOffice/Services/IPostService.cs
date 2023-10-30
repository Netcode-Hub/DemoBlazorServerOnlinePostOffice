using DemoBlazorServerOnlinePostOffice.Data.Models;

namespace DemoBlazorServerOnlinePostOffice.Services
{
    public interface IPostService
    {
        Task<List<Box>> GetPostOfficeBoxes();
        Task<List<MyDocument>> MyDocuments(int userId);
        Task<(bool flag, string message)> SendFiles(FileSentModel fileSent);
        Task<(bool flag, string message)> DeleteFileAsync(int fileId);
        Task<int> OpenBoxAsync(OpenBox openBox);
    }
}
