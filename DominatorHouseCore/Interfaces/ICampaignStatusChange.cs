using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface ICampaignStatusChange
    {
        bool ChangeCampaignStatus(SocialNetworks socialNetworks, DominatorAccountModel dominatorAccount, string Module, bool Restart);
    }
}
