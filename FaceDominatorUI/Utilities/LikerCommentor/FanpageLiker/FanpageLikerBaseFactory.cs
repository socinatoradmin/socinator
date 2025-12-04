using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.LikerCommentor.FanpageLiker
{
    public class FanpageLikerBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.FanpageLiker)
                .AddReportFactory(new FanpageLikerReports())
                .AddViewCampaignFactory(new FanpageLikerViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}