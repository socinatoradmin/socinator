using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Requests;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.TmblrUtility;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class ResponseHandler
    {
        protected readonly JObject RespJ;
        protected readonly IResponseParameter Response;
        public readonly JsonParser parser = JsonParser.GetInstance;
        public ResponseHandler()
        {

        }

        public ResponseHandler(IResponseParameter responeParameter)
        {
            try
            {
                Response = responeParameter;
                if (Response.HasError)
                {
                    WebHelper.WebExceptionIssue errorMsgWebrequest;

                    try
                    {
                        errorMsgWebrequest = ((WebException)Response.Exception).GetErrorMsgWebrequest();
                    }
                    catch (Exception)
                    {
                        errorMsgWebrequest = new WebHelper.WebExceptionIssue
                        {
                            MessageLong = Response.Exception.Message
                        };
                    }

                    Success = false;
                    Issue = new TumblrIssue
                    {
                        Message = errorMsgWebrequest.MessageLong,
                        Error = TumblrError.FailedRequest
                    };
                }
                else
                {
                    try
                    {

                        if (!Response.Response.IsValidJson())
                        {
                            var decodedResponse = TumblrUtility.GetDecodedResponseOfJson(Response.Response);
                            RespJ = parser.ParseJsonToJObject(decodedResponse);
                        }
                        else
                            RespJ = parser.ParseJsonToJObject(Response.Response);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public TumblrIssue Issue { get; }

        public bool Success { get; protected set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Response.Response))
                return Response.Response;
            return string.Empty;
        }

        public class TumblrIssue
        {
            public bool ChangeStatus { get; set; }

            public TumblrError? Error { get; set; }

            public string Message { get; set; }
        }
    }
}