using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.Interface;
using System;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdProcessors.InviterProcessor
{
    public class BaseFbInviterProcessor : BaseFbProcessor
    {
        private IResponseHandler _userInfoResponseHandler;

        /*
                private readonly object _lockReachedMaxTweetActionPerUser = new object();
        */

        readonly FdJobProcess _objFdJobProcess;



        protected BaseFbInviterProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService,
            IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager,
            IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager,
                processScopeModel)
        {
            _objFdJobProcess = (FdJobProcess)jobProcess;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            throw new NotImplementedException();
        }

        public void ProcessDataForInviter(QueryInfo queryInfo, ref JobProcessResult jobProcessResult,
                    List<FacebookUser> objLstFacebookUser, IEntity objEntity)
        {
            if (queryInfo.QueryTypeEnum == "CustomProfileUrl")
            {
                objLstFacebookUser.ForEach(x =>
                {
                    x.UserId = ObjFdRequestLibrary.GetFriendUserId(AccountModel, x.ProfileUrl).UserId;
                });
            }

            objLstFacebookUser = CheckBlacklistUser(objLstFacebookUser);

            _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            foreach (var user in objLstFacebookUser)
            {
                _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                try
                {
                    if (AlreadyInteractedUser(user))
                        continue;

                    _userInfoResponseHandler = ObjFdRequestLibrary.GetDetailedInfoUserMobileScraper(user, AccountModel, false, true);

                    if (_userInfoResponseHandler == null)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, _ActivityType, string.Format("LangKeyCannotSendRequest".FromResourceDictionary(), $"{FdConstants.FbHomeUrl}{user.UserId}"));
                        continue;
                    }

                    _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    SendToPerformActivity(ref jobProcessResult, objEntity, queryInfo, _userInfoResponseHandler.ObjFdScraperResponseParameters.FacebookUser);


                    if (jobProcessResult.HasNoResult)
                        break;

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                _objFdJobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }

        }





        // ReSharper disable once RedundantAssignment
        public void SendToPerformActivity(ref JobProcessResult jobProcessResult, IEntity objEntity,
                    QueryInfo queryInfo, FacebookUser objUser)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

            jobProcessResult = JobProcess.FinalProcess(new ScrapeResultNew()
            {
                ResultEntity = objEntity,
                QueryInfo = queryInfo,
                ResultUser = objUser
            });
        }

    }
}

