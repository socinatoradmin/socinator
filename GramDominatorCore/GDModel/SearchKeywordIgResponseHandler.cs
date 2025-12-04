using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GramDominatorCore.GDModel
{
    public class SearchKeywordIgResponseHandler : GDLibrary.Response.IGResponseHandler
    {
        public int ScrollCount {  get; set; }
        public SearchKeywordIgResponseHandler(IResponseParameter response, List<string> itemList = null,int ScrollCount=0,bool IsBrowser=true) : base(response)
        {
            this.ScrollCount = ScrollCount;
            if (!Success)
                return;
            try
            {
                var dataNodes = IsBrowser ? JsonConvert.DeserializeObject<List<string>>(response.Response)
                    :new List<string> { response?.Response};
                dataNodes.Reverse();
                foreach (var dataNode in dataNodes)
                {
                    try
                    {
                        if (dataNode.Contains("{\"data\":{\"xdt_api__v1__fbsearch__topsearch_connection\"") ||
                                            dataNode.Contains("{\"users\":[{\"position\""))
                        {
                            Success = true;
                            GetUserList(dataNode, itemList);
                        }
                        else
                        {
                            var jsonHand = new JsonHandler(dataNode);
                            var jsonArray = jsonHand.GetJToken("data", "xdt_api__v1__fbsearch__topsearch_connection", "users");
                            var hasMore = jsonHand.GetElementValue("has_more").Contains("True");
                            if (hasMore)
                            {
                                HasMore = hasMore;
                                PageId = jsonHand.GetElementValue("page_token");
                                RankId = jsonHand.GetElementValue("rank_token");
                            }
                            else
                            {
                                HasMore = false;
                            }
                            foreach (JToken jtoken in jsonArray)
                            {

                                List<InstagramUser> usersList = UsersList;
                                InstagramUser instagramUser = new InstagramUser(jsonHand.GetJTokenValue(jtoken, "pk"),
                                    jsonHand.GetJTokenValue(jtoken, "username"))
                                {
                                    HasAnonymousProfilePicture =
                                        Convert.ToBoolean(jsonHand.GetJTokenValue(jtoken, "has_anonymous_profile_picture")),
                                    ProfilePicUrl = jsonHand.GetJTokenValue(jtoken, "profile_pic_url"),
                                    FullName = jsonHand.GetJTokenValue(jtoken, "full_name")
                                };
                                int num1 = Convert.ToBoolean(jsonHand.GetJTokenValue(jtoken, "is_verified")) ? 1 : 0;
                                instagramUser.IsVerified = num1 != 0;
                                int num2 = Convert.ToBoolean(jsonHand.GetJTokenValue(jtoken, "is_private")) ? 1 : 0;
                                instagramUser.IsPrivate = num2 != 0;
                                if (!usersList.Any(x => x.Username == instagramUser.Username))
                                    usersList.Add(instagramUser);
                            }
                        }
                        if(UsersList != null && UsersList.Count > 0)
                            break;
                    }
                    catch { }
                }
            }
            catch (Exception)
            {
                
            }

        }

        private void GetUserList(string response,List<string> itemList)
        {
            try
            {
                if (response.Contains("{\"data\":{\"xdt_api__v1__fbsearch__topsearch_connection\""))
                {
                    var jsonHand = new JsonHandler(response);
                    var jsonArray = jsonHand.GetJToken("data", "xdt_api__v1__fbsearch__topsearch_connection", "users");

                    foreach (var user in jsonArray)
                    {
                        try
                        {

                            InstagramUser instagramUser = new InstagramUser(jsonHand.GetJTokenValue(user, "user", "pk"),
                            jsonHand.GetJTokenValue(user, "user", "username"))
                            {
                                ProfilePicUrl = jsonHand.GetJTokenValue(user, "user", "profile_pic_url"),
                                FullName = jsonHand.GetJTokenValue(user, "user", "full_name")
                            };
                            if (UsersList.Any(x => x.Username == instagramUser.Username))
                                continue;
                            int num1 = Convert.ToBoolean(jsonHand.GetJTokenValue(user, "user", "is_verified")) ? 1 : 0;
                            instagramUser.IsVerified = num1 != 0;
                            UsersList.Add(instagramUser);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    } 
                }
                else
                {
                    var jsonHand = new JsonHandler(response);
                    var jsonArray = jsonHand.GetJToken("users");

                    foreach (var user in jsonArray)
                    {
                        try
                        {
                            InstagramUser instagramUser = new InstagramUser(jsonHand.GetJTokenValue(user, "user", "pk"),
                            jsonHand.GetJTokenValue(user, "user", "username"))
                            {
                                ProfilePicUrl = jsonHand.GetJTokenValue(user, "user", "profile_pic_url"),
                                FullName = jsonHand.GetJTokenValue(user, "user", "full_name")
                            };
                            int num1 = Convert.ToBoolean(jsonHand.GetJTokenValue(user, "user", "is_verified")) ? 1 : 0;
                            instagramUser.IsVerified = num1 != 0;
                            int num2 = Convert.ToBoolean(jsonHand.GetJTokenValue(user, "user", "is_private")) ? 1 : 0;
                            instagramUser.IsPrivate = num2 != 0;
                            bool.TryParse(jsonHand.GetJTokenValue(user, "user", "friendship_status", "following"), out bool following);
                            instagramUser.IsFollowing = following;
                            bool.TryParse(jsonHand.GetJTokenValue(user, "user", "friendship_status", "is_bestie"), out bool is_bestie);
                            instagramUser.IsBestie = is_bestie;
                            bool.TryParse(jsonHand.GetJTokenValue(user, "user", "friendship_status", "outgoing_request"), out bool outgoing_request);
                            instagramUser.OutgoingRequest = outgoing_request;
                            UsersList.Add(instagramUser);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<InstagramUser> UsersList { get; set; } = new List<InstagramUser>();
        public string PageId { get; set; }
        public string RankId { get; set; }
        public bool HasMore { get; set; }
    }

}
