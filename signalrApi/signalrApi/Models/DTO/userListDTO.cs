using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace signalrApi.Models.DTO
{
    public class userListDTO
    {
        public string Username { get; set; }

        public bool LoggedIn { get; set; }

    }
}
