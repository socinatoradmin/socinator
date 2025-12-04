using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDResponse.AdDetailsResponse
{

    public class AdReactionDetailsResponseHandler : FdResponseHandler, IResponseHandler
    {
        public bool HasMoreResults { get; set; }

        public string EntityId { get; set; }

        public string PageletData { get; set; }

        public bool Status { get; set; }

        public FdScraperResponseParameters ObjFdScraperResponseParameters { get; set; } = new FdScraperResponseParameters();


        public AdReactionDetailsResponseHandler(IResponseParameter responseParameter, FacebookAdsDetails adDetails)
            : base(responseParameter)
        {

            if (responseParameter.HasError || responseParameter.Response == null)
                return;

            ObjFdScraperResponseParameters.FacebookAdsDetails = adDetails;

            string decodedResponse = FdFunctions.GetDecodedResponse(responseParameter.Response);

            GetAdDetails(decodedResponse, ObjFdScraperResponseParameters.FacebookAdsDetails);
        }


        public void GetAdDetails(string reactionDetials, FacebookAdsDetails adDetails)
        {

            try
            {
                var reactionDetails = Regex.Split(reactionDetials, ",comment_count:").Skip(1).ToList();

                var currentReactionData = string.Empty;

                var reaction_Count = string.Empty;

                var commentCount = string.Empty;

                var shareCount = string.Empty;

                var ad_id = string.Empty;


                if (reactionDetails.Count == 0)
                {
                    reactionDetails = Regex.Split(reactionDetials, "\"comment_count\":").Skip(1).ToList();

                    currentReactionData = reactionDetails.FirstOrDefault(x => x.Contains($"share_fbid\":\"{adDetails.Id}\""));

                    ad_id = FdRegexUtility.FirstMatchExtractor(currentReactionData, "\"shareableStoryContext\":{\"ad_id\":(.*?),");

                    if (string.IsNullOrEmpty(ad_id))
                        ad_id = FdRegexUtility.FirstMatchExtractor(currentReactionData, "\"shareableStoryContext\":{\"ad_id\":(.*?),");

                    reaction_Count = FdRegexUtility.FirstMatchExtractor(currentReactionData, "\"reaction_count\":{\"count\":(.*?)},");

                    commentCount = FdRegexUtility.FirstMatchExtractor(currentReactionData, "{\"total_count\":(.*?)}");

                    shareCount = FdRegexUtility.FirstMatchExtractor(currentReactionData, "\"share_count\":{\"count\":(.*?)}");
                }
                else
                {
                    currentReactionData = reactionDetails.FirstOrDefault(x => x.Contains($"share_fbid:\"{adDetails.Id}\""));

                    ad_id = FdRegexUtility.FirstMatchExtractor(currentReactionData, "shareableStoryContext:{ad_id:(.*?),");

                    reaction_Count = FdRegexUtility.FirstMatchExtractor(currentReactionData, "reaction_count:{count:(.*?)}");

                    commentCount = FdRegexUtility.FirstMatchExtractor(currentReactionData, "{total_count:(.*?)}");

                    shareCount = FdRegexUtility.FirstMatchExtractor(currentReactionData, "share_count:{count:(.*?)}");
                }

                adDetails.AdId = FdFunctions.GetIntegerOnlyString(ad_id);

                adDetails.LikersCount = reaction_Count;

                adDetails.SharerCount = shareCount;

                adDetails.CommentorCount = commentCount;
            }
            catch (Exception)
            {

            }
        }
    }
}
