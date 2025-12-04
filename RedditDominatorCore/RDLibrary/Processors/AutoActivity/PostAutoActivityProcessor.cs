using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using RedditDominatorCore.RDLibrary.BrowserManager;
using RedditDominatorCore.RDLibrary.DAL;
using RedditDominatorCore.RDLibrary.Processors.Post;
using System;
namespace RedditDominatorCore.RDLibrary.Processors.AutoActivity
{
    internal class PostAutoActivityProcessor : BaseRedditPostProcessor
    {
        private readonly IGenericFileManager _genericFileManager;
        public PostAutoActivityProcessor(IProcessScopeModel processScopeModel, IRdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbGlobalService globalService, IDbCampaignService campaignService, IRedditFunction redditFunction, IRdBrowserManager browserManager, IGenericFileManager genericFile)
            : base(processScopeModel, jobProcess, dbAccountService, globalService, campaignService, redditFunction, browserManager)
        {
            _genericFileManager = genericFile;
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            try
            {
                if (_genericFileManager != null)
                {
                    var OtherConfigModel = _genericFileManager.GetModel<RedditOtherConfigModel>(ConstantVariable.GetOtherRedditSettingsFile()) ??
                        new RedditOtherConfigModel();
                    if (OtherConfigModel != null && !OtherConfigModel.IsEnableFeedActivity)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage,
                        JobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        JobProcess.DominatorAccountModel.UserName, JobProcess.ActivityType, "Please Enable Auto Activity From Other Configuration.");
                        jobProcessResult.IsProcessCompleted = true;
                        return;
                    }
                }
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                StartAutoActivity(ref jobProcessResult);
                JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException) { throw new OperationCanceledException(); }
            finally
            {
                if (_browserManager != null && _browserManager.BrowserWindow != null) _browserManager.CloseBrowser();
                jobProcessResult.IsProcessCompleted = true;
            }
        }
    }
}
