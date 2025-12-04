using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;

namespace PinDominatorCore.Response
{
    public class UserConnectedWithMessageResponseHandler : PdResponseHandler
    {
        public List<string> LstUserConnectedWithMessage = new List<string>();
        public string Bookmark = string.Empty;

        public UserConnectedWithMessageResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonHand = new JsonHandler(response.Response);
            var jToken = jsonHand.GetJToken("resource_response", "data");

            var jTokenBookmark = jsonHand.GetJToken("resource_response");
            Bookmark = jsonHand.GetJTokenValue(jTokenBookmark, "bookmark");

            foreach (var token in jToken)
                try
                {

                    var userToken = jsonHand.GetJTokenOfJToken(token, "users")?.Children();
                    foreach (var subToken in userToken)
                    {
                        var userConnectedWithMessage = new UserConnectedWithMessage();
                        userConnectedWithMessage.Username = jsonHand.GetJTokenValue(subToken, "username");
                        LstUserConnectedWithMessage.Add(userConnectedWithMessage.Username);
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
        }

        public class UserConnectedWithMessage
        {
            public string Username { get; set; }
        }
    }
}
