using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace signalrApi.Models.Identity
{
    public class DeleteData
    {
        [Required]
        public string Username { get; set; }
    }
}
