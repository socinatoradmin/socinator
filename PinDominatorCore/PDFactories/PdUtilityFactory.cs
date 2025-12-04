using DominatorHouseCore.Enums;
using PinDominatorCore.Interface;

namespace PinDominatorCore.PDFactories
{
    public class PdUtilityFactory : IPdUtilityFactory
    {
        public ActivityType ModuleName { get; set; }

        public IPdReportFactory PdReportFactory { get; set; }

        public IPdViewCampaign PdViewCampaign { get; set; }
    }
}