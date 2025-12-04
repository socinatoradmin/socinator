using DominatorHouseCore.Enums;
using FaceDominatorCore.Interface;

namespace FaceDominatorCore.FDFactories
{
    public class FdUtilityFactory : IFdUtilityFactory
    {
        public ActivityType ModuleName { get; set; }

        public IFdReportFactory FdReportFactory { get; set; }

        public IFdViewCampaign FdViewCampaign { get; set; }

    }
}