using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using signalrApi.Models;
using signalrApi.Models.DTO;
using signalrApi.Repositories.MessageRepos;
using signalrApi.services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace signalrApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private IMessageRepository messageRepository;
        private readonly IUserManager userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessagesController(IMessageRepository messageRepository, IUserManager userManager, IHttpContextAccessor _httpContextAccessor)
        {
            this.messageRepository = messageRepository;
            this.userManager = userManager;
            this._httpContextAccessor = _httpContextAccessor;
        }
        
        [HttpPost("msgsender")]
        public async Task<IEnumerable<Message>> GetMessagesSender(userDTO input)
        {
            var messages = await messageRepository.GetMessagesBySender(input.Username);
            return messages;
        }


        [HttpPost("msgrec")]
        public async Task<IEnumerable<Message>> GetMessagesRecipient(userDTO input)
        {
            var messages = await messageRepository.GetMessagesByRecipient(input.Username);
            return messages;
        }
        [Authorize]
        [HttpPost("mymsg")]
        public async Task<IEnumerable<Message>> GetMyMessages()
        {
            var username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = await userManager.FindByNameAsync(username);
            var messages = await messageRepository.GetMyMessages(user);
            return messages;
        }

        [HttpPost("new")]
        public async Task<bool> CreateNewMessage(messageCreateDTO msg)
        {
            var user = await userManager.FindByNameAsync(msg.Sender);

            var message = new Message
            {
                Sender = msg.Sender,
                Recipient = msg.Recipient,
                Contents = msg.Contents,
                UserId = user.Id,
                Date = DateTime.Now,
            };

            await messageRepository.CreateNewMessage(message);
            return false;
        }

        // DELETE api/<MessageController>/5
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
           var message =  await messageRepository.DeleteMessage(id);

            if(message == null)
            {
                return NotFound();
            }

            return true;
        }

        //Deleting all messages sent by a specific user
        [Authorize(Roles = "admin")]
        [HttpPost("delsender")]
        public async Task<ActionResult<bool>> DeleteBySender(userDTO user)
        {
            await messageRepository.DeleteMessagesBySender(user.Username);

            return true;
        }
    }
}
