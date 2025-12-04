using DominatorHouseCore.Enums;

namespace FaceDominatorCore.Interface
{
    public interface IFdUtilityFactory
    {
        ActivityType ModuleName { get; set; }

        IFdReportFactory FdReportFactory { get; set; }

        IFdViewCampaign FdViewCampaign { get; set; }


    }
}