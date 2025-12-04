using System.Collections.Generic;
using ThreadUtils;
using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using LinkedDominatorCore.Factories;
using LinkedDominatorCore.LDLibrary.DAL;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;

namespace LinkedDominatorCore.LDLibrary.Processor.Users.NonQueryType
{
    internal class BlockUserProcessor : BaseLinkedinUserProcessor, IQueryProcessor
    {
        public BlockUserProcessor(ILdJobProcess jobProcess, IDbCampaignService campaignService,
            ILdFunctionFactory ldFunctionFactory, IDelayService delayService, IProcessScopeModel processScopeModel) :
            base(jobProcess, campaignService, ldFunctionFactory, delayService, processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var nonQueryClass = new MapperModel();
            ClassMapper.SetModelClass(ref nonQueryClass, ActivityType, LdJobProcess);
            nonQueryClass.SetCustomList();

            var interactedUser = new List<InteractedUsers>();
            var customUserList = new List<LinkedinUser>();

            //UserProfileUrl
            foreach (var customUserProfileUrl in nonQueryClass.ListCustomUser)
                AddCustomUrlToList(customUserProfileUrl, interactedUser);

            if (RemoveOrSkipAlreadyInteractedUsers(interactedUser))
            {
                jobProcessResult.HasNoResult = true;
                return;
            }

            interactedUser.ForEach(user => customUserList.Add(new LinkedinUser {ProfileUrl = user.UserProfileUrl}));

            // finally go for to process to block user
            ProcessLinkedinUsersFromUserList(QueryInfo.NoQuery, ref jobProcessResult, customUserList);

            jobProcessResult.HasNoResult = true;
        }
    }
}