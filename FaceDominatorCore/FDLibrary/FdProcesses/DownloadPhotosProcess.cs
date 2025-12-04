using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDRequest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AccountInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts;
using CampaignInteractedPosts = DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts;

namespace FaceDominatorCore.FDLibrary.FdProcesses
{
    public class DownloadPhotosProcess : FdJobProcessInteracted<AccountInteractedPosts>
    {
        public PostLikerModel PostLikerModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public IFdRequestLibrary FdRequestLibrary { get; set; }

        public DownloadPhotosProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IFdRequestLibrary fdRequestLibrary,
            IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {
            FdRequestLibrary = fdRequestLibrary;
            PostLikerModel = processScopeModel.GetActivitySettingsAs<PostLikerModel>();
            AccountModel = DominatorAccountModel;
            CheckJobProcessLimitsReached();
        }


        public override void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {

        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {

            JobProcessResult jobProcessResult = new JobProcessResult();
            var comment = string.Empty;

            string folderPath;

            FacebookPostDetails objFacebookPostDetails = (FacebookPostDetails)scrapeResult.ResultPost;


            try
            {
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();


                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookPostDetails.Id);

                ReactionType objReactionType = ReactionType.Like;

                string filepath;

                var count = 0;

                List<Tuple<MediaType, string>> url = GetMediaUrls(objFacebookPostDetails);

                if (objFacebookPostDetails.QueryType == PostOptions.Albums.ToString())
                {
                    folderPath = FdConstants.DownloadFolderPathAlbums(objFacebookPostDetails.OwnerName,
                                objFacebookPostDetails.AlbumName);

                    url.ForEach(x =>
                    {
                        if (x.Item1 == MediaType.Video)
                        {
                            filepath = folderPath + $"{objFacebookPostDetails.Id}" + $"_{count}.mp4";
                            FdFunctions.FdFunctions.DownLoadMediaFromUrlAsync(x.Item2, filepath, folderPath);
                        }
                        else if (x.Item1 == MediaType.Image)
                        {
                            filepath = folderPath + $"{objFacebookPostDetails.Id}" + $"_{count}.jpg";
                            FdFunctions.FdFunctions.DownLoadMediaFromUrlAsync(x.Item2, filepath, folderPath);
                        }
                        count++;
                    });
                }
                else
                {
                    folderPath = FdConstants.DownloadFolderPathDocuments + $@"{DominatorAccountModel.AccountBaseModel.UserName}\";

                    url.ForEach(x =>
                    {
                        if (x.Item1 == MediaType.Video)
                        {
                            filepath = folderPath + $"{objFacebookPostDetails.Id}"
                                        + $"_{count}.mp4";
                            FdFunctions.FdFunctions.DownLoadMediaFromUrlAsync(x.Item2, filepath, folderPath);
                        }
                        else if (x.Item1 == MediaType.Image)
                        {
                            filepath = folderPath + $"{objFacebookPostDetails.Id}"
                                        + $"_{count}.jpg";
                            FdFunctions.FdFunctions.DownLoadMediaFromUrlAsync(x.Item2, filepath, folderPath);
                        }
                        count++;
                    });
                }


                GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, objFacebookPostDetails.Id);

                IncrementCounters();
                jobProcessResult.IsProcessSuceessfull = true;

                AddProfileScraperDataToDatabase(scrapeResult, objReactionType, comment,
                    PostLikerModel.LikerCommentorConfigModel.IsLikeTypeFilterChkd, folderPath);


                DelayBeforeNextActivity();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }

        private List<Tuple<MediaType, string>> GetMediaUrls(FacebookPostDetails objFacebookPostDetails)
        {
            List<Tuple<MediaType, string>> listModel = new List<Tuple<MediaType, string>>();

            if (objFacebookPostDetails.MediaType == MediaType.Video &&
                (objFacebookPostDetails.MediaUrl.Contains("https://") ||
                   objFacebookPostDetails.MediaUrl.Contains("http://")))
            {

                listModel.Add(new Tuple<MediaType, string>(MediaType.Video, objFacebookPostDetails.MediaUrl));
            }

            if (objFacebookPostDetails.MediaType != MediaType.Video)
            {
                if (objFacebookPostDetails.OtherMediaUrl == "NA" && objFacebookPostDetails.MediaUrl != "NA")
                {
                    listModel.Add(new Tuple<MediaType, string>(MediaType.Image, objFacebookPostDetails.MediaUrl));
                }
                else if (string.IsNullOrEmpty(objFacebookPostDetails.OtherMediaUrl) && !string.IsNullOrEmpty(objFacebookPostDetails.MediaUrl))
                {
                    listModel.Add(new Tuple<MediaType, string>(MediaType.Image, objFacebookPostDetails.MediaUrl));
                }
                else if (objFacebookPostDetails.OtherMediaUrl != "NA")
                {
                    var media = Regex.Split(objFacebookPostDetails.OtherMediaUrl, ",");


                    foreach (var eachMedia in media)
                    {
                        var newMedia = eachMedia.Replace("||", string.Empty);
                        if (newMedia.Contains("https://external") && newMedia.Contains("url="))
                        {
                            newMedia = Utilities.GetBetween(newMedia, "url=", "&");
                            newMedia = Uri.UnescapeDataString(newMedia);
                        }
                        listModel.Add(new Tuple<MediaType, string>(MediaType.Image, newMedia));
                    }
                }
            }

            return listModel;
        }



        void AddProfileScraperDataToDatabase(ScrapeResultNew scrapeResult, ReactionType objReactionType,
            string comment, bool isLiked, string folderPath)
        {
            try
            {

                var likeType = objReactionType.ToString();

                if (!isLiked)
                {
                    likeType = " ";
                }

                FacebookPostDetails group = (FacebookPostDetails)scrapeResult.ResultPost;
                var jobActivityConfigurationManager = InstanceProvider.GetInstance<IJobActivityConfigurationManager>();

                var modulesetting = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];

                group.DownLoadPath = folderPath;

                if (modulesetting == null)
                    return;

                if (modulesetting.IsTemplateMadeByCampaignMode)
                {
                    DbCampaignService.Add(new CampaignInteractedPosts
                    {
                        ActivityType = ActivityType.ToString(),
                        QueryType = group.QueryType,
                        QueryValue = group.QueryValue,
                        AccountEmail = DominatorAccountModel.AccountBaseModel.UserName,
                        PostId = group.Id,
                        LikeType = likeType,
                        Comment = comment,
                        InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                        InteractionDateTime = DateTime.Now,
                        PostDescription = JsonConvert.SerializeObject(group),
                        OwnerId = group.OwnerId,

                    });
                }

                DbAccountService.Add(new AccountInteractedPosts
                {
                    ActivityType = ActivityType.ToString(),
                    QueryType = group.QueryType,
                    QueryValue = group.QueryValue,
                    PostId = group.Id,
                    LikeType = likeType,
                    Comment = comment,
                    InteractionTimeStamp = DateTimeUtilities.GetEpochTime(),
                    InteractionDateTime = DateTime.Now,
                    PostDescription = JsonConvert.SerializeObject(group),
                    OwnerId = group.OwnerId,

                });
            }
            catch (Exception ex)
            {
                ex.ErrorLog(ex.Message);
            }
        }
    }
}
