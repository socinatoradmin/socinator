using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace QuoraDominatorCore.Interface
{
    public interface IQdUtilityFactory
    {
        ActivityType ModuleName { set; }

        IQdReportFactory QdReportFactory { get; set; }

        IViewCampaignsFactory QdViewCampaign { get; set; }
    }
}