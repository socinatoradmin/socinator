using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class BoardResponse : PdResponseHandler
    {
        public BoardResponse(IResponseParameter response) : base(response)
        {
            try
            {
                if (string.IsNullOrEmpty(response.Response))
                {
                    Success = false;
                    return;
                }

                var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
                try
                {
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
                }
                catch (Exception e)
                {
                    Success = false;
                    e.DebugLog();
                }

                try
                {
                    BoardId = jsonHand.GetElementValue("resource_response", "data", "id");
                    Url = jsonHand.GetElementValue("resource_response", "data", "url");

                    if (string.IsNullOrEmpty(BoardId))
                        BoardId = jsonHand.GetElementValue("resource", "options", "board_id");
                    if (string.IsNullOrEmpty(Url))
                        Url = jsonHand.GetElementValue("client_context", "visible_url");
                }
                catch
                {
                    // ignored
                }

                if (string.IsNullOrEmpty(BoardId)) Success = false;
            }
            catch (Exception ex)
            {
                Success = false;
                ex.DebugLog();
            }
        }

        public string Url { get; set; }

        public string BoardName { get; set; }

        public string BoardDescription { get; set; }

        public string BoardId { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }
        public List<PinterestBoardSections> BoardSections { get; set; } = new List<PinterestBoardSections>();
    }
}