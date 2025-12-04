using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse.InviterResponse
{
    public class GroupInviterResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool Status { get; set; }

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public string FailedReason { get; set; }

        public GroupInviterResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            if (responseParameter.Response.Contains("groupsMembershipAddedDialog"))
                Status = true;
            else if (responseParameter.Response.Contains("errorSummary"))
                FailedReason = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "errorSummary\":\"(.*?)\"");

        }
    }
}
