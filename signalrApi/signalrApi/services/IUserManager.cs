using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using signalrApi.Data;
using signalrApi.Models;
using signalrApi.Models.DTO;
using signalrApi.Models.Identity;
using signalrApi.Repositories.UserChannelRepos;
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
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private knotSlackDbContext _context;

        public UserManagerWrapper(UserManager<ksUser> userManager, IConfiguration configuration, knotSlackDbContext _context, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.roleManager = roleManager;
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

        public async Task<IdentityResult> CreateAsync(ksUser user, string password, string role)
        {
           var result = await userManager.CreateAsync(user, password);
           await userManager.AddToRoleAsync(user, role);
           await AddNewUserToGeneral(user.UserName);

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

        public async Task<string> CreateToken(ksUser user)
        {
            var secret = configuration["JWT:Secret"];
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            var signingKey = new SymmetricSecurityKey(secretBytes);
            var roles = (List<string>)await userManager.GetRolesAsync(user);

            var tokenClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim("UserId", user.Id),
                new Claim(ClaimTypes.Role, roles[0]),
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

        public async Task<bool> AdminCheck()
        {
            if (!await roleManager.RoleExistsAsync("admin"))
            {
                var newRole = new IdentityRole();
                newRole.Name = "admin";
                await roleManager.CreateAsync(newRole);

                return false;
            } else
            {
                var admins = await userManager.GetUsersInRoleAsync("admin");

                if(admins.Count == 0)
                {
                    return false;
                } else
                {
                    return true;
                }
            }
        }

        public async Task<IList<string>> GetUserRoles(ksUser user)
        {
            var thisUser = await FindByNameAsync(user.UserName);
            return await userManager.GetRolesAsync(thisUser);
        }

        public async Task<IdentityResult> DeleteUser(ksUser user)
        {
            var deletedUser = await FindByNameAsync(user.UserName);

            return await userManager.DeleteAsync(deletedUser);
        }

        public async Task<UserWithToken> CreateUserWToken(ksUser user)
        {
            return new UserWithToken
            {
                UserId = user.UserName,
                Token = await CreateToken(user),
                Channels = await GetUserChannels(user),
                Roles = (List<string>)await GetUserRoles(user),
                LastVisited = DateTime.Now,
            };
        }

        public async Task<List<createChannelDTO>> GetUserChannels(ksUser user)
        {
            var userChannels = await _context.UserChannels
                .Where(uc => uc.UserId == user.Id)
                .Select(uc => new createChannelDTO
                {
                    name = uc.Channel.Name,
                    type = uc.Channel.Type,
                }).ToListAsync();


            return userChannels;
        }
        public async Task<UserChannel> AddNewUserToGeneral(string username)
        {
            var user = await userManager.FindByNameAsync(username);

            var newUC = new UserChannel
            {
                UserId = user.Id,
                ChannelName = "General",
            };

            _context.UserChannels.Add(newUC);
            await _context.SaveChangesAsync();

            return newUC;
        }

        public async Task<bool> IsUserAdmin(ksUser user)
        {
            return await userManager.IsInRoleAsync(user, "admin");
        }
    }

    public interface IUserManager
    {
        Task<ksUser> FindByNameAsync(string username);
        Task<bool> CheckPasswordAsync(ksUser user, string password);
        Task AccessFailedAsync(ksUser user);
        Task<IdentityResult> CreateAsync(ksUser user, string password, string role);
        Task<bool> AdminCheck();
        Task<bool> IsUserAdmin(ksUser user);
        Task<IList<string>> GetUserRoles(ksUser user);
        Task<ksUser> FindByIdAsync(string userId);
        Task<ksUser> FindAllLoggedInUsers();
        Task<IdentityResult> UpdateAsync(ksUser user);
        Task<IdentityResult> DeleteUser(ksUser user);
        Task<UserWithToken> CreateUserWToken(ksUser user);
        Task<List<createChannelDTO>> GetUserChannels(ksUser user);
        Task<UserChannel> AddNewUserToGeneral(string username);
        Task<string> CreateToken(ksUser user);
    }
}
