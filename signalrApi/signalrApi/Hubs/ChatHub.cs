using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using signalrApi.Data;
using signalrApi.Models;
using signalrApi.Models.DTO;
using signalrApi.Repositories.MessageRepos;
using signalrApi.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalrApi.Hubs
{
    public class ChatHub : Hub, IChatHub
    {
        private knotSlackDbContext _context;
        private readonly IMessageRepository messageRepository;
        private readonly IUserManager userManager;

        public ChatHub(knotSlackDbContext _context, IUserManager userManager, IMessageRepository messageRepository)
        {
            this._context = _context;
            this.userManager = userManager;
            this.messageRepository = messageRepository;
        }

        public async Task SendMessage(string sender, string recipient, string contents)
        {
            var message = await WriteMessage(sender, recipient, contents);

            await messageRepository.CreateNewMessage(message);

            await Clients.All.SendAsync("ReceiveMessage", message);
        }

        public async Task<Message> WriteMessage(string sender, string recipient, string contents)
        {
            var user = await userManager.FindByNameAsync(sender);
            
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                Contents = contents,
                UserId = (user != null) ? user.Id : null,
                Date = DateTime.Now,
            };

            return message;
        }

        public async Task SendPrivateMessage(string sender, string recipient, string contents)
        {
            var message = await WriteMessage(sender, recipient, contents);

            await messageRepository.CreateNewMessage(message);

            var thisUser = Clients.User(recipient);

            await Clients.User(recipient).SendAsync("ReceiveMessage", message);

            await Clients.Caller.SendAsync("ReceiveMessage", message);

            Console.WriteLine(thisUser);
        }

        public async Task AddToGroup(string channelName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, channelName);

           
        }

        public async Task RemoveFromGroup(string channelName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, channelName);

        }

        public async Task SendGroupMessage(string sender, string recipient, string contents)
        {
            var message = await WriteMessage(sender, recipient, contents);

            await messageRepository.CreateNewMessage(message);

            await Clients.Group(recipient).SendAsync("ReceiveMessage", message);

            //Console.WriteLine(recipient);
        }

        public async Task DisplayUsers()
        {
            var users = _context.Users
                .Where(user => user.LoggedIn)
                .Select(user => new userListDTO
                {
                    Username = user.Email,
                    LoggedIn = user.LoggedIn
                }).ToList();

            if (users != null && users.Count > 0)
            {
                await Clients.All.SendAsync("ShowUsers", users.ToArray());
            }
        }

        //method for testing and experimentation
        public async Task GetContext() 
        {
            string msg = $"ConnectionID: {Context.ConnectionId}, User: {Context.User.ToString()}";
            await Clients.Caller.SendAsync("ShowContext", msg);
        }

        public async Task SendUpdatedUser(string username, bool loggedIn)
        {
            var updatedUser = new userListDTO
            {
                Username = username,
                LoggedIn = loggedIn,
            };
            if (Clients != null) {
                await Clients.All.SendAsync("UpdateUsers", updatedUser);
            };
        }

        public async Task UpdateLastVisited(string username)
        {
            var thisUser = await userManager.FindByNameAsync(username);

            thisUser.LastVisited = DateTime.Now;

            await userManager.UpdateAsync(thisUser);
        }


        //public async Task RemoveUser(string user)
        //{
        //    Users.Remove(Users.Find(x => x == user));

        //    await Clients.All.SendAsync("ShowUsers", Users.ToArray());
        //}
    }

    public interface IChatHub
    {
        Task SendMessage(string sender, string recipient, string contents);
        Task<Message> WriteMessage(string sender, string recipient, string contents);
        Task SendPrivateMessage(string sender, string recipient, string contents);
        Task AddToGroup(string channelName);
        Task RemoveFromGroup(string channelName);
        Task SendGroupMessage(string sender, string recipient, string contents);
        Task SendUpdatedUser(string username, bool loggedIn);
        Task DisplayUsers();
        //method for testing and experimentation
         Task GetContext();
        Task UpdateLastVisited(string username);
    }
}
