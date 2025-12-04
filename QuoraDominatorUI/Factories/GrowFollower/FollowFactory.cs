using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.GrowFollower;
using QuoraDominatorUI.Utility.ViewCampaign.GrowFollower;

namespace QuoraDominatorUI.Factories.GrowFollower
{
    public class FollowFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Follow)
                .AddReportFactory(new FollowReports())
                .AddViewCampaignFactory(new FollowViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}