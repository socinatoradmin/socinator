using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary;
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
    public class HashPostInfoWeb
    {
        public HashPostInfoWeb(string response)
        {
            try
            {
                var jsonString = Utilities.GetBetween(response, ">window._sharedData =", ";</script>").Trim();
                JObject JResp = JObject.Parse(jsonString);
               // var jHand = new JsonHandler(jsonString);
                //maxId = jHand.GetElementValue("entry_data","TagPage",0,"graphql","hashtag","edge_hashtag_to_media","page_info","end_cursor");
                maxId = JResp["entry_data"]["TagPage"][0]["graphql"]["hashtag"]["edge_hashtag_to_media"]["page_info"]["end_cursor"]?.ToString();
                
                foreach (JToken jtoken in JResp["entry_data"]["TagPage"][0]["graphql"]["hashtag"]["edge_hashtag_to_media"]["edges"])
                {
                    InstagramPost instagramPost = new InstagramPost();
                    instagramPost.Code = jtoken["node"]["shortcode"]?.ToString();
                    JToken jtoken1 = jtoken["node"]["edge_media_to_caption"]["edges"];
                    //[0]["node"]["text"]
                    jtoken1 = jtoken1.Count()!=0 ? jtoken["node"]["edge_media_to_caption"]["edges"][0]["node"]["text"] :string.Empty;
                    instagramPost.Caption = jtoken1?.ToString()??ToString();
                    instagramPost.Id = jtoken["node"]["id"]?.ToString();
                    instagramPost.CommentsDisabled =Convert.ToBoolean(jtoken["node"]["comments_disabled"]);
                    instagramPost.CommentCount = Convert.ToInt32(jtoken["node"]["edge_media_to_comment"]["count"]?.ToString());
                    instagramPost.LikeCount = Convert.ToInt32(jtoken["node"]["edge_liked_by"]["count"]?.ToString());
                    instagramPost.TakenAt = Convert.ToInt32(jtoken["node"]["taken_at_timestamp"]?.ToString());
                    string mediaType = jtoken["node"]["__typename"]?.ToString();

                    if (mediaType.Contains("GraphImage"))
                        instagramPost.MediaType = MediaType.Image;
                    else if (mediaType.Contains("GraphVideo"))
                        instagramPost.MediaType = MediaType.Video;
                    else
                        instagramPost.MediaType = MediaType.Album;

                    var postOwnerName = jtoken["node"]["owner"]["id"]?.ToString();
                    instagramPost.Pk = postOwnerName.ToString();
                    PostUser.Add(postOwnerName);

                    lstInstagramPost.Add(instagramPost);
                }
            }
            catch (Exception )
            {
                //ignored
            }
        }
        public List<InstagramPost> lstInstagramPost = new List<InstagramPost>();
        public List<string> PostUser = new List<string>();
        public string maxId = string.Empty;
    }
}
