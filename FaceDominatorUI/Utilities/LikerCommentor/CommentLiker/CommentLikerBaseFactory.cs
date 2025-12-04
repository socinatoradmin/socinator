using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.LikerCommentor.CommentLiker
{
    public class CommentLikerBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.LikeComment)
                .AddReportFactory(new CommentLikerReports())
                .AddViewCampaignFactory(new CommentLikerViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}