using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse.LikeCommentsResponse
{
    public class CommentLikerResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; } = true;
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public CommentLikerResponseHandler(IResponseParameter responseParameter, ReactionType reactionType)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            if (responseParameter.Response.Contains($"aria-label=\"Remove {reactionType.GetDescriptionAttr()}\"")
                || responseParameter.Response.Contains($"aria-label=\"Change {reactionType.GetDescriptionAttr()}"))
                Status = true;
            if (responseParameter.Response.Contains("aria-pressed=\"true\""))
                Status = true;

            if (responseParameter.Response.StartsWith("Already Liked Comment"))
            {
                Status = false;
                ObjFdScraperResponseParameters.FailedReason = "Already Liked Comment";
            }

            if (responseParameter.Response.Contains("errorSummary"))
            {
                Status = false;
                ObjFdScraperResponseParameters.FailedReason = "Unknown ERROR";
            }
        }
    }
}
