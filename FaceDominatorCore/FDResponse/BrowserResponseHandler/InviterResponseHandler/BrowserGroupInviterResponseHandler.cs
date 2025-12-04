using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.InviterResponseHandler
{
    public class BrowserGroupInviterResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool IsInvited { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool HasMoreResults { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
                                                                = new FdScraperResponseParameters();

        public BrowserGroupInviterResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            if (responseParameter.Response.Contains("groupsMembershipAddedDialog") || responseParameter.Response.Contains("group_add_member"))
            {
                IsInvited = true;
                Status = true;
            }
            else if (responseParameter.Response.Contains("errorSummary"))
            {
                ObjFdScraperResponseParameters.FailedReason = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "errorSummary\":\"(.*?)\"");
            }
            else
            {
                ObjFdScraperResponseParameters.FailedReason = responseParameter.Response;
            }
        }
    }
}
