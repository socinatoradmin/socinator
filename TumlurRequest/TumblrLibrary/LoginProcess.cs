using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using Tumblr.TumblrRequest;
using Tumblr.TumblrResponseHandler;

namespace Tumblr.TumblrLibrary
{
    public class LoginProcess 
    {

        /// <summary>
        /// Log In with tumblr
        /// </summary>
        /// <param name="dominatorAccountModel"></param>
        public void LogIn(DominatorAccountModel dominatorAccountModel)
        {

            Tumblr.TumblrRequest.TumblrRequestParameter requestParameter = new Tumblr.TumblrRequest.TumblrRequestParameter();
            TumblrHttpHelper httpHelper = new TumblrHttpHelper(requestParameter);
            IResponseParameter responseparameter = httpHelper.GetRequest("https://www.tumblr.com/login");

            string Email = dominatorAccountModel.AccountBaseModel.UserName;
            string Password = dominatorAccountModel.AccountBaseModel.Password;
 
            string Login_url = "https://www.tumblr.com/login";
            IResponseParameter responseparameterLogin = httpHelper.GetRequest(Login_url);
            string form_key = Utilities.GetBetween(responseparameterLogin.Response, " id=\"tumblr_form_key\" content=\"", "\">");
            form_key = Uri.EscapeDataString(form_key);
            Email = Uri.EscapeDataString(Email);
            Password = Uri.EscapeDataString(Password);

            PostDataElement postDataElement = new PostDataElement()
            {
                DetermineEmail = Email,

                UserEmail = Email,

                UserPassword = Password,

                TumblelogName = string.Empty,

                UserAge = string.Empty,

                Context = "other",

                Version = "STANDARD",

                Follow = string.Empty,

                HttpReferer = "https%3A%2F%2Fwww.tumblr.com%2Flogout",

                FormKey = form_key,

                SeenSuggestion = "0",

                UsedSuggestion = "0",

                UsedAutoSuggestion =  "0",

                AboutTumblrSlide =  "",

                RandomUsernameSuggestions = "%5B%22VirtualPerfectionStudent%22%2C%22MaximumStudentTimeMachine%22%2C%22FamousCollectionCloud%22%2C%22SlowlyProfoundBouquet%22%2C%22BeardedCollectorWonderland%22%5D",

                Action = "signup_determine"
            };

            string serializedPostData =  JsonConvert.SerializeObject(postDataElement);

            byte[] byteArrayData = Encoding.UTF8.GetBytes(serializedPostData);

            LogInResponseHandler loggedInResponse =   new LogInResponseHandler(httpHelper.PostRequest(Login_url, byteArrayData));

            try
            {
                IResponseParameter responseParameterAfterLogIn = httpHelper.PostRequest(Login_url, byteArrayData);
            }
            catch (Exception ex)
            {
            }


        }

    }

  
}
