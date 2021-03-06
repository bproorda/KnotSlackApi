﻿using signalrApi.Models;
using signalrApi.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace signalrApi.Repositories.ChannelRepos
{
   public interface IChannelRepository
    {
        Task<List<string>> GetMyChannels(string user);

        Task<Channel> CreateNewChannel(createChannelDTO input);

        
    }
}
