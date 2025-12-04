using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace TwtDominatorCore.Response.ProfileReponseHandlerPack
{
    public class UpdateProfileFullNameResponseHandler : TdResponseHandler
    {
        string FullNameFromResponse = string.Empty;
        public UpdateProfileFullNameResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                Success = response.Response.Contains("screen_name");
                FullNameFromResponse = Utilities.GetBetween(response.Response, "\"name\":\"", "\",");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}