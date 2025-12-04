using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;
using TwtDominatorCore.TDFactories;
using TwtDominatorCore.TDUtility;

namespace TwtDominatorUI.Utility.TwtBlasterReportPack.DeletePack
{
    public class DeleteBaseFactory : ITDBaseFactory
    {
        public ITDUtilityFactory TDUtilityFactory()
        {
            var utilityFactory = new TDUtilityFactory();

            var builder = new TDBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.Delete)
                .AddReportFactory(new DeleteReport())
                .AddViewCampaignFactory(new DeleteViewCampaign());

            return builder.TDUtilityFactory;
        }
    }
}