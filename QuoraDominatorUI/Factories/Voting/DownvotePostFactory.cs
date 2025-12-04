using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.Voting;
using QuoraDominatorUI.Utility.ViewCampaign.Voting;

namespace QuoraDominatorUI.Factories.Voting
{
    public class DownvotePostFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.DownVotePost)
                .AddReportFactory(new DownvotePostReport())
                .AddViewCampaignFactory(new DownvotePostViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}
