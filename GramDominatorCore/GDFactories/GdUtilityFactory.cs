using DominatorHouseCore.Enums;
using GramDominatorCore.Interface;

namespace GramDominatorCore.GDFactories
{
    public class GdUtilityFactory : IGdUtilityFactory
    {
        public ActivityType ModuleName { get; set; }

        public IGdReportFactory GdReportFactory { get; set; }

        public IGdViewCampaign GdViewCampaign { get; set; }
    }
}
