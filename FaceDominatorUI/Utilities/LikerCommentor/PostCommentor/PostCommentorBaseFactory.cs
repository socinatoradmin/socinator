using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.LikerCommentor.PostCommentor
{
    public class PostCommentorBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.PostCommentor)
                .AddReportFactory(new PostCommentorReports())
                .AddViewCampaignFactory(new PostCommmentorViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}