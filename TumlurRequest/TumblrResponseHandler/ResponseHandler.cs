using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Requests;
using Newtonsoft.Json.Linq;
using NLog;
using Tumblr.Enum;

namespace Tumblr.TumblrResponseHandler
{
    public class ResponseHandler
    {

        protected readonly ILogger Logger = LogManager.GetLogger(nameof(TumblrResponseHandler));
        protected readonly JObject RespJ;
        protected readonly IResponseParameter response;

        public ResponseHandler(IResponseParameter responeParameter)
        {
            this.response = responeParameter;

            if (response.HasError)
            {

                WebHelper.WebExceptionIssue errorMsgWebrequest = null;


                try
                {
                    errorMsgWebrequest = ((WebException)response.Exception).GetErrorMsgWebrequest();
                }
                catch (Exception e)
                {
                    errorMsgWebrequest = new WebHelper.WebExceptionIssue()
                    {
                        MessageLong = response.Exception.Message
                    };
                }

                this.Success = false;
                this.Issue = new TumblrIssue()
                {
                    Message = errorMsgWebrequest.MessageLong,
                    Error = TumblrError.FailedRequest
                };
            }
        }

        public TumblrIssue Issue { get; }

        public bool Success { get; protected set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(response.Response))
                return response.Response;
            return string.Empty;
        }
        public class TumblrIssue
        {
            public bool ChangeStatus { get; set; }

            public TumblrError? Error { get; set; }

            public string Message { get; set; }

            public string Status { get; set; }
        }

    }
}
