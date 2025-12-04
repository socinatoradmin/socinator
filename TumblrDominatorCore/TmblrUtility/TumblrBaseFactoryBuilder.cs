using DominatorHouseCore.Enums;
using TumblrDominatorCore.Interface;

namespace TumblrDominatorCore.TmblrUtility
{
    public class TumblrBaseFactoryBuilder
    {
        public TumblrBaseFactoryBuilder(ITumblrUtilityFactory tumblrUtilityFactory)
        {
            TumblrUtilityFactory = tumblrUtilityFactory;
        }

        public ITumblrUtilityFactory TumblrUtilityFactory { get; set; }

        public TumblrBaseFactoryBuilder AddModuleName(ActivityType moduleName)
        {
            TumblrUtilityFactory.ModuleName = moduleName;
            return this;
        }

        public TumblrBaseFactoryBuilder AddReportFactory(ITumblrReportFactory reportFactory)
        {
            TumblrUtilityFactory.TumblrReportFactory = reportFactory;
            return this;
        }

        public TumblrBaseFactoryBuilder AddViewCampaignFactory(ITumblrViewCampaign tumblrViewCampaign)
        {
            TumblrUtilityFactory.TumblrViewCampaign = tumblrViewCampaign;
            return this;
        }
    }
}