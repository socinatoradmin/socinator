using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.LikerCommentor.WebPostCommentLiker
{
    public class WebPostCommentLikerBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.WebPostLikeComment)
                .AddReportFactory(new WebPostCommentLikerReports())
                .AddViewCampaignFactory(new WebPostCommentLikerViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}