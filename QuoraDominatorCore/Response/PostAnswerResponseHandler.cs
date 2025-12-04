using DominatorHouseCore.Interfaces;

namespace QuoraDominatorCore.Response
{
    public class PostAnswerResponseHandler : QuoraResponseHandler
    {
        public PostAnswerResponseHandler(IResponseParameter response) : base(response)
        {
            if (RespJ == null)
            {
            }
        }
    }
}