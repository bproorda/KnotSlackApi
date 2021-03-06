﻿using signalrApi.Models.DTO;
using signalrApi.Models.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace signalrApi.Models
{
    public class Channel
    {
        [Key]
        [Required]
        public string Name { get; set; }

        public string Type { get; set; }

        public List<UserChannel> UserChannels { get; set; }

    }
}
