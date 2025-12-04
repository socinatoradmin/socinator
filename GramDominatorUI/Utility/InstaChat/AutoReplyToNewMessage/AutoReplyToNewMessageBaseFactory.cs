using DominatorHouseCore.Enums;
using GramDominatorCore.GDFactories;
using GramDominatorCore.Interface;
using GramDominatorCore.Utility;

namespace GramDominatorUI.Utility.InstaChat.AutoReplyToNewMessage
{
    internal class AutoReplyToNewMessageBaseFactory : IGdBaseFactory
    {
        public IGdUtilityFactory GdUtilityFactory()
        {
            var utilityFactory = new GdUtilityFactory();

            var builder = new GdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AutoReplyToNewMessage)
                .AddReportFactory(new AutoReplyToNewMessageReports())
                .AddViewCampaignFactory(new AutoReplyToNewMessageViewCampaign());

            return builder.GdUtilityFactory;
        }
    }
}