using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorCore.Response.ProfileReponseHandlerPack
{
    public class UpdateProfileContactNumberResponseHandler : TdResponseHandler
    {
        public UpdateProfileContactNumberResponseHandler(IResponseParameter response, bool IsConfirmation = false) :
            base(response)
        {
            try
            {
                if (IsConfirmation)
                {
                    #region after successfully sending contact confirmation

                    var resp = HtmlAgilityHelper.MethodGetStringFromId(response.Response,
                        "settings-alert-close"); //settings-alert-close

                    Success = !resp.Contains("Incorrect code");

                    // just in case for if we get issue in client because of language
                    if (!Success)
                        new Exception().DebugLog("Update screen Name Response : " + resp);

                    #endregion
                }
                else
                {
                    #region before sending contact confirmation

                    var resp = HtmlAgilityHelper.MethodGetStringFromId(response.Response, "device-registration-form");

                    if (!string.IsNullOrEmpty(resp?.Trim()) &&
                        !resp.Contains("<input type=\"hidden\" name=\"device_type\" value=\"phone\">") ||
                        resp.Contains("Verification code"))
                        Success = true;
                    else
                        Success = false;

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Success = false;
                ex.DebugLog();
            }
        }
    }
}