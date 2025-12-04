using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.BaseResponse;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.CommonResponse
{
    public class GetStoryDetailsResponseHandler : FdResponseHandler
    {
        public List<FacebookPostDetails> ListOwnStories { get; set; }

        public GetStoryDetailsResponseHandler(IResponseParameter responseParameter)
            : base(responseParameter)
        {

            if (ListOwnStories == null)
                ListOwnStories = new List<FacebookPostDetails>();

            GetLastStoryDetails(responseParameter.Response);

        }

        private void GetLastStoryDetails(string response)
        {
            try
            {
                response = Regex.Split(response, "}}}}}")[0] + "}}}}}";

                JObject jObject = JObject.Parse(response);

                var storyDetails = jObject["o0"]["data"]["me"]["story_bucket"]["nodes"][0]["threads"]["nodes"];

                foreach (var token in storyDetails)
                {
                    try
                    {
                        FacebookPostDetails objFdMessageDetails = new FacebookPostDetails
                        {
                            Id = token["id"].ToString(),
                            PostedDateTime = DateTimeUtilities.EpochToDateTimeLocal(int.Parse(token["time"].ToString()))
                        };

                        ListOwnStories.Add(objFdMessageDetails);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
