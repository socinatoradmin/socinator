using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using System.Collections.Generic;

namespace FaceDominatorCore.FDLibrary.FdProcessors.CommentProcessor
{
    public class CustomCommentLikerProcessor : BaseFbCommentLikerProcessor
    {

        public CustomCommentLikerProcessor(IFdJobProcess jobProcess, IDbAccountServiceScoped dbAccountService,
            IDbCampaignServiceScoped campaignService, IFdRequestLibrary objFdRequestLibrary,
            IFdBrowserManager browserManager, IProcessScopeModel processScopeModel)
            : base(jobProcess, dbAccountService, campaignService, objFdRequestLibrary, browserManager, processScopeModel)
        {
        }


        protected override void Process(QueryInfo queryInfo, ref JobProcessResult jobProcessResult)
        {
            var fbPost = FdConstants.getFaceBookPOstFromUrlOrId(queryInfo.QueryValue);
            FilterAndStartFinalProcessForEachPost(queryInfo, jobProcessResult, new List<FacebookPostDetails>() { fbPost });
            jobProcessResult.HasNoResult = true;
        }
    }
}
