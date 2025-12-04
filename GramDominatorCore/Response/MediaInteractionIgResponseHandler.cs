using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDModel;

namespace GramDominatorCore.Response
{
    public abstract class MediaInteractionIgResponseHandler : GDLibrary.Response.IGResponseHandler
    {
        protected MediaInteractionIgResponseHandler(IResponseParameter response)
            : base(response)
        {
        }

        public bool HasMoreResults { get; set; }

        public int InteractionCount { get; protected set; }

        public string MaxId { get;set; }

        public List<InstagramUser> UserList { get; protected set; }
        public List<string> Likers { get; set; } = new List<string>();
    }
}
