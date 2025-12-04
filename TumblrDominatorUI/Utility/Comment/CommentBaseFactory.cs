using DominatorHouseCore.Enums;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrFactory;

namespace TumblrDominatorUI.Utility.Comment
{
    public class CommentBaseFactory : ITumblrBaseFactory
    {
        public ITumblrUtilityFactory TumblrUtilityFactory()
        {
            var utilityFactory = new TumblrUtilityFactory();

            var builder = new TumblrBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Comment)
                .AddReportFactory(new CommentReports())
                .AddViewCampaignFactory(new CommentViewCampaign());

            return builder.TumblrUtilityFactory;
        }
    }
}