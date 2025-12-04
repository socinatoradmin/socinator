using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.FacebookModels;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;
using System.Linq;

namespace FaceDominatorCore.FDResponse.EventsResponse
{
    public class EventCreaterResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public string EventId = string.Empty;

        public FdEvents FdEvents = new FdEvents();

        public string ErrorMsg = string.Empty;

        public EventCreaterResponseHandler(IResponseParameter responseParameter, EventCreaterManagerModel eventCreaterManagerModel)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            try
            {
                if (responseParameter.Response.Contains("errorDescription"))
                    return;

                FdEvents.EventId = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "eventID\":\"(.*?)\"");
                if (string.IsNullOrEmpty(FdEvents.EventId))
                    FdEvents.EventId = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "event_mall_(.*?)\"");
                if (string.IsNullOrEmpty(FdEvents.EventId))
                    FdEvents.EventId = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "event_id=(.*?)&");
                if (string.IsNullOrEmpty(FdEvents.EventId))
                {
                    var eventUrl = HtmlParseUtility.GetAttributeValueFromTagName(responseParameter.Response, "a", "aria-label", "Edit", "href");
                    FdEvents.EventId = FdRegexUtility.FirstMatchExtractor(eventUrl, "/events/edit/(.*?)/");
                }

                if (responseParameter.Response.Contains("Add a clear name for your event"))
                {
                    ErrorMsg = "Add a clear name for your event";
                    Status = false;
                    return;
                }
                if (responseParameter.Response.Contains("Event names need to use standard capitalisation"))
                {
                    ErrorMsg = "Event names need to use standard capitalisation";
                    Status = false;
                    return;
                }

                if (responseParameter.Response.Contains("event-create-dialog-confirm-button"))
                {
                    ErrorMsg = "Unknown";
                    Status = false;
                    return;
                }

                if (!string.IsNullOrEmpty(FdEvents.EventId))
                {
                    FdEvents.Id = eventCreaterManagerModel.Id;
                    FdEvents.EventName = eventCreaterManagerModel.EventName;
                    FdEvents.EventDescription = eventCreaterManagerModel.EventDescription;
                    FdEvents.EventStartDate = eventCreaterManagerModel.EventStartDate;
                    FdEvents.EventEndDate = eventCreaterManagerModel.EventEndDate;
                    FdEvents.EventLocation = eventCreaterManagerModel.EventLocation;
                    FdEvents.MediaPath = eventCreaterManagerModel.FbMultiMediaModel.MediaPaths.FirstOrDefault().MediaPath;
                    FdEvents.EventType = eventCreaterManagerModel.EventType;
                    FdEvents.IsShowGuestList = eventCreaterManagerModel.IsShowGuestList;
                    FdEvents.IsGuestCanInviteFriends = eventCreaterManagerModel.IsGuestCanInviteFriends;
                    FdEvents.IsPostMustApproved = eventCreaterManagerModel.IsPostMustApproved;
                    FdEvents.IsQuesOnMessanger = eventCreaterManagerModel.IsQuesOnMessanger;
                    Status = true;
                }
                else
                    Status = false;
            }
            catch (Exception ex)
            {
                Status = false;
                ex.DebugLog();
            }

        }
    }
}
