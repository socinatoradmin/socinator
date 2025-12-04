using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.LikerCommentor.PostLiker
{
    public class PostLikerBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.PostLiker)
                .AddReportFactory(new PostLikerReports())
                .AddViewCampaignFactory(new PostlikerViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}