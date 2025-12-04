using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.Messages;
using QuoraDominatorUI.Utility.ViewCampaign.Messages;

namespace QuoraDominatorUI.Factories.Messages
{
    public class BroadcastMessagesFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.BroadcastMessages)
                .AddReportFactory(new BroadcastMessagesReports())
                .AddViewCampaignFactory(new BroadcastMessagesViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}