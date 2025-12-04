using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using Newtonsoft.Json.Linq;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using System;
using System.Collections.Generic;
namespace QuoraDominatorCore.Response
{
    public class ScrapePostResponseHandler : QuoraResponseHandler
    {
        public bool HasMoreResult { get; set; } =false;
        public int PaginationCount {  get; set; }
        public List<PostDetails> PostCollection { get; set; }=new List<PostDetails>();
        public ScrapePostResponseHandler(IResponseParameter response,bool IsBrowser=false) : base(response)
        {
            try
            {
                if(!IsBrowser)
                {
                    var jsonObject = jsonHandler.ParseJsonToJObject(response.Response);
                    var datas = jsonHandler.GetJTokenOfJToken(jsonObject, "data", "searchConnection");
                    bool.TryParse(jsonHandler.GetJTokenValue(datas, "pageInfo", "hasNextPage"), out bool hasMoreResult);
                    HasMoreResult = hasMoreResult;
                    int.TryParse(jsonHandler.GetJTokenValue(datas, "pageInfo", "endCursor"), out int endCursor);
                    PaginationCount = endCursor;
                    var Posts=jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(datas,"edges"));
                    if (Posts != null && Posts.HasValues)
                        Posts.ForEach(post =>
                        {
                            var Node = jsonHandler.GetJTokenOfJToken(post, "node", "post");
                            var PostId = jsonHandler.GetJTokenValue(Node, "pid");
                            if (!PostCollection.Exists(x => x.PostId == PostId))
                            {
                                int.TryParse(jsonHandler.GetJTokenValue(Node, "numDisplayComments"), out int commentCount);
                                int.TryParse(jsonHandler.GetJTokenValue(Node, "numShares"), out int shareCount);
                                int.TryParse(jsonHandler.GetJTokenValue(Node, "numUpvotes"), out int upvoteCount);
                                int.TryParse(jsonHandler.GetJTokenValue(Node, "numViews"), out int viewsCount);
                                long.TryParse(jsonHandler.GetJTokenValue(Node, "creationTime"), out long created);
                                var postUrl = jsonHandler.GetJTokenValue(Node, "url");
                                PostCollection.Add(
                                    new PostDetails()
                                    {
                                        PostId = PostId,
                                        PostUrl = !string.IsNullOrEmpty(postUrl) && postUrl.StartsWith("http")?postUrl:$"{QdConstants.HomePageUrl}{postUrl}",
                                        PostTitle = GetPostTitle(Node),
                                        Created = new DateTime(created,DateTimeKind.Local),
                                        CommentCount = commentCount,
                                        ShareCount = shareCount,
                                        ViewsCount = viewsCount,
                                        UpvoteCount = upvoteCount,
                                        ObjectId=jsonHandler.GetJTokenValue(Node, "oid")
                                    });
                            }
                        });
                }
            }catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private string GetPostTitle(JToken node)
        {
            var title=string.Empty;
            try
            {
                var TitleJson = jsonHandler.GetJTokenValue(node, "content");
                TitleJson=string.IsNullOrEmpty(TitleJson) ? jsonHandler.GetJTokenValue(node, "contentQtextDocument", "legacyJson") : TitleJson;
                TitleJson=string.IsNullOrEmpty(TitleJson) ? jsonHandler.GetJTokenValue(node, "title") : TitleJson;
                var jObject = jsonHandler.ParseJsonToJObject(TitleJson);
                var Titles = jsonHandler.GetJArrayElement(jsonHandler.GetJTokenValue(jObject, "sections"));
                if (Titles != null && Titles.HasValues)
                    Titles.ForEach(Title =>
                    {
                        var titleText = jsonHandler.GetJTokenValue(Title, "spans",0,"text");
                        if(!string.IsNullOrEmpty(titleText))
                            title += titleText+" ";
                    });
            }
            catch (Exception)
            {
            }
            return title;
        }
    }
}
