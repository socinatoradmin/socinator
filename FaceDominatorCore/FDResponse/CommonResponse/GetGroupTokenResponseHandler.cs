using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class GetGroupTokenResponseHandler : FdResponseHandler
    {
        public string Token { get; set; }

        public GetGroupTokenResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            Token = FdRegexUtility.FirstMatchExtractor(responseParameter.Response, "\"token\":\"(.*?)\"");

        }

    }
}
