using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using System;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{

    public class FbUserIdResponseHandler : FdResponseHandler
    {

        public string UserId = string.Empty;

        public string FbDtsg = string.Empty;

        public FbUserIdResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            try
            {
                //Regex.Matches(familyNameResponse, ">(.*?)<", RegexOptions.Singleline)[0].Groups[1].ToString();
                //UserId = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "USER_ID\":\"(.*?)\"");
                UserId = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, FDLibrary.FdClassLibrary.FdConstants.EntityIdRegex);
                if (string.IsNullOrEmpty(UserId))
                    UserId = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "entity_id\":(.*?),");
                if (string.IsNullOrEmpty(UserId))
                    UserId = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "entity_id:(.*?),");

                if (Regex.Matches(responseParameter.Response, "fb_dtsg\" value=\"(.*?)\"", RegexOptions.Singleline).Count > 0)
                    FbDtsg = Uri.EscapeDataString(Regex.Matches(responseParameter.Response, "fb_dtsg\" value=\"(.*?)\"", RegexOptions.Singleline)[0].Groups[1].ToString());

                UserId = FdFunctions.GetIntegerOnlyString(UserId);

            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }
        }
    }
}

