using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.Factories;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using System;
using ThreadUtils;

namespace GramDominatorCore.GDLibrary.Process
{
    public class AddStoryProcess : GdJobProcessInteracted<InteractedPosts>
    {
        public AddStoryProcess(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped, 
            IGdQueryScraperFactory queryScraperFactory, IGdHttpHelper httpHelper,
            IGdBrowserManager gdBrowser, IDelayService _delayService)
            : base(processScopeModel, accountServiceScoped, 
                  queryScraperFactory, httpHelper, gdBrowser, _delayService)
        {
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            var jobProcessResult = new JobProcessResult();
            try
            {
                var InstaPost = scrapeResult.ResultPost as InstagramPost;
                JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                GlobusLogHelper.log.Info(Log.StartedActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, InstaPost?.Code);
                if (InstaPost != null)
                {
                    foreach (var media in InstaPost.RepostMedia)
                    {
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        var AddedStory = instaFunct.StoryUpload(DominatorAccountModel, InstaPost,media).Result;
                        if(AddedStory != null && AddedStory.Success)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $" {InstaPost?.Code} at {AddedStory?.StoryUrl} as Story");
                            AddRepostedDataToDataBase(scrapeResult, AddedStory,null);
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"{media} as Story failed to upload.");
                        }
                        if (InstaPost.RepostMedia.Count > 1)
                        {
                            var delay = RandomUtilties.GetRandomNumber(20, 5);
                            DelayBeforeNextActivity(delay); // Delay between each story upload
                        }
                        JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                    
                    IncrementCounters();
                    jobProcessResult.IsProcessSuceessfull = true;
                    DelayBeforeNextActivity();
                }
            }
            catch(OperationCanceledException opc) {
                throw new OperationCanceledException(opc?.Message);
            }
            catch(Exception e)
            {
                e.DebugLog();
            }
            return jobProcessResult;
        }

        private void AddRepostedDataToDataBase(ScrapeResultNew scrapResult, UploadMediaResponse uploadPhotoResponse, string UserLst = null)
        {
            JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            InstagramPost instagramPost = (InstagramPost)scrapResult.ResultPost;

            // Add data to respected campaign InteractedPosts table
            if (!string.IsNullOrEmpty(CampaignId))
            {
                var dboperationCampaign =
                    new DbOperations(CampaignId,DominatorHouseCore.Enums.SocialNetworks.Instagram, ConstantVariable.GetCampaignDb);

                if (ModuleSetting.EnableDelayBetweenPerformingActionOnSamePost || ModuleSetting.IsPerformActionFromRandomPercentageOfAccounts)
                {
                    string permalink = instagramPost.Code.GetUrlFromCode();

                    var interactedPost =
                        dboperationCampaign.GetSingle<DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts>(
                            x => x.Permalink == permalink && x.ActivityType == ActivityType &&
                                 x.Username == DominatorAccountModel.AccountBaseModel.UserName &&
                                 (x.Status == "Pending" || x.Status == "Working"));

                    if (interactedPost != null)
                    {
                        interactedPost.InteractionDate = DateTimeUtilities.GetEpochTime();
                        interactedPost.MediaType = uploadPhotoResponse.MediaType;
                        interactedPost.ActivityType = ActivityType;
                        interactedPost.OriginalMediaCode = instagramPost.Code;
                        interactedPost.OriginalMediaOwner = instagramPost.User.Username;
                        interactedPost.PkOwner = uploadPhotoResponse.Code;
                        interactedPost.UsernameOwner = DominatorAccountModel.AccountBaseModel.UserName;
                        interactedPost.Username = DominatorAccountModel.AccountBaseModel.UserName;
                        interactedPost.QueryType = scrapResult.QueryInfo.QueryType;
                        interactedPost.QueryValue = scrapResult.QueryInfo.QueryValue;
                        interactedPost.Status = "Success";
                        interactedPost.Comment = instagramPost.CommentText;
                        dboperationCampaign.Update(interactedPost);
                    }
                }
                else
                {
                    CampaignDbOperation?.Add(new DominatorHouseCore.DatabaseHandler.GdTables.Campaigns.InteractedPosts()
                    {
                        InteractionDate = DateTimeUtilities.GetEpochTime(),
                        MediaType = uploadPhotoResponse.MediaType,
                        ActivityType = ActivityType,
                        OriginalMediaCode = instagramPost.Code,
                        OriginalMediaOwner = instagramPost.User.Username,
                        PkOwner = uploadPhotoResponse.Code,
                        UsernameOwner = DominatorAccountModel.AccountBaseModel.UserName,
                        Username = DominatorAccountModel.AccountBaseModel.UserName,
                        QueryType = scrapResult.QueryInfo.QueryType,
                        QueryValue = scrapResult.QueryInfo.QueryValue,
                        UsersTag = UserLst,
                        Comment = instagramPost.CommentText,
                        Status = "Success"

                    });
                }
            }


            // Add data to respected Account InteractedPosts table
            AccountDbOperation.Add(
                new InteractedPosts()
                {
                    InteractionDate = DateTimeUtilities.GetEpochTime(),
                    MediaType = uploadPhotoResponse.MediaType,
                    ActivityType = ActivityType,
                    OriginalMediaCode = instagramPost.Code,
                    OriginalMediaOwner = instagramPost.User.Username,
                    PkOwner = uploadPhotoResponse.Code,
                    UsernameOwner = DominatorAccountModel.AccountBaseModel.UserName,
                    Username = DominatorAccountModel.AccountBaseModel.UserName,
                    QueryType = scrapResult.QueryInfo.QueryType,
                    QueryValue = scrapResult.QueryInfo.QueryValue,
                    Comment = instagramPost.CommentText
                });

            // Add data to respected Account's FeedInfoes table
            AccountDbOperation.Add(
                new FeedInfoes()
                {
                    Caption = instagramPost.Caption,
                    TakenAt = uploadPhotoResponse.TakenAt,
                    MediaId = uploadPhotoResponse.MediaId,
                    MediaCode = uploadPhotoResponse.Code,
                    MediaType = uploadPhotoResponse.MediaType,
                    PostedBy = "Software"
                });
        }
    }
}
