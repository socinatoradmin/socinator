using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore;

namespace GramDominatorCore.WebResponseHandler
{
    public class PostLocationWebResponseHandler
    {
        public PostLocationWebResponseHandler(string response)
        {
            try
            {
                var jsonString = Utilities.GetBetween(response, ">window._sharedData =", ";</script>").Trim();
                JObject JResp = JObject.Parse(jsonString);
                maxId = JResp["entry_data"]["LocationsPage"][0]["graphql"]["location"]["edge_location_to_media"]["page_info"]["end_cursor"]?.ToString();

                foreach (JToken jtoken in JResp["entry_data"]["LocationsPage"][0]["graphql"]["location"]["edge_location_to_media"]["edges"])
                {
                    InstagramPost instagramPost = new InstagramPost();
                    instagramPost.Code = jtoken["node"]["shortcode"]?.ToString();
                    JToken jtoken1 = jtoken["node"]["edge_media_to_caption"]["edges"];
                    //[0]["node"]["text"]
                    jtoken1 = jtoken1.Count() != 0 ? jtoken["node"]["edge_media_to_caption"]["edges"][0]["node"]["text"] : string.Empty;
                    instagramPost.Caption = jtoken1?.ToString() ?? ToString();
                    instagramPost.Id = jtoken["node"]["id"]?.ToString();
                    instagramPost.CommentsDisabled = Convert.ToBoolean(jtoken["node"]["comments_disabled"]);
                    instagramPost.CommentCount = Convert.ToInt32(jtoken["node"]["edge_media_to_comment"]["count"]?.ToString());
                    instagramPost.LikeCount = Convert.ToInt32(jtoken["node"]["edge_liked_by"]["count"]?.ToString());
                    instagramPost.TakenAt = Convert.ToInt32(jtoken["node"]["taken_at_timestamp"]?.ToString());
                    var postOwnerName = jtoken["node"]["owner"]["id"]?.ToString();
                    instagramPost.Pk = postOwnerName.ToString();
                    var Isvideo  =Convert.ToBoolean(jtoken["node"]["is_video"]);
                    instagramPost.MediaType = !Isvideo ? MediaType.Image : MediaType.Video;
                   

                    PostUser.Add(postOwnerName);
                    lstInstagramPost.Add(instagramPost);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public List<InstagramPost> lstInstagramPost = new List<InstagramPost>();
        public List<string> PostUser = new List<string>();
        public string maxId = string.Empty;
    }
}
