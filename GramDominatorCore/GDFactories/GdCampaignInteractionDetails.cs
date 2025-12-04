using System.Collections.Generic;
using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;

namespace GramDominatorCore.GDFactories
{
    public class GdCampaignInteractionDetails //: ICampaignInteractionDetails
    {
    //    private static GdCampaignInteractionDetails _instance;

    //    public static GdCampaignInteractionDetails GetInstance() => _instance ?? (_instance = new GdCampaignInteractionDetails());

    //    private GdCampaignInteractionDetails()
    //    {
    //        CampaignInteractedCollections = new Dictionary<string, CampaignInteractionDataModel>();
    //    }

    //    public Dictionary<string, CampaignInteractionDataModel> CampaignInteractedCollections { get; set; }


    //    public void InitializeInteraction()
    //    {
    //        SocinatorInitialize.GetSocialLibrary(SocialNetworks.Instagram).GetNetworkCoreFactory().CampaignInteractionDetails.ReadInteractedData();
    //        CampaignInteractedUtility = new CampaignInteractedUtility(SocialNetworks.Instagram);
    //    }

    //    public void UpdateInteractedData()
    //    {
    //        // Save all the GdCampaignInteractionDetails.CampaignInteractedCollections datas to bin file

    //        var campaignInteractionDataModel = new CampaignInteractionViewModel { CampaignInteractedCollections = CampaignInteractedCollections };

    //        var campaignInteractedData = new List<CampaignInteractionViewModel> { campaignInteractionDataModel };
    //       var binFileHelper= InstanceProvider.GetInstance<IBinFileHelper>();
    //        binFileHelper.UpdateCampaignInteractedDetails(campaignInteractedData, SocialNetworks.Instagram);
    //    }


    //    public void ReadInteractedData()
    //    {
    //        // Initialize all camapign interacted data to GdCampaignInteractionDetails.CampaignInteractedCollections
    //        var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
    //        var saveData = binFileHelper.GetCampaignInteractedDetails(SocialNetworks.Instagram);

    //        if (saveData.Count > 0)
    //            CampaignInteractedCollections = saveData[0].CampaignInteractedCollections;
    //    }


    //    public CampaignInteractedUtility CampaignInteractedUtility { get; set; }


    }
}
