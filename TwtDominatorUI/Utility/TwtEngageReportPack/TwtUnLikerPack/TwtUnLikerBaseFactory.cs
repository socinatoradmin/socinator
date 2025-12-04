using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.TwtEngageReportPack.TwtUnLikerPack
{
    public class TwtUnLikerBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Unlike)
                .AddReportFactory(new TwtUnLikerReport())
                .AddViewCampaignFactory(new TwtUnLikerViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}