using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse
{
    public class EventAndMessangerResponse : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public bool IsEventInviteSent { get; set; }

        public EventAndMessangerResponse(IResponseParameter responseParameter)
            : base(responseParameter)
        {

            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            if (responseParameter.Response.Contains("Invites Sent"))
            {
                IsEventInviteSent = true;
                Status = true;
            }
            else if (responseParameter.Response.Contains("Your message has been sent"))
            {
                IsEventInviteSent = true;
                Status = true;
            }
            else
            {
                IsEventInviteSent = false;
                Status = false;
            }
        }
    }
}
