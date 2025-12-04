using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDUtility;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace GramDominatorCore.GDModel
{
    [Localizable(false)]
    public class UserFeedIgResponseHandler : GDLibrary.Response.IGResponseHandler
    {
        public UserFeedIgResponseHandler(IResponseParameter response, bool Http = false)
            : base(response)
        {
            if (!Success)
                return;
            if (!Http)
            {
                var dataList = JsonConvert.DeserializeObject<List<string>>(response.Response);
                if (dataList.Count > 0)
                {
                    foreach (var item in dataList)
                    {
                        if (string.IsNullOrEmpty(item) || item.Contains("<!DOCTYPE")) continue;
                        GetFeedInfo(item);
                    }
                    HasMoreResults = false;
                    return;
                }
                else
                {
                    HasMoreResults = false;
                    return;
                }
            }
            else
            {
                GetUserInfo(response.Response);
            }
        }
        public UserFeedIgResponseHandler()
        {

        }
        public UserFeedIgResponseHandler GetUserInfo(string item)
        {
            try
            {
                var obj = handler.ParseJsonToJObject(item);
                MaxId = handler.GetJTokenValue(obj, "next_max_id");
                HasMoreResults = !string.IsNullOrEmpty(MaxId);
                var DataArray = handler.GetJArrayElement(handler.GetJTokenValue(obj, "items"));
                if (DataArray != null && DataArray.HasValues)
                {
                    Items.AddRange(DataArray.GetImageData());
                }
            }
            catch { }
            return this;
        }
        private void GetFeedInfo(string item)
        {
            try
            {
                var IsUnique = false;
                var RespJ = handler.ParseJsonToJObject(item);
                var DataArray = handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "items"));
                if (DataArray is null || DataArray.Count == 0)
                {
                    IsUnique = true;
                    DataArray = handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "data", "xdt_api__v1__feed__user_timeline_graphql_connection", "edges"));
                }
                if (DataArray != null && DataArray.Count > 0)
                {
                    Items.AddRange(DataArray.GetImageData(IsUnique));
                }
            }
            catch (System.Exception)
            {

            }
        }

        public bool HasMoreResults { get; set; }

        public List<InstagramPost> Items { get; set; } = new List<InstagramPost>();

        public string MaxId { get; set; }
    }
}
