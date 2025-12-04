using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.TwtMessengerReportPack.AutoReplyPack
{
    public class AutoReplyBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.AutoReplyToNewMessage)
                .AddReportFactory(new AutoReplyReport())
                .AddViewCampaignFactory(new AutoReplyViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}