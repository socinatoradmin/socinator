using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.TwtMessengerReportPack.BroadCastMessagePack
{
    public class BroadCastMessageBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.BroadcastMessages)
                .AddReportFactory(new BroadCastMessageReport())
                .AddViewCampaignFactory(new BroadCastMessageViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}