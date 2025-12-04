using ThreadUtils;
using DominatorHouseCore;
using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using PinDominatorCore.PDLibrary.DAL;
using PinDominatorCore.PDModel;
using PinDominatorCore.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PinDominatorCore.PDLibrary.Processors.User
{
    public class SendMessageToFollowerProcessor : BasePinterestUserProcessor
    {        
        public SendMessageToFollowerProcessor(IPdJobProcess jobProcess, 
            IDbGlobalService globalService, IDbCampaignService campaignService,
            IPinFunction objPinFunct, IDelayService delayService) :
            base(jobProcess, globalService, campaignService, objPinFunct, delayService)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType,
                    "LangKeySearchingForMessagesFromNewFollowers".FromResourceDictionary());
                
                FollowerAndFollowingPtResponseHandler response = null;
                var bookmark = string.Empty;
                do
                {
                    response = PinFunction.GetUserFollowers(JobProcess.DominatorAccountModel.AccountBaseModel.ProfileId, JobProcess.DominatorAccountModel, bookmark);
                    GlobusLogHelper.log.Info(Log.FoundXResults, JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                                                       JobProcess.DominatorAccountModel.AccountBaseModel.UserName, response.UsersList.Count,
                                                       queryInfo.QueryType, "", ActivityType);
                    
                    var listOfUsers = new List<PinterestUser>();
                    bookmark = response.BookMark;
                    listOfUsers = response.UsersList;
                    listOfUsers = AlreadyInteractedUser(listOfUsers);
                    listOfUsers = FilterBlackListUser(TemplateModel, listOfUsers);
                    StartProcessForListOfUsers(queryInfo, ref jobProcessResult, listOfUsers);
                }
                while (!bookmark.Contains("-end-"));
                if (jobProcessResult.HasNoResult)
                    GlobusLogHelper.log.Info(Log.NoMoreDataToPerform,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
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
    }
}