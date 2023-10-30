using DemoBlazorServerOnlinePostOffice.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DemoBlazorServerOnlinePostOffice.Services
{
    public class PostService : IPostService
    {
        private readonly AppDbContext appDbContext;

        public PostService(AppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<Box>> GetPostOfficeBoxes()
        {
            var results = await appDbContext.Boxes.ToListAsync();
            return results!;
        }

        public async Task<List<MyDocument>> MyDocuments(int userId)
        {
            var myDocs = new List<MyDocument>();
            //Get all specific user signed in files
            var getUserFiles = await appDbContext.FileSents.Where(_ => _.ReceiverId == userId).ToListAsync();
            if (getUserFiles is null) return null!;

            //Cached all users for queries.
            var allUsers = await appDbContext.UserAccounts.ToListAsync();
            //populate each file sent details.
            foreach (var item in getUserFiles)
            {
                var getSenderDetails = allUsers.FirstOrDefault(_ => _.Id == item.SenderId);
                if (getSenderDetails is not null)
                {
                    myDocs.Add(new MyDocument() { FileId = item.Id, SenderName = getSenderDetails.Name, FileName = item.FileName, DateSent = item.DateSent });
                }
            }
            return myDocs;
        }


        public async Task<(bool flag, string message)> SendFiles(FileSentModel fileSent)
        {
            if (fileSent is null || fileSent.FileNames!.Count == 0) return (false, "Error occured, model is empty");

            //check if the box used is available
            var chkIfBoxExist = await appDbContext.Boxes.FirstOrDefaultAsync(_ => _.Number == fileSent!.ReceiverBoxNumber);
            if (chkIfBoxExist is null) return (false, "Sorry your box number is not available");

            var getBoxOwnerDetails = await appDbContext.UserAccounts.Where(_ => _.BoxNumberAssigned == fileSent.ReceiverBoxNumber).FirstOrDefaultAsync();
            if (getBoxOwnerDetails is null) return (false, "Sorry your box number is inactive");

            foreach (string file in fileSent!.FileNames!)
            {
                appDbContext.FileSents.Add(new FileSent() { FileName = file, SenderId = fileSent.SenderId, ReceiverId = getBoxOwnerDetails.Id, DateSent = fileSent.DateSent });
                await appDbContext.SaveChangesAsync();
            }
            return (true, "File Sent!");
        }

        public async Task<(bool flag, string message)> DeleteFileAsync(int fileId)
        {
            var getFile = await appDbContext.FileSents.FirstOrDefaultAsync(_ => _.Id == fileId);
            if (getFile is null) return (false, "Sorry file not found");

            appDbContext.FileSents.Remove(getFile);
            await appDbContext.SaveChangesAsync();
            return (true, $"{getFile.FileName} deleted successfully");
        }

        public async Task<int> OpenBoxAsync(OpenBox openBox)
        {
            var getUser = await appDbContext.UserAccounts.Where(_ => _.BoxNumberAssigned == openBox.BoxNumber && _.BoxKey!.Equals(openBox.Key)).FirstOrDefaultAsync();
            return getUser?.Id ?? 0;
        }
    }
}
