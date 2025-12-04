using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json.Linq;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary
{
    public class DownloadPhotoProcess : GdJobProcessInteracted<InteractedPosts>
    {
        public DownloadPhotosModel DownloadPhotosModel { get; set; }

        public DownloadPhotoProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped, IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper, IGdBrowserManager gdBrowser, IDelayService _delayService) :
            base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, gdBrowser,_delayService)
        {          
            DownloadPhotosModel = JsonConvert.DeserializeObject<DownloadPhotosModel>(templateModel.ActivitySettings);
            if (DownloadPhotosModel.DownloadedFolderPath == ConstantVariable.GetDownloadedMediaFolderPath)
            {
                DownloadPhotosModel.DownloadedFolderPath +=
                    $"\\{ConstantVariable.ApplicationName}\\{DateTime.Now.Date.ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture)}";

                DirectoryUtilities.CreateDirectory(DownloadPhotosModel.DownloadedFolderPath);
            }
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            JobProcessResult jobProcessResult = new JobProcessResult();
            GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
               DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code);
            string filePath=string.Empty;                  
            try
            {
                InstagramPost instagramPost = (InstagramPost)scrapeResult.ResultPost;
                
                MediaInfoIgResponseHandler mediaInfo = null;
                if (DominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    mediaInfo = 
                        GramStatic.IsBrowser ?
                        instaFunct.GdBrowserManager.MediaInfo(DominatorAccountModel, instagramPost.Code, JobCancellationTokenSource.Token)
                        : instaFunct.MediaInfo(DominatorAccountModel, AccountModel, instagramPost.Code, JobCancellationTokenSource.Token).Result;
                    instagramPost = mediaInfo.InstagramPost;
                }
                if (DownloadPhotosModel.isDownloadAlbum || DownloadPhotosModel.isDownloadImage || DownloadPhotosModel.isDownloadVideo)
                {
                    filePath = DownloaMedia(instagramPost, filePath, scrapeResult);
                    if (string.IsNullOrEmpty(filePath))
                    {
                        return jobProcessResult;
                    }
                }
                var objPostRequiredDatas = GetRequiredData(instagramPost);
                #region Media Download Process
                if (objPostRequiredDatas.Count==0||objPostRequiredDatas.Count>0)
                {
                    if(File.Exists(filePath))
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"with code {scrapeResult.ResultPost.Code} to location : {filePath}");

                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                        $"Successfully Scraped post {scrapeResult.ResultPost.Code} info.");
                    #region save post

                    if (DownloadPhotosModel.IsSavePost)
                    {
                        CommonIgResponseHandler commonIgResponseHandler = instaFunct.SavePost(instagramPost.Pk,DominatorAccountModel,AccountModel,JobCancellationTokenSource.Token);
                        if (commonIgResponseHandler!=null && commonIgResponseHandler.Success)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                                $"Successfully post {scrapeResult.ResultPost.Code} Saved ");
                        }
                    }
                    #endregion

                    if (objPostRequiredDatas.Count == 0)
                        AddScrapedPhotoDataIntoDataBase(scrapeResult, instagramPost);
                    else
                        AddScrapedPhotoDataInfoIntoDataBase(scrapeResult, instagramPost, objPostRequiredDatas);
                    IncrementCounters();
                    jobProcessResult.IsProcessSuceessfull = true;
                }               
                else
                {
                    GlobusLogHelper.log.Info(Log.ActivityFailed, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, scrapeResult.ResultPost.Code, "");
                    jobProcessResult.IsProcessSuceessfull = false;
                }
               DelayBeforeNextActivity();
                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return jobProcessResult;
        }


        private string GetMediaExtension(string mediaUrl)
        {

            if (mediaUrl.Contains("?"))
                mediaUrl = Utilities.GetBetween(mediaUrl, "", "?");

            return mediaUrl.Split('.').Last();
        }

        private void AddScrapedPhotoDataIntoDataBase(ScrapeResultNew scrapeResult, InstagramPost instagramPost)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            // Add data to respected campaign InteractedPosts table
            if (!string.IsNullOrEmpty(CampaignId))
            {
                CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                {
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    MediaType = instagramPost.MediaType,
                    ActivityType = ActivityType.PostScraper,
                    PkOwner = instagramPost.Code,
                    UsernameOwner = instagramPost.User?.Username,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    TotalComment =Convert.ToString(instagramPost.CommentCount),
                    TotalLike = Convert.ToString(instagramPost.LikeCount),
                    PostLocation = instagramPost.Location!=null?instagramPost.Location.Name:"",
                    TakenAt = instagramPost.TakenAt,
                    PostUrl= $"https://www.instagram.com/p/{instagramPost.Code}/",
                Status = "Success"

                });
            }

            // Add data to respected Account InteractedPosts table
            AccountDbOperation.Add(
                new InteractedPosts()
                {
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    MediaType = instagramPost.MediaType,
                    ActivityType = ActivityType.PostScraper,
                    PkOwner = instagramPost.Code,
                    UsernameOwner = instagramPost.User?.Username,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    QueryType = scrapeResult.QueryInfo.QueryType,
                    QueryValue = scrapeResult.QueryInfo.QueryValue,
                    TotalComment = Convert.ToString(instagramPost.CommentCount),
                    TotalLike = Convert.ToString(instagramPost.LikeCount),
                    PostLocation = instagramPost.Location != null ? instagramPost.Location.Name : "",
                    TakenAt = instagramPost.TakenAt,
                    Status = "Success",                   
                });
        }

        private void AddScrapedPhotoDataInfoIntoDataBase(ScrapeResultNew scrapeResult, InstagramPost instagramPost, List<PostRequiredDatas> objPostRequiredDatas = null)
        {
            // Add data to respected campaign InteractedPosts table
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                if (!string.IsNullOrEmpty(CampaignId))
                {
                    if (objPostRequiredDatas != null)
                        CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                        {
                            InteractionDate = DateTimeUtilities.GetEpochTime(),
                            MediaType = instagramPost.MediaType,
                            ActivityType = ActivityType.PostScraper,
                            PkOwner = instagramPost.Code,
                            UsernameOwner = instagramPost.User?.Username,
                            Username = DominatorAccountModel.AccountBaseModel.UserName,
                            QueryType = scrapeResult.QueryInfo.QueryType,
                            QueryValue = scrapeResult.QueryInfo.QueryValue,
                            TotalComment = objPostRequiredDatas?.FirstOrDefault()?.totalComments,
                            TotalLike = objPostRequiredDatas?.FirstOrDefault()?.totalLikes,
                            PostLocation = objPostRequiredDatas?.FirstOrDefault()?.location,
                            TakenAt = objPostRequiredDatas?.FirstOrDefault()?.dataAndTime != string.Empty
                                ? Convert.ToInt32(objPostRequiredDatas?.FirstOrDefault()?.dataAndTime)
                                : 0,
                            Status = "Success",
                            Comment = instagramPost?.CommentText,
                            PostUrl = objPostRequiredDatas[0].PostUrl
                        });
                }

                // Add data to respected Account InteractedPosts table
                if (objPostRequiredDatas != null)
                    AccountDbOperation.Add(
                        new InteractedPosts()
                        {
                            InteractionDate = DateTimeUtilities.GetEpochTime(),
                            MediaType = instagramPost.MediaType,
                            ActivityType = ActivityType.PostScraper,
                            PkOwner = instagramPost.Code,
                            UsernameOwner = instagramPost.User?.Username,
                            Username = DominatorAccountModel.AccountBaseModel.UserName,
                            QueryType = scrapeResult.QueryInfo.QueryType,
                            QueryValue = scrapeResult.QueryInfo.QueryValue,
                            TotalComment = objPostRequiredDatas?.FirstOrDefault()?.totalComments,
                            TotalLike = objPostRequiredDatas?.FirstOrDefault()?.totalLikes,
                            PostLocation = objPostRequiredDatas?.FirstOrDefault()?.location,
                            TakenAt = objPostRequiredDatas?.FirstOrDefault()?.dataAndTime != string.Empty
                                ? Convert.ToInt32(objPostRequiredDatas?.FirstOrDefault()?.dataAndTime)
                                : 0,
                            Status = "Success",
                            Comment = instagramPost?.CommentText,
                            PostUrl = objPostRequiredDatas?.FirstOrDefault()?.PostUrl
                        });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }

        private string DownloaMedia(InstagramPost instagramPost,string filePath, ScrapeResultNew scrapeResult)
        {
            try
            {
                WebClient webclient = new WebClient();
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"Trying to download post {scrapeResult.ResultPost.Code}.");
                if (DownloadPhotosModel.isDownloadAlbum && instagramPost.MediaType == MediaType.Album)
                {
                    if (instagramPost.MediaType == MediaType.Album)
                    {
                        for (var iter = 0; iter < instagramPost.Album.Count; iter++)
                        {
                            var carouselMedia = instagramPost.Album[iter];
                            if ((MediaType)carouselMedia.MediaType == MediaType.Image)
                            {
                                filePath = $"{DownloadPhotosModel.DownloadedFolderPath}\\{iter + 1}-{instagramPost.Code}.jpg";//{GetMediaExtension(carouselMedia.Images[0].Url)}";
                                webclient.DownloadFile(carouselMedia.Images[0].Url, filePath);
                            }
                        }
                    }
                }else if (DownloadPhotosModel.isDownloadImage && instagramPost.MediaType == MediaType.Image)
                {
                    filePath = $"{DownloadPhotosModel.DownloadedFolderPath}\\{instagramPost.Code}.jpg";//{GetMediaExtension(instagramPost.Images.First().Url)}";
                    webclient.DownloadFile(instagramPost.Images.First().Url, filePath);
                    if (ModuleSetting.IsChangeHashOfImage)
                        filePath = CalculateMD5Hash(filePath);
                }else if (DownloadPhotosModel.isDownloadVideo && instagramPost.MediaType == MediaType.Video)
                {
                    //https://www.instagram.com/reel/CGP3Dm_p2Xf/
                    filePath = $"{DownloadPhotosModel.DownloadedFolderPath}\\{instagramPost.Code}.mp4";// {GetMediaExtension(instagramPost.Video.Url)}";
                    webclient.DownloadFile(instagramPost.Video.Url, filePath);
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    $"{scrapeResult.ResultPost.Code} Is Not Type Of Selected Download Media.");
                    filePath = DownloadPhotosModel.DownloadedFolderPath;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return filePath;
        }

        private List<PostRequiredDatas> GetRequiredData(InstagramPost instagramPost)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            ObservableCollection<DownloadPhotosModel.PostRequiredData> lstReqData = DownloadPhotosModel.ListUserRequiredData;
            PostRequiredDatas objPostRequiredDatas = new PostRequiredDatas();
           List<PostRequiredDatas> listPostRequiredDatas = new List<PostRequiredDatas>();
            try
            {
                var reqcount = lstReqData.Count(x => x.IsSelected);
                if (reqcount != 0)
                {
                    foreach (DownloadPhotosModel.PostRequiredData dataList in lstReqData)
                    {
                        if (dataList.ItemName.Contains("Likes") && dataList.IsSelected)
                            objPostRequiredDatas.totalLikes = instagramPost.LikeCount.ToString()??string.Empty;
                        else if (dataList.ItemName.Contains("Comments") && dataList.IsSelected)
                            objPostRequiredDatas.totalComments = instagramPost.CommentCount.ToString()??string.Empty;
                        else if (dataList.ItemName.Contains("date & Time") && dataList.IsSelected)
                            objPostRequiredDatas.dataAndTime = instagramPost.TakenAt.ToString()??string.Empty;
                        else if (dataList.ItemName.Contains("Location") && dataList.IsSelected)
                            objPostRequiredDatas.location = instagramPost.Location != null ? instagramPost.Location.Name : string.Empty;
                        else if (dataList.ItemName.Contains("Post Url") && dataList.IsSelected)
                            objPostRequiredDatas.PostUrl = $"https://www.instagram.com/p/{instagramPost.Code}/";
                    }
                    listPostRequiredDatas.Add(objPostRequiredDatas);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }  
            return listPostRequiredDatas;
        }

        public static string CalculateMD5Hash(string input)
        {
            string newImage = String.Empty;
            try
            {
                byte[] inputBytes = File.ReadAllBytes(input);
                byte[] bArray = new byte[inputBytes.Length + 1];
                inputBytes.CopyTo(bArray, 0);
                bArray[bArray.Length - 1] = Convert.ToByte('\0');
                newImage = input.Split('.')[0] + "_hash.jpg";
                File.WriteAllBytes(newImage, bArray);

                if (File.Exists(input))
                    File.Delete(input);
            }
            catch (Exception )
            {
            }
            return newImage;
        }

        public static string GetMd5HashCode(string input, MD5 md5, byte[] inputBytes)
        {
            StringBuilder sb = new StringBuilder();
            // byte[] inputBytess = File.ReadAllBytes(input);
            byte[] hash1 = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string

            for (int i = 0; i < hash1.Length; i++)
            {
                sb.Append(hash1[i].ToString("X2"));
            }
            return sb.ToString();
        }

    }
}
