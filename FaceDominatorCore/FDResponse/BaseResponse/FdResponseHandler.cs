using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Requests;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.Utility;
using System;
using System.Net;
using static DominatorHouseCore.Requests.WebHelper;

namespace FaceDominatorCore.FDResponse.BaseResponse
{
    public class FdResponseHandler
    {
        public readonly JsonParser parser = JsonParser.GetInstance;
        /// <summary>
        /// To get the briefy details about web errors.
        /// </summary>
        public FdErrorDetails FbErrorDetails { get; set; }

        /// <summary>
        /// To specify the response status.
        /// </summary>
        //public bool Success { get; set; }

        /// <summary>
        /// To check before process the response, whether it contains any web exceptions or not
        /// </summary>
        /// <param name="responseParameter">Response data</param>
        public FdResponseHandler(IResponseParameter responseParameter)
        {
            if (!responseParameter.HasError)
                return;

            WebExceptionIssue webExceptionIssue;
            // Success = false;

            try
            {
                webExceptionIssue = ((WebException)responseParameter.Exception).GetErrorMsgWebrequest();
            }
            catch (Exception)
            {
                webExceptionIssue = new WebExceptionIssue
                {
                    MessageLong = responseParameter.Exception?.Message ?? "LangKeyWebException".FromResourceDictionary()
                };
            }

            FbErrorDetails = new FdErrorDetails
            {
                Description = webExceptionIssue.MessageLong,
                FacebookErrors = FacebookErrors.Failed,
                IsStatusChangedRequired = false,
                Status = "LangKeyWebException".FromResourceDictionary()
            };

        }

    }
}