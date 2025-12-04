using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FaceDominatorCore.FDResponse.ScrapersResponse
{
    public class GenderInfoResponseHandler : FdResponseHandler
    {
        public FacebookUser ObjFacebookUser { get; set; }

        public GenderInfoResponseHandler(IResponseParameter responseParameter, FacebookUser facebookUser)
            : base(responseParameter)
        {
            this.ObjFacebookUser = facebookUser;

            if (responseParameter.HasError)
                return;

            if (base.FbErrorDetails == null)
            {
                try
                {                    
                    var data = Regex.Replace(responseParameter.Response, "\\\\([^u])", "\\\\$1");
                    string decodedResponse = Regex.Replace(responseParameter.Response, "\\\\([^u])", "\\\\$1").Replace("\\", "");
                    decodedResponse = WebUtility.HtmlDecode(decodedResponse);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
