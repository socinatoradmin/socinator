using ThreadUtils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.DAL;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDModel;
using GramDominatorCore.Response;
using System;
using System.Collections.Generic;
using GramDominatorCore.Utility;

namespace GramDominatorCore.GDLibrary.Processor.User
{
    public class UsersWhoLikedPostProcessor : BaseInstagramUserProcessor
    {
        public UsersWhoLikedPostProcessor(IGdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbCampaignService campaignService, IProcessScopeModel processScopeModel, IDelayService _delayService, IGdBrowserManager gdBrowserManager) :
            base(jobProcess, dbAccountService, campaignService, processScopeModel, _delayService,gdBrowserManager)
        {

        }
        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                Token.ThrowIfCancellationRequested();

                QueryType = queryInfo.QueryType;
                string idFromCode = CheckPostId(queryInfo);
                while (!jobProcessResult.IsProcessCompleted && !jobProcessResult.HasNoResult)
                {
                    Token.ThrowIfCancellationRequested();
                    var instaUserList = new List<InstagramUser>();
                    var mediaLikers = 
                        GramStatic.IsBrowser ?
                        GdBrowserManager.GetMediaLikers(DominatorAccountModel, queryInfo.QueryValue, Token)
                        : InstaFunction.GetMediaLikers(DominatorAccountModel, idFromCode, Token, jobProcessResult.maxId);
                    if (!CheckingLoginRequiredResponse(mediaLikers.ToString(), "", queryInfo))
                        return;
                    if (mediaLikers.Success)
                    {
                        #region Process for "UsersWhoLikedPost" query parameter
                        var usersList = FilterWhitelistBlacklistUsers(mediaLikers.UserList);
                        CheckUserInDatabase(usersList ?? mediaLikers.UserList);
                        if (ActivityType == ActivityType.UserScraper && ModuleSetting.IsTaggedPostUser)
                            GetTaggedUser(queryInfo, jobProcessResult, usersList ?? mediaLikers.UserList);
                        else
                        {
                            if (ActivityType == ActivityType.Follow)
                            {
                                if (mediaLikers.UserList.Count > 30)
                                {
                                    int count = 0;
                                    for (int i = 0; i < mediaLikers.UserList.Count; i++)
                                    {
                                        count++;
                                        instaUserList.Add(mediaLikers.UserList[i]);
                                        if (count == 30 || mediaLikers.UserList.Count - 1 == i)
                                        {
                                            count = 0;
                                            instaUserList = GetUserInfoDetails(instaUserList);
                                            jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, instaUserList);
                                            instaUserList = new List<InstagramUser>();
                                        }
                                    }
                                }
                                else
                                {
                                    usersList = GetUserInfoDetails(usersList ?? mediaLikers.UserList);
                                    jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? mediaLikers.UserList);
                                }
                            }
                            else
                            {
                                jobProcessResult = FilterAndStartFinalProcess(queryInfo, jobProcessResult, usersList ?? mediaLikers.UserList);
                            }

                        }
                        #endregion
                        jobProcessResult.maxId = mediaLikers.MaxId;
                        if (string.IsNullOrEmpty(jobProcessResult.maxId))
                            CheckNoMoreDataForWithQuery(ref jobProcessResult);
                    }
                    else
                        jobProcessResult.maxId = null;

                    CheckNoMoreDataForWithQuery(ref jobProcessResult);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
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
        }

    }
}
