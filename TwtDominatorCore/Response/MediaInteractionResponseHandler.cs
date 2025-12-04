using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.Response
{
    public class MediaInteractionResponseHandler : TdResponseHandler
    {
        protected MediaInteractionResponseHandler(IResponseParameter response)
            : base(response)
        {
        }

        public bool HasMoreResults { get; protected set; }

        public string MinPosition { get; protected set; }

        public List<TwitterUser> UserList { get; protected set; }
    }
}