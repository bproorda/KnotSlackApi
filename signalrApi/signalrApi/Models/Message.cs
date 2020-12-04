using signalrApi.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace signalrApi.Models
{
    public class Message
    {   [Key]
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string Sender { get; set; }

        public string Recipient { get; set; }

        public DateTime Date { get; set; }

        [MaxLength]
        public string Contents { get; set; }

        //Nav prop
        public UserMessage UserMessage { get; set; }
    }
}
