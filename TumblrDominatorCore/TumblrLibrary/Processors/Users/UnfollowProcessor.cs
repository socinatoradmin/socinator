using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrLibrary.DAL;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction;
using TumblrDominatorCore.TumblrLibrary.TumblrFunction.TumblrBrowserManager;
using TumblrDominatorCore.TumblrLibrary.TumblrProcesses;
using TumblrDominatorCore.TumblrResponseHandler;

namespace TumblrDominatorCore.TumblrLibrary.Processors.Users
{
    internal class UnfollowProcessor : BaseTumblrUserProcessor, IQueryProcessor
    {
        private readonly ITumblrBrowserManager _browser;
        private int pageId;
        private readonly ITumblrFunct tumblrFucntion;
        private string MainPageUrl;
        private SearchforFollowingsorFollowersResponse searchUsersForFollowingResponse;
        public UnfollowProcessor(ITumblrBrowserManager browser, IProcessScopeModel processScopeModel,
            ITumblrJobProcess jobProcess, IDbAccountServiceScoped dbAccountService, IDbGlobalService globalService,
            IDbCampaignService campaignService, ITumblrFunct tumblrFunct) :
            base(processScopeModel, jobProcess, dbAccountService, globalService, campaignService, tumblrFunct)
        {
            _browser = browser;
            tumblrFucntion = tumblrFunct;
        }

        public UnfollowerModel UnFollowerModel { get; set; }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

            try
            {
                var templatesFileManager = InstanceProvider.GetInstance<ITemplatesFileManager>();
                var lstUserList = new List<TumblrUser>();
                UnFollowerModel = JsonConvert.DeserializeObject<UnfollowerModel>(templatesFileManager.GetTemplateById(JobProcess.TemplateId)?.ActivitySettings);

                if (UnFollowerModel.IsChkCustomUsersListChecked)
                {
                    Regex.Split(UnFollowerModel.CustomUsersList, "\r\n").ToList().ForEach(x =>
                    {
                        lstUserList.Add(TumblrUtility.getTumblrUserFromPostUrlorBlogUrlOrUsername(x));
                    });
                }
                if (UnFollowerModel.IsChkPeopleFollowedBySoftwareChecked)
                {
                    _dbAccountService.GetInteractedUsers(ActivityType.Follow).ForEach(
                        x =>
                        {
                            try
                            {
                                lstUserList.Add(new TumblrUser
                                {
                                    Username = x.InteractedUsername,
                                    PageUrl = x.PageUrl,
                                    TumblrsFormKey = ""
                                });
                            }
                            catch (Exception)
                            {
                            }
                        });
                }
                if (UnFollowerModel.IsChkPeopleFollowedOutsideSoftwareChecked)
                {
                    if (!JobProcess.DominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        if (string.IsNullOrEmpty(MainPageUrl))
                            MainPageUrl = ConstantHelpDetails.GetUsersOwnFollowingsAPI;
                        searchUsersForFollowingResponse =
                            tumblrFucntion.GetAcccountFollowings(JobProcess.DominatorAccountModel, pageId, MainPageUrl);
                        if (searchUsersForFollowingResponse != null && !string.IsNullOrEmpty(searchUsersForFollowingResponse.NextPageUrl))
                            MainPageUrl = searchUsersForFollowingResponse.NextPageUrl;
                    }
                    else
                        searchUsersForFollowingResponse = _browser.GetFollowingsOrFollowers(ConstantHelpDetails.FollowingUrl, pageId);
                    if (searchUsersForFollowingResponse == null || searchUsersForFollowingResponse.LstTumblrUser == null)
                        GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.AccountBaseModel.UserName, JobProcess.ActivityType.ToString(), "Error While searching for Followings");
                    if (searchUsersForFollowingResponse != null && searchUsersForFollowingResponse.Success)
                    {
                        if (searchUsersForFollowingResponse.TotalFollowings == 0 && searchUsersForFollowingResponse.LstTumblrUser.Count == 0)
                            GlobusLogHelper.log.Info(JobProcess.DominatorAccountModel.UserName + " have " +
                                                      searchUsersForFollowingResponse.TotalFollowings + " Followings ");
                        if (searchUsersForFollowingResponse.LstTumblrUser.Count > 0)
                            searchUsersForFollowingResponse.LstTumblrUser.RemoveAll(x
                                 => _dbAccountService.GetInteractedUsers(ActivityType.Follow).Any(y => y.InteractedUsername.Contains(x.Username)));
                        if (searchUsersForFollowingResponse.TotalFollowings > 0 && searchUsersForFollowingResponse.LstTumblrUser.Count == 0)
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage,
                            JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                            JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                            "No post Found for LikedOutsideSoftwere in this Page");
                        }
                        else
                            lstUserList.AddRange(searchUsersForFollowingResponse.LstTumblrUser);
                    }
                }
                #region database checking
                var skippeduserCount = lstUserList.RemoveAll(x =>
                _dbAccountService.GetUnfollowedUsers(ActivityType).Any(y => y.InteractedUsername.Contains(x.Username)));

                if (skippeduserCount > 0)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                "Successfully Skipped " + skippeduserCount + " No of Users as already UnFollowed from this Software");

                #endregion
                if (lstUserList.Count > 0)
                    FilterAndStartFinalProcess(queryInfo, ref jobProcessResult, lstUserList);
                else
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                                        "No Users Found To UnFollow");
                if (!jobProcessResult.IsProcessSuceessfull)
                    pageId++;

            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    " method=>> StartUnFollowProcess",
                    JobProcess.DominatorAccountModel.UserName + " " + ex.Message);
            }
            if (string.IsNullOrEmpty(searchUsersForFollowingResponse.NextPageUrl) || string.IsNullOrEmpty(MainPageUrl)
                || (pageId == 5 && !string.IsNullOrEmpty(searchUsersForFollowingResponse.NextPageUrl)))
            {
                GlobusLogHelper.log.Info(Log.ProcessCompleted,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                jobProcessResult.IsProcessCompleted = true;
            }
        }
    }
}