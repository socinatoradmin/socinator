using ThreadUtils;
using DominatorHouseCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwtDominatorCore.Database;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDLibrary.GeneralLibrary.DAL;
using TwtDominatorCore.TDModels;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary.Subprocesses
{
    public class CommentOnUserNotificationTweetsSubprocessPerComment : CommentOnUserNotificationTweetsSubprocess<CommentModel>
    {

        public CommentOnUserNotificationTweetsSubprocessPerComment(IProcessScopeModel processScopeModel,
            ITwitterFunctionFactory twitterFunctionFactory, ITdJobProcess jobProcess,
            IDbInsertionHelper dbInsertionHelper,  IDelayService threadUtility, Func<CommentModel, int> getAfterActionRange,
            Func<CommentModel, int> getAfterActionDelay) : base(processScopeModel, twitterFunctionFactory,
            jobProcess, dbInsertionHelper, threadUtility, getAfterActionRange, getAfterActionDelay)
        {
        }

        protected override List<string> GetComments(CommentModel model)
        {
            if (model.CommentTextToCommentOnTweetsFromUserNotifications != null)
                return model.CommentTextToCommentOnTweetsFromUserNotifications.Split(',').ToList();
            return new List<string>();
        }
    }
}
