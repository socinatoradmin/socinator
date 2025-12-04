using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response.ProfileReponseHandlerPack
{
    public class UpdateProfileEmailResponseHandler : TdResponseHandler
    {
        public UpdateProfileEmailResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                var resp = HtmlAgilityHelper.getStringTextFromClassName(response.Response, "settings-alert");
                Success = resp.Contains("confirm your new email address.") || resp.Contains("settings-alert-close");

                // just in case for if we get issue in client because of language
                if (!Success)
                    new Exception().DebugLog("Update email Response : " + resp);
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }
    }
}