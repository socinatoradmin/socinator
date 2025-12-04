using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.GrowFollower;
using QuoraDominatorUI.Utility.ViewCampaign.GrowFollower;

namespace QuoraDominatorUI.Factories.GrowFollower
{
    public class UnfollowFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Unfollow)
                .AddReportFactory(new UnfollowReports())
                .AddViewCampaignFactory(new UnFollowViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}