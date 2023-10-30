using DemoBlazorServerOnlinePostOffice.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoBlazorServerOnlinePostOffice.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext appDbContext;
        private readonly IConfiguration config;

        public AccountService(AppDbContext appDbContext, IConfiguration config)
        {
            this.appDbContext = appDbContext;
            this.config = config;
        }

        public async Task<(bool flag, string message)> RegisterAsync(UserAccountModel model)
        {
            //Check if user email contains admin and admin role is already occupied.
            string role = string.Empty;
            if (model.Email!.ToLower().Contains("admin"))
            {
                var chkRole = await appDbContext.UserRoles.FirstOrDefaultAsync(_ => _.RoleName!.ToLower().Equals("admin"));
                if (chkRole is null)
                    role = "Admin";
                else
                    return (false, "Admin already registered");
            }
            role = "User";

            //Check if the email is already used in registration.
            var chk = await appDbContext.UserAccounts.FirstOrDefaultAsync(_ => _.Email!.ToLower().Equals(model.Email!.ToLower()));
            if (chk is not null) return (false, "User already registered");

            var availableBoxes = await appDbContext.Boxes.Where(_ => _.Assigned == false).ToListAsync();
            if (availableBoxes.Count == 0) return (false, "Sorry, all Boxes are occupied");

            // Generate Box Key for the user
            string boxKey = string.Empty;
            boxKey = GenerateBoxKey();

            var chkKey = await appDbContext.UserAccounts.Where(_ => _.BoxKey!.Equals(boxKey)).FirstOrDefaultAsync();
            while (chkKey != null)
            {
                boxKey = GenerateBoxKey();
            }

            var entity = appDbContext.UserAccounts.Add(
           new UserAccount()
           {
               Email = model.Email,
               Name = model.Name,
               Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
               Location = model.Location,
               BoxNumberAssigned = availableBoxes.First().Number,
               BoxKey = boxKey
           }).Entity;
            await appDbContext.SaveChangesAsync();

            //Update the boxes and assigned the number.
            var box = await appDbContext.Boxes.FirstOrDefaultAsync(_ => _.Number == entity.BoxNumberAssigned);
            box!.Assigned = true;
            await appDbContext.SaveChangesAsync();

            //Save the Role asigned to the database   
            appDbContext.UserRoles.Add(new UserRole() { RoleName = role, UserId = entity.Id });
            await appDbContext.SaveChangesAsync();
            return (true, $"Suceesfully Registered:{Environment.NewLine}Your box Number is : Box {box.Number}{Environment.NewLine}Your Box Key is: {boxKey}");
        }

        private static string GenerateBoxKey()
        {
            int length = 10;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rand = new Random();
            string randomString = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
            return randomString;
        }


        public async Task<string> LoginAsync(Login model)
        {
            //Check if user email exist
            var chk = await appDbContext.UserAccounts.FirstOrDefaultAsync(_ => _.Email!.ToLower().Equals(model.Email!.ToLower()));
            if (chk is null) return null!;

            //Check password
            if (!BCrypt.Net.BCrypt.Verify(model.Password, chk.Password))
                return null!;

            //Find User Role from the User-role table
            var getRole = await appDbContext.UserRoles.FirstOrDefaultAsync(_ => _.UserId == chk.Id);
            if (getRole is null) return null!;

            //Generate Token
            return GenerateToken(chk.Id, model.Email, getRole.RoleName);
        }

        private string GenerateToken(int id, string? email, string? roleName)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,id.ToString()!),
                new Claim(ClaimTypes.Name,email!),
                new Claim(ClaimTypes.Role,roleName!)
            };
            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: userClaims,
                expires: DateTime.UtcNow.AddSeconds(10),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}
