using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Groups.GroupUnjoiner
{
    public class GroupUnjoinerBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.GroupUnJoiner)
                .AddReportFactory(new GroupUnJoinerReports())
                .AddViewCampaignFactory(new GroupUnjoinerViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}