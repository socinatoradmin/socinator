using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.Messages;
using QuoraDominatorUI.Utility.ViewCampaign.Messages;

namespace QuoraDominatorUI.Factories.Messages
{
    public class AutoReplyToNewMessageFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AutoReplyToNewMessage)
                .AddReportFactory(new AutoReplyToNewMessageReports())
                .AddViewCampaignFactory(new AutoReplyToNewMessageViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}