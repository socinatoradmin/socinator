using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using imagetyperzapi;
using System;
using System.Collections.Generic;
using System.Threading;

namespace YoutubeDominatorCore.YDUtility
{
    public class ImageTypersHelper
    {
        private readonly ImagetyperzAPI _imgTypObj;

        public ImageTypersHelper(string accessToken)
        {
            _imgTypObj = new ImagetyperzAPI(accessToken);
        }


        public string MyBalance() => _imgTypObj.account_balance();


        public string SubmitSiteKey(string captchaUrl, string siteKey)
        {
            var d = new Dictionary<string, string> { { "page_url", captchaUrl }, { "sitekey", siteKey } };
            System.Net.ServicePointManager.Expect100Continue = false;

            return _imgTypObj.submit_recaptcha(captchaUrl, siteKey);
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
                    GlobusLogHelper.log.Info(Log.CustomMessage, userName, SocialNetworks.YouTube, "Captcha",
                        "Got Exception 'IMAGE_TIMED_OUT' while running function in_progress, Breaking the progress loop now");
                    break;
                }
            }

            if (isProgress)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, userName, SocialNetworks.YouTube, "Captcha",
                    "IMAGE_TIMED_OUT : Going to claim for Bad Captcha");
                SetBadCaptcha(captchaId, userName);
                return "";
            }

            var gResponseToken = _imgTypObj.retrieve_captcha(captchaId);

            GlobusLogHelper.log.Info(Log.CustomMessage, userName, SocialNetworks.YouTube, "Captcha",
                "Captcha Solved");
            return gResponseToken;
        }

        public void SetBadCaptcha(string captchaId, string userName)
        {
            try
            {
                var setBadCaptchaResponse = _imgTypObj.set_captcha_bad(captchaId);

                GlobusLogHelper.log.Info(Log.CustomMessage, userName, SocialNetworks.YouTube, "Captcha",
                    setBadCaptchaResponse.Contains("SUCCESS")
                    ? "Applied for Refund Successfully with ImageTypers"
                    : "Error => Got Wrong Response, Could Not Apply for Refund");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string submit_image(string captcha_ImagePath, string userName)
        {
            string captcha_text = string.Empty;
            try
            {
                captcha_text = _imgTypObj.solve_captcha(captcha_ImagePath);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, userName, SocialNetworks.YouTube, "Captcha",
                                        ex.Message);
            }
            return captcha_text;
        }
    }

}
