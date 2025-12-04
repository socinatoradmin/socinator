using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace GramDominatorCore.Response
{
    [Localizable(false)]
    public class UserFeedResponse : IGResponseHandler
    {
        public UserFeedResponse(IResponseParameter response)
            : base(response)
        {
            if (!Success)
                return;
            var dataList = new List<string>();
            try
            {
                dataList = JsonConvert.DeserializeObject<List<string>>(response.Response);
            }
            catch (Exception)
            {
                dataList = null;
            }
            if (dataList != null && dataList.Count > 0)
            {
                GetMediaData(dataList);
                return;
            }
        }

        private void GetMediaData(List<string> dataList)
        {
            foreach (var item in dataList)
            {
                var IsChecked = false;
                var jsonHandler = new JsonHandler(item);

                var bkColumnContext = "";
                var bkToken = jsonHandler.GetJToken("tree", "bk.components.screen.Wrapper", "content", "bk.components.Flexbox",
                                                                "children", 2, "bk.components.Flexbox", "children", 0,
                                                                "bk.components.Flexbox", "children", 0,
                                                                "bk.components.Collection", "children");
                bkToken = bkToken is null || !bkToken.HasValues ? jsonHandler.GetJToken("tree", "bk.components.Flexbox", "children", 0, "bk.components.screen.Wrapper", "content", "bk.components.Flexbox",
                                                                "children", 2, "bk.components.Flexbox", "children", 0,
                                                                "bk.components.Flexbox", "children", 0,
                                                                "bk.components.Collection", "children") : bkToken;
                foreach (var bk in bkToken)
                {
                    bkColumnContext = jsonHandler.GetJTokenValue(bk, "bk.components.Flexbox", "on_bind");
                    if (bkColumnContext.Contains("\"media_id\", \"media_code\", \"media_product_type\", \"media_type\", \"media_image_url\","))
                        break;
                }


                var rawElementList = System.Text.RegularExpressions.Regex.Split(bkColumnContext, "(bk.action.array.Make, \"media_id\", \"media_code\", \"media_product_type\", \"media_type\", \"media_image_url\", \"location_name\", \"icon\", \"margin_right\")");
                for (int i = 1; i <= rawElementList.Length; i++)
                {
                    var post = new InstagramPost();
                    if (i % 2 == 0)
                    {
                        if(!IsChecked)
                            IsChecked = true;
                        var eachData = rawElementList[i].Split(',');
                        var postId = eachData[3].Trim().Trim('"');
                        postId = string.IsNullOrEmpty(postId) || postId.Contains("username") ? eachData[7].Trim().Trim('"') : postId;
                        if (!string.IsNullOrEmpty(postId) && !Items.Any(x=>x.Code == postId))
                        {
                            post.Code = postId;
                            post.HasLiked = true;
                            Items.Add(post);
                        }
                    }
                }
                if(Items.Count == 0 || !IsChecked)
                {
                    var jsonTokenString = GdUtilities.GetOwnLikedPostJToken(item);
                    var data = Regex.Split(jsonTokenString, "bk.components.FoaGestureExtension");
                    foreach(var token in data.Skip(1))
                    {
                        if(!string.IsNullOrEmpty(token) && token.Contains("\"on_tap\""))
                        {
                            var Node = Utilities.GetBetween(token, "\"on_tap\":", "\"_style\"");
                            var Url = Utilities.GetBetween(Node, "bk.action.string.Concat", ")");
                            Url = string.IsNullOrEmpty(Url) ? Utilities.GetBetween(Node, "bk.action.navigation.OpenUrlV2", "(") : Url;
                            if(!string.IsNullOrEmpty(Url))
                            {
                                var code = Regex.Split(Url?.Replace(",",""),"\\\"")?.LastOrDefault(y=>y!=string.Empty)?.Replace("\\","")?.Trim();
                                code = string.IsNullOrEmpty(code) ?Url.Split('/')?.LastOrDefault(z=>z!=string.Empty): code;
                                code = Regex.Replace(code, "\\\\.*", "");
                                if (!string.IsNullOrEmpty(code))
                                {
                                    Items.Add(new InstagramPost { Code = code,HasLiked = true });
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool HasMoreResults { get; }

        public List<InstagramPost> Items { get; } = new List<InstagramPost>();

        public string MaxId { get; }
    }
}
