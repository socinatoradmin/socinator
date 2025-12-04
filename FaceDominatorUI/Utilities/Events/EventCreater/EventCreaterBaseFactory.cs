using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Events.EventCreater
{
    internal class EventCreaterBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.EventCreator)
                .AddReportFactory(new EventCreaterReports())
                .AddViewCampaignFactory(new EventCreaterViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}