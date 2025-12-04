using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse.LikeCommentsResponse
{
    public class LikeFanpageResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool Status { get; set; }

        public string Error { get; set; } = string.Empty;

        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public LikeFanpageResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError || string.IsNullOrEmpty(responseParameter.Response))
                return;

            if (!responseParameter.Response.Contains("errorSummary") && !responseParameter.Response.Contains("Not Found"))
                Status = true;
            else if (responseParameter.Response.Contains("errorSummary") && responseParameter.Response.Contains("Page like feature limit"))
                Error = "You've been temporally blocked from using it";
            else if (!responseParameter.Response.Contains("page_profile_liked_button_test_id"))
                Error = "You've been temporally blocked from using it";
            else if (responseParameter.Response.Contains("errorSummary") && responseParameter.Response.Contains("Already Liked"))
                Status = true;
        }

        public LikeFanpageResponseHandler(IResponseParameter responseParameter, string Response)
            : base(responseParameter)
        {
            Status = !string.IsNullOrEmpty(Response);
        }
    }
}
