using DominatorHouseCore.Models;
using ImageTypers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GramDominatorCore.CaptchaSolvers
{
    public class ImageTyperzHelper
    {
        private readonly ImageTypersAPI _imageTypersAPI;
        private readonly string _proxy = string.Empty;
        public ImageTyperzHelper(string accessToken,string proxy=null)
        {
            _imageTypersAPI = new ImageTypersAPI(accessToken);
            if (!string.IsNullOrEmpty(proxy))
                _proxy = proxy;
        }

        public bool IsAuthenticated(out string message)
        {
            GetBalance(out message);
            return message == "Success";
        }

        public string GetBalance(out string message)
        {
            message = "Success";

            try
            {
                return _imageTypersAPI.account_balance();
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("AUTHENTICATION_FAILED"))
                    message = "Please check your ImageTypers access token key";
                else if (ex.ToString().Contains("The remote server returned an error: (417) Expectation Failed."))
                    message = "Please check your system proxy settings";
                else
                    message = "Unknown error from Image Typers";
            }

            return "$0";
        }


        public string SubmitSiteKey(string captchaUrl, string siteKey)
        {
            var dictSubmitRecaptchaDetails = new Dictionary<string, string>
            {
                {"page_url", captchaUrl},
                { "sitekey", siteKey},
                { "type", "1"}
            };
            if(!string.IsNullOrEmpty(_proxy))
                dictSubmitRecaptchaDetails.Add("proxy ", _proxy.ToString());
            return _imageTypersAPI.submit_recaptcha(dictSubmitRecaptchaDetails);
        }

        public string SolveCaptcha(string imageUrl, bool caseSensitive = false)
        {
            var captchaText = string.Empty;

            try
            {
                captchaText = _imageTypersAPI.solve_captcha(imageUrl, caseSensitive);
            }
            catch (Exception)
            {

            }

            return captchaText;
        }

        public string GetGResponseCaptcha(string captchaId, int delayInProgress = 10)
        {
            var isProgress = true;
            while (isProgress)
            {
                try
                {
                    isProgress = _imageTypersAPI.in_progress(captchaId);

                    if (isProgress)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(delayInProgress));
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.ToString().Contains("IMAGE_TIMED_OUT")) continue;
                    //GlobusLogHelper.log.Info("Got Exception 'IMAGE_TIMED_OUT' while running function in_progress, Breaking the progress loop now");
                    break;
                }
            }

            if (isProgress)
            {
                //GlobusLogHelper.log.Info("IMAGE_TIMED_OUT : Going to claim for Bad Captcha");
                SetBadCaptcha(captchaId);
                return "";
            }

            var gResponseToken = _imageTypersAPI.retrieve_captcha(captchaId);

            //GlobusLogHelper.log.Info("Got g-Response token - " + gResponseToken);
            return gResponseToken;
        }

        public void SetBadCaptcha(string captchaId)
        {
            try
            {
                var setBadCaptchaResponse = _imageTypersAPI.set_captcha_bad(captchaId);

                // GlobusLogHelper.log.Info(setBadCaptchaResponse.Contains("SUCCESS")
                //     ? "Applied for Refund Successfully with ImageTypers"
                //     : "Error => Got Wrong Response, Could Not Apply for Refund");

                //lock (CaptchaResolverUtility.LockerForBadCaptcha)
                //{
                //    GlobusFileHelper.AppendStringToTextfileNewLine($"{DateTime.Now}<:>{captchaId}", GlobalClass.BadCaptchaDetails);
                //}
            }
            catch (Exception)
            {
                // ex.DebugLog();
            }
        }

    }
}
