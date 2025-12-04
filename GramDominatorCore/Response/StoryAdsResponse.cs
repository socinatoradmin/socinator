using GramDominatorCore.GDLibrary.Response;
using System;
using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;
using GramDominatorCore.GDModel;

namespace GramDominatorCore.Response
{
    public class StoryAdsResponse : IGResponseHandler
    {
        public StoryAdsResponse(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            JObject inner = RespJ["reels"].Value<JObject>();                       
            var JResponse = inner.Values();
            try
            {
                foreach (var ads in JResponse)
                {
                
                    StoryAdsDetails storyAdsDetails = new StoryAdsDetails();
                    bool isCaption;
                    try
                    {
                        storyAdsDetails.id = ads["id"].ToString();
                    }
                    catch (Exception)
                    {
                        storyAdsDetails.id = "";
                    }
                    try
                    {
                        storyAdsDetails.ad_id = ads["items"][0]["injected"]["ad_id"].ToString();
                    }
                    catch (Exception)
                    {
                        storyAdsDetails.ad_id = "";
                    }
                    storyAdsDetails.post_date = ads["items"][0]["taken_at"].ToString();
                    try
                    {
                        
                        storyAdsDetails.code = ads["items"][0]["code"].Value<string>();
                    }
                    catch (Exception )
                    {
                        storyAdsDetails.code = "";
                    }
                    try
                    {
                        storyAdsDetails.postOwner = ads["items"][0]["user"]["full_name"].ToString();
                    }
                    catch (Exception)
                    {
                        storyAdsDetails.postOwner = "";
                    }
                    try
                    {
                        storyAdsDetails.postOwnerImage = ads["items"][0]["user"]["profile_pic_url"].ToString();
                    }
                    catch (Exception)
                    {
                        storyAdsDetails.postOwnerImage = "";
                    }
                    try
                    {
                        storyAdsDetails.mediaType = ads["items"][0]["media_type"].ToString();
                    }
                    catch (Exception)
                    {
                        storyAdsDetails.mediaType = "";
                    }
                    try
                    {
                        storyAdsDetails.likeCount = ads["items"][0]["like_count"].ToString();
                    }
                    catch (Exception)
                    {
                        storyAdsDetails.likeCount = "0";
                    }
                    try
                    {
                        storyAdsDetails.CommentCount = ads["items"][0]["comment_count"].ToString();
                    }
                    catch (Exception)
                    {
                        storyAdsDetails.CommentCount = "0";
                    }
                    try
                    {
                        storyAdsDetails.newsFeedDescription = ads["items"][0]["caption"]["text"].ToString();
                        isCaption = true;
                    }
                    catch (Exception)
                    {
                        isCaption = false;
                    }
                    try
                    {
                        if (!isCaption)
                        {
                            storyAdsDetails.newsFeedDescription = ads["items"][0]["user"]["caption"].ToString();
                            isCaption = true;
                        }
                    }
                    catch (Exception)
                    {
                        isCaption = false;
                    }
                    try
                    {
                        if (!isCaption)
                        {
                            storyAdsDetails.newsFeedDescription = ads["items"][0]["caption"].ToString();
                            // ReSharper disable once RedundantAssignment
                            isCaption = true;
                        }
                    }
                    catch (Exception)
                    {
                        storyAdsDetails.newsFeedDescription = ""; //ignored
                    }
                    try
                    {
                        storyAdsDetails.callToAction = ads["items"][0]["link_text"].ToString();
                    }
                    catch (Exception)
                    {

                        storyAdsDetails.callToAction = "";
                    }
                    try
                    {
                        storyAdsDetails.discriptionLink = ads["items"][0]["android_links"][0]["webUri"].ToString();
                        if (string.IsNullOrEmpty(storyAdsDetails.discriptionLink))
                        {
                            storyAdsDetails.discriptionLink = ads["items"][0]["android_links"][1]["redirectUri"].ToString();
                        }
                    }
                    catch (Exception)
                    {
                        storyAdsDetails.discriptionLink = ads["items"][0]["story_cta"][0]["links"][0]["webUri"].ToString();
                    }
                    try
                    {
                        storyAdsDetails.image_video_url = ads["items"][0]["image_versions2"]["candidates"][0]["url"].ToString();
                    }
                    catch (Exception)
                    {
                        storyAdsDetails.image_video_url = "";
                    }
                    lstStoryDetails.Add(storyAdsDetails);
                }
            }
            catch (Exception )
            {
                
            }
            
        }
       public List<StoryAdsDetails> lstStoryDetails = new List<StoryAdsDetails>();

    }
}
