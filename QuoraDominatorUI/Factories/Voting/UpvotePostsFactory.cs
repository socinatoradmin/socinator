using DominatorHouseCore.Enums;
using QuoraDominatorCore.Interface;
using QuoraDominatorCore.QDFactories;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorUI.Utility.Report.Voting;
using QuoraDominatorUI.Utility.ViewCampaign.Voting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuoraDominatorUI.Factories.Voting
{
    public class UpvotePostsFactory : IQdBaseFactory
    {
        public IQdUtilityFactory QdUtilityFactory()
        {
            var utilityFactory = new QdUtilityFactory();

            var builder = new QdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.UpvotePost)
                .AddReportFactory(new UpvotePostsReport())
                .AddViewCampaignFactory(new UpvotePostsViewCampaign());

            return builder.QdUtilityFactory;
        }
    }
}
