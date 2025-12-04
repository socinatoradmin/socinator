using DominatorHouseCore.Enums;

namespace TumblrDominatorCore.Interface
{
    public interface ITumblrUtilityFactory
    {
        ActivityType ModuleName { get; set; }

        ITumblrReportFactory TumblrReportFactory { get; set; }

        ITumblrViewCampaign TumblrViewCampaign { get; set; }
    }
}