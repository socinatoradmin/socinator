using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Interfaces;
using YoutubeDominatorCore.Response;

namespace YoutubeDominatorCore.YoutubeLibrary
{
    public class PostChannelResponseHandler : YDResponseHandler
    {
        public PostChannelResponseHandler(IResponseParameter response) : base(response)
        {
        }
    }
}
