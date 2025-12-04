using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.MessageModel;
using FaceDominatorCore.FDViewModel.MessageViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace FaceDominatorUI.FDViews.FbMessanger
{
    public class MessageToPlacesBase : ModuleSettingsUserControl<MessageToPlacesViewModel, MessageToPlacesModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (Model.LstDisplayManageMessageModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneMessage".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for FanapgeLiker.xaml
    /// </summary>
    public partial class MessageToPlaces
    {
        public MessageToPlaces()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: MessageToFanpageFooter,
                queryControl: MessageToFanpagesSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.MessageToPlaces,
                moduleName: FdMainModule.Messanger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.SendMessageToBasedOnLocationVideoTutorialsLink;
            KnowledgeBaseLink = "https://help.socinator.com/support/solutions/articles/42000088699-facebook-auto-send-messasge-to-places";
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static MessageToPlaces CurrentMessageToPlaces { get; set; }

        public static MessageToPlaces GetSingeltonObjectMessageToPlaces()
        {
            return CurrentMessageToPlaces ?? (CurrentMessageToPlaces = new MessageToPlaces());
        }
    }
}