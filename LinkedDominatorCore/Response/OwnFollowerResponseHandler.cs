using DominatorHouseCore.Interfaces;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkedDominatorCore.Response
{
    public class OwnFollowerResponseHandler: LdResponseHandler
    {
        public List<LinkedinUser> FollowersList { get; } = new List<LinkedinUser>();
        public bool HasMoreResult=false;
        public int PaginationCount = 0;
        public string ErrorMessage {  get; set; }=string.Empty;
        public OwnFollowerResponseHandler(IResponseParameter response,bool IsBrowser=false):base(response)
        {
            if(!Success)
                return;
            if (IsBrowser)
            {
                var list = JsonConvert.DeserializeObject<List<string>>(response.Response);
                foreach(var data in list)
                {
                    var jobject = handler.ParseJsonToJObject(data);
                    GetFollowers(jobject);
                }
            }
            else
            {
                var jobject = handler.ParseJsonToJObject(response?.Response);
                GetFollowers(jobject);
            }
            
        }

        private void GetFollowers(JObject jobject)
        {
            try
            {
                var paginationToken = handler.GetJTokenOfJToken(jobject, "data", "data", "searchDashClustersByAll", "paging");
                int.TryParse(handler.GetJTokenValue(paginationToken, "count"), out int count);
                PaginationCount = count;
                int.TryParse(handler.GetJTokenValue(paginationToken, "start"), out int start);
                int.TryParse(handler.GetJTokenValue(paginationToken, "total"), out int total);
                HasMoreResult = (total - start) >= 10;
                var followers = handler.GetJArrayElement(handler.GetJTokenValue(jobject, "included"));
                if (followers != null && followers.HasValues)
                {
                    foreach (var follower in followers)
                    {
                        var trackingId = handler.GetJTokenValue(follower, "trackingId");
                        if (string.IsNullOrEmpty(trackingId))
                            continue;
                        var profileId = handler.GetJTokenValue(follower, "entityUrn");
                        var linkedInUser = new LinkedinUser()
                        {
                            TrackingId = trackingId,
                            PublicIdentifier = Utils.GetBetween(profileId, "(urn:li:fsd_profile:", ",MYNETWORK_CURATION_HUB"),
                            ProfileId = Utils.GetBetween(profileId, "(urn:li:fsd_profile:", ",MYNETWORK_CURATION_HUB"),
                            FullName = handler.GetJTokenValue(follower, "title", "text"),
                            HeadlineTitle = handler.GetJTokenValue(follower, "primarySubtitle", "text"),
                            ProfileUrl = handler.GetJTokenValue(follower, "navigationUrl")
                        };
                        if (!FollowersList.Any(x => x.PublicIdentifier == linkedInUser.PublicIdentifier))
                            FollowersList.Add(linkedInUser);
                    }
                }
                else
                    ErrorMessage = "No Followers Found In This Profile";
            }
            catch { }
        }
    }
}
