using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class SyncResponse : IGResponseHandler
    {
        public SyncResponse(IResponseParameter response) : base(response)
        {
        }
    }
}
