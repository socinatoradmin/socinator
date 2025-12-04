using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDResponse.GroupsResponse
{


    public class UnjoinGroupResponseHandler : FdResponseHandler, IResponseHandler
    {

        public bool HasMoreResults { get; set; } = true;

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; }
            = new FdScraperResponseParameters();

        public bool IsUnjoinedGroup { get; set; }

        public UnjoinGroupResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            if (responseParameter.Response.Contains("Arbiter\",\"inform"))
            {
                IsUnjoinedGroup = true;
                Status = true;
            }


        }
    }
}
