using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FaceDominatorCore.FDLibrary.FdProcessors.InviterProcessor
{
    public class EventInviterProcessor : BaseFbProcessor
    {
        public EventInviterProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            FilterAndStartFinalProcessForEventInviter(jobProcessResult, "profile");
        }

        private void FilterAndStartFinalProcessForEventInviter(JobProcessResult jobProcessResult, string query)
        {
            foreach (var eventUrl in JobProcess.ModuleSetting.InviterDetailsModel.ListEventUrl)
            {
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var eventDetails = ObjFdRequestLibrary.GetEventDetailsFromUrl(AccountModel, eventUrl);

                    eventDetails.FdEvents.EventId = FdRegexUtility.FirstMatchExtractor(eventUrl, "https://www.facebook.com/events/(.*?)/");

                    eventDetails.FdEvents.EventId = string.IsNullOrEmpty(eventDetails.FdEvents.EventId) ? eventUrl.Split('/').LastOrDefault()
                        : eventDetails.FdEvents.EventId;

                    var lstUserId = JobProcess.ModuleSetting.SelectAccountDetailsModel.AccountFriendsPair.Where(y => y.Key == AccountModel.AccountId).Select(x => x.Value).ToList();

                    AppplyPostFilterAndStartFinalProcessForEvemtInviter(ref jobProcessResult, lstUserId, query, eventDetails.FdEvents);

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (jobProcessResult.IsProcessCompleted)
                        break;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            jobProcessResult.HasNoResult = !jobProcessResult.IsProcessCompleted;
        }

        private void AppplyPostFilterAndStartFinalProcessForEvemtInviter(ref JobProcessResult jobProcessResult, List<string> lstUserId, string query, FdEvents eventDetails)
        {

            foreach (var friendUrl in lstUserId)
            {
                try
                {
                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    bool isIgnore = false;

                    FacebookUser objUser = new FacebookUser();

                    var lstFriends = _dbAccountService.Get<Friends>();

                    //var friendId = ObjFdRequestLibrary.GetFriendUserId(AccountModel, friendUrl).UserId;

                    var friendId = FdRegexUtility.FirstMatchExtractor(friendUrl, "https://www.facebook.com/(.*?)/");
                    friendId = string.IsNullOrEmpty(friendId) ? friendUrl.Split('/').LastOrDefault() : friendId;

                    if (lstFriends.FirstOrDefault(y => y.FriendId == friendId) == null)
                        isIgnore = true;
                    else
                    {
                        var friendDetails = lstFriends.FirstOrDefault(y => y.FriendId == friendId);
                        if (friendDetails != null)
                        {
                            objUser.UserId = friendDetails.FriendId;
                            objUser.ProfileUrl = friendDetails.ProfileUrl;
                            if (string.IsNullOrEmpty(objUser.ProfilePicUrl) && !string.IsNullOrEmpty(friendDetails.DetailedUserInfo))
                                objUser.ProfilePicUrl = new JsonHandler(friendDetails.DetailedUserInfo).GetElementValue("ScrapedProfileUrl");
                            objUser.Familyname = friendDetails.FullName;
                        }
                    }

                    if (_dbAccountService.DoesInteractedUserExist(friendId, _ActivityType))
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, _ActivityType, $"Already sent event invitation to user {friendId}");
                        continue;
                    }

                    if (!isIgnore)
                    {
                        FilterAndStartFinalProcessForEachEventInvite(query, friendUrl, eventDetails, out jobProcessResult,
                            objUser);
                    }

                    if (JobProcess.IsStopped())
                        break;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private void FilterAndStartFinalProcessForEachEventInvite(string query, string friendUrl, FdEvents eventDetails, out JobProcessResult jobProcessResult, FacebookUser objFacebookUser)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                QueryInfo objQueryInfo = new QueryInfo { QueryType = query, QueryValue = friendUrl };

                jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
                {
                    ResultUser = objFacebookUser,
                    QueryInfo = objQueryInfo,
                    ResultEvent = eventDetails
                });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                jobProcessResult = new JobProcessResult();
            }
        }

    }
}
