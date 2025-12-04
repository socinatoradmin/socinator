using DominatorHouseCore.Enums;

namespace YoutubeDominatorCore.Interface
{
    public interface IYdUtilityFactory
    {
        ActivityType ModuleName { get; set; }

        IYdReportFactory YdReportFactory { get; set; }

        IYdViewCampaign YdViewCampaign { get; set; }
    }
}