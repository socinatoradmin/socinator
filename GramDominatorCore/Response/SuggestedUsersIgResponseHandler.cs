using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace GramDominatorCore.GDLibrary.Response
{
    [Localizable(false)]
    public class SuggestedUsersIgResponseHandler : IGResponseHandler
    {
        public SuggestedUsersIgResponseHandler(IResponseParameter response,bool IsBrowser=true)
            : base(response)
        {
            if (!Success)
                return;    
            if(response.Response.Contains("{\"groups\":[{\"type\":\"Recommended\",\"items\"") || 
                    response.Response.Contains("{\"more_available\":false,\"max_id\""))
            {
                GetUsersList(response.Response);
                if (UsersList.Count <= 0)
                    Success = false;
                return;
            }       
            try
            {
                var obj = handler.ParseJsonToJObject(response?.Response);
                if(obj != null)
                {
                    MaxID = handler.GetJTokenValue(obj, "max_id");
                    bool.TryParse(handler.GetJTokenValue(obj, "more_available"), out bool more_available);
                    HasMoreResult = more_available;
                    var SuggestedUsers = handler.GetJArrayElement(handler.GetJTokenValue(obj, "suggested_users", "suggestions"));
                    if(SuggestedUsers != null && SuggestedUsers.HasValues)
                    {
                        foreach(var user in SuggestedUsers)
                        {
                            try
                            {
                                bool.TryParse(handler.GetJTokenValue(user, "user", "is_verified"), out bool is_verified);
                                bool.TryParse(handler.GetJTokenValue(user, "user", "is_private"), out bool is_private);
                                bool.TryParse(handler.GetJTokenValue(user, "user", "has_anonymous_profile_picture"), out bool has_anonymous_profile_picture);
                                var model = new InstagramUser
                                {
                                    Pk = handler.GetJTokenValue(user,"user","pk"),
                                    ProfilePicUrl = handler.GetJTokenValue(user, "user", "profile_pic_url"),
                                    Username = handler.GetJTokenValue(user,"user", "username"),
                                    IsVerified = is_verified,
                                    IsPrivate = is_private,
                                    HasAnonymousProfilePicture = has_anonymous_profile_picture,
                                    FullName = handler.GetJTokenValue(user,"user", "full_name")
                                };
                                if (!UsersList.Any(x => x.Username == model.Username))
                                    UsersList.Add(model);
                            }
                            catch { }
                        }
                    }
                }

                //foreach (JToken jtoken in (JArray)RespJ["users"])
                //{
                //    List<InstagramUser> usersList = UsersList;
                //    InstagramUser instagramUser = new InstagramUser(jtoken["pk"].ToString(),
                //        jtoken["username"].ToString())
                //    {
                //        ProfilePicUrl = jtoken["profile_pic_url"].ToString(),
                //        FullName = jtoken["full_name"].ToString()
                //    };
                //    int num1 = Convert.ToBoolean(jtoken["is_verified"].ToString()) ? 1 : 0;
                //    instagramUser.IsVerified = num1 != 0;
                //    int num2 = Convert.ToBoolean(jtoken["is_private"].ToString()) ? 1 : 0;
                //    instagramUser.IsPrivate = num2 != 0;
                //    usersList.Add(instagramUser);
                //}
            }
            catch (Exception ex)
            {
                ex.DebugLog();          
            }                      
        }

        private void GetUsersList(string response)
        {
            try
            {
                var jsonHandler = new JsonHandler(response);
                if (response.Contains("{\"more_available\":false,\"max_id\""))
                {
                    var suggestedUsers = jsonHandler.GetJToken("suggested_users", "suggestions");
                    foreach(var user in suggestedUsers)
                    {
                        InstagramUser instagramUser = new InstagramUser(jsonHandler.GetJTokenValue(user, "user", "pk"),
                            jsonHandler.GetJTokenValue(user, "user", "username"))
                        {
                            HasAnonymousProfilePicture =
                                Convert.ToBoolean(jsonHandler.GetJTokenValue(user, "user", "has_anonymous_profile_picture")),
                            ProfilePicUrl = jsonHandler.GetJTokenValue(user, "user", "profile_pic_url"),
                            FullName = jsonHandler.GetJTokenValue(user, "user", "full_name")
                        };
                        int num1 = Convert.ToBoolean(jsonHandler.GetJTokenValue(user, "user", "is_verified")) ? 1 : 0;
                        instagramUser.IsVerified = num1 != 0;
                        int num2 = Convert.ToBoolean(jsonHandler.GetJTokenValue(user, "user", "is_private")) ? 1 : 0;
                        instagramUser.IsPrivate = num2 != 0;
                        UsersList.Add(instagramUser);
                    }
                }
                else
                {
                    var groups = jsonHandler.GetJToken("groups");
                    foreach (var group in groups)
                    {
                        var type = jsonHandler.GetJTokenValue(group, "type");
                        if (type == "Recommended")
                        {
                            var items = jsonHandler.GetJTokenOfJToken(group, "items");
                            foreach (var item in items)
                            {
                                InstagramUser instagramUser = new InstagramUser(jsonHandler.GetJTokenValue(item, "user", "pk"),
                            jsonHandler.GetJTokenValue(item, "user", "username"))
                                {
                                    HasAnonymousProfilePicture =
                                Convert.ToBoolean(jsonHandler.GetJTokenValue(item, "user", "has_anonymous_profile_picture")),
                                    ProfilePicUrl = jsonHandler.GetJTokenValue(item, "user", "profile_pic_url"),
                                    FullName = jsonHandler.GetJTokenValue(item, "user", "full_name")
                                };
                                int num1 = Convert.ToBoolean(jsonHandler.GetJTokenValue(item, "user", "is_verified")) ? 1 : 0;
                                instagramUser.IsVerified = num1 != 0;
                                int num2 = Convert.ToBoolean(jsonHandler.GetJTokenValue(item, "user", "is_private")) ? 1 : 0;
                                instagramUser.IsPrivate = num2 != 0;
                                UsersList.Add(instagramUser);
                            }
                        }
                    }
                }
                
            }
            catch (Exception)
            {
                
            }
        }

        public List<InstagramUser> UsersList { get; } = new List<InstagramUser>();
        public bool HasMoreResult { get; set; }
        public string MaxID { get; set; }
    }
}
