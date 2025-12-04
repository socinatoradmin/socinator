using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GramDominatorCore.Response
{
    public class CloseFriendsResponseHandler : IGResponseHandler
    {
        public string FailedMessage {  get; set; }
        public List<string> CloseFriendsList { get; set; }= new List<string>();
        public bool HasMore {  get; set; }
        public string MaxID {  get; set; }
        public CloseFriendsResponseHandler(IResponseParameter responseParameter,bool GetCloseFriendList=false):base(responseParameter)
        {
            try
            {
                if(responseParameter != null && responseParameter?.Response == "Already Have Close Friend")
                {
                    FailedMessage = responseParameter.Response;
                    Success = false;
                    return;
                }else if (GetCloseFriendList)
                {
                    var list = JsonConvert.DeserializeObject<List<string>>(responseParameter.Response);
                    foreach (var item in list)
                    {
                        GetCloseFriendListFromResponse(item);
                    }
                    Success = true;
                    HasMore = true;
                    return;
                }
                var jObject = handler.ParseJsonToJObject(responseParameter.Response);
                bool.TryParse(handler.GetJTokenValue(jObject, "data", "xdt_set_besties", 0, "friendship_status", "is_bestie"), out bool IsBestie);
                int.TryParse(handler.GetJTokenValue(jObject, "data", "xdt__settings__get_screen_dependencies", "string_server_values",0, "value"), out int friendCount);
                var text = handler.GetJTokenValue(jObject, "data", "xdt__settings__get_screen_dependencies", "string_server_values", 0, "server_value_id");
                Success = IsBestie || response?.Response == "Friends" || (friendCount >=0 && (!string.IsNullOrEmpty(text) && text.Contains("close_friends")));
            }
            catch { }
        }

        private void GetCloseFriendListFromResponse(string item)
        {
            try
            {
                var data = Regex.Split(item, "bk.action.i64.Const");
                foreach(var item2 in data)
                {
                    var userName = Utilities.GetBetween(item2, "), \"", "\"");
                    if(!string.IsNullOrEmpty(userName) && !CloseFriendsList.Any(x=>x==userName))
                        CloseFriendsList.Add(userName);
                }
            }
            catch { }
        }
    }
}
