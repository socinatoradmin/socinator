using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class CheckOffensiveCommentResponseHandler: IGResponseHandler
    {
        public CheckOffensiveCommentResponseHandler(IResponseParameter response)  : base(response)
        {
            if (!Success)
                return;

            var jToken = handler.ParseJsonToJObject(RespJ.ToString());
            bool.TryParse(handler.GetJTokenValue(jToken, "is_offensive"),out bool isOffensive);
            is_offensive = isOffensive;
        }

        public bool is_offensive { get; set; }
    }
}
