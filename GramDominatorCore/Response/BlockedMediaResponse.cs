using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class BlockedMediaResponse : IGResponseHandler
    {
        public BlockedMediaResponse(IResponseParameter response)
            : base(response)
        {

        }
    }
}
