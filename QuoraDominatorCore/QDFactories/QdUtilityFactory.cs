using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using QuoraDominatorCore.Interface;

namespace QuoraDominatorCore.QDFactories
{
    public class QdUtilityFactory : IQdUtilityFactory
    {
        public ActivityType ModuleName { get; set; }
        public IQdReportFactory QdReportFactory { get; set; }
        public IViewCampaignsFactory QdViewCampaign { get; set; }
    }
}