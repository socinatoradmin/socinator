using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using DominatorHouseCore;
using CommonServiceLocator;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using Unity;
using DominatorHouseCore.Enums;

namespace GramDominatorCore.PowerAdSpy
{
    public interface IInstaAdScrappers
    {
        void InstaDataScraper(DominatorAccountModel dominatorAccountModel, AccountModel accountModel);
    }
    public class InstaAdsScrappers : IInstaAdScrappers
    {
        static readonly object lockerRequest = new object();
        static readonly object PostaDataAsJsonlockerRequest = new object();
        public IGdHttpHelper httpHelper;
        IInstaFunction instaFunct;
        IGdBrowserManager AdBrowserManager;
        IAccountScopeFactory AccountScopeFactory;
        public static Dictionary<string, string> AlreadyRunningDataScrapping = new Dictionary<string, string>();
        public InstaAdsScrappers(IGdHttpHelper HttpHelper, IInstaFunction instaFunction)
        {
            httpHelper = HttpHelper;
            instaFunct = instaFunction;
        }
        public void InstaDataScraper(DominatorAccountModel dominatorAccountModel, AccountModel accountModel)
        {
            
            if (AlreadyRunningDataScrapping.ContainsKey(dominatorAccountModel.UserName))
                return;

            if (dominatorAccountModel.IsRunProcessThroughBrowser)
            {
                AccountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                AdBrowserManager = AccountScopeFactory[$"{dominatorAccountModel.AccountId}_AdsScrapper"].Resolve<IGdBrowserManager>();
                var isLoggedIn = AdBrowserManager.BrowserLogin(dominatorAccountModel, CancellationToken.None,LoginType.InitialiseBrowser);
                if(isLoggedIn && string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.UserId))
                {
                    //var userInfo = AdBrowserManager.GetUserInfo(dominatorAccountModel, dominatorAccountModel.UserName, CancellationToken.None);
                    var userInfo = instaFunct.SearchUsername(dominatorAccountModel, dominatorAccountModel.UserName, CancellationToken.None);
                    dominatorAccountModel.AccountBaseModel.UserId = userInfo.instaUserDetails.Pk;
                }
            }
            #region Variable                 
            InstaFeedData instaFeedData = new InstaFeedData();
            List<string> lst_Id = new List<string>();
            string country = string.Empty;
            string gender = string.Empty;
            CommonIgResponseHandler FeedResponse;
            string DynamicContent;
            string city = string.Empty;
            string state = string.Empty;
            string accountMaxid = string.Empty;
            int Pagecount = 0;
            #endregion
            #region  Getting Feeds Ads
            try
            {
                AlreadyRunningDataScrapping.Add(dominatorAccountModel.UserName, "Active");

                GetGender(ref gender, ref city, ref state, ref country, dominatorAccountModel);
                CheckAdsUsingApi(dominatorAccountModel, gender, country);
                string url = "https://i.instagram.com/api/v1/feed/timeline/";
                if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    
                    FeedResponse = instaFunct.GetFeedTimeLineData(dominatorAccountModel, accountModel);
                    if (FeedResponse == null || !FeedResponse.Success)
                    {
                        SleepRandomMilliseconds(5500, 5000); //Thread.Sleep(5 * 1000);
                        FeedResponse = instaFunct.GetFeedTimeLineData(dominatorAccountModel, accountModel);
                    }
                    do
                    {
                        DynamicContent = "is_prefetch=0&feed_view_info=[{\"media_id\":\"";
                        for (int pd = 0; pd < instaFeedData.listid.Count; pd++)
                        {
                            if (pd == instaFeedData.listid.Count - 1)
                            {
                                DynamicContent += instaFeedData.listid[pd] + "\",\"media_pct\":1,\"time_info\":{\"75\":" + GetRandom() + ",\"10\":" + GetRandom() + ",\"50\":" + GetRandom() + ",\"25\":" + GetRandom() + "},\"ts\":" + instaFeedData.listTimestamp[pd] + ",\"media_height\":" + instaFeedData.ImageList[pd] + "}]&latest_story_pk="
                               + instaFeedData.listid[0] + "&phone_id=" + dominatorAccountModel.DeviceDetails.PhoneId
                               + "&max_id=" + accountMaxid + "&reason=pagination" + "&device_id="
                               + dominatorAccountModel.DeviceDetails.DeviceId + "&_uuid="
                               + accountModel.Uuid;
                            }
                            else
                                DynamicContent += instaFeedData.listid[pd] + "\",\"media_pct\":1,\"time_info\":{\"75\":" + GetRandom() + ",\"10\":" + GetRandom() + ",\"50\":" + GetRandom() + ",\"25\":" + GetRandom() + "},\"ts\":" + instaFeedData.listTimestamp[pd] + ",\"media_height\":" + instaFeedData.ImageList[pd] + "},{\"media_id\":\"";
                        }
                        if (!string.IsNullOrEmpty(accountMaxid))
                        {
                            CommonIgResponseHandler mobilePageSrcNews = new CommonIgResponseHandler(httpHelper.PostRequest(url, DynamicContent));
                            FeedResponse = mobilePageSrcNews;
                        }
                        instaFeedData = GetFeedAds(FeedResponse.ToString(), dominatorAccountModel, accountModel, city, state, country);
                        if (!string.IsNullOrEmpty(instaFeedData.maxId))
                        {
                            accountMaxid = instaFeedData.maxId;
                            SleepRandomMilliseconds(5500, 5000); //Thread.Sleep(TimeSpan.FromSeconds(5)); // Thread.Sleep(TimeSpan.FromMinutes(5));
                            foreach (string userListId in instaFeedData.listid)
                            {
                                var ids = userListId.Split('_').ToList();
                                lst_Id.Add(ids[1]);
                            }

                            CheckStoryAdsUsingApi(dominatorAccountModel, gender, country);
                        againhit:
                            SleepRandomMilliseconds(7500, 7000); //Thread.Sleep(TimeSpan.FromSeconds(7));
                            StoryAdsResponse storyAdsResponse = instaFunct.GetStoryAds(dominatorAccountModel, accountModel, lst_Id);
                            if (storyAdsResponse == null)
                            {
                                SleepRandomMilliseconds(5500, 5000); //Thread.Sleep(TimeSpan.FromSeconds(5));
                                continue;
                            }
                            if (storyAdsResponse.ToString().Contains("You Were Logged Out"))
                                return;
                            if (storyAdsResponse == null)
                            {
                                SleepRandomMilliseconds(5500, 5000); //Thread.Sleep(TimeSpan.FromSeconds(5));// Thread.Sleep(TimeSpan.FromMinutes(5));//Thread.Sleep(TimeSpan.FromSeconds(5));
                                goto againhit;
                            }
                            GetStoryAds(storyAdsResponse.lstStoryDetails, storyAdsResponse, dominatorAccountModel, accountModel, city, state, country);
                            SleepRandomMilliseconds(5500, 5000); //Thread.Sleep(TimeSpan.FromSeconds(5)); //Thread.Sleep(TimeSpan.FromMinutes(5)); //
                            Pagecount++;
                            if (Pagecount == 20)
                                return;
                        }
                    } while (!string.IsNullOrEmpty(instaFeedData.maxId));
                }
                else
                {
                    FeedResponse = AdBrowserManager.GetFeedTimeLineData(dominatorAccountModel, CancellationToken.None);
                    var dataList = new List<string>();
                    var insertedAds = new List<string>();
                    dataList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(FeedResponse.ToString());
                    foreach(var data in dataList)
                    {
                        instaFeedData = GetFeedAdsBrowser(data.ToString(), dominatorAccountModel, accountModel, city, state, country,insertedAds);
                        insertedAds.AddRange(instaFeedData.InsertedAdsId);
                    }
                }
                

                
                AlreadyRunningDataScrapping.Remove(dominatorAccountModel.UserName);
            }
            catch (Exception)
            {
                //removing Account From here, when any exception comes
                AlreadyRunningDataScrapping.Remove(dominatorAccountModel.UserName);
                // ignored
            }
            finally
            {
                if (dominatorAccountModel.IsRunProcessThroughBrowser)
                    AdBrowserManager.CloseBrowser();
            }

            #endregion

        }

        public InstaFeedData GetFeedAds(string FeedResponse, DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string city, string state, string country)
        {
            #region local variable
            InstaFeedData objJsonFeedData = new InstaFeedData();
            string max_Id;
            string mediaTypeInNumberFormat;
            List<ResultFeedTimeLineData> listResultFeedTimeLineData = new List<ResultFeedTimeLineData>();
            List<InstaFeedTimeLineData> listFeedTimeLineData;
            List<string> listId = new List<string>();
            List<string> listheight = new List<string>();
            List<string> listTimestamp = new List<string>();
            JObject Resp;
            Resp = JObject.Parse(FeedResponse);
            string label = string.Empty;
            #endregion
            #region  Functionality
            try
            {
                max_Id = Resp["next_max_id"].ToString();
                foreach (JToken jtoken in Resp["feed_items"])
                {
                    #region Local varible 
                    InstaFeedTimeLineData objFeedTimeLineDatas = new InstaFeedTimeLineData();
                    ResultFeedTimeLineData objResultFeedLineData = new ResultFeedTimeLineData();
                    SortedDictionary<string, string> AdsData = new SortedDictionary<string, string>();
                    string AlbumType = string.Empty;
                    var postOwnerName = false;
                    var descriptionlink = false;
                    var ImageVideoUrl = false;
                    var callToAction = false;
                    #endregion
                    try
                    {
                        #region Getting Feed Data for pagination
                        try
                        {
                            objFeedTimeLineDatas.mediaId = jtoken["media_or_ad"]["id"].ToString();
                            objFeedTimeLineDatas.post_date = jtoken["media_or_ad"]["taken_at"].ToString();
                            try
                            {
                                objFeedTimeLineDatas.ImageHeight = jtoken["media_or_ad"]["image_versions2"]["candidates"][0]["height"].ToString();
                            }
                            catch (Exception)
                            {
                                objFeedTimeLineDatas.ImageHeight = jtoken["media_or_ad"]["carousel_media"][0]["image_versions2"]["candidates"][0]["height"].ToString();
                            }

                            listheight.Add(objFeedTimeLineDatas.ImageHeight);
                            listId.Add(objFeedTimeLineDatas.mediaId);
                            listTimestamp.Add(objFeedTimeLineDatas.post_date);
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                        #endregion
                        // string label = jtoken["media_or_ad"]["injected"]["label"].ToString();
                        try
                        {
                            label = jtoken["media_or_ad"]["injected"]["label"].ToString();//["injected"]["label"].ToString();
                        }
                        catch (Exception)
                        {

                            continue;
                        }
                        if (!string.IsNullOrEmpty(label))
                        {
                            #region ad Id
                            objFeedTimeLineDatas.ad_id = jtoken["media_or_ad"]["injected"]["ad_id"].ToString();
                            //AdsData.Add("ad_id", objFeedTimeLineDatas.ad_id);
                            
                            #endregion
                            #region Post date
                            objFeedTimeLineDatas.post_date = jtoken["media_or_ad"]["taken_at"].ToString();
                            AdsData.Add("post_date", objFeedTimeLineDatas.post_date);
                            #endregion                          
                            #region Image code
                            objFeedTimeLineDatas.code = jtoken["media_or_ad"]["code"] != null ? jtoken["media_or_ad"]["code"].ToString() : "";
                            AdsData.Add("ad_id", objFeedTimeLineDatas.code);
                            #endregion
                            #region post Owner name

                            try
                            {
                                if (!postOwnerName)
                                {
                                    objFeedTimeLineDatas.post_owner = jtoken["media_or_ad"]["user"]["full_name"].ToString();
                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.post_owner))
                                    {
                                        string[] data = objFeedTimeLineDatas.post_owner.Split(' ');
                                        int pdata = data.Length;
                                        int errorCounter = Regex.Matches(data[pdata - 1], @"[a-zA-Z]").Count;
                                        string postownernames = string.Empty;
                                        if (errorCounter == 0)
                                        {
                                            for (int i = 0; i <= pdata - 2; i++)
                                            {
                                                postownernames = postownernames + " " + data[i];
                                            }
                                        }
                                        else
                                        {
                                            AdsData.Add("post_owner", objFeedTimeLineDatas.post_owner);
                                            postOwnerName = true;
                                        }
                                        if (!string.IsNullOrEmpty(postownernames))
                                        {
                                            AdsData.Add("post_owner", postownernames);
                                            postOwnerName = true;
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                postOwnerName = false;
                            }
                            try
                            {
                                if (!postOwnerName)
                                {
                                    if (string.IsNullOrEmpty(objFeedTimeLineDatas.post_owner) || objFeedTimeLineDatas.post_owner == null)
                                        objFeedTimeLineDatas.post_owner = jtoken["media_or_ad"]["user"]["username"].ToString();
                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.post_owner))
                                    {
                                        AdsData.Add("post_owner", objFeedTimeLineDatas.post_owner);
                                        postOwnerName = true;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                postOwnerName = false;
                            }

                            try
                            {
                                if (!postOwnerName)
                                    AdsData.Add("post_owner", "Post Owner");
                            }
                            catch (Exception)
                            {
                                // ignored
                            }

                            #endregion
                            #region Post Owner Image
                            try
                            {
                                objFeedTimeLineDatas.post_owner_image = jtoken["media_or_ad"]["user"]["profile_pic_url"].ToString();
                                AdsData.Add("post_owner_image", objFeedTimeLineDatas.post_owner_image);
                            }
                            catch (Exception)
                            {
                                objFeedTimeLineDatas.post_owner_image = "";
                                AdsData.Add("post_owner_image", objFeedTimeLineDatas.post_owner_image);
                            }
                            #endregion
                            #region Image Type
                            objFeedTimeLineDatas.MediaType = jtoken["media_or_ad"]["media_type"].ToString();
                            mediaTypeInNumberFormat = objFeedTimeLineDatas.MediaType;
                            objFeedTimeLineDatas.MediaType = objFeedTimeLineDatas.MediaType.Contains("1") ? "IMAGE" : objFeedTimeLineDatas.MediaType.Contains("2") ? "VIDEO" : "IMAGE";
                            if (objFeedTimeLineDatas.MediaType.Contains("VIDEO") || mediaTypeInNumberFormat.Contains("2"))
                            {

                            }
                            if (mediaTypeInNumberFormat.Contains("8"))
                                AlbumType = "Album";
                            AdsData.Add("type", objFeedTimeLineDatas.MediaType);
                            #endregion
                            #region Post video Url
                            try
                            {
                                if (objFeedTimeLineDatas.MediaType.Contains("VIDEO"))
                                {
                                    if (!ImageVideoUrl)
                                    {
                                        objFeedTimeLineDatas.image_video_url = jtoken["media_or_ad"]["video_versions"][0]["url"].ToString();
                                        if (objFeedTimeLineDatas.image_video_url.Contains(".mp4"))
                                        {
                                            if (!string.IsNullOrEmpty(objFeedTimeLineDatas.image_video_url))
                                            {
                                                AdsData.Add("image_video_url", objFeedTimeLineDatas.image_video_url);
                                                ImageVideoUrl = true;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                ImageVideoUrl = false;
                            }
                            try
                            {
                                if (objFeedTimeLineDatas.MediaType.Contains("IMAGE") && string.IsNullOrEmpty(AlbumType))
                                {
                                    if (!ImageVideoUrl)
                                    {
                                        objFeedTimeLineDatas.image_video_url = jtoken["media_or_ad"]["image_versions2"]["candidates"][0]["url"].ToString();
                                        if (!string.IsNullOrEmpty(objFeedTimeLineDatas.image_video_url))
                                        {
                                            AdsData.Add("image_video_url", objFeedTimeLineDatas.image_video_url);
                                            ImageVideoUrl = true;
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                ImageVideoUrl = false;
                            }
                            try
                            {
                                if (objFeedTimeLineDatas.MediaType.Contains("IMAGE") && string.IsNullOrEmpty(AlbumType))
                                {
                                    if (!ImageVideoUrl)
                                    {
                                        objFeedTimeLineDatas.image_video_url = jtoken["media_or_ad"]["collection_media"][0]["image_versions2"]["candidates"][0]["url"].ToString();
                                        if (!string.IsNullOrEmpty(objFeedTimeLineDatas.image_video_url))
                                        {
                                            AdsData.Add("image_video_url", objFeedTimeLineDatas.image_video_url);
                                            ImageVideoUrl = true;
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                ImageVideoUrl = false;
                            }
                            try
                            {
                                if (AlbumType.Contains("Album"))
                                {
                                    if (!ImageVideoUrl)
                                    {
                                        int imageCount = 0;
                                        foreach (JToken CrouserMedia in jtoken["media_or_ad"]["carousel_media"])
                                        {
                                            // int cm = CrouserMedia["image_versions2"]["candidates"].Count();
                                            int cm = jtoken["media_or_ad"]["carousel_media"].Count();
                                            //if (string.IsNullOrEmpty(objFeedTimeLineDatas.image_video_url))
                                            //{

                                            foreach (JToken crouserImage in CrouserMedia["image_versions2"]["candidates"])
                                            {
                                                //if (cm > 1)
                                                //    objFeedTimeLineDatas.image_video_url += crouserImage["url"] + "||";
                                                //else
                                                //    objFeedTimeLineDatas.image_video_url = crouserImage["url"].ToString();
                                                if (imageCount == 0 && cm > 1)
                                                {
                                                    objFeedTimeLineDatas.image_video_url += crouserImage["url"].ToString();
                                                    imageCount++;
                                                }
                                                else if (imageCount > 0 && cm > 1)
                                                {
                                                    objFeedTimeLineDatas.other_multimedia += crouserImage["url"] + "||";
                                                }
                                                else
                                                    objFeedTimeLineDatas.image_video_url = crouserImage["url"].ToString();

                                                break;
                                            }

                                            //}
                                        }
                                        if (objFeedTimeLineDatas.other_multimedia.Contains("||"))
                                        {
                                            string urll = objFeedTimeLineDatas.other_multimedia;
                                            objFeedTimeLineDatas.other_multimedia = urll.Substring(0, objFeedTimeLineDatas.other_multimedia.Length - 2);
                                        }
                                        if (objFeedTimeLineDatas.image_video_url.Contains(".mp4"))
                                            AdsData["type"] = "VIDEO";

                                        if (!string.IsNullOrEmpty(objFeedTimeLineDatas.image_video_url) || objFeedTimeLineDatas.image_video_url == null)
                                        {
                                            AdsData.Add("image_video_url", objFeedTimeLineDatas.image_video_url);
                                            AdsData.Add("other_multimedia", objFeedTimeLineDatas.other_multimedia);
                                            // ReSharper disable once RedundantAssignment
                                            ImageVideoUrl = true;
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                // ReSharper disable once RedundantAssignment
                                ImageVideoUrl = false;
                            }

                            #endregion                            
                            #region Description Link

                            try
                            {
                                if (!descriptionlink)
                                {
                                    objFeedTimeLineDatas.destination_url = jtoken["media_or_ad"]["android_links"][0]["redirectUri"].ToString();
                                    if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                    {
                                        objFeedTimeLineDatas.destination_url = jtoken["media_or_ad"]["android_links"][1]["redirectUri"].ToString();
                                    }
                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                    {
                                        AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                                        descriptionlink = true;
                                    }
                                }
                            }
                            catch (Exception)
                            {

                                descriptionlink = false;
                            }
                            try
                            {
                                if (!descriptionlink)
                                {

                                    if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                    {
                                        foreach (JToken jt in jtoken["media_or_ad"]["android_links"])
                                        {
                                            if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url) || objFeedTimeLineDatas.destination_url == null)
                                                objFeedTimeLineDatas.destination_url = jt["webUri"].ToString();

                                        }
                                    }
                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                    {
                                        AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                                        descriptionlink = true;
                                    }
                                }
                            }
                            catch (Exception)
                            {

                                descriptionlink = false;
                            }
                            try
                            {

                                if (!descriptionlink)
                                {
                                    if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                    {
                                        foreach (JToken CrouserMedia in jtoken["media_or_ad"]["carousel_media"])
                                        {
                                            if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url) || objFeedTimeLineDatas.destination_url == null)
                                            {
                                                objFeedTimeLineDatas.destination_url = CrouserMedia["android_links"]["webUri"].ToString();

                                            }
                                        }
                                    }
                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                    {
                                        AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                                        descriptionlink = true;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                descriptionlink = false;
                            }

                            try
                            {
                                if (!descriptionlink)
                                {
                                    if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                    {
                                        objFeedTimeLineDatas.destination_url = jtoken["media_or_ad"]["link"].ToString();
                                        //objFeedTimeLineDatas.destination_url=HttpUtility.UrlEncode(objFeedTimeLineDatas.destination_url);
                                    }
                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                    {
                                        AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                                        descriptionlink = true;
                                    }
                                }

                            }
                            catch (Exception)
                            {

                                descriptionlink = false;
                            }
                            try
                            {
                                if (!descriptionlink)
                                {
                                    if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                    {
                                        foreach (JToken CrouserMedia in jtoken["media_or_ad"]["carousel_media"])
                                        {
                                            if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url) || objFeedTimeLineDatas.destination_url == null)
                                            {
                                                objFeedTimeLineDatas.destination_url = CrouserMedia["link"].ToString();
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                        {
                                            AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                                            descriptionlink = true;
                                        }
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                descriptionlink = false;
                            }
                            try
                            {
                                if (!descriptionlink)
                                {
                                    objFeedTimeLineDatas.destination_url = "www.instagram.com/p/" + objFeedTimeLineDatas.code + "/";
                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                    {
                                        AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                                        // ReSharper disable once RedundantAssignment
                                        descriptionlink = true;
                                    }
                                }

                            }
                            catch
                            {
                                // ignored
                            }

                            #endregion
                            #region Call To Action
                            try
                            {
                                objFeedTimeLineDatas.call_to_action = jtoken["media_or_ad"]["headline"]["text"].ToString();
                                if (!string.IsNullOrEmpty(objFeedTimeLineDatas.call_to_action))
                                {
                                    // objFeedTimeLineDatas.call_to_action= HttpUtility.UrlEncode(objFeedTimeLineDatas.call_to_action);
                                    AdsData.Add("call_to_action", objFeedTimeLineDatas.call_to_action);
                                    callToAction = true;
                                }

                            }
                            catch (Exception)
                            {
                                callToAction = false;
                            }
                            try
                            {
                                if (!callToAction)
                                {
                                    if (string.IsNullOrEmpty(objFeedTimeLineDatas.call_to_action))
                                        objFeedTimeLineDatas.call_to_action = jtoken["media_or_ad"]["link_text"].ToString();

                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.call_to_action))
                                    {
                                        //objFeedTimeLineDatas.call_to_action=HttpUtility.UrlEncode(objFeedTimeLineDatas.call_to_action);
                                        AdsData.Add("call_to_action", objFeedTimeLineDatas.call_to_action);
                                        callToAction = true;
                                    }
                                }

                            }
                            catch (Exception)
                            {
                                callToAction = false;
                            }
                            try
                            {
                                if (!callToAction)
                                {
                                    foreach (JToken calltoAction in jtoken["media_or_ad"]["carousel_media"])
                                    {
                                        if (string.IsNullOrEmpty(objFeedTimeLineDatas.call_to_action))
                                            objFeedTimeLineDatas.call_to_action = calltoAction["link_text"].ToString();

                                    }
                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.call_to_action))
                                    {
                                        AdsData.Add("call_to_action", objFeedTimeLineDatas.call_to_action);
                                        // ReSharper disable once RedundantAssignment
                                        callToAction = true;
                                    }
                                }

                            }
                            catch (Exception)
                            {
                                // ReSharper disable once RedundantAssignment
                                callToAction = false;
                            }
                            if (!callToAction)
                            {
                                AdsData.Add("call_to_action", "");
                            }
                            #endregion
                            #region news Feed Description
                            objFeedTimeLineDatas.news_feed_description = jtoken["media_or_ad"]["caption"]["text"].ToString();
                            // objFeedTimeLineDatas.news_feed_description=HttpUtility.UrlEncode(objFeedTimeLineDatas.news_feed_description);
                            AdsData.Add("news_feed_description", objFeedTimeLineDatas.news_feed_description);
                            #endregion                     
                            #region PostId Url
                            objFeedTimeLineDatas.ad_url = "www.instagram.com/p/" + objFeedTimeLineDatas.code + "/";
                            // objFeedTimeLineDatas.ad_url= HttpUtility.UrlEncode(objFeedTimeLineDatas.ad_url);
                            AdsData.Add("ad_url", objFeedTimeLineDatas.ad_url);
                            #endregion
                            #region Like count
                            objFeedTimeLineDatas.likes = jtoken["media_or_ad"]["like_count"].ToString();
                            AdsData.Add("likes", objFeedTimeLineDatas.likes);
                            #endregion
                            #region comment Count
                            objFeedTimeLineDatas.comment = jtoken["media_or_ad"]["comment_count"].ToString();
                            AdsData.Add("comment", objFeedTimeLineDatas.comment);
                            #endregion
                            #region other postdata
                            AdsData.Add("ad_title", "");
                            //AdsData.Add("version", "1.0.11");
                            AdsData.Add("version", Constants.SocinatorLatestVersion);
                            AdsData.Add("side_url", "Not implemented yet");
                            AdsData.Add("platform", "1");
                            AdsData.Add("upper_age", "30");
                            AdsData.Add("lower_age", "18");
                            AdsData.Add("category", "Software");
                            AdsData.Add("first_seen", "12312121");
                            AdsData.Add("last_seen", "46545454");
                            AdsData.Add("ad_position", "FEED");
                            AdsData.Add("instagram_id", dominatorAccountModel.AccountBaseModel.UserId);
                            AdsData.Add("ad_text", "");
                            AdsData.Add("share", "0");
                            #endregion
                            #region city state country
                            AdsData.Add("city", city);
                            AdsData.Add("state", state);
                            AdsData.Add("country", country);
                            #endregion
                            #region  Hitting PowerAdSpy Api
                            objResultFeedLineData.objFeedTimeLineData = objFeedTimeLineDatas;
                            listResultFeedTimeLineData.Add(objResultFeedLineData);
                            try
                            {
                                string mainApi = "https://gramapi.poweradspy.com/instaAdsData";
                                //string devApi="https://instaapi.poweradspy.com/instaAdsData";
                                //string testApi= "http://instaapi.test.poweradspy.com/instaAdsData";
                                //string mainApi = "https://instagram-dev.poweradspy.com/instaAdsData";
                                //var seriliazePayload = JsonConvert.SerializeObject(AdsData);
                                string res = PostDataAsJson(mainApi, AdsData);
                                if (string.IsNullOrEmpty(res))
                                {
                                    SleepRandomMilliseconds(5500, 5000); //Thread.Sleep(5 * 1000);
                                    res = PostDataAsJson(mainApi, AdsData);
                                }
                            }
                            catch (Exception)
                            {
                                // ignored
                            }

                            #endregion
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                #region Adding data in list
                listFeedTimeLineData = listResultFeedTimeLineData.Select(x => x.objFeedTimeLineData).ToList();
                objJsonFeedData.listUserFeedInfo = listFeedTimeLineData;
                objJsonFeedData.listid = listId;
                objJsonFeedData.listTimestamp = listTimestamp;
                objJsonFeedData.ImageList = listheight;
                if (!string.IsNullOrEmpty(max_Id))
                    objJsonFeedData.maxId = max_Id;
                #endregion
            }
            catch (Exception)
            {
                // ignored
            }

            #endregion
            return objJsonFeedData;
        }

        public void GetStoryAds(List<StoryAdsDetails> storyList, StoryAdsResponse storyAdResponse, DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string city, string state, string country)
        {
            try
            {
                foreach (StoryAdsDetails data in storyList)
                {
                    if (string.IsNullOrEmpty(data.code))
                        continue;
                    SortedDictionary<string, string> AdsData = new SortedDictionary<string, string>
                    {
                        {"ad_id", data.code}, {"post_date", data.post_date}
                    };
                    var owner = DetectEmoji(data.postOwner);

                    AdsData.Add("post_owner", owner);
                    AdsData.Add("post_owner_image", data.postOwnerImage);
                    string mediaTypeInNumber = data.mediaType;
                    string mType = mediaTypeInNumber.Contains("1") ? "IMAGE" : mediaTypeInNumber.Contains("2") ? "VIDEO" : "IMAGE";
                    AdsData.Add("type", mType);
                    AdsData.Add("image_video_url", data.image_video_url);
                    AdsData.Add("destination_url", data.discriptionLink);
                    AdsData.Add("call_to_action", data.callToAction);
                    var discription = DetectEmoji(data.newsFeedDescription);
                    AdsData.Add("news_feed_description", discription);
                    string url = "www.instagram.com/p/" + data.code + "/";
                    AdsData.Add("ad_url", url);
                    AdsData.Add("likes", data.likeCount);
                    AdsData.Add("comment", data.CommentCount);
                    AdsData.Add("ad_title", "");
                    //AdsData.Add("version", "1.0.11");
                    AdsData.Add("version",Constants.SocinatorLatestVersion);
                    AdsData.Add("side_url", "Not implemented yet");
                    AdsData.Add("platform", "1");
                    AdsData.Add("upper_age", "30");
                    AdsData.Add("lower_age", "18");
                    AdsData.Add("category", "Software");
                    AdsData.Add("first_seen", "12312121");
                    AdsData.Add("last_seen", "46545454");
                    AdsData.Add("ad_position", "FEED");
                    AdsData.Add("instagram_id", dominatorAccountModel.AccountBaseModel.UserId ?? accountModel.DsUserId);
                    AdsData.Add("ad_text", "");
                    AdsData.Add("share", "0");
                    AdsData.Add("city", city);
                    AdsData.Add("state", state);
                    AdsData.Add("country", country);
                    AdsData.Add("ad_type", "1");
                    if (mType.Contains("VIDEO"))
                    {

                    }
                    string mainApi = "https://gramapi.poweradspy.com/instaAdsData";
                    //string devapi = "https://instaapi.poweradspy.com/instaAdsData";
                    //string testapi = "http://instaapi.test.poweradspy.com/instaAdsData";
                    //string mainApi =   "https://instagram-dev.poweradspy.com/instaAdsData";
                    // var seriliazePayload = JsonConvert.SerializeObject(AdsData);
                    string res = PostDataAsJson(mainApi, AdsData);
                    //  string idFromCodes = CheckPostId(data.code);
                    //  var mediaInfoResponse = instaFunct.MediaInfo(dominatorAccountModel, accountModel, idFromCodes, CancellationToken.None);
                    //var mediaComments=  instaFunct.GetMediaComments(dominatorAccountModel,idFromCodes,CancellationToken.None,"");
                    //  var mediaLikes = instaFunct.GetMediaLikers(dominatorAccountModel,idFromCodes, CancellationToken.None, "");

                    if (string.IsNullOrEmpty(res))
                    {
                        SleepRandomMilliseconds(5500, 5000); //Thread.Sleep(TimeSpan.FromSeconds(5));
                        res = PostDataAsJson(mainApi, AdsData);
                        // Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public string DetectEmoji(string postOwner)
        {
            var EmojiPattern = @"(?:\uD83D(?:\uDD73\uFE0F?|\uDC41(?:(?:\uFE0F(?:\u200D\uD83D\uDDE8\uFE0F?)?|\u200D\uD83D\uDDE8\uFE0F?))?|[\uDDE8\uDDEF]\uFE0F?|\uDC4B(?:\uD83C[\uDFFB-\uDFFF])?|\uDD90(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|[\uDD96\uDC4C\uDC48\uDC49\uDC46\uDD95\uDC47\uDC4D\uDC4E\uDC4A\uDC4F\uDE4C\uDC50\uDE4F\uDC85\uDCAA\uDC42\uDC43\uDC76\uDC66\uDC67](?:\uD83C[\uDFFB-\uDFFF])?|\uDC71(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2640\u2642]\uFE0F?))?)|\u200D(?:[\u2640\u2642]\uFE0F?)))?|\uDC68(?:(?:\uD83C(?:\uDFFB(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFC-\uDFFF]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFC(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB\uDFFD-\uDFFF]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFD(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB\uDFFC\uDFFE\uDFFF]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFE(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB-\uDFFD\uDFFF]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFF(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D\uDC68\uD83C[\uDFFB-\uDFFE]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?)|\u200D(?:\uD83E[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD]|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D(?:\uDC69\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC68\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?|[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92])|\u2708\uFE0F?|\u2764(?:\uFE0F\u200D\uD83D(?:\uDC8B\u200D\uD83D\uDC68|\uDC68)|\u200D\uD83D(?:\uDC8B\u200D\uD83D\uDC68|\uDC68)))))?|\uDC69(?:(?:\uD83C(?:\uDFFB(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D(?:\uDC69\uD83C[\uDFFC-\uDFFF]|\uDC68\uD83C[\uDFFC-\uDFFF])|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFC(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D(?:\uDC69\uD83C[\uDFFB\uDFFD-\uDFFF]|\uDC68\uD83C[\uDFFB\uDFFD-\uDFFF])|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFD(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D(?:\uDC69\uD83C[\uDFFB\uDFFC\uDFFE\uDFFF]|\uDC68\uD83C[\uDFFB\uDFFC\uDFFE\uDFFF])|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFE(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D(?:\uDC69\uD83C[\uDFFB-\uDFFD\uDFFF]|\uDC68\uD83C[\uDFFB-\uDFFD\uDFFF])|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?|\uDFFF(?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83D(?:\uDC69\uD83C[\uDFFB-\uDFFE]|\uDC68\uD83C[\uDFFB-\uDFFE])|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?)|\u200D(?:\uD83E[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD]|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D(?:\uDC69\u200D\uD83D(?:\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?)|\uDC66(?:\u200D\uD83D\uDC66)?|\uDC67(?:\u200D\uD83D[\uDC66\uDC67])?|[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92])|\u2708\uFE0F?|\u2764(?:\uFE0F\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])|\u200D\uD83D(?:\uDC8B\u200D\uD83D[\uDC68\uDC69]|[\uDC68\uDC69])))))?|[\uDC74\uDC75](?:\uD83C[\uDFFB-\uDFFF])?|[\uDE4D\uDE4E\uDE45\uDE46\uDC81\uDE4B\uDE47\uDC6E](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDD75(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC82\uDC77](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDC78(?:\uD83C[\uDFFB-\uDFFF])?|\uDC73(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC72\uDC70\uDC7C](?:\uD83C[\uDFFB-\uDFFF])?|[\uDC86\uDC87\uDEB6](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDC83\uDD7A](?:\uD83C[\uDFFB-\uDFFF])?|\uDD74(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|\uDC6F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|[\uDEA3\uDEB4\uDEB5](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDEC0\uDECC\uDC6D\uDC6B\uDC6C](?:\uD83C[\uDFFB-\uDFFF])?|\uDDE3\uFE0F?|\uDC15(?:\u200D\uD83E\uDDBA)?|[\uDC3F\uDD4A\uDD77\uDD78\uDDFA\uDEE3\uDEE4\uDEE2\uDEF3\uDEE5\uDEE9\uDEF0\uDECE\uDD70\uDD79\uDDBC\uDD76\uDECD\uDDA5\uDDA8\uDDB1\uDDB2\uDCFD\uDD6F\uDDDE\uDDF3\uDD8B\uDD8A\uDD8C\uDD8D\uDDC2\uDDD2\uDDD3\uDD87\uDDC3\uDDC4\uDDD1\uDDDD\uDEE0\uDDE1\uDEE1\uDDDC\uDECF\uDECB\uDD49]\uFE0F?|[\uDE00\uDE03\uDE04\uDE01\uDE06\uDE05\uDE02\uDE42\uDE43\uDE09\uDE0A\uDE07\uDE0D\uDE18\uDE17\uDE1A\uDE19\uDE0B\uDE1B-\uDE1D\uDE10\uDE11\uDE36\uDE0F\uDE12\uDE44\uDE2C\uDE0C\uDE14\uDE2A\uDE34\uDE37\uDE35\uDE0E\uDE15\uDE1F\uDE41\uDE2E\uDE2F\uDE32\uDE33\uDE26-\uDE28\uDE30\uDE25\uDE22\uDE2D\uDE31\uDE16\uDE23\uDE1E\uDE13\uDE29\uDE2B\uDE24\uDE21\uDE20\uDE08\uDC7F\uDC80\uDCA9\uDC79-\uDC7B\uDC7D\uDC7E\uDE3A\uDE38\uDE39\uDE3B-\uDE3D\uDE40\uDE3F\uDE3E\uDE48-\uDE4A\uDC8B\uDC8C\uDC98\uDC9D\uDC96\uDC97\uDC93\uDC9E\uDC95\uDC9F\uDC94\uDC9B\uDC9A\uDC99\uDC9C\uDDA4\uDCAF\uDCA2\uDCA5\uDCAB\uDCA6\uDCA8\uDCA3\uDCAC\uDCAD\uDCA4\uDC40\uDC45\uDC44\uDC8F\uDC91\uDC6A\uDC64\uDC65\uDC63\uDC35\uDC12\uDC36\uDC29\uDC3A\uDC31\uDC08\uDC2F\uDC05\uDC06\uDC34\uDC0E\uDC2E\uDC02-\uDC04\uDC37\uDC16\uDC17\uDC3D\uDC0F\uDC11\uDC10\uDC2A\uDC2B\uDC18\uDC2D\uDC01\uDC00\uDC39\uDC30\uDC07\uDC3B\uDC28\uDC3C\uDC3E\uDC14\uDC13\uDC23-\uDC27\uDC38\uDC0A\uDC22\uDC0D\uDC32\uDC09\uDC33\uDC0B\uDC2C\uDC1F-\uDC21\uDC19\uDC1A\uDC0C\uDC1B-\uDC1E\uDC90\uDCAE\uDD2A\uDDFE\uDDFB\uDC92\uDDFC\uDDFD\uDD4C\uDED5\uDD4D\uDD4B\uDC88\uDE82-\uDE8A\uDE9D\uDE9E\uDE8B-\uDE8E\uDE90-\uDE9C\uDEF5\uDEFA\uDEB2\uDEF4\uDEF9\uDE8F\uDEA8\uDEA5\uDEA6\uDED1\uDEA7\uDEF6\uDEA4\uDEA2\uDEEB\uDEEC\uDCBA\uDE81\uDE9F-\uDEA1\uDE80\uDEF8\uDD5B\uDD67\uDD50\uDD5C\uDD51\uDD5D\uDD52\uDD5E\uDD53\uDD5F\uDD54\uDD60\uDD55\uDD61\uDD56\uDD62\uDD57\uDD63\uDD58\uDD64\uDD59\uDD65\uDD5A\uDD66\uDD25\uDCA7\uDEF7\uDD2E\uDC53-\uDC62\uDC51\uDC52\uDCFF\uDC84\uDC8D\uDC8E\uDD07-\uDD0A\uDCE2\uDCE3\uDCEF\uDD14\uDD15\uDCFB\uDCF1\uDCF2\uDCDE-\uDCE0\uDD0B\uDD0C\uDCBB\uDCBD-\uDCC0\uDCFA\uDCF7-\uDCF9\uDCFC\uDD0D\uDD0E\uDCA1\uDD26\uDCD4-\uDCDA\uDCD3\uDCD2\uDCC3\uDCDC\uDCC4\uDCF0\uDCD1\uDD16\uDCB0\uDCB4-\uDCB8\uDCB3\uDCB9\uDCB1\uDCB2\uDCE7-\uDCE9\uDCE4-\uDCE6\uDCEB\uDCEA\uDCEC-\uDCEE\uDCDD\uDCBC\uDCC1\uDCC2\uDCC5-\uDCD0\uDD12\uDD13\uDD0F-\uDD11\uDD28\uDD2B\uDD27\uDD29\uDD17\uDD2C\uDD2D\uDCE1\uDC89\uDC8A\uDEAA\uDEBD\uDEBF\uDEC1\uDED2\uDEAC\uDDFF\uDEAE\uDEB0\uDEB9-\uDEBC\uDEBE\uDEC2-\uDEC5\uDEB8\uDEAB\uDEB3\uDEAD\uDEAF\uDEB1\uDEB7\uDCF5\uDD1E\uDD03\uDD04\uDD19-\uDD1D\uDED0\uDD4E\uDD2F\uDD00-\uDD02\uDD3C\uDD3D\uDD05\uDD06\uDCF6\uDCF3\uDCF4\uDD31\uDCDB\uDD30\uDD1F-\uDD24\uDD34\uDFE0-\uDFE2\uDD35\uDFE3-\uDFE5\uDFE7-\uDFE9\uDFE6\uDFEA\uDFEB\uDD36-\uDD3B\uDCA0\uDD18\uDD33\uDD32\uDEA9])|\uD83E(?:[\uDD1A\uDD0F\uDD1E\uDD1F\uDD18\uDD19\uDD1B\uDD1C\uDD32\uDD33\uDDB5\uDDB6\uDDBB\uDDD2](?:\uD83C[\uDFFB-\uDFFF])?|\uDDD1(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:\uD83E(?:\uDD1D\u200D\uD83E\uDDD1\uD83C[\uDFFB-\uDFFF]|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?))?)|\u200D(?:\uD83E(?:\uDD1D\u200D\uD83E\uDDD1|[\uDDB0\uDDB1\uDDB3\uDDB2\uDDAF\uDDBC\uDDBD])|\u2695\uFE0F?|\uD83C[\uDF93\uDFEB\uDF3E\uDF73\uDFED\uDFA4\uDFA8]|\u2696\uFE0F?|\uD83D[\uDD27\uDCBC\uDD2C\uDCBB\uDE80\uDE92]|\u2708\uFE0F?)))?|[\uDDD4\uDDD3](?:\uD83C[\uDFFB-\uDFFF])?|[\uDDCF\uDD26\uDD37](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDD34\uDDD5\uDD35\uDD30\uDD31\uDD36](?:\uD83C[\uDFFB-\uDFFF])?|[\uDDB8\uDDB9\uDDD9-\uDDDD](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDDDE\uDDDF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?|[\uDDCD\uDDCE\uDDD6\uDDD7\uDD38](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDD3C(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|[\uDD3D\uDD3E\uDD39\uDDD8](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDD23\uDD70\uDD29\uDD2A\uDD11\uDD17\uDD2D\uDD2B\uDD14\uDD10\uDD28\uDD25\uDD24\uDD12\uDD15\uDD22\uDD2E\uDD27\uDD75\uDD76\uDD74\uDD2F\uDD20\uDD73\uDD13\uDDD0\uDD7A\uDD71\uDD2C\uDD21\uDD16\uDDE1\uDD0E\uDD0D\uDD1D\uDDBE\uDDBF\uDDE0\uDDB7\uDDB4\uDD3A\uDDB0\uDDB1\uDDB3\uDDB2\uDD8D\uDDA7\uDDAE\uDD8A\uDD9D\uDD81\uDD84\uDD93\uDD8C\uDD99\uDD92\uDD8F\uDD9B\uDD94\uDD87\uDDA5\uDDA6\uDDA8\uDD98\uDDA1\uDD83\uDD85\uDD86\uDDA2\uDD89\uDDA9\uDD9A\uDD9C\uDD8E\uDD95\uDD96\uDD88\uDD8B\uDD97\uDD82\uDD9F\uDDA0\uDD40\uDD6D\uDD5D\uDD65\uDD51\uDD54\uDD55\uDD52\uDD6C\uDD66\uDDC4\uDDC5\uDD5C\uDD50\uDD56\uDD68\uDD6F\uDD5E\uDDC7\uDDC0\uDD69\uDD53\uDD6A\uDD59\uDDC6\uDD5A\uDD58\uDD63\uDD57\uDDC8\uDDC2\uDD6B\uDD6E\uDD5F-\uDD61\uDD80\uDD9E\uDD90\uDD91\uDDAA\uDDC1\uDD67\uDD5B\uDD42\uDD43\uDD64\uDDC3\uDDC9\uDDCA\uDD62\uDD44\uDDED\uDDF1\uDDBD\uDDBC\uDE82\uDDF3\uDE90\uDDE8\uDDE7\uDD47-\uDD49\uDD4E\uDD4F\uDD4D\uDD4A\uDD4B\uDD45\uDD3F\uDD4C\uDE80\uDE81\uDDFF\uDDE9\uDDF8\uDDF5\uDDF6\uDD7D\uDD7C\uDDBA\uDDE3-\uDDE6\uDD7B\uDE71-\uDE73\uDD7E\uDD7F\uDE70\uDDE2\uDE95\uDD41\uDDEE\uDE94\uDDFE\uDE93\uDDAF\uDDF0\uDDF2\uDDEA-\uDDEC\uDE78-\uDE7A\uDE91\uDE92\uDDF4\uDDF7\uDDF9-\uDDFD\uDDEF])|[\u263A\u2639\u2620\u2763\u2764]\uFE0F?|\u270B(?:\uD83C[\uDFFB-\uDFFF])?|[\u270C\u261D](?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|\u270A(?:\uD83C[\uDFFB-\uDFFF])?|\u270D(?:(?:\uD83C[\uDFFB-\uDFFF]|\uFE0F))?|\uD83C(?:\uDF85(?:\uD83C[\uDFFB-\uDFFF])?|\uDFC3(?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFC7\uDFC2](?:\uD83C[\uDFFB-\uDFFF])?|\uDFCC(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFC4\uDFCA](?:(?:\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|\uDFCB(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\uDFF5\uDF36\uDF7D\uDFD4-\uDFD6\uDFDC-\uDFDF\uDFDB\uDFD7\uDFD8\uDFDA\uDFD9\uDFCE\uDFCD\uDF21\uDF24-\uDF2C\uDF97\uDF9F\uDF96\uDF99-\uDF9B\uDF9E\uDFF7\uDD70\uDD71\uDD7E\uDD7F\uDE02\uDE37]\uFE0F?|\uDFF4(?:(?:\u200D\u2620\uFE0F?|\uDB40\uDC67\uDB40\uDC62\uDB40(?:\uDC65\uDB40\uDC6E\uDB40\uDC67\uDB40\uDC7F|\uDC73\uDB40\uDC63\uDB40\uDC74\uDB40\uDC7F|\uDC77\uDB40\uDC6C\uDB40\uDC73\uDB40\uDC7F)))?|\uDFF3(?:(?:\uFE0F(?:\u200D\uD83C\uDF08)?|\u200D\uD83C\uDF08))?|\uDDE6\uD83C[\uDDE8-\uDDEC\uDDEE\uDDF1\uDDF2\uDDF4\uDDF6-\uDDFA\uDDFC\uDDFD\uDDFF]|\uDDE7\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEF\uDDF1-\uDDF4\uDDF6-\uDDF9\uDDFB\uDDFC\uDDFE\uDDFF]|\uDDE8\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDEE\uDDF0-\uDDF5\uDDF7\uDDFA-\uDDFF]|\uDDE9\uD83C[\uDDEA\uDDEC\uDDEF\uDDF0\uDDF2\uDDF4\uDDFF]|\uDDEA\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDED\uDDF7-\uDDFA]|\uDDEB\uD83C[\uDDEE-\uDDF0\uDDF2\uDDF4\uDDF7]|\uDDEC\uD83C[\uDDE6\uDDE7\uDDE9-\uDDEE\uDDF1-\uDDF3\uDDF5-\uDDFA\uDDFC\uDDFE]|\uDDED\uD83C[\uDDF0\uDDF2\uDDF3\uDDF7\uDDF9\uDDFA]|\uDDEE\uD83C[\uDDE8-\uDDEA\uDDF1-\uDDF4\uDDF6-\uDDF9]|\uDDEF\uD83C[\uDDEA\uDDF2\uDDF4\uDDF5]|\uDDF0\uD83C[\uDDEA\uDDEC-\uDDEE\uDDF2\uDDF3\uDDF5\uDDF7\uDDFC\uDDFE\uDDFF]|\uDDF1\uD83C[\uDDE6-\uDDE8\uDDEE\uDDF0\uDDF7-\uDDFB\uDDFE]|\uDDF2\uD83C[\uDDE6\uDDE8-\uDDED\uDDF0-\uDDFF]|\uDDF3\uD83C[\uDDE6\uDDE8\uDDEA-\uDDEC\uDDEE\uDDF1\uDDF4\uDDF5\uDDF7\uDDFA\uDDFF]|\uDDF4\uD83C\uDDF2|\uDDF5\uD83C[\uDDE6\uDDEA-\uDDED\uDDF0-\uDDF3\uDDF7-\uDDF9\uDDFC\uDDFE]|\uDDF6\uD83C\uDDE6|\uDDF7\uD83C[\uDDEA\uDDF4\uDDF8\uDDFA\uDDFC]|\uDDF8\uD83C[\uDDE6-\uDDEA\uDDEC-\uDDF4\uDDF7-\uDDF9\uDDFB\uDDFD-\uDDFF]|\uDDF9\uD83C[\uDDE6\uDDE8\uDDE9\uDDEB-\uDDED\uDDEF-\uDDF4\uDDF7\uDDF9\uDDFB\uDDFC\uDDFF]|\uDDFA\uD83C[\uDDE6\uDDEC\uDDF2\uDDF3\uDDF8\uDDFE\uDDFF]|\uDDFB\uD83C[\uDDE6\uDDE8\uDDEA\uDDEC\uDDEE\uDDF3\uDDFA]|\uDDFC\uD83C[\uDDEB\uDDF8]|\uDDFD\uD83C\uDDF0|\uDDFE\uD83C[\uDDEA\uDDF9]|\uDDFF\uD83C[\uDDE6\uDDF2\uDDFC]|[\uDFFB-\uDFFF\uDF38-\uDF3C\uDF37\uDF31-\uDF35\uDF3E-\uDF43\uDF47-\uDF53\uDF45\uDF46\uDF3D\uDF44\uDF30\uDF5E\uDF56\uDF57\uDF54\uDF5F\uDF55\uDF2D-\uDF2F\uDF73\uDF72\uDF7F\uDF71\uDF58-\uDF5D\uDF60\uDF62-\uDF65\uDF61\uDF66-\uDF6A\uDF82\uDF70\uDF6B-\uDF6F\uDF7C\uDF75\uDF76\uDF7E\uDF77-\uDF7B\uDF74\uDFFA\uDF0D-\uDF10\uDF0B\uDFE0-\uDFE6\uDFE8-\uDFED\uDFEF\uDFF0\uDF01\uDF03-\uDF07\uDF09\uDFA0-\uDFA2\uDFAA\uDF11-\uDF20\uDF0C\uDF00\uDF08\uDF02\uDF0A\uDF83\uDF84\uDF86-\uDF8B\uDF8D-\uDF91\uDF80\uDF81\uDFAB\uDFC6\uDFC5\uDFC0\uDFD0\uDFC8\uDFC9\uDFBE\uDFB3\uDFCF\uDFD1-\uDFD3\uDFF8\uDFA3\uDFBD\uDFBF\uDFAF\uDFB1\uDFAE\uDFB0\uDFB2\uDCCF\uDC04\uDFB4\uDFAD\uDFA8\uDF92\uDFA9\uDF93\uDFBC\uDFB5\uDFB6\uDFA4\uDFA7\uDFB7-\uDFBB\uDFA5\uDFAC\uDFEE\uDFF9\uDFE7\uDFA6\uDD8E\uDD91-\uDD9A\uDE01\uDE36\uDE2F\uDE50\uDE39\uDE1A\uDE32\uDE51\uDE38\uDE34\uDE33\uDE3A\uDE35\uDFC1\uDF8C])|\u26F7\uFE0F?|\u26F9(?:(?:\uFE0F(?:\u200D(?:[\u2642\u2640]\uFE0F?))?|\uD83C(?:[\uDFFB-\uDFFF](?:\u200D(?:[\u2642\u2640]\uFE0F?))?)|\u200D(?:[\u2642\u2640]\uFE0F?)))?|[\u2618\u26F0\u26E9\u2668\u26F4\u2708\u23F1\u23F2\u2600\u2601\u26C8\u2602\u26F1\u2744\u2603\u2604\u26F8\u2660\u2665\u2666\u2663\u265F\u26D1\u260E\u2328\u2709\u270F\u2712\u2702\u26CF\u2692\u2694\u2699\u2696\u26D3\u2697\u26B0\u26B1\u26A0\u2622\u2623\u2B06\u2197\u27A1\u2198\u2B07\u2199\u2B05\u2196\u2195\u2194\u21A9\u21AA\u2934\u2935\u269B\u2721\u2638\u262F\u271D\u2626\u262A\u262E\u25B6\u23ED\u23EF\u25C0\u23EE\u23F8-\u23FA\u23CF\u2640\u2642\u2695\u267E\u267B\u269C\u2611\u2714\u2716\u303D\u2733\u2734\u2747\u203C\u2049\u3030\u00A9\u00AE\u2122]\uFE0F?|[\u0023\u002A\u0030-\u0039](?:\uFE0F\u20E3|\u20E3)|[\u2139\u24C2\u3297\u3299\u25FC\u25FB\u25AA\u25AB]\uFE0F?|[\u2615\u26EA\u26F2\u26FA\u26FD\u2693\u26F5\u231B\u23F3\u231A\u23F0\u2B50\u26C5\u2614\u26A1\u26C4\u2728\u26BD\u26BE\u26F3\u267F\u26D4\u2648-\u2653\u26CE\u23E9-\u23EC\u2B55\u2705\u274C\u274E\u2795-\u2797\u27B0\u27BF\u2753-\u2755\u2757\u26AB\u26AA\u2B1B\u2B1C\u25FE\u25FD])";
            Regex rgx = new Regex(EmojiPattern);
            if (rgx.IsMatch(postOwner))
            {
                string result = Regex.Replace(postOwner, EmojiPattern, "");
                return result;
            }
            return postOwner;
        }

        public Dictionary<IpLocationDetails, string> GetIpDetails(DominatorAccountModel dominatorAccountModel)
        {
            var dictIpDetails = new Dictionary<IpLocationDetails, string>();
            string proxyLocationDetails = string.Empty;
            //http://api.db-ip.com/v2/{apiKey}/{ipAddressList}
            try
            {
                lock (lockerRequest)
                {
                    try
                    {                        
                        SleepRandomMilliseconds(3000, 2000); //Thread.Sleep(2000);
                        IRequestParameters requestParameters = httpHelper.GetRequestParameter();
                        httpHelper.SetRequestParameter(requestParameters);
                        IgRequestParameters objIgHttpHelper = new IgRequestParameters(dominatorAccountModel.UserAgentMobile);
                        string ips;
                        string ipUrl = "https://app.multiloginapp.com/WhatIsMyIP";
                        if (string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp))
                        {
                            string ip = objIgHttpHelper.IgGetResponse(ipUrl);
                            ips = Utilities.GetBetween(ip, "pti-header bgm-green\">", "/h2>");
                            ips = Utilities.GetBetween(ips, ">", "<").Trim('\n').Trim(' ');
                        }
                        else
                            ips = dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp;

                        //f36e244f142c952d67116fbbba1aca49f74579d1
                        //var locationUrl = $"http://api.db-ip.com/v2/9ace4c1b65f77a7fa6c57f4de28cca79f80ee68f/{ips.Trim()}";
                        var locationUrl = $"http://ip-api.com/json/";
                        string response = objIgHttpHelper.IgGetResponse(locationUrl);
                        if (proxyLocationDetails.Contains("INVALID_ADDRESS"))
                        {
                            string ip = objIgHttpHelper.IgGetResponse("https://app.multiloginapp.com/WhatIsMyIP");
                            ip = Utilities.GetBetween(ip, "pti-header bgm-green\">", "/h2>");
                            ips = Utilities.GetBetween(ip, ">", "<");
                            //locationUrl = $"https://api.db-ip.com/v2/17f5666a35e4e5bad778c00e3dbe2016559917ff/{ips.Trim()}";
                            //locationUrl = $"http://api.db-ip.com/v2/f36e244f142c952d67116fbbba1aca49f74579d1/{ips.Trim()}";
                            //locationUrl = $"http://api.db-ip.com/v2/9ace4c1b65f77a7fa6c57f4de28cca79f80ee68f/{ips.Trim()}";
                            locationUrl = $"http://ip-api.com/json/";
                            objIgHttpHelper.IgGetResponse(locationUrl);
                        }
                        JObject JResp;
                        JResp = JObject.Parse(response);

                        var city = JResp["city"].ToString();
                        //var state = JResp["stateProv"].ToString();
                        var state = JResp["regionName"].ToString();
                        //var country = JResp["countryName"].ToString();
                        var country = JResp["country"].ToString();
                        dictIpDetails.Add(IpLocationDetails.City, !string.IsNullOrEmpty(city) ? city : null);
                        dictIpDetails.Add(IpLocationDetails.State, !string.IsNullOrEmpty(state) ? state : null);
                        dictIpDetails.Add(IpLocationDetails.Country, !string.IsNullOrEmpty(country) ? country : null);

                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return dictIpDetails;
        }
        public string PostDataAsJson(string resourceUrl, SortedDictionary<string, string> requestParameters)
        {
            var result = string.Empty;

            lock (PostaDataAsJsonlockerRequest)
            {
                try
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(resourceUrl);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";
                    httpWebRequest.KeepAlive = true;

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        var serializedData = JsonConvert.SerializeObject(requestParameters, Formatting.Indented);
                        streamWriter.Write(serializedData);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        result = streamReader.ReadToEnd();
                    }

                }
                catch (Exception)
                {
                    // Console.WriteLine("Error : " + ex.StackTrace);
                }
            }

            return result;
        }

        //ToDo: Replace it with RandomUtilties
        public int GetRandom()
        {
            Random random = new Random();
            return random.Next(50, 3500);
        }

        //ToDo: Move it Utilities
        private void SleepRandomMilliseconds(int max, int min)
        {
            var delay = RandomUtilties.GetRandomNumber(max, min);
            //ToDo: Use IDelayService.ThreadSleep
            Thread.Sleep(delay);
        }

        public void GetGender(ref string gender, ref string city, ref string state, ref string country, DominatorAccountModel dominatorAccountmodel)
        {
            try
            {
                var userInfo = instaFunct.SearchUsername(dominatorAccountmodel, dominatorAccountmodel.UserName, CancellationToken.None);
                dominatorAccountmodel.AccountBaseModel.UserId = userInfo.Pk;
                //if (!dominatorAccountmodel.IsRunProcessThroughBrowser)
                //{
                //    userInfo = instaFunct.SearchUsername(dominatorAccountmodel, dominatorAccountmodel.UserName, CancellationToken.None); 
                //}
                //else
                //{
                //    userInfo = AdBrowserManager.GetUserInfo(dominatorAccountmodel, dominatorAccountmodel.UserName, CancellationToken.None);
                //    dominatorAccountmodel.AccountBaseModel.UserId = userInfo.Pk;
                //}
                //  string url = "https://api.namsor.com/onomastics/api/json/gender/" + UnknownUsername + "/" + "null";               
                gender = userInfo.Gender;
                if (userInfo.Gender == null || userInfo.Gender.Contains("Unknown"))
                {
                    string usergender = GetGenderByAPI(dominatorAccountmodel, dominatorAccountmodel.UserName);
                    gender = usergender;
                }
                Dictionary<IpLocationDetails, string> UserData = GetIpDetails(dominatorAccountmodel);
                country = UserData[IpLocationDetails.Country];
                city = UserData[IpLocationDetails.City];
                state = UserData[IpLocationDetails.State];
            }
            catch (Exception)
            {
                // ignored
            }
        }
        public string CheckAdsUsingApi(DominatorAccountModel dominatorAccountModel, string gender, string country)
        {
            string IsAddedId = string.Empty;
            try
            {
                SortedDictionary<string, string> AdsUserData = new SortedDictionary<string, string>
            {
                {"instagram_id", dominatorAccountModel.AccountBaseModel.UserId},
                {"instagram_username", dominatorAccountModel.UserName },
                {"name", dominatorAccountModel.AccountBaseModel.UserName},
                {"other_places_lived", ""},
                {"current_country", country},
                {"gender", gender},
                {"age", ""},
                {"relationship_status", ""}
            };
                  string mainuserUrl = "https://gramapi.poweradspy.com/instagram_user_data";
            //      string mainuserUrl = "https://instagram-dev.poweradspy.com/instagram_user_data";
                IsAddedId = PostDataAsJson(mainuserUrl, AdsUserData);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return IsAddedId;
        }

        public void CheckStoryAdsUsingApi(DominatorAccountModel dominatorAccountModel, string gender, string country)
        {
            try
            {
                SortedDictionary<string, string> AdsUserData = new SortedDictionary<string, string>
            {
                {"instagram_id", dominatorAccountModel.AccountBaseModel.UserId},
                {"name", dominatorAccountModel.AccountBaseModel.UserName},
                {"other_places_lived", ""},
                {"current_country", country},
                {"gender", gender},
                {"age", ""},
                {"relationship_status", ""}
            };
                //https://gramapi.poweradspy.com/instagram_user_data
                // string userUrl = "https://instaapi.poweradspy.com/instagram_user_data";
                string userUrl = "https://gramapi.poweradspy.com/instagram_user_data";
                PostDataAsJson(userUrl, AdsUserData);
            }
            catch (Exception)
            {

            }
        }

        public string GetGenderByAPI(DominatorAccountModel dominatorAccountModel, string userName)
        {
            try
            {
                IgRequestParameters objIgHttpHelper = new IgRequestParameters(dominatorAccountModel.UserAgentMobile);
                string url = $"https://genderapi.io/instagram?q={userName}";
                string response = objIgHttpHelper.IgGetResponse(url);
                JObject JResp = JObject.Parse(response);
                string gender = JResp["gender"].ToString();
                if (!gender.Contains("null"))
                    return gender;
            }
            catch (Exception)
            {
                //ignored
            }
            return "Unknown";
        }

        protected string CheckPostId(string queryInfo)
        {
            try
            {
                string tempCode = queryInfo.Length > 100 ? queryInfo.Remove(11, queryInfo.Length - 11) : queryInfo;
                tempCode = tempCode.Trim();
                if (tempCode.Contains("www.instagram.com"))
                    tempCode = tempCode.Split('/')[4];
                if (tempCode.Length > 11)
                    tempCode = tempCode.Substring(0, 11).Trim();
                return (tempCode.GetCodeFromUrl() ?? tempCode).GetIdFromCode();
            }
            catch (Exception)
            {
                return null;
            }

        }

        public InstaFeedData GetFeedAdsBrowser(string FeedResponse, DominatorAccountModel dominatorAccountModel, AccountModel accountModel, string city, string state, string country, List<string> insertedAdsId)
        {
            InstaFeedData objJsonFeedData = new InstaFeedData();
            string max_Id;
            string mediaTypeInNumberFormat;
            List<ResultFeedTimeLineData> listResultFeedTimeLineData = new List<ResultFeedTimeLineData>();
            List<InstaFeedTimeLineData> listFeedTimeLineData;
            List<string> listId = new List<string>();
            List<string> duplicateAds = new List<string>();
            List<string> listheight = new List<string>();
            List<string> listTimestamp = new List<string>();
            string label = string.Empty;
            var jsonHandler = new JsonHandler(FeedResponse);
            var postData = jsonHandler.GetJToken("data", "xdt_api__v1__feed__timeline__connection", "edges");
            foreach(var ads in postData)
            {
                var adsData = jsonHandler.GetJTokenOfJToken(ads, "node", "ad");
                if (!adsData.HasValues)
                    continue;
                InstaFeedTimeLineData objFeedTimeLineDatas = new InstaFeedTimeLineData();
                ResultFeedTimeLineData objResultFeedLineData = new ResultFeedTimeLineData();
                SortedDictionary<string, string> AdsData = new SortedDictionary<string, string>();
                string AlbumType = string.Empty;
                var postOwnerName = false;
                var descriptionlink = false;
                var ImageVideoUrl = false;
                var callToAction = false;
                try
                {
                    #region Getting Feed Data for pagination
                    try
                    {
                        objFeedTimeLineDatas.mediaId = jsonHandler.GetJTokenValue(adsData,"media_id");
                        objFeedTimeLineDatas.post_date = jsonHandler.GetJTokenValue(adsData,"items",0,"taken_at");
                        try
                        {
                            objFeedTimeLineDatas.ImageHeight = jsonHandler.GetJTokenValue(adsData, "items", 0, "original_height");
                        }
                        catch (Exception)
                        {
                            //objFeedTimeLineDatas.ImageHeight = jtoken["media_or_ad"]["carousel_media"][0]["image_versions2"]["candidates"][0]["height"].ToString();
                        }

                        listheight.Add(objFeedTimeLineDatas.ImageHeight);
                        listId.Add(objFeedTimeLineDatas.mediaId);
                        listTimestamp.Add(objFeedTimeLineDatas.post_date);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    #endregion
                    label = jsonHandler.GetJTokenValue(adsData,"label");
                    //try
                    //{
                    //    label = jtoken["media_or_ad"]["injected"]["label"].ToString();//["injected"]["label"].ToString();
                    //}
                    //catch (Exception ex)
                    //{

                    //    continue;
                    //}
                    if (!string.IsNullOrEmpty(label))
                    {
                        #region ad Id
                        objFeedTimeLineDatas.ad_id = jsonHandler.GetJTokenValue(adsData,"ad_id");
                        //AdsData.Add("ad_id", objFeedTimeLineDatas.ad_id);

                        #endregion
                        #region Post date
                        objFeedTimeLineDatas.post_date = jsonHandler.GetJTokenValue(adsData,"items",0,"taken_at");
                        AdsData.Add("post_date", objFeedTimeLineDatas.post_date);
                        #endregion
                        #region Image code
                        objFeedTimeLineDatas.code = jsonHandler.GetJTokenValue(adsData, "items", 0, "code");
                        if (insertedAdsId.Any(x => x.Contains(objFeedTimeLineDatas.code)))
                            continue;
                        AdsData.Add("ad_id", objFeedTimeLineDatas.code);
                        #endregion
                        #region post Owner name

                        try
                        {
                            if (!postOwnerName)
                            {
                                objFeedTimeLineDatas.post_owner = jsonHandler.GetJTokenValue(adsData,"items",0,"user","full_name");
                                if (!string.IsNullOrEmpty(objFeedTimeLineDatas.post_owner))
                                {
                                    string[] data = objFeedTimeLineDatas.post_owner.Split(' ');
                                    int pdata = data.Length;
                                    int errorCounter = Regex.Matches(data[pdata - 1], @"[a-zA-Z]").Count;
                                    string postownernames = string.Empty;
                                    if (errorCounter == 0)
                                    {
                                        for (int i = 0; i <= pdata - 2; i++)
                                        {
                                            postownernames = postownernames + " " + data[i];
                                        }
                                    }
                                    else
                                    {
                                        AdsData.Add("post_owner", objFeedTimeLineDatas.post_owner);
                                        postOwnerName = true;
                                    }
                                    if (!string.IsNullOrEmpty(postownernames))
                                    {
                                        AdsData.Add("post_owner", postownernames);
                                        postOwnerName = true;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            postOwnerName = false;
                        }
                        try
                        {
                            if (!postOwnerName)
                            {
                                if (string.IsNullOrEmpty(objFeedTimeLineDatas.post_owner) || objFeedTimeLineDatas.post_owner == null)
                                    objFeedTimeLineDatas.post_owner = jsonHandler.GetJTokenValue(adsData, "user", "username");
                                if (!string.IsNullOrEmpty(objFeedTimeLineDatas.post_owner))
                                {
                                    AdsData.Add("post_owner", objFeedTimeLineDatas.post_owner);
                                    postOwnerName = true;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            postOwnerName = false;
                        }

                        try
                        {
                            if (!postOwnerName)
                                AdsData.Add("post_owner", "Post Owner");
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        #endregion
                        #region Post Owner Image
                        try
                        {
                            objFeedTimeLineDatas.post_owner_image = jsonHandler.GetJTokenValue(adsData, "items", 0, "user", "profile_pic_url");
                            AdsData.Add("post_owner_image", objFeedTimeLineDatas.post_owner_image);
                        }
                        catch (Exception)
                        {
                            objFeedTimeLineDatas.post_owner_image = "";
                            AdsData.Add("post_owner_image", objFeedTimeLineDatas.post_owner_image);
                        }
                        #endregion
                        #region Image Type
                        objFeedTimeLineDatas.MediaType = jsonHandler.GetJTokenValue(adsData, "media_type");
                        mediaTypeInNumberFormat = objFeedTimeLineDatas.MediaType;
                        objFeedTimeLineDatas.MediaType = objFeedTimeLineDatas.MediaType.Contains("1") ? "IMAGE" : objFeedTimeLineDatas.MediaType.Contains("2") ? "VIDEO" : "IMAGE";
                        if (objFeedTimeLineDatas.MediaType.Contains("VIDEO") || mediaTypeInNumberFormat.Contains("2"))
                        {

                        }
                        if (mediaTypeInNumberFormat.Contains("8"))
                            AlbumType = "Album";
                        AdsData.Add("type", objFeedTimeLineDatas.MediaType);
                        #endregion
                        #region Post video Url
                        try
                        {
                            if (objFeedTimeLineDatas.MediaType.Contains("VIDEO"))
                            {
                                if (!ImageVideoUrl)
                                {
                                    objFeedTimeLineDatas.image_video_url = jsonHandler.GetJTokenValue(adsData, "items", 0, "video_versions", 0, "url");
                                    if (objFeedTimeLineDatas.image_video_url.Contains(".mp4"))
                                    {
                                        if (!string.IsNullOrEmpty(objFeedTimeLineDatas.image_video_url))
                                        {
                                            AdsData.Add("image_video_url", objFeedTimeLineDatas.image_video_url);
                                            ImageVideoUrl = true;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            ImageVideoUrl = false;
                        }
                        try
                        {
                            if (objFeedTimeLineDatas.MediaType.Contains("IMAGE") && string.IsNullOrEmpty(AlbumType))
                            {
                                if (!ImageVideoUrl)
                                {
                                    objFeedTimeLineDatas.image_video_url = jsonHandler.GetJTokenValue(adsData, "items", 0, "image_versions2", "candidates", 0, "url");
                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.image_video_url))
                                    {
                                        AdsData.Add("image_video_url", objFeedTimeLineDatas.image_video_url);
                                        ImageVideoUrl = true;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            ImageVideoUrl = false;
                        }
                        try
                        {
                            if (objFeedTimeLineDatas.MediaType.Contains("IMAGE") && string.IsNullOrEmpty(AlbumType))
                            {
                                if (!ImageVideoUrl)
                                {
                          //          objFeedTimeLineDatas.image_video_url = jtoken["media_or_ad"]["collection_media"][0]["image_versions2"]["candidates"][0]["url"].ToString();
                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.image_video_url))
                                    {
                                        AdsData.Add("image_video_url", objFeedTimeLineDatas.image_video_url);
                                        ImageVideoUrl = true;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            ImageVideoUrl = false;
                        }
                        try
                        {
                            if (AlbumType.Contains("Album"))
                            {
                                if (!ImageVideoUrl)
                                {
                                    int imageCount = jsonHandler.GetJTokenOfJToken(adsData,"items").Count();
                                    int count = 0;
                                    foreach (JToken item in jsonHandler.GetJTokenOfJToken(adsData,"items"))
                                    {
                                        if (count == 0 && imageCount > 1)
                                        {
                                            objFeedTimeLineDatas.image_video_url = jsonHandler.GetJTokenValue(item, "image_versions2", "candidates", 0, "url");
                                            count++;
                                        }
                                        else if (count > 0 && imageCount > 1 && count < imageCount)
                                        {
                                            objFeedTimeLineDatas.other_multimedia += jsonHandler.GetJTokenValue(item, "image_versions2", "candidates", 0, "url") + "||";
                                            count++;
                                        }
                                        else
                                            objFeedTimeLineDatas.image_video_url = jsonHandler.GetJTokenValue(item, "image_versions2", "candidates", 0, "url");
                                        //var mediaType = jsonHandler.GetJTokenValue(item, "media_type");
                                        //mediaType = mediaType == "1" ? "IMAGE" : "VIDEO";
                                        //if (mediaType.Contains("IMAGE"))
                                        //{

                                        //    //if (count < imageCount)
                                        //    //{
                                        //    //    objFeedTimeLineDatas.image_video_url +=
                                        //    //                jsonHandler.GetJTokenValue(item, "image_versions2","candidates", 0, "url") + "||";
                                        //    //    count++;
                                        //    //}   
                                        //    //else
                                        //    //    objFeedTimeLineDatas.image_video_url = jsonHandler.GetJTokenValue(item, "image_versions2", 0, "url");

                                        //}



                                        //foreach (JToken crouserImage in CrouserMedia["image_versions2"]["candidates"])
                                        //{
                                        //    //if (cm > 1)
                                        //    //    objFeedTimeLineDatas.image_video_url += crouserImage["url"] + "||";
                                        //    //else
                                        //    //    objFeedTimeLineDatas.image_video_url = crouserImage["url"].ToString();
                                        //if (imageCount == 0 && cm > 1)
                                        //{
                                        //    objFeedTimeLineDatas.image_video_url += crouserImage["url"].ToString();
                                        //    imageCount++;
                                        //}
                                        //    else if (imageCount > 0 && cm > 1)
                                        //    {
                                        //        objFeedTimeLineDatas.other_multimedia += crouserImage["url"] + "||";
                                        //    }
                                        //    else
                                        //        objFeedTimeLineDatas.image_video_url = crouserImage["url"].ToString();

                                        //    break;
                                        //}

                                        //}
                                    }
                                    if (objFeedTimeLineDatas.other_multimedia.Contains("||"))
                                    {
                                        string urll = objFeedTimeLineDatas.other_multimedia;
                                        objFeedTimeLineDatas.other_multimedia = urll.Substring(0, objFeedTimeLineDatas.other_multimedia.Length - 2);
                                    }
                                    if (objFeedTimeLineDatas.image_video_url.Contains(".mp4"))
                                        AdsData["type"] = "VIDEO";

                                    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.image_video_url) || objFeedTimeLineDatas.image_video_url == null)
                                    {
                                        AdsData.Add("image_video_url", objFeedTimeLineDatas.image_video_url);
                                        AdsData.Add("other_multimedia", objFeedTimeLineDatas.other_multimedia);
                                        // ReSharper disable once RedundantAssignment
                                        ImageVideoUrl = true;
                                    }
                                    //if (objFeedTimeLineDatas.other_multimedia.Contains("||"))
                                    //{
                                    //    string urll = objFeedTimeLineDatas.other_multimedia;
                                    //    objFeedTimeLineDatas.other_multimedia = urll.Substring(0, objFeedTimeLineDatas.other_multimedia.Length - 2);
                                    //}
                                    //if (objFeedTimeLineDatas.image_video_url.Contains(".mp4"))
                                    //    AdsData["type"] = "VIDEO";

                                    //if (!string.IsNullOrEmpty(objFeedTimeLineDatas.image_video_url) || objFeedTimeLineDatas.image_video_url == null)
                                    //{
                                    //    AdsData.Add("image_video_url", objFeedTimeLineDatas.image_video_url);
                                    //    // ReSharper disable once RedundantAssignment
                                    //    ImageVideoUrl = true;
                                    //}
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // ReSharper disable once RedundantAssignment
                            ImageVideoUrl = false;
                        }

                        #endregion
                        #region Description Link

                        try
                        {
                            if (!descriptionlink)
                            {
                                objFeedTimeLineDatas.destination_url = jsonHandler.GetJTokenValue(adsData, "items", 0, "link");
                                //if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                //{
                                //    objFeedTimeLineDatas.destination_url = jtoken["media_or_ad"]["android_links"][1]["redirectUri"].ToString();
                                //}
                                if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                                {
                                    AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                                    descriptionlink = true;
                                }
                            }
                        }
                        catch (Exception)
                        {

                            descriptionlink = false;
                        }
                        try
                        {
                            //if (!descriptionlink)
                            //{

                            //    if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                            //    {
                            //        foreach (JToken jt in jtoken["media_or_ad"]["android_links"])
                            //        {
                            //            if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url) || objFeedTimeLineDatas.destination_url == null)
                            //                objFeedTimeLineDatas.destination_url = jt["webUri"].ToString();

                            //        }
                            //    }
                            //    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                            //    {
                            //        AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                            //        descriptionlink = true;
                            //    }
                            //}
                        }
                        catch (Exception)
                        {

                            descriptionlink = false;
                        }
                        try
                        {

                            //if (!descriptionlink)
                            //{
                            //    if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                            //    {
                            //        foreach (JToken CrouserMedia in jtoken["media_or_ad"]["carousel_media"])
                            //        {
                            //            if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url) || objFeedTimeLineDatas.destination_url == null)
                            //            {
                            //                objFeedTimeLineDatas.destination_url = CrouserMedia["android_links"]["webUri"].ToString();

                            //            }
                            //        }
                            //    }
                            //    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                            //    {
                            //        AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                            //        descriptionlink = true;
                            //    }
                            //}
                        }
                        catch (Exception)
                        {
                            descriptionlink = false;
                        }

                        try
                        {
                            //if (!descriptionlink)
                            //{
                            //    if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                            //    {
                            //        objFeedTimeLineDatas.destination_url = jtoken["media_or_ad"]["link"].ToString();
                            //        //objFeedTimeLineDatas.destination_url=HttpUtility.UrlEncode(objFeedTimeLineDatas.destination_url);
                            //    }
                            //    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                            //    {
                            //        AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                            //        descriptionlink = true;
                            //    }
                            //}

                        }
                        catch (Exception)
                        {

                            descriptionlink = false;
                        }
                        try
                        {
                            //if (!descriptionlink)
                            //{
                            //    if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                            //    {
                            //        foreach (JToken CrouserMedia in jtoken["media_or_ad"]["carousel_media"])
                            //        {
                            //            if (string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url) || objFeedTimeLineDatas.destination_url == null)
                            //            {
                            //                objFeedTimeLineDatas.destination_url = CrouserMedia["link"].ToString();
                            //            }
                            //        }
                            //        if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                            //        {
                            //            AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                            //            descriptionlink = true;
                            //        }
                            //    }
                            //}
                        }
                        catch (Exception)
                        {
                            descriptionlink = false;
                        }
                        try
                        {
                            //if (!descriptionlink)
                            //{
                            //    objFeedTimeLineDatas.destination_url = "www.instagram.com/p/" + objFeedTimeLineDatas.code + "/";
                            //    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.destination_url))
                            //    {
                            //        AdsData.Add("destination_url", objFeedTimeLineDatas.destination_url);
                            //        // ReSharper disable once RedundantAssignment
                            //        descriptionlink = true;
                            //    }
                            //}

                        }
                        catch
                        {
                            // ignored
                        }

                        #endregion
                        #region Call To Action
                        try
                        {
                            objFeedTimeLineDatas.call_to_action = jsonHandler.GetJTokenValue(adsData, "items", 0, "link_text");
                            if (!string.IsNullOrEmpty(objFeedTimeLineDatas.call_to_action))
                            {
                                // objFeedTimeLineDatas.call_to_action= HttpUtility.UrlEncode(objFeedTimeLineDatas.call_to_action);
                                AdsData.Add("call_to_action", objFeedTimeLineDatas.call_to_action);
                                callToAction = true;
                            }

                        }
                        catch (Exception)
                        {
                            callToAction = false;
                        }
                        try
                        {
                            //if (!callToAction)
                            //{
                            //    if (string.IsNullOrEmpty(objFeedTimeLineDatas.call_to_action))
                            //        objFeedTimeLineDatas.call_to_action = jtoken["media_or_ad"]["link_text"].ToString();

                            //    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.call_to_action))
                            //    {
                            //        //objFeedTimeLineDatas.call_to_action=HttpUtility.UrlEncode(objFeedTimeLineDatas.call_to_action);
                            //        AdsData.Add("call_to_action", objFeedTimeLineDatas.call_to_action);
                            //        callToAction = true;
                            //    }
                            //}

                        }
                        catch (Exception)
                        {
                            callToAction = false;
                        }
                        try
                        {
                            //if (!callToAction)
                            //{
                            //    foreach (JToken calltoAction in jtoken["media_or_ad"]["carousel_media"])
                            //    {
                            //        if (string.IsNullOrEmpty(objFeedTimeLineDatas.call_to_action))
                            //            objFeedTimeLineDatas.call_to_action = calltoAction["link_text"].ToString();

                            //    }
                            //    if (!string.IsNullOrEmpty(objFeedTimeLineDatas.call_to_action))
                            //    {
                            //        AdsData.Add("call_to_action", objFeedTimeLineDatas.call_to_action);
                            //        // ReSharper disable once RedundantAssignment
                            //        callToAction = true;
                            //    }
                            //}

                        }
                        catch (Exception)
                        {
                            // ReSharper disable once RedundantAssignment
                            callToAction = false;
                        }
                        if (!callToAction)
                        {
                            AdsData.Add("call_to_action", "");
                        }
                        #endregion
                        #region news Feed Description
                        objFeedTimeLineDatas.news_feed_description = jsonHandler.GetJTokenValue(adsData, "items", 0, "caption", "text");
                        // objFeedTimeLineDatas.news_feed_description=HttpUtility.UrlEncode(objFeedTimeLineDatas.news_feed_description);
                        AdsData.Add("news_feed_description", objFeedTimeLineDatas.news_feed_description);
                        #endregion
                        #region PostId Url
                        objFeedTimeLineDatas.ad_url = "www.instagram.com/p/" + objFeedTimeLineDatas.code + "/";
                        // objFeedTimeLineDatas.ad_url= HttpUtility.UrlEncode(objFeedTimeLineDatas.ad_url);
                        AdsData.Add("ad_url", objFeedTimeLineDatas.ad_url);
                        #endregion
                        #region Like count
                        objFeedTimeLineDatas.likes = jsonHandler.GetJTokenValue(adsData,"items",0,"like_count");
                        AdsData.Add("likes", objFeedTimeLineDatas.likes);
                        #endregion
                        #region comment Count
                        objFeedTimeLineDatas.comment = jsonHandler.GetJTokenValue(adsData, "items", 0, "comment_count");
                        AdsData.Add("comment", objFeedTimeLineDatas.comment);
                        #endregion
                        #region other postdata
                        AdsData.Add("ad_title", "");
                        //AdsData.Add("version", "1.0.11");
                        AdsData.Add("version", Constants.SocinatorLatestVersion);
                        AdsData.Add("side_url", "Not implemented yet");
                        AdsData.Add("platform", "1");
                        AdsData.Add("upper_age", "30");
                        AdsData.Add("lower_age", "18");
                        AdsData.Add("category", "Software");
                        AdsData.Add("first_seen", "12312121");
                        AdsData.Add("last_seen", "46545454");
                        AdsData.Add("ad_position", "FEED");
                        AdsData.Add("instagram_id", dominatorAccountModel.AccountBaseModel.UserId);
                        AdsData.Add("ad_text", "");
                        AdsData.Add("share", "0");
                        #endregion
                        #region city state country
                        AdsData.Add("city", city);
                        AdsData.Add("state", state);
                        AdsData.Add("country", country);
                        #endregion
                        #region  Hitting PowerAdSpy Api
                        objResultFeedLineData.objFeedTimeLineData = objFeedTimeLineDatas;
                        listResultFeedTimeLineData.Add(objResultFeedLineData);
                        try
                        {
                            string mainApi = "https://gramapi.poweradspy.com/instaAdsData";
                            //string devApi="https://instaapi.poweradspy.com/instaAdsData";
                            //string testApi= "http://instaapi.test.poweradspy.com/instaAdsData";
                      //      string mainApi = "https://instagram-dev.poweradspy.com/instaAdsData";
                            //var seriliazePayload = JsonConvert.SerializeObject(AdsData);
                            string res = PostDataAsJson(mainApi, AdsData);
                            
                            if (string.IsNullOrEmpty(res))
                            {
                                SleepRandomMilliseconds(5500, 5000); //Thread.Sleep(5 * 1000);
                                res = PostDataAsJson(mainApi, AdsData);
                            }
                            if(res.Contains("Ad inserted successfully"))
                            {
                        //        GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        //dominatorAccountModel.UserName, "Ad Scrapping", $"Successfully Inserted Ad : {objFeedTimeLineDatas.code}");
                                duplicateAds.Add(objFeedTimeLineDatas.code.ToString());
                            }
                                
                            else if(res.Contains("Ad already present"))
                            {
                        //        GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        //dominatorAccountModel.UserName, "Ad Scrapping", $"Ad Already Present : {objFeedTimeLineDatas.code}");
                                duplicateAds.Add(objFeedTimeLineDatas.code.ToString());
                            }
                                
                        }
                        catch (Exception)
                        {
                            // ignored
                        }

                        #endregion
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            objJsonFeedData.InsertedAdsId.AddRange(duplicateAds);

            return objJsonFeedData;
        }
    }
}

