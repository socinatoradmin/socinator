using DominatorHouseCore.Enums;
using TwtDominatorCore.Interface;

namespace TwtDominatorCore.TDUtility
{
    public class TDBaseFactoryBuilder
    {
        public TDBaseFactoryBuilder(ITDUtilityFactory tdUtilityFactory)
        {
            TDUtilityFactory = tdUtilityFactory;
        }

        public ITDUtilityFactory TDUtilityFactory { get; set; }

        public TDBaseFactoryBuilder AddModuleName(ActivityType moduleName)
        {
            TDUtilityFactory.ModuleName = moduleName;
            return this;
        }

        public TDBaseFactoryBuilder AddReportFactory(ITDReportFactory reportFactory)
        {
            TDUtilityFactory.TDReportFactory = reportFactory;
            return this;
        }

        public TDBaseFactoryBuilder AddViewCampaignFactory(ITDViewCampaign tdViewCampaign)
        {
            TDUtilityFactory.TDViewCampaign = tdViewCampaign;
            return this;
        }
    }
}