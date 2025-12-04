using ThreadUtils;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;
using System.Collections.Generic;
using System.Linq;

namespace PinDominatorCore.PDLibrary.Processors.User
{
    public class CustomUserProcessor : BasePinterestUserProcessor
    {
        private UserNameInfoPtResponseHandler _userNameInfoPtResponseHandler;
        public CustomUserProcessor(IPdJobProcess jobProcess, IDbGlobalService globalService,
            IDbCampaignService campaignService, IPinFunction objPinFunct, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct, delayService)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var arrUser = queryInfo.QueryValue.Split('/');
            if (queryInfo.QueryValue.Contains("/") && (arrUser.Length >= 5 && !string.IsNullOrEmpty(arrUser[4]) ||
                                                       !(arrUser.Length >= 4 && arrUser[0].Contains("http") &&
                                                         arrUser[2].Contains("pinterest") &&
                                                         !string.IsNullOrEmpty(arrUser[3]))))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType,
                    string.Format("LangKeyCheckSomeUrlSeemsIncorrect".FromResourceDictionary(), "LangKeyUser".FromResourceDictionary()));
                jobProcessResult.IsProcessCompleted = true;
                return;
            }
            _userNameInfoPtResponseHandler = PinFunction.GetUserDetails(queryInfo.QueryValue, JobProcess.DominatorAccountModel).Result;

            if (_userNameInfoPtResponseHandler == null || !_userNameInfoPtResponseHandler.Success)
            {
                GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, ActivityType);
                jobProcessResult.HasNoResult = true;
            }
            else
            {
                var usersList = new List<PinterestUser>();
                usersList.Add(_userNameInfoPtResponseHandler);
                usersList = AlreadyInteractedUserWithSameQuery(usersList,queryInfo);
                usersList = FilterBlackListUser(TemplateModel, usersList);
                if (usersList.Count != 0)
                    StartProcessForListOfUsers(queryInfo, ref jobProcessResult, usersList);
                jobProcessResult.HasNoResult = true;
            }
        }
    }
}