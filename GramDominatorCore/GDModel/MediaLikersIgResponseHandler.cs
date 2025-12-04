using System;
using System.ComponentModel;
using System.Linq;
using DominatorHouseCore.Interfaces;
using GramDominatorCore.Response;
using Newtonsoft.Json.Linq;
using DominatorHouseCore.Utility;
using System.Collections.Generic;

namespace GramDominatorCore.GDModel
{
    [Localizable(false)]
    public class MediaLikersIgResponseHandler : MediaInteractionIgResponseHandler
    {
        public MediaLikersIgResponseHandler(IResponseParameter response)
            : base(response)
        {
            if (!Success)
                return;
            if (response.Response.Contains("<!DOCTYPE html>"))
            {
                Success = true;
                UserList = new System.Collections.Generic.List<InstagramUser>();
                if (response.Response.Contains("end_cursor"))
                    MaxId = Utilities.GetBetween(response.Response, "end_cursor\":\"", "\"");
                GetUsersList(response.Response,UserList);
                return;
            }
            int.TryParse(handler.GetJTokenValue(RespJ, "user_count"), out int count);
            InteractionCount = count;
            UserList = handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "users")).Select(user =>
            {
                InstagramUser instagramUser =
                    new InstagramUser(handler.GetJTokenValue(user,"pk"),handler.GetJTokenValue(user, "username"))
                    {
                        ProfilePicUrl = handler.GetJTokenValue(user, "profile_pic_url"),
                        FullName = handler.GetJTokenValue(user, "full_name")
                    };
                bool.TryParse(handler.GetJTokenValue(user, "is_verified"), out bool num1);
                instagramUser.IsVerified = num1;
                bool.TryParse(handler.GetJTokenValue(user, "is_private"), out bool num2);
                bool.TryParse(handler.GetJTokenValue(user, "friendship_status", "following"), out bool num3);
                instagramUser.IsPrivate = num2 && !num3;
                Likers.Add(instagramUser.Pk);
                return instagramUser;
            }).ToList();
            try
            {
                MaxId = handler.GetJTokenValue(RespJ, "next_max_id");
                if (string.IsNullOrEmpty(MaxId))
                    return;
            }
            catch (Exception)
            {
                
            }
            HasMoreResults = true;
        }

        private void GetUsersList(string response, List<InstagramUser> userList)
        {
            var decodedResponse = Constants.GetDecodedResponse(response);
            var lst = HtmlParseUtility.GetListInnerHtmlFromPartialTagName(decodedResponse, "div", "class", "x1dm5mii x16mil14 xiojian x1yutycm x1lliihq x193iq5w xh8yej3");
            foreach(var user in lst)
            {
                InstagramUser usr = new InstagramUser();
                usr.Username = System.Text.RegularExpressions.Regex.Match(user, "href=\"(.*?)\"").Groups[1].Value;
                usr.ProfilePicUrl = System.Text.RegularExpressions.Regex.Match(user, "src=\"(.*?)\"").Groups[1].Value;
                usr.FullName = HtmlParseUtility.GetInnerTextFromTagName(user, "span", "class", "x1lliihq x193iq5w x6ikm8r x10wlt62 xlyipyv xuxw1ft");
                userList.Add(usr);
            }
        }
    }
}
