using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace FaceDominatorCore.FDResponse.MessagesResponse
{
    public class IncommingPageMessageResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        //  public List<FdMessageDetails> IncommingMessageList { get; set; }


        public IncommingPageMessageResponseHandler
                (DominatorAccountModel account, IResponseParameter responseParameter, ref string timeStampPrecise, string pageId)
                : base(responseParameter)
        {
            ObjFdScraperResponseParameters.MessageDetailsList = new List<FdMessageDetails>();
            string previousTimeStampPrecise = timeStampPrecise;
            string jsonFormate = "{\"o0\":" + Utilities.GetBetween(responseParameter.Response, "{\"o0\":", "}}}}}") + "}}}}}";
            var jsonResponse = JObject.Parse(jsonFormate);
            var totalConversions = jsonResponse["o0"]["data"]["viewer"]["message_threads"]["nodes"];

            foreach (var Conversion in totalConversions)
            {
                FdMessageDetails fdMessageDetails = new FdMessageDetails();
                timeStampPrecise = Conversion["last_message"]["nodes"][0]["timestamp_precise"].ToString();
                fdMessageDetails.MessageSenderId = Conversion["last_message"]["nodes"][0]["message_sender"]["messaging_actor"]["id"].ToString();
                fdMessageDetails.Message = Conversion["last_message"]["nodes"][0]["snippet"].ToString();
                fdMessageDetails.MessageSenderName = Conversion["all_participants"]["edges"][0]["node"]["messaging_actor"]["name"].ToString();
                string senderType = Conversion["all_participants"]["edges"][0]["node"]["messaging_actor"]["__typename"].ToString();

                if (fdMessageDetails.MessageSenderId != account.AccountBaseModel.UserId
                     && fdMessageDetails.MessageSenderId != pageId
                         && senderType == "User"
                              && previousTimeStampPrecise != timeStampPrecise)
                {
                    PageletData = timeStampPrecise;
                    ObjFdScraperResponseParameters.MessageDetailsList.Add(fdMessageDetails);
                }
            }

            if (previousTimeStampPrecise == timeStampPrecise)
                HasMoreResults = false;


            if (ObjFdScraperResponseParameters.MessageDetailsList.Count > 0)
            {
                Status = true;
                HasMoreResults = true;
            }
        }
    }
}
