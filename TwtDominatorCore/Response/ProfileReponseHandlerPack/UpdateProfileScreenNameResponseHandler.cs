using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response.ProfileReponseHandlerPack
{
    public class UpdateProfileScreenNameResponseHandler : TdResponseHandler
    {
        public UpdateProfileScreenNameResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                try
                {
                    Success = response.Response.Contains("screen_name");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                //settings-alert-close
                //var resp = HtmlAgilityHelper.getStringInnerTextFromClassName(response.Response, "settings-alert");
                //Success = resp.Equals("Thanks, your settings have been saved.");

                // just in case for if we get issue in client because of language
                if (!Success)
                    new Exception().DebugLog("Update screen Name Response : " + response.Response);
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }
    }
}