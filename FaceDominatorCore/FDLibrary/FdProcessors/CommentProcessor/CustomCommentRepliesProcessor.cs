using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor
{
    public class CustomCommentRepliesProcessor : BaseFbCommentLikerProcessor
    {
        public CustomCommentRepliesProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary, IFdBrowserManager browserManager,
            IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager,
                processScopeModel)
        {
        }

        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            JobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            var commentUrl = queryInfo.QueryValue.Replace("www.facebook.com", "m.facebook.com");
            if (AccountModel.IsRunProcessThroughBrowser)
                Browsermanager.LoadPageSource(AccountModel, commentUrl);


            var commentID = FdRegexUtility.FirstMatchExtractor(queryInfo.QueryValue, "comment_id=(.*?)[&]");
            commentID = string.IsNullOrEmpty(commentID) ? FdFunctions.FdFunctions.GetIntegerOnlyString(Regex.Split(queryInfo.QueryValue, "comment_id=").Count() > 1 ? Regex.Split(queryInfo.QueryValue, "comment_id=")[1] : string.Empty).ToString() : commentID; //FdRegexUtility.FirstMatchExtractor(queryInfo.QueryValue, "comment_id=(.*?)\"");

            ProcessDataOfCommentReplies(queryInfo, ref jobProcessResult, new List<FdPostCommentDetails>() { new FdPostCommentDetails() { CommentUrl = queryInfo.QueryValue, CommentId = commentID } });

            jobProcessResult.HasNoResult = true;

            #region LanguageChange 
            //if (AccountModel.IsRunProcessThroughBrowser)
            //    Browsermanager.ChangeLanguage(AccountModel, FdConstants.AccountLanguage[AccountModel.AccountBaseModel.UserId]);
            //else
            //    ObjFdRequestLibrary.ChangeLanguage(AccountModel, FdConstants.AccountLanguage[AccountModel.AccountBaseModel.UserId]); 
            #endregion

        }

    }
}
