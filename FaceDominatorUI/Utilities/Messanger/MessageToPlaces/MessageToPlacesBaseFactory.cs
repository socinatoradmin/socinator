using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.Interface;
using FaceDominatorCore.Utility;

namespace FaceDominatorUI.Utilities.Messanger.SendGreetingsToFriends
{
    public class MessageToPlacesBaseFactory : IFdBaseFactory
    {
        public IFdUtilityFactory FdUtilityFactory()
        {
            var utilityFactory = new FdUtilityFactory();

            var builder = new FdBaseFactoryBuilder(utilityFactory)
                .AddModuleName(ActivityType.MessageToPlaces)
                .AddReportFactory(new MessageToPlacesReport())
                .AddViewCampaignFactory(new MessageToPlacesViewCampaign());

            return builder.FdUtilityFactory;
        }
    }
}