using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Groups.MakeGroupAdmin
{
    public class MakeGroupAdminBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.MakeAdmin)
                .AddReportFactory(new MakeGroupAdminReport())
                .AddViewCampaignFactory(new MakeGroupAdminViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}