using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse.InviterResponse
{

    public class PageInviterResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; } = true;
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public PageInviterResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError || string.IsNullOrEmpty(responseParameter.Response))
                return;

            Status = !responseParameter.Response.Contains("error");
        }
    }
}
