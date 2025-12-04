using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GramDominatorCore.WebResponseHandler
{
    public class PostInfoWeb
    {
        public PostInfoWeb(string response)
        {
            try
            {
                if (response == null)
                    return;
                
                //var jsonString = Utilities.GetBetween(response, ">window._sharedData =", ";</script>").Trim();
                //JObject jsonData = JObject.Parse(jsonString);
                //var jHand = new JsonHandler(jsonString);
                //var jtoken = jHand.GetJToken("entry_data", "PostPage", 0, "graphql", "shortcode_media");
                //instagramPost = new InstagramPost();
                //instagramPost.Code = jtoken["shortcode"]?.ToString();
                //JToken jtoken1 = jtoken["edge_media_to_caption"]["edges"];
                ////[0]["node"]["text"]
                //jtoken1 = jtoken1.Count() != 0 ? jtoken["edge_media_to_caption"]["edges"][0]["node"]["text"] : string.Empty;
                //instagramPost.Caption = jtoken1?.ToString() ?? ToString();
                //instagramPost.Id = jtoken["id"]?.ToString();
                //instagramPost.CommentsDisabled = Convert.ToBoolean(jtoken["comments_disabled"]);
                //try
                //{
                //    instagramPost.CommentCount= Convert.ToInt32(jtoken["edge_media_preview_comment"]["count"]?.ToString());
                //}
                //catch (Exception)
                //{
                //    JToken jtoken2 = jtoken["edge_media_to_comment"]["count"];
                //    instagramPost.CommentCount = Convert.ToInt32(jtoken2 != null ? jtoken2 : 0);
                //}
                 
               
                //// instagramPost.CommentCount = Convert.ToInt32(jtoken["edge_media_preview_comment"]["count"]?.ToString());
                //instagramPost.LikeCount = Convert.ToInt32(jtoken["edge_media_preview_like"]["count"]?.ToString());
                //instagramPost.TakenAt = Convert.ToInt32(jtoken["taken_at_timestamp"]?.ToString());
                //var postOwnerId = jtoken["owner"]["id"]?.ToString();
                //instagramPost.Pk = postOwnerId.ToString();
                //string mediaType = jtoken["__typename"]?.ToString();
                //var Isvideo = Convert.ToBoolean(jtoken["is_video"]);
                //instagramPost.MediaType = !Isvideo ? MediaType.Image : MediaType.Video;
                //if (mediaType.Contains("GraphImage"))
                //    instagramPost.MediaType = MediaType.Image;
                //else if (mediaType.Contains("GraphVideo"))
                //    instagramPost.MediaType = MediaType.Video;
                //else
                //    instagramPost.MediaType = MediaType.Album;

                //var userJtoken = jHand.GetJToken("entry_data", "PostPage", 0, "graphql", "shortcode_media", "owner");

                //instagramPost.User = new InstagramUser()
                //{
                //    UserId = userJtoken["id"].ToString(),
                //    Username = userJtoken["username"].ToString()
                //};

                // PostUser.Add(postOwnerId);



            }
            catch (Exception )
            {
                //ignored
            }
        }
        public InstagramPost instagramPost = new InstagramPost();
       // public List<string> PostUser = new List<string>();
    }
}
