using DominatorHouseCore.Enums;

namespace PinDominatorCore.Interface
{
    public interface IPdUtilityFactory
    {
        ActivityType ModuleName { get; set; }

        IPdReportFactory PdReportFactory { get; set; }

        IPdViewCampaign PdViewCampaign { get; set; }
    }
}