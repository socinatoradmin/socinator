using DominatorHouseCore.Enums;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.Utility
{
    public class FdBaseFactoryBuilder
    {
        public IFdUtilityFactory FdUtilityFactory { get; set; }

        public FdBaseFactoryBuilder (IFdUtilityFactory fdUtilityFactory)
        {
            FdUtilityFactory = fdUtilityFactory;
        }

        public FdBaseFactoryBuilder AddModuleName (ActivityType moduleName)
        {
            FdUtilityFactory.ModuleName = moduleName;
            return this;
        }

        public FdBaseFactoryBuilder AddReportFactory (IFdReportFactory reportFactory)
        {
            FdUtilityFactory.FdReportFactory = reportFactory;
            return this;
        }

        public FdBaseFactoryBuilder AddViewCampaignFactory (IFdViewCampaign fdViewCampaign)
        {
            FdUtilityFactory.FdViewCampaign = fdViewCampaign;
            return this;
        }
    }
}