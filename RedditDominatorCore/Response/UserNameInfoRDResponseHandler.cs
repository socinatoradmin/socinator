using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using System;
using System.Linq;

namespace RedditDominatorCore.Response
{
    public class UserNameInfoRdResponseHandler : RdResponseHandler
    {
        public UserNameInfoRdResponseHandler(IResponseParameter response) : base(response)
        {
            try
            {
                //var json2 = RdConstants.GetJsonPageResponse(response.Response);
                //var jsonobject = JObject.Parse(json2);
                //PostKarma = jsonobject["user"]["account"]["karma"]["fromPosts"].ToString();
                //CommentKarma = jsonobject["user"]["account"]["karma"]["fromComments"].ToString();
                //AwardedKarma = (Convert.ToInt32(jsonobject["user"]["account"]["karma"]["fromAwardsReceived"].ToString()) +
                //               Convert.ToInt32(jsonobject["user"]["account"]["karma"]["fromAwardsGiven"].ToString())).ToString();
                var jObject = jsonHandler.ParseJsonToJObject(response.Response);
                if (jObject == null)
                {
                    var innerTextData = HtmlParseUtility.GetInnerTextFromTagName(response.Response, "aside", "aria-label", "Profile information");
                    var listOfInnerText = innerTextData.Split('\n')
                                            .Select(x => x.Trim())
                                            .Where(x => !string.IsNullOrWhiteSpace(x))
                                            .ToList();
                    CommentKarma = listOfInnerText.FirstOrDefault(x => x.Contains("comment"))?.Split(' ')[0];
                    PostKarma = listOfInnerText.FirstOrDefault(x => x.Contains("post"))?.Split(' ')[0];

                }
                else
                {
                    PostKarma = jsonHandler.GetJTokenValue(jObject, "data", "link_karma");
                    CommentKarma = jsonHandler.GetJTokenValue(jObject, "data", "comment_karma");
                    AwardedKarma = jsonHandler.GetJTokenValue(jObject, "data", "awarder_karma");
                    Profilepicurl = jsonHandler.GetJTokenValue(jObject, "data", "icon_img");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public string PostKarma { get; set; }
        public string CommentKarma { get; set; }
        public string AwardedKarma { get; set; }
        public string Profilepicurl { get; set; }
    }
}