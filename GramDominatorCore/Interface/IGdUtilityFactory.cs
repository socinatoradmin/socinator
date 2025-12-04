using DominatorHouseCore.Enums;

namespace GramDominatorCore.Interface
{
    public interface IGdUtilityFactory
    {
        ActivityType ModuleName { get; set; }

        IGdReportFactory GdReportFactory { get; set; }

        IGdViewCampaign GdViewCampaign { get; set; }
    }
}
