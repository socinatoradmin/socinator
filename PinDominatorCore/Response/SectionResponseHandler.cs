using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PinDominatorCore.Response
{
    public class SectionResponseHandler: PdResponseHandler
    {
        public SectionResponseHandler(IResponseParameter response) : base(response)
        {
                if (string.IsNullOrEmpty(response.Response))
                {
                    Success = false;
                    return;
                }
                try
                {
                    var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
                    if (jsonHand.GetJToken("resource_response", "error").HasValues)
                    {
                        var message = jsonHand.GetElementValue("resource_response", "error", "message_detail");
                        Issue = new PinterestIssue
                        {
                            Message = string.IsNullOrEmpty(message) ? jsonHand.GetElementValue("resource_response", "error", "message") : message
                        };
                        Success = false;
                        return;
                    }
                    SectionId = jsonHand.GetElementValue("resource_response", "data", "id");
                    SectionTitle = jsonHand.GetElementValue("resource_response", "data", "title");
                    BoardUrl = jsonHand.GetElementValue("resource_response", "data", "board", "url");
                    BoardId = jsonHand.GetElementValue("resource_response", "data", "board", "id");
                    BoardName = jsonHand.GetElementValue("resource_response", "data", "board", "name");
                    BoardDescription = jsonHand.GetElementValue("resource_response", "data", "board", "description");
                    BoardId = string.IsNullOrEmpty(BoardId) ? jsonHand.GetElementValue("resource", "options", "board_id") : BoardId;
                    BoardUrl = string.IsNullOrEmpty(BoardUrl) ? jsonHand.GetElementValue("client_context", "visible_url") : BoardUrl;
                    Success = true;
                }
                catch (Exception e)
                {
                    Success = false;
                    e.DebugLog();
                }
        }
        public string SectionTitle { get; set; }
        public string SectionId { get; set; }
        public string BoardName { get; set; }
        public string BoardDescription { get; set; }
        public string BoardId { get; set; }
        public string BoardUrl { get; set; }

    }
}
