using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.GDModel
{
    public class DeleteMediaIgResponseHandler : IGResponseHandler
    {
        public DeleteMediaIgResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            var obj = handler.ParseJsonToJObject(response?.Response);
            bool.TryParse(handler.GetJTokenValue(obj, "did_delete"), out bool did_delete);
            DidDelete = did_delete;
            if (!Success)
            {
                bool.TryParse(handler.GetJTokenValue(obj, "status"), out bool status);
                Success = status;
            }
        }

        public bool DidDelete { get; set; }
    }
}
