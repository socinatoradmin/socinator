using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;

namespace TwtDominatorCore.Response.ProfileReponseHandlerPack
{
    public class UpdateProfileWebsiteUrlResponseHandler : TdResponseHandler
    {
        public UpdateProfileWebsiteUrlResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                Success = response.Response.Contains("display_url") && response.Response.Contains("expanded_url");
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }
    }
}