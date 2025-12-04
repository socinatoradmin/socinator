using System;
using System.Collections.Generic;
using System.Linq;
using ThreadUtils;
using DominatorHouseCore.Models;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Subprocesses
{
    internal class CommentOntweetSubprocessPerRetweet : CommentOntweetSubprocess<RetweetModel>
    {
        public CommentOntweetSubprocessPerRetweet(IProcessScopeModel processScopeModel,
            ITwitterFunctionFactory twitterFunctionFactory,
            IDbInsertionHelper dbInsertionHelper, IDelayService threadUtility,
            Func<RetweetModel, int> getAfterActionRange,
            Func<RetweetModel, int> getAfterActionDelay) : base(processScopeModel, twitterFunctionFactory,
            dbInsertionHelper, threadUtility, getAfterActionRange, getAfterActionDelay)

        {
        }

        protected override List<string> GetComments(RetweetModel model)
        {
            if (model.UploadedCommentInput != null)
            {
                return model.UploadedCommentInput.Split(',').ToList();
            }else
                return new List<string>();
        }

        protected override bool IsSpintax(RetweetModel model)
        {
            return model.IsSpintax;
        }
    }
}