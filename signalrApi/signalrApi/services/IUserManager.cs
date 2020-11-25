using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using signalrApi.Data;
using signalrApi.Models.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace signalrApi.services
{
    public class UserManagerWrapper : IUserManager
    {
        private readonly UserManager<ksUser> userManager;
        private readonly IConfiguration configuration;
        private knotSlackDbContext _context;

        public UserManagerWrapper(UserManager<ksUser> userManager, IConfiguration configuration, knotSlackDbContext _context)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this._context = _context;
        }

        public Task AccessFailedAsync(ksUser user)
        {
            return userManager.AccessFailedAsync(user);
        }

        public Task<bool> CheckPasswordAsync(ksUser user, string password)
        {
            return userManager.CheckPasswordAsync(user, password);
        }

        public Task<IdentityResult> CreateAsync(ksUser user, string password, string role)
        {
            var result = userManager.CreateAsync(user, password);
            AddRole(user, role);
            return result;
        }

        public Task<ksUser> FindByIdAsync(string userId)
        {
            return userManager.FindByIdAsync(userId);
        }

        public Task<ksUser> FindByNameAsync(string username)
        {
            return userManager.FindByNameAsync(username);
        }

        public Task<IdentityResult> UpdateAsync(ksUser user)
        {
            return userManager.UpdateAsync(user);
        }

        public string CreateToken(ksUser user)
        {
            var secret = configuration["JWT:Secret"];
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            var signingKey = new SymmetricSecurityKey(secretBytes);

            var tokenClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim("UserId", user.Id),
            };

            var token = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddSeconds(36000),
                claims: tokenClaims,
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }

        public Task<ksUser> FindAllLoggedInUsers()
        {
            throw new NotImplementedException();
        }

        public void AddRole(ksUser user, string role)
        {
            userManager.AddToRoleAsync(user, role);
        }

        public bool AdminCheck()
        {
            if (_context.Roles.Any(r => r.Name == "admin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task<IList<string>> GetUserRoles(ksUser user)
        {
            return userManager.GetRolesAsync(user);
        }
    }

    public interface IUserManager
    {
        Task<ksUser> FindByNameAsync(string username);
        Task<bool> CheckPasswordAsync(ksUser user, string password);
        Task AccessFailedAsync(ksUser user);
        Task<IdentityResult> CreateAsync(ksUser user, string password, string role);
        void AddRole(ksUser user, string role);
        bool AdminCheck();
        Task<IList<string>> GetUserRoles(ksUser user);
        Task<ksUser> FindByIdAsync(string userId);
        Task<ksUser> FindAllLoggedInUsers();
        Task<IdentityResult> UpdateAsync(ksUser user);
        string CreateToken(ksUser user);
    }
}
