using DominatorHouseCore;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDRequest;
using System;
using AccountInteractedUsres = DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedUsers;


namespace FaceDominatorCore.FDLibrary.FdProcesses
{

    public class FdWebpageLikerCommentorProcess : FdJobProcessInteracted<AccountInteractedUsres>
    {
        public WebpageLikerCommentorModel WebpageLikerCommentorModel { get; set; }

        public DominatorAccountModel Account { get; set; }

        public FdWebpageLikerCommentorProcess(IProcessScopeModel processScopeModel,
            IDbAccountServiceScoped accountServiceScoped,
            IFdQueryScraperFactory queryScraperFactory,
            IFdHttpHelper fdHttpHelper, IFdLoginProcess fdLogInProcess, IDbCampaignServiceScoped dbCampaignServiceScoped)
            : base(processScopeModel, accountServiceScoped, queryScraperFactory, fdHttpHelper, fdLogInProcess, dbCampaignServiceScoped)
        {

            WebpageLikerCommentorModel = processScopeModel.GetActivitySettingsAs<WebpageLikerCommentorModel>();
            AccountModel = DominatorAccountModel;
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            JobProcessResult jobProcessResult = new JobProcessResult();

            GlobusLogHelper.log.Info("Send Friend Request => " + scrapeResult.ResultUser.UserId + " with account => " + AccountModel.AccountBaseModel.UserName + " module => " + ActivityType);

            try
            {
                DelayBeforeNextActivity();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return jobProcessResult;
        }
    }
}
