using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Requests;
using PinDominatorCore.PDModel;
using PinDominatorCore.Utility;
using System;
using System.Net;

namespace PinDominatorCore.Response
{
    public abstract class PdResponseHandler
    {
        public JsonJArrayHandler handler => JsonJArrayHandler.GetInstance;
        protected PdResponseHandler(IResponseParameter response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }
            if (response.HasError)
            {
                try
                {
                    var type = response.Exception.GetType();
                    if (type.Name.Equals("WebException"))
                    {
                        var webExceptionIssue = ((WebException)response.Exception).GetErrorMsgWebrequest();
                        Issue = new PinterestIssue
                        {
                            Message = webExceptionIssue.MessageSolution,
                            Status = webExceptionIssue.MessageShort
                        };
                    }
                    else
                    {
                        Issue = new PinterestIssue
                        {
                            Message = response.Exception.Message
                        };
                    }
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }

                Success = false;
                return;
            }
            if (response.Response.Contains(".pinterest."))
            {
                Success = true;
            }

            if (response.Response.Contains("\"email\":")) Success = true;
        }

        public PinterestIssue Issue { get; set; }

        public bool Success { get; set; }
    }
}