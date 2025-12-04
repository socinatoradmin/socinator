using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using System;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    class PostDeleteNodeIdResponseHandler : FdResponseHandler
    {
        public string NodeId = string.Empty;
        public PostDeleteNodeIdResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            if (FbErrorDetails == null)
            {
                try
                {
                    NodeId = FdRegexUtility.GetMatchCount(responseParameter.Response, "nodeID:\"(.*?)\"") > 2 ? FdRegexUtility.GetNthMatch(responseParameter.Response, "nodeID:\"(.*?)\"", 1) : string.Empty;
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }
            }
        }
    }
}
