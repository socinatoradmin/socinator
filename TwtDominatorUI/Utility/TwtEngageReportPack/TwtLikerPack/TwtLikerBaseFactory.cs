using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.TwtEngageReportPack.TwtLikerPack
{
    public class TwtLikerBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Like)
                .AddReportFactory(new TwtLikerReport())
                .AddViewCampaignFactory(new TwtLikerViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}