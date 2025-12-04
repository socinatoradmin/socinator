using DominatorHouseCore.Enums;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDFactories;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.Utility.ReportVideoUtility
{
    public class ReportVideoBaseFactory : IYdBaseFactory
    {
        public IYdUtilityFactory YdUtilityFactory()
        {
            var utilityFactory = new YdUtilityFactory();

            var builder = new YdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.ReportVideo)
                .AddReportFactory(new ReportVideoReports())
                .AddViewCampaignFactory(new ReportVideoViewCampaign());

            return builder.YdUtilityFactory;
        }
    }
}