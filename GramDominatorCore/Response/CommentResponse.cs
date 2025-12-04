using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;

namespace GramDominatorCore.Response
{
    public class CommentResponse : IGResponseHandler
    {
       
        public CommentResponse(IResponseParameter response)  : base(response)
        {
            if (Success)
            {
                if (response.Response.Contains("<!DOCTYPE html>"))
                {
                    Success = true;
                    return;
                }
                if(response.Response.Contains("{\"data\":{\"xdt_web__comments__media_id__add_queryable\"")
                    || response.Response.Contains("\"status\":\"ok\""))
                {
                    Success = true;
                    return;
                }
            }
        }
    }

}
