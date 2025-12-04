using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using RedditDominatorCore.RDLibrary;
using RedditDominatorCore.RDModel;
using System;

namespace RedditDominatorCore.Response
{
    public class UserResponseHandler : RdResponseHandler
    {
        public UserResponseHandler(IResponseParameter response) : base(response)
        {
            RedditUser = new RedditUser();
            GetUserDetails(response);
            //GetAuthenticationDetails(response);
        }

        public RedditUser RedditUser { get; set; }

        private void GetAuthenticationDetails(IResponseParameter responseParameter)
        {
            try
            {
                RedditUser.PaginationParameter = new PaginationParameter();
                var jsonResponse = RdConstants.GetJsonPageResponse(responseParameter.Response);
                var jsonObject = JObject.Parse(jsonResponse);
                RedditUser.PaginationParameter.SessionTracker = jsonObject["user"]["sessionTracker"].ToString();
                RedditUser.PaginationParameter.Reddaid = jsonObject["user"]["reddaid"]?.ToString();
                var loidLoid = jsonObject["user"]["loid"]["loid"].ToString();
                var loidBlob = jsonObject["user"]["loid"]["blob"].ToString();
                var loidCreated = jsonObject["user"]["loid"]["loidCreated"].ToString();
                var loidVersion = jsonObject["user"]["loid"]["version"].ToString();
                RedditUser.PaginationParameter.Loid = loidLoid + "." + loidVersion + "." + loidCreated + "." + loidBlob;
                RedditUser.PaginationParameter.AccessToken = jsonObject["user"]["session"]["accessToken"].ToString();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void GetUserDetails(IResponseParameter responseParameter)
        {
            try
            {
                var handler = jsonHandler;
                var jObject = handler.ParseJsonToJObject(responseParameter.Response);
                if (jObject != null)
                {
                    var data = handler.GetJTokenOfJToken(jObject, "data");
                    RedditUser.AccountIcon = handler.GetJTokenValue(data, "icon_img");
                    int.TryParse(handler.GetJTokenValue(data, "comment_karma"), out int commentCount);
                    RedditUser.CommentKarma = commentCount;
                    int.TryParse(handler.GetJTokenValue(data, "created"), out int created);
                    RedditUser.Created = DominatorHouseCore.Utility.DateTimeUtilities.EpochToDateTimeUtc(created);
                    RedditUser.DisplayName = handler.GetJTokenValue(data, "name");
                    RedditUser.DisplayNamePrefixed = $"{RdConstants.NewRedditHomePageAPI}/{RedditUser.DisplayName}/";
                    RedditUser.HasUserProfile = !string.IsNullOrEmpty(RedditUser.AccountIcon);
                    RedditUser.Id = handler.GetJTokenValue(jObject, "kind") + "_" + handler.GetJTokenValue(data, "id");
                    bool.TryParse(handler.GetJTokenValue(data, "is_employee"), out bool is_employee);
                    RedditUser.IsEmployee = is_employee;
                    bool.TryParse(handler.GetJTokenValue(data, "is_friend"), out bool is_friend);
                    RedditUser.IsFollowing = is_friend;
                    bool.TryParse(handler.GetJTokenValue(data, "is_gold"), out bool is_gold);
                    RedditUser.IsGold = is_gold;
                    bool.TryParse(handler.GetJTokenValue(data, "is_mod"), out bool is_mod);
                    RedditUser.IsMod = is_mod;
                    bool.TryParse(handler.GetJTokenValue(data, "pref_show_snoovatar"), out bool pref_show_snoovatar);
                    RedditUser.PrefShowSnoovatar = pref_show_snoovatar;
                    int.TryParse(handler.GetJTokenValue(data, "link_karma"), out int krmaCount);
                    RedditUser.PostKarma = krmaCount;
                    bool.TryParse(handler.GetJTokenValue(data, "accept_pms"), out bool accept_pms);
                    RedditUser.IsNsfw = accept_pms;
                    RedditUser.Url = RedditUser.DisplayNamePrefixed;
                    RedditUser.Username = RedditUser.DisplayName;
                    RedditUser.UserId = RedditUser.Id;
                }
                else
                {
                    var ListKarmaCount= HtmlParseUtility.GetListInnerHtmlFromPartialTagName(responseParameter.Response, "span", "data-testid", "karma-number");
                    RedditUser.UserId=Utilities.GetBetween(responseParameter.Response, "author-id=\"", "\"");
                    RedditUser.Username=Utilities.GetBetween(responseParameter.Response, "author-name=\"", "\"");
                    if (string.IsNullOrEmpty(RedditUser.UserId))
                    {
                        RedditUser.UserId= Utilities.GetBetween(responseParameter.Response, "id&quot;:&quot;", "&quot");
                        RedditUser.Username = Utilities.GetBetween(responseParameter.Response, "name&quot;:&quot;", "&quot");
                    }
                    int.TryParse(ListKarmaCount[0].Replace(",", ""), out int postCount);
                    RedditUser.PostKarma = postCount;
                    if (ListKarmaCount.Count > 1)
                    {
                        int.TryParse(ListKarmaCount[1].Replace(",", ""), out int commentKarma);
                        RedditUser.CommentKarma = commentKarma;
                    }
                    RedditUser.IsFollowing = responseParameter.Response.Contains("is-followed");
                    RedditUser.Id = RedditUser.UserId;
                    var utcDateTime = Utilities.GetBetween(responseParameter.Response, "datetime=\"", "\"");
                    RedditUser.Created = !string.IsNullOrEmpty(utcDateTime) ? DateTimeOffset.Parse(utcDateTime).UtcDateTime:DateTime.MinValue;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}