using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using ImageTypers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PinDominatorCore.PDUtility
{
    public class ImageTypersHelper
    {
        private readonly ImageTypersAPI _imgTypObj;

        public ImageTypersHelper(string accessToken)
        {
            _imgTypObj = new ImageTypersAPI(accessToken);
        }


        public string MyBalance() => _imgTypObj.account_balance();


        public string SubmitSiteKey(string captchaUrl, string siteKey)
        {
            var d = new Dictionary<string, string> { { "page_url", captchaUrl }, { "sitekey", siteKey } };
            System.Net.ServicePointManager.Expect100Continue = false;

            return _imgTypObj.submit_recaptcha(d);
        }

        public string GetGResponseCaptcha(string captchaId, string userName, int delayInProgress = 2)
        {
            var isProgress = true;
            while (isProgress)
            {
                try
                {
                    isProgress = _imgTypObj.in_progress(captchaId);

                    if (isProgress)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(delayInProgress));
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.ToString().Contains("IMAGE_TIMED_OUT")) continue;
                    GlobusLogHelper.log.Info(Log.CustomMessage, userName, SocialNetworks.Pinterest, "Captcha",
                        "Got Exception 'IMAGE_TIMED_OUT' while running function in_progress, Breaking the progress loop now");
                    break;
                }
            }

            if (isProgress)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, userName, SocialNetworks.Pinterest, "Captcha",
                    "IMAGE_TIMED_OUT : Going to claim for Bad Captcha");
                SetBadCaptcha(captchaId, userName);
                return "";
            }

            var gResponseToken = _imgTypObj.retrieve_captcha(captchaId);

            GlobusLogHelper.log.Info(Log.CustomMessage, userName, SocialNetworks.Pinterest, "Captcha",
                "Captcha Solved");
            return gResponseToken;
        }

        public void SetBadCaptcha(string captchaId, string userName)
        {
            try
            {
                var setBadCaptchaResponse = _imgTypObj.set_captcha_bad(captchaId);
                
                GlobusLogHelper.log.Info(Log.CustomMessage, userName, SocialNetworks.Pinterest, "Captcha",
                    setBadCaptchaResponse.Contains("SUCCESS")
                    ? "Applied for Refund Successfully with ImageTypers"
                    : "Error => Got Wrong Response, Could Not Apply for Refund");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
