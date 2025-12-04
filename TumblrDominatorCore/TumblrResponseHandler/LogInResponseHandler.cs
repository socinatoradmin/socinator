using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using System;
using System.Text.RegularExpressions;
namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class LogInResponseHandler : MainPageResponseHandler
    {
        public LogInResponseHandler()
        { }
        public LogInResponseHandler(IResponseParameter responseParameter) : base(responseParameter)
        {
            try
            {

                Response = base.Response;
                if (Response.Response.StartsWith("[]"))
                {
                    Response.Response = Regex.Replace(Response.Response, "csrf :(.*)", "");
                }

                if (responseParameter.Response.Equals("[]") || responseParameter.Response.Contains("\"isLoggedIn\":true") || responseParameter.Response.Contains("{\"meta\":{\"status\":200,\"msg\":\"OK\"},\"response\":{\"user\":{\"name\""))
                {
                    Success = true;
                    IsLoggedIn = true;
                }
                else if (responseParameter.Response.Contains("\"cookieBootstrap\":{},\"routeSet\":\"main\"") && responseParameter.Response.Contains("\"isLoggedIn\":false"))
                {
                    Success = true;
                    IsLoggedIn = false;
                }
                else
                {
                    Success = false;
                    IsLoggedIn = false;
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }
        }

        public new IResponseParameter Response { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}