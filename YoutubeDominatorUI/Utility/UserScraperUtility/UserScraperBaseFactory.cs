using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeDominatorCore.Factories;
using YoutubeDominatorCore.Interface;
using YoutubeDominatorCore.YDUtility;

namespace YoutubeDominatorUI.Utility.UserScraperUtility
{
    public class UserScraperBaseFactory : IYdBaseFactory
    {
        public IYdUtilityFactory YdUtilityFactory()
        {
            var utilityFactory = new YdUtilityFactory();

            var builder = new YdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.UserScraper)
                .AddReportFactory(new UserScraperReports())
                .AddViewCampaignFactory(new UserScraperViewCampaign());

            return builder.YdUtilityFactory;
        }
    }
}
