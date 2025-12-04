using System;
using System.Collections.Generic;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class FindMessageResponseHandler : PdResponseHandler
    {
        public Dictionary<KeyValuePair<string, string>, string> ListUsersAndText =
            new Dictionary<KeyValuePair<string, string>, string>();

        public List<KeyValuePair<string, string>> SenderAndReceiver = new List<KeyValuePair<string, string>>();
        
        public FindMessageResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
            var jsonToken = jsonHand.GetJToken("resource_response", "data");

            foreach (var token in jsonToken)
                try
                {
                    var userNameReplyed = jsonHand.GetJTokenValue(token, "users", 0, "username");
                    if (!string.IsNullOrEmpty(userNameReplyed) && userNameReplyed.Contains("pinterestindia"))
                        continue;
                    var sender = jsonHand.GetJTokenValue(token, "last_message", "sender", "username");
                    if (userNameReplyed.Equals(sender))
                        userNameReplyed = jsonHand.GetJTokenValue(token, "users", 1, "username");
                    SenderAndReceiver.Add(new KeyValuePair<string, string>(userNameReplyed, sender));
                    var text = jsonHand.GetJTokenValue(token, "last_message", "text");
                    ListUsersAndText.Add(new KeyValuePair<string, string>(userNameReplyed, sender), text);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            BookMark = jsonHand.GetElementValue("resource", "options", "bookmarks", 0);
            HasMoreResults = !BookMark.Contains("end");
        }

        public bool HasMoreResults { get; set; }

        public string BookMark { get; set; }
    }
}