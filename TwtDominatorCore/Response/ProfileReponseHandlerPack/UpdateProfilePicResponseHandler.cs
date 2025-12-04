using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace TwtDominatorCore.Response.ProfileReponseHandlerPack
{
    public class UpdateProfilePicResponseHandler : TdResponseHandler
    {
        public string UpdatedProfilePicUrl = string.Empty;

        public UpdateProfilePicResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                Success = response.Response.Contains("profile_image_url");
                if (Success)
                    UpdatedProfilePicUrl =
                        Utilities.GetBetween(response.Response, "\"profile_image_url\":\"", "\"").Replace("\\", "");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}