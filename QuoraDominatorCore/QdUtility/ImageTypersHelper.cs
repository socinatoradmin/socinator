using DominatorHouseCore.Models;
using ImageTypers;
using System;
using System.Collections.Generic;
using System.Threading;

namespace QuoraDominatorCore.QdUtility
{
    public class ImageTypersHelper
    {
        private readonly ImageTypersAPI _imgTypObj;
        private readonly Proxy _proxy;

        public ImageTypersHelper(string accessToken, Proxy proxy = null)
        {
            _imgTypObj = new ImageTypersAPI(accessToken);
            _proxy = proxy;
        }

        public bool IsAuthenticated()
        {
            try
            {
                MyBalance();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string MyBalance()
        {
            return _imgTypObj.account_balance();
        }


        public string SubmitSiteKey(string captchaUrl, string siteKey)
        {
            var d = new Dictionary<string, string> { { "page_url", captchaUrl }, { "sitekey", siteKey } };
            //if(!string.IsNullOrEmpty(_proxy.ToString()))
            // d.Add("proxy ", _proxy.ToString());

            return _imgTypObj.submit_recaptcha(d);
        }

        public string GetGResponseCaptcha(string captchaId, int delayInProgress = 7)
        {
            var isProgress = true;
            while (isProgress)
                try
                {
                    isProgress = _imgTypObj.in_progress(captchaId);

                    if (isProgress) Thread.Sleep(TimeSpan.FromSeconds(delayInProgress));
                }
                catch (Exception ex)
                {
                    if (!ex.ToString().Contains("IMAGE_TIMED_OUT")) continue;
                    break;
                }

            if (isProgress)
            {
                SetBadCaptcha(captchaId);
                return "";
            }

            var gResponseToken = _imgTypObj.retrieve_captcha(captchaId);

            return gResponseToken;
        }

        public void SetBadCaptcha(string captchaId)
        {
            try
            {
                _imgTypObj.set_captcha_bad(captchaId);
            }
            catch (Exception)
            {
            }
        }
    }
}
