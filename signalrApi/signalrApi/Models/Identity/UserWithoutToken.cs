using signalrApi.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalrApi.Models.Identity
{
    public class UserWithoutToken
    {
        public string UserId { get; set; }

        public DateTime LastVisited { get; set; }
        public List<createChannelDTO> Channels { get; set; }

        public List<string> Roles { get; set; }
    }
}
