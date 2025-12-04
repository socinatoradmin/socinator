using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDModel;
using PinDominatorCore.PDUtility;

namespace PinDominatorCore.Response
{
    public class PinLikersResponseHandler : PdResponseHandler
    {
        public PinLikersResponseHandler(IResponseParameter response) : base(response)
        {
            if (string.IsNullOrEmpty(response.Response))
            {
                Success = false;
                return;
            }

            var jsonHand = new DominatorHouseCore.Utility.JsonHandler(response.Response);
            var jsonToken = jsonHand.GetJToken("resource_response", "data");

            foreach (var token in jsonToken)
            {
                var user = new PinterestUser
                {
                    Username = jsonHand.GetJTokenValue(token, "user", "username"),
                    UserId = jsonHand.GetJTokenValue(token, "user", "id"),
                    FullName = jsonHand.GetJTokenValue(token, "user", "full_name"),
                    ProfilePicUrl = jsonHand.GetJTokenValue(token, "user", "image_medium_url")
                };

                UserList.Add(user);
            }

            BookMark = Utilities.GetBetween(response.Response, PdConstants.BookMark, "\"").Replace("=", "");
            HasMoreResult = !BookMark.Contains("end");
        }

        public bool HasMoreResult { get; set; }
        public string BookMark { get; set; }
        public List<PinterestUser> UserList { get; } = new List<PinterestUser>();
    }
}