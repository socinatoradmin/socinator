using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;

namespace Tumblr.TumblrLibrary
{
    public class TumblrFunct
    {

        private readonly DominatorAccountModel dominatorAccount;

        private HttpHelper HttpHelper { get; set; }

        public TumblrFunct(DominatorAccountModel instaAcc)
        {
            try
            {
                this.dominatorAccount = instaAcc;
                this.HttpHelper = dominatorAccount.HttpHelper;
            }
            catch (Exception Ex)
            {
                GlobusLogHelper.log.Error(Ex.Message + Ex.StackTrace);
            }
        }



        //JsonElements jsonElements = new JsonElements()
        //{
        //    DeviceId = this._dominatorAccount.DeviceDetails.DeviceId,
        //    Csrftoken = this._Account.CsrfToken,
        //    Username = this._dominatorAccount.AccountBaseModel.UserName,
        //    Guid = this._dominatorAccount.DeviceDetails.PhoneId,
        //    Password = this._dominatorAccount.AccountBaseModel.Password,
        //    ContentType = "application/x-www-form-urlencoded; charset=UTF-8",
        //    LoginAttemptCount = "0",
        //};


        //IgRequestParameters RequestParameter =
        //    (IgRequestParameters)IgHttpHelper.GetRequestParameter();
        //RequestParameter.Body = jsonElements;
        //RequestParameter.Url = string.Format("accounts/login/");
        //string url = RequestParameter.GenerateUrl(string.Format("accounts/login/"));
        //url = Constants.ApiUrl + url;
        //RequestParameter.DontSign();
        //byte[] postData = RequestParameter.GenerateBody();

        //    return new LoginIgResponseHandler(_dominatorAccount.HttpHelper.PostRequest(url, postData));

    }
}
