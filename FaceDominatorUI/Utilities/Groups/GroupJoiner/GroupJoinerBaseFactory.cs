using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Groups.GroupJoiner
{
    public class GroupJoinerBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.GroupJoiner)
                .AddReportFactory(new GroupJoinerReports())
                .AddViewCampaignFactory(new GroupJoinerViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}