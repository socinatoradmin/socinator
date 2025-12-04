using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdProcessors.FanpageProcessor
{
    public class BaseFbFanpageProcessor : BaseFbProcessor
    {
        //        private readonly object _lockReachedMaxTweetActionPerUser = new object();

        IResponseHandler _objFanpageScraperResponseHandler;

        private readonly FdJobProcess _objFdJobProcess;

        private readonly IAccountScopeFactory _accountScopeFactory;

        protected BaseFbFanpageProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
            _accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            _objFdJobProcess = (FdJobProcess)jobProcess;
            _objFanpageScraperResponseHandler = null;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {

        }

        public void ProcessDataOfPages(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
                    List<FanpageDetails> objLstFanpageDetails)
        {
            var filteredPages = false;
            foreach (var page in objLstFanpageDetails)
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                try
                {
                    if (AlreadyInteractedPages(page, AccountModel))
                    { filteredPages = true; continue; }

                    if (AlreadyInteractedPagesCustom(page, AccountModel))
                    { filteredPages = true; continue; }
                    //for custom we scrapped previously 
                    if (AccountModel.IsRunProcessThroughBrowser)
                        _objFanpageScraperResponseHandler = queryInfo.QueryTypeEnum != "CustomPageList"
                            ? Browsermanager.GetFullPageDetails(AccountModel, page, isNewWindow: false, isCloseBrowser: false)
                            : new FanpageScraperResponseHandler(new ResponseParameter() { Response = string.Empty }, new List<string>(), page);
                    else
                        _objFanpageScraperResponseHandler = _ActivityType == ActivityType.MessageToPlaces ?
                            ObjFdRequestLibrary.GetFanpageDetails(AccountModel, page, true) : ObjFdRequestLibrary.GetFanpageDetails(AccountModel, page);

                    JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    FilterData(queryInfo, ref jobProcessResult, _objFanpageScraperResponseHandler.ObjFdScraperResponseParameters.FanpageDetails);
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            if (filteredPages)
                GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType,
                            "Succefully Skipped the Filtered Pages");

        }


        protected void ApplyFanpageFilter(ref bool isVerifiedFilter, ref bool isLikedByFriends,
            ref FanpageCategory objFanpageCategory, QueryInfo queryInfo)
        {
            if (JobProcess.ModuleSetting.FanpageFilterModel.IsVerifiedPage)
                isVerifiedFilter = true;

            if (JobProcess.ModuleSetting.FanpageFilterModel.IsLikedByMyFriends)
                isLikedByFriends = true;

            if (JobProcess.ModuleSetting.FanpageFilterModel.IsFanpageCategory)
            {
                objFanpageCategory = (FanpageCategory)Enum.Parse(typeof(FanpageCategory),
                    JobProcess.ModuleSetting.FanpageFilterModel.SelectedCategory);
            }
        }

        private void FilterData(QueryInfo queryInfo, ref JobProcessResult jobProcessResult, FanpageDetails objFanpageDetails)
        {
            try
            {
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (_ActivityType == ActivityType.MessageToPlaces && !objFanpageDetails.CanSendMessage)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                        AccountModel.AccountBaseModel.UserName, _ActivityType, "SuccessFully Skipped as Can Not Send Message to this Page =>" + objFanpageDetails.FanPageID);
                    return;
                }

                if (JobProcess.ModuleSetting.FanpageFilterModel.IsLikedByMyFriends)
                {
                    bool isLikedByMyfriend;

                    bool.TryParse(objFanpageDetails.IsLikedByFriend, out isLikedByMyfriend);

                    if (!isLikedByMyfriend)
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                        AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyFanpageDosentMatchWithFilter".FromResourceDictionary(), objFanpageDetails.FanPageUrl));
                        return;
                    }

                }

                if (JobProcess.ModuleSetting.FanpageFilterModel.IsLikersRangeChecked)
                {
                    int likersCount;

                    Int32.TryParse(objFanpageDetails.FanPageLikerCount, out likersCount);

                    if (!JobProcess.ModuleSetting.FanpageFilterModel.LikersBetWeen.InRange(likersCount))
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                    AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyFanpageDosentMatchWithFilter".FromResourceDictionary(), objFanpageDetails.FanPageUrl));
                        return;
                    }

                }

                if (JobProcess.ModuleSetting.FanpageFilterModel.IsVerifiedPage)
                {
                    bool.TryParse(objFanpageDetails.IsVerifiedPage, out bool isVerified);
                    if (!isVerified)
                    {
                        _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                                        AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyFanpageDosentMatchWithFilter".FromResourceDictionary(), objFanpageDetails.FanPageUrl));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.Message);
            }

            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            SendToPerformActivity(ref jobProcessResult, objFanpageDetails, queryInfo);

        }



        // ReSharper disable once RedundantAssignment
        public void SendToPerformActivity(ref JobProcessResult jobProcessResult, FanpageDetails objFanpageDetails,
                    QueryInfo queryInfo)
        {

            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
            {
                ResultPage = objFanpageDetails,
                QueryInfo = queryInfo
            });
        }

    }
}
