using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;
using System.Linq;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.Post
{
    public class DeletePostProcessor : BaseInstagramPostProcessor
    {
        private List<InteractedPosts> LstInteractedPosts { get; set; } = new List<InteractedPosts>();
        public DeletePostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService delayService, IGdBrowserManager gdBrowserManager)
            : base(jobProcess, dbAccountService, campaignService, processScopeModel, delayService, gdBrowserManager)
        {
        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();
                var lstNewFeeds = new List<InstagramPost>();
                var allPosts=new List<FeedInfoes>();
                var browser = GramStatic.IsBrowser;
                if (ModuleSetting.ChkDeletePostWhichIsPostedByOutsideSoftware &&
                     ModuleSetting.ChkDeletePostWhichIsPostedBySoftware)
                {
                    allPosts = DbAccountService.GetFeedInfos().ToList();
                    if(allPosts.Count == 0)
                    {
                        var feedResponse =
                            browser ?
                            InstaFunction.GdBrowserManager.GetUserFeed(DominatorAccountModel, DominatorAccountModel.AccountBaseModel.UserName, DominatorAccountModel.Token)
                            : InstaFunction.GetUserFeed(DominatorAccountModel,new AccountModel(DominatorAccountModel), DominatorAccountModel.AccountBaseModel.UserName, DominatorAccountModel.Token);
                        lstNewFeeds.AddRange(feedResponse?.Items);
                    }
                }
                else if (ModuleSetting.ChkDeletePostWhichIsPostedByOutsideSoftware)
                {
                    var feedResponse = 
                        browser ?
                        InstaFunction.GdBrowserManager.GetUserFeed(DominatorAccountModel, DominatorAccountModel.AccountBaseModel.UserName, DominatorAccountModel.Token)
                        : InstaFunction.GetUserFeed(DominatorAccountModel, new AccountModel(DominatorAccountModel), DominatorAccountModel.AccountBaseModel.UserName, DominatorAccountModel.Token);
                    lstNewFeeds.AddRange(feedResponse?.Items);
                }
                else if (ModuleSetting.ChkDeletePostWhichIsPostedBySoftware)
                    allPosts = DbAccountService.GetFeedInfos().Where(x => x.PostedBy == "Software").ToList();

                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Please Check delete Post Option");
                    return;
                }

                if (allPosts.Count == 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "No Post is availble in Database");
                else
                {
                    #region Perform operation with database feeds
                    allPosts.ForEach(post =>
                    {
                        Token.ThrowIfCancellationRequested();
                        InstagramPost instagramPost = new InstagramPost
                        {
                            Caption = post.Caption,
                            Code = post.MediaCode,
                            Pk = post.MediaId,
                            MediaType = post.MediaType,
                            CommentCount = post.CommentCount,
                            CommentsDisabled = post.CommentsDisabled,
                            TakenAt = post.TakenAt
                        };

                        if (post.MediaType == MediaType.Video)
                            instagramPost.ViewCount = post.ViewCount;

                        lstNewFeeds.Add(instagramPost);
                    });

                    var filteredFeeds = FilterWhitelistBlacklistUsersFromFeeds(lstNewFeeds);
                  //filteredFeeds.RemoveAll(x => x.Code != "CIAnKzZHFOw");
                    filteredFeeds.Shuffle();
                    //checking data from account db
                    LstInteractedPosts = DbAccountService.GetInteractedPosts(DominatorAccountModel.UserName, ActivityType).ToList();
                    filteredFeeds.RemoveAll(x => LstInteractedPosts.Any(y => y.PkOwner == x.Code));
                    foreach (var instagramPost in filteredFeeds)
                    {
                        FilterAndStartFinalProcessForOnePost(QueryInfo.NoQuery, ref jobProcessResult, instagramPost);

                        Token.ThrowIfCancellationRequested();
                    }
                    #endregion
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException e)
            {
                foreach (Exception ex in e.InnerExceptions)
                    Console.WriteLine(ex.Message);
            }
            catch (Exception)
            {
                // ex.DebugLog();
            }
            finally
            {
                jobProcessResult.IsProcessCompleted = true;
            }
        }
    }
}
