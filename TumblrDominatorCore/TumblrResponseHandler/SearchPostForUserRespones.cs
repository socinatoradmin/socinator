using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TumblrDominatorCore.Models;

namespace TumblrDominatorCore.TumblrResponseHandler
{
    public class SearchPostForUserRespones : ResponseHandler
    {
        public SearchPostForUserRespones(IResponseParameter responeParameter) : base(responeParameter)
        {
            LstTumblrPost = new List<TumblrPost>();

            if (responeParameter == null)
                return;
            try
            {
                var searchBlogsRowPageSource = Response.Response;
                if (searchBlogsRowPageSource.Contains("csrf"))
                    searchBlogsRowPageSource = "{" + Utilities.GetBetween(searchBlogsRowPageSource, "{", "csrf");

                Success = true;
                var jObject = JObject.Parse(searchBlogsRowPageSource);

                LstTumblrPost = new List<TumblrPost>();

                foreach (var eachUser in jObject["response"]["posts"])
                    try
                    {
                        var tumblrPost = new TumblrPost
                        {
                            Id = eachUser["id"].ToString(),
                            BlogName = eachUser["blog"]["name"].ToString(),
                            BlogUrl = eachUser["blog"]["url"].ToString(),
                            PostType = eachUser["type"].ToString(),
                            PostUrl = eachUser["post_url"].ToString(),
                            OwnerUsername = eachUser["blog"]["name"].ToString(),
                            RebloggedRootId = eachUser["reblog_key"].ToString(),
                            PostKey = eachUser["blog"]["key"].ToString(),
                            CanLike = eachUser["can_like"].ToString().Contains("True") ? true : false,
                            CanReply = eachUser["can_reply"].ToString().Contains("True") ? true : false,
                            IsLiked = eachUser["liked"].ToString().Contains("True") ? true : false,
                            isFollowedPostOwner = eachUser["followed"].ToString().Contains("True") ? true : false
                        };
                        var note = eachUser["notes"]["current"].ToString();
                        note = note.Replace("notes", "").Replace("note", "").Replace(",", "");
                        tumblrPost.Caption = eachUser["summary"].ToString();
                        tumblrPost.NotesCount = note;
                        tumblrPost.PostComment = Convert.ToBoolean(eachUser["can_send_in_message"].ToString());
                        tumblrPost.PostPostedTimeStamp = Convert.ToInt64(eachUser["timestamp"]);
                        LstTumblrPost.Add(tumblrPost);
                    }


                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public List<TumblrPost> LstTumblrPost { get; set; }
    }
}