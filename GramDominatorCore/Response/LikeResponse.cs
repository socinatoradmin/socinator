using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class LikeResponse : IGResponseHandler
    {
        public LikeResponse(IResponseParameter response) : base(response)
        {
            if (Success)
            {
                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    Success = true;
                    return;
                }
                if(response.Response.Contains("{\"data\":{\"xdt_api__v1__media__media_id__like\"")
                    || response.Response.Contains("{\"data\":{\"xdt_mark_media_like\""))
                {
                    Success = true;
                    return;
                }
            }
        }
        public bool Blocked { get; set; }
        public bool NotClicked { get; set; }
    }
}
