using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GramDominatorCore.Response
{
    public class PostStoryResponse : IGResponseHandler
    {
        public PostStoryResponse(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            //if (response.Response.StartsWith("{\"payload\":{\"payloads\":"))
            //{
            //    GetUserStories();
            //    return;
            //}
            var jsonHandler = new JsonHandler(response.Response);
            var reelsData = jsonHandler.GetJToken("require", 0, 3, 0, "__bbox", "require", 0, 3, 1, "__bbox", "result", "data");
            if (reelsData.ToString().Contains("\"xdt_api__v1__feed__reels_media\""))
            {
                GetUserReels(reelsData);
                return;
            }
            try
            {
                LstUsers = new List<UsersStory>();
                foreach (var Users in RespJ["reels"])
                {
                    UsersStory objUsersStory = new UsersStory();
                    foreach (var UserItem in Users)
                    {
                        objUsersStory.UserId = UserItem["id"].ToString();
                        foreach (var item in UserItem["items"])
                        {
                            UsersPostStory objUsersPostStory = new UsersPostStory();
                            objUsersPostStory.UserMediaId = item["id"].ToString();
                            objUsersPostStory.PostId = item["pk"].ToString();
                            objUsersPostStory.PostTime = item["taken_at"].ToString();
                            objUsersPostStory.UserId = item["user"]["pk"].ToString();
                            objUsersPostStory.currentTime = Convert.ToString(DateTimeUtilities.GetCurrentEpochTime(DateTime.Now));
                            objUsersPostStory.MediaType = item["media_type"].ToString();
                            objUsersStory.LstMedia.Add(objUsersPostStory);
                        }
                        LstUsers.Add(objUsersStory);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GetUserReels ( JToken reelsData )
        {
            try
            {
                LstUsers = new List<UsersStory>();
                var jsonHandler = new JsonHandler(reelsData);
                var data = jsonHandler.GetJToken("xdt_api__v1__feed__reels_media", "reels_media");
                if(data != null && data.HasValues)
                {
                    foreach (var users in data)
                    {
                        var objUsersStory = new UsersStory();
                        objUsersStory.UserId = users["id"].ToString();
                        foreach (var item in users["items"])
                        {
                            if (item["product_type"].ToString() != "story")
                                continue;
                            UsersPostStory objUsersPostStory = new UsersPostStory();
                            objUsersPostStory.UserMediaId = item["id"].ToString();
                            objUsersPostStory.PostId = item["pk"].ToString();
                            objUsersPostStory.PostTime = item["taken_at"].ToString();
                            objUsersPostStory.UserId = item["user"]["pk"].ToString();
                            objUsersPostStory.currentTime = Convert.ToString(DateTimeUtilities.GetCurrentEpochTime(DateTime.Now));
                            objUsersPostStory.MediaType = item["media_type"].ToString();
                            objUsersStory.LstMedia.Add(objUsersPostStory);
                        }
                        LstUsers.Add(objUsersStory); 
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GetUserStories()
        {
            try
            {
                LstUsers = new List<UsersStory>();
                foreach (var Users in RespJ["tray"])
                {
                    if (Users["items"]!=null && Users["items"].HasValues)
                    {
                        UsersStory objUsersStory = new UsersStory();
                        objUsersStory.UserId = Users["id"].ToString();
                        foreach (var item in Users["items"])
                        {
                            if (item["product_type"].ToString() != "story")
                                continue;
                            UsersPostStory objUsersPostStory = new UsersPostStory();
                            objUsersPostStory.UserMediaId = item["id"].ToString();
                            objUsersPostStory.PostId = item["pk"].ToString();
                            objUsersPostStory.PostTime = item["taken_at"].ToString();
                            objUsersPostStory.UserId = item["user"]["pk"].ToString();
                            objUsersPostStory.currentTime = Convert.ToString(DateTimeUtilities.GetCurrentEpochTime(DateTime.Now));
                            objUsersPostStory.MediaType = item["media_type"].ToString();
                            objUsersStory.LstMedia.Add(objUsersPostStory);
                        }
                        LstUsers.Add(objUsersStory); 
                    }
                    else
                    {
                        try
                        {
                            UsersStory objUsersStory = new UsersStory();
                            objUsersStory.UserId = Users["id"].ToString();
                            if (Users["product_type"].ToString() != "story")
                                continue;
                            UsersPostStory objUsersPostStory = new UsersPostStory();
                            objUsersPostStory.UserMediaId = Users["id"].ToString();
                            objUsersPostStory.PostId = Users["pk"].ToString();
                            objUsersPostStory.PostTime = Users["taken_at"].ToString();
                            objUsersPostStory.UserId = Users["user"]["pk"].ToString();
                            objUsersPostStory.currentTime = Convert.ToString(DateTimeUtilities.GetCurrentEpochTime(DateTime.Now));
                            objUsersPostStory.MediaType = Users["media_type"].ToString();
                            objUsersStory.LstMedia.Add(objUsersPostStory);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<UsersStory> LstUsers { get; set; }
    }
}
