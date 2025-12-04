using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.LikerCommentor.ReplyToComment
{
    public class ReplyToCommentBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.ReplyToComment)
                .AddReportFactory(new ReplyToCommentReports())
                .AddViewCampaignFactory(new ReplyToCommentViewCampain());

            return builder.FdUtilityFactory;
        }
    }
}