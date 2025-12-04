using DominatorHouseCore.Enums;
using TumblrDominatorCore.Interface;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.TumblrFactory;

namespace TumblrDominatorUI.Utility.Message
{
    public class MessageBaseFactory : ITumblrBaseFactory
    {
        public ITumblrUtilityFactory TumblrUtilityFactory()
        {
            var utilityFactory = new TumblrUtilityFactory();

            var builder = new TumblrBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.BroadcastMessages)
                .AddReportFactory(new MessageReports())
                .AddViewCampaignFactory(new MessageViewCampaign());

            return builder.TumblrUtilityFactory;
        }
    }
}