namespace GramDominatorCore.GDFactories
{
    public class GdGlobalInteractionDetails //: IGlobalInteractionDetails
    {
        //public GlobalInteractedUtility GlobalInteractedUtility { get; set; }

        //private static GdGlobalInteractionDetails _instance;

        //public static GdGlobalInteractionDetails GetInstance() => _instance ?? (_instance = new GdGlobalInteractionDetails());

        //public void InitializeInteraction()
        //{
        //    SocinatorInitialize.GetSocialLibrary(SocialNetworks.Instagram).GetNetworkCoreFactory().GlobalInteractionDetails.ReadInteractedData();
        //    GlobalInteractedUtility = new GlobalInteractedUtility(SocialNetworks.Instagram);
        //}

        //public void UpdateInteractedData()
        //{
        //    // Save all the GdGlobalInteractionDetails.GlobalInteractedCollections datas to bin file

        //    var globalInteractionViewModel = new GlobalInteractionViewModel { GlobalInteractedCollections = GlobalInteractedCollections };

        //    var globalInteractedDatas = new List<GlobalInteractionViewModel> { globalInteractionViewModel };
        //    var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
        //    binFileHelper.UpdateGlobalInteractedDetails(globalInteractedDatas, SocialNetworks.Instagram);
        //}

        //public void ReadInteractedData()
        //{
        //    // Initialize all global interacted data to GdGlobalInteractionDetails.GlobalInteractedCollections
        //    var binFileHelper = InstanceProvider.GetInstance<IBinFileHelper>();
        //    var saveData = binFileHelper.GetGlobalInteractedDetails(SocialNetworks.Instagram);

        //    if (saveData.Count > 0)
        //        GlobalInteractedCollections = saveData[0].GlobalInteractedCollections;
        //}

        //public Dictionary<ActivityType, GlobalInteractionDataModel> GlobalInteractedCollections { get; set; }
    }
}
