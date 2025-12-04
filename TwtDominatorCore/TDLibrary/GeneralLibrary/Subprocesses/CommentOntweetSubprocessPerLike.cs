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
    public class CommentOntweetSubprocessPerLike : CommentOntweetSubprocess<LikeModel>
    {
        public CommentOntweetSubprocessPerLike(IProcessScopeModel processScopeModel,
            ITwitterFunctionFactory twitterFunctionFactory,
            IDbInsertionHelper dbInsertionHelper, IDelayService threadUtility, Func<LikeModel, int> getAfterActionRange,
            Func<LikeModel, int> getAfterActionDelay) : base(processScopeModel, twitterFunctionFactory,
            dbInsertionHelper, threadUtility, getAfterActionRange, getAfterActionDelay)
        {
        }

        protected override List<string> GetComments(LikeModel model)
        {
            if (model.UploadedCommentInput != null)
                return model.UploadedCommentInput.Split(',').ToList();
            return new List<string>();
        }

        protected override bool IsSpintax(LikeModel model)
        {
            return model.IsSpintax;
        }
    }
}