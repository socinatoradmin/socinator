using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.LikerCommentor.PostLikerCommentor
{
    public class PostLikerCommentorBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.PostLikerCommentor)
                .AddReportFactory(new PostLikerCommentorReports())
                .AddViewCampaignFactory(new PostlikerCommmentorViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}