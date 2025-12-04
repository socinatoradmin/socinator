using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.UpdateAccountResponseHandler
{
    public class UpdateProfilePicResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }

        public UpdateProfilePicResponseHandler(IResponseParameter responseParameter, EditProfileModel editProfile) : base(responseParameter)
        {
            if (string.IsNullOrEmpty(responseParameter.Response))
                return;

            var decodedResponse = FdFunctions.GetHtmlDecodedResponse(responseParameter.Response);
            var profilePicData = HtmlParseUtility.GetInnerHtmlFromTagName(
                                                            decodedResponse,
                                                            "div",
                                                            "aria-label",
                                                            "Profile picture actions");
            var profilePicUrl = HtmlParseUtility.GetAttributeValueFromTagName(profilePicData,
                                                                                "image",
                                                                                "preserveaspectratio",
                                                                                "xMidYMid slice",
                                                                                "xlink:href");
            if (!string.IsNullOrEmpty(profilePicUrl))
            {
                editProfile.ProfilePicPath = profilePicUrl;
                Status = true;
            }

        }


    }
}
