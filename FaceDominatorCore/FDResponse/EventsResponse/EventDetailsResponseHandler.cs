using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;

namespace FaceDominatorCore.FDResponse.EventsResponse
{

    public class EventDetailsResponseHandler : FdResponseHandler, IResponseHandler
    {

        public FdEvents FdEvents { get; set; } = new FdEvents();

        public bool HasMoreResults { get; set; } = true;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public EventDetailsResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            try
            {

                FdEvents.EventName = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, FdConstants.PageTitleRegx);
                FdEvents.EventId = Utilities.GetBetween(responseParameter.Response, "\"eventid\":", ",");

                if (string.IsNullOrEmpty(FdEvents.EventId))
                    FdEvents.EventId = Utilities.GetBetween(responseParameter.Response, "eventID:\"", "\"");

                FdEvents.EventId = FDLibrary.FdFunctions.FdFunctions.GetIntegerOnlyString(FdEvents.EventId);

                FdEvents.OwnerId = "";


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

    }
}
