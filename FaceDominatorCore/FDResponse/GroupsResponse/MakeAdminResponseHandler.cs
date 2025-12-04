
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.GroupsResponse
{
    public class MakeAdminResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }
        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool Status { get; set; }
        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; } = new FdScraperResponseParameters();

        public string ErrorValue = string.Empty;
        public MakeAdminResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {
            if (responseParameter.HasError)
                return;

            Status = true;
        }

        public MakeAdminResponseHandler(IResponseParameter responseParameter, string error)
            : base(responseParameter)
        {
            ErrorValue = error;
        }

        public int GetClickCount(string value, string userId)
        {
            int count = 0;
            try
            {
                JObject jObject = JObject.Parse(value.Replace("for (;;);", string.Empty));
                var htmlValue = jObject["payload"]["results_xhp"]["markup"]["__html"].ToString();
                string[] valueStrings = Regex.Split(htmlValue, "id=\"search_");

                foreach (var valueStringe in valueStrings)
                {
                    if (valueStringe.Contains(userId))
                        break;
                    count++;
                }

                if (valueStrings.Length == count)
                    count = 0;

            }
            catch (Exception)
            {

            }
            return count;
        }
    }
}

