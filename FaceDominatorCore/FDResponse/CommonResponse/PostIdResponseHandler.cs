using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using System;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class PostIdResponseHandler : FdResponseHandler
    {

        public string PostId = string.Empty;

        public PostIdResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            string postId = string.Empty;

            try
            {
                if (responseParameter.Response.Contains("ft_ent_identifier"))
                {
                    postId = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, FdConstants.EntIdentifierPostIdRegex);
                }
                else
                {
                    var postUrl = Regex.Matches(responseParameter.Response, "<meta http-equiv=(.*?)>", RegexOptions.Singleline)[0].Groups[1].ToString();

                    postId = postUrl.Contains("/videos/")
                        ? FdRegexUtility.FirstMatchExtractor(responseParameter.Response, FdConstants.VideoPostRegex)
                        : FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "video_id\":\"(.*?)\"");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

            postId = FdFunctions.GetIntegerOnlyString(postId);

            PostId = postId;
        }
    }
}
