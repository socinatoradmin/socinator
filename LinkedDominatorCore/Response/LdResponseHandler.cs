using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Request;
using DominatorHouseCore.Requests;
using DominatorHouseCore.Utility;
using LinkedDominatorCore.LDUtility;
using LinkedDominatorCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace LinkedDominatorCore.Response
{
    // ReSharper disable once InconsistentNaming
    public abstract class LdResponseHandler
    {
        protected readonly IResponseParameter Response;
        public JsonJArrayHandler handler => JsonJArrayHandler.GetInstance;
        protected JObject RespJ;

        public LdResponseHandler()
        {
        }

        protected LdResponseHandler(IResponseParameter response)
        {
            Response = response;
            if (response.HasError)
            {
                try
                {
                    ((WebException) response.Exception).GetErrorMsgWebrequest();
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }

                Success = false;
            }
            else
            {
                if (response.Response.IsValidJson())
                {
                    try
                    {
                        RespJ = JObject.Parse(response.Response);
                        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                        // this might throw exception when not present
                        Success = RespJ["login_result"]?.ToString() != null
                            || RespJ != null;
                    }
                    catch (Exception)
                    {
                        //ex.DebugLog();
                        Success = true;
                    }
                }

                else if (!string.IsNullOrEmpty(response.Response))
                {
                    Success = true;
                }
            }
        }

        protected LdResponseHandler(string response)
        {
            if (response.Contains("<!DOCTYPE html>"))
            {
                Response = new ResponseParameter {Response = response};
                Success = true;
            }
            else
            {
                Success = false;
            }
        }

        public bool Success { get; protected set; }
        public int InValidLinkedInUserCount { get; protected set; }
        protected void SetRespJ(string startString, string endstring)
        {
            var ldDataHelper= LdDataHelper.GetInstance;
            var pageResponse =
                ldDataHelper.GetJsonDataFromPageSource(Response.Response, startString, endstring);
            if (pageResponse.Equals(startString))
                pageResponse =
                    ldDataHelper.GetJsonDataFromPageSource(WebUtility.HtmlDecode(Response.Response),
                        startString, endstring);
            RespJ = JObject.Parse(pageResponse);
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Response.Response))
                return Response.Response;
            return string.Empty;
        }
    }
}