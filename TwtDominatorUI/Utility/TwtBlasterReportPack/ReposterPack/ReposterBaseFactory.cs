using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.TwtBlasterReportPack.ReposterPack
{
    public class ReposterBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Reposter)
                .AddReportFactory(new ReposterReport())
                .AddViewCampaignFactory(new ReposterViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}