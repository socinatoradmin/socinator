using System;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;

namespace TwtDominatorCore.Response.ProfileReponseHandlerPack
{
    public class UpdateProfileBioResponseHandler : TdResponseHandler
    {
        public UpdateProfileBioResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                PageResponse = response.Response;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string PageResponse { get; set; } = string.Empty;
    }
}