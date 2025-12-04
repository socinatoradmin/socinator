using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System.Linq;
using System.Windows;

namespace DominatorUIUtility.ViewModel
{
    public class CampaignStatusChange : ICampaignStatusChange
    {
        private ICampaignsFileManager campaignManager { get; set; }
        public CampaignStatusChange()
        {
            campaignManager = InstanceProvider.GetInstance<ICampaignsFileManager>();
        }
        public bool ChangeCampaignStatus(SocialNetworks socialNetworks, DominatorAccountModel dominatorAccount, string Module, bool Restart)
        {
            var lstCampaigns = campaignManager.Where(x => x.SocialNetworks == socialNetworks && x.Status == "Active" && x.SubModule == Module
            && x.SelectedAccountList.Any(z=>z == dominatorAccount.AccountBaseModel.UserName))?.ToList();
            if(lstCampaigns != null && lstCampaigns.Count > 0)
            {
                foreach(var campData in lstCampaigns)
                {
                    campData.Status = Restart ? "Active" : "Paused";
                    campaignManager.Edit(campData);
                }
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                var viewModel = Campaigns.GetCampaignsInstance(socialNetworks).CampaignViewModel;
                var lstCampaign = viewModel.LstCampaignDetails;
                var targets = lstCampaign.Where(x=>x.SubModule == Module && x.Status == "Active" 
                && x.SelectedAccountList.Any(y=>y == dominatorAccount.AccountBaseModel.UserName))?.ToList();
                if (targets != null && targets.Count > 0)
                {
                    foreach(var camp  in targets)
                    {
                        camp.Status = Restart ? "Active" : "Paused";
                        viewModel.StatusChangeCommand.Execute(camp);
                    }
                }
            });
            return true;
        }
    }
}
