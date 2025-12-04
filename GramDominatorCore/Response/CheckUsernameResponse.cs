using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class CheckUsernameResponse : IGResponseHandler
    {
        public CheckUsernameResponse(IResponseParameter response)  : base(response)
        {
            if (!Success)
                return;

            var jToken = handler.ParseJsonToJObject(RespJ.ToString());
            NewUsername = handler.GetJTokenValue(jToken, "username");
            bool.TryParse(handler.GetJTokenValue(jToken, "available"),out bool isAvailable);
            IsAvailable = isAvailable;
        }

        public string NewUsername { get; set; }

        public bool IsAvailable { get; set; }
    }

}
