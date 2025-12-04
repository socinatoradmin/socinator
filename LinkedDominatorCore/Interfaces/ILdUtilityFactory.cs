using DominatorHouseCore.Enums;

namespace LinkedDominatorCore.Interfaces
{
    public interface ILdUtilityFactory
    {
        ActivityType ModuleName { get; set; }

        ILdReportFactory LdReportFactory { get; set; }

        ILdViewCampaign LdViewCampaign { get; set; }
    }
}