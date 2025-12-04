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
    public class MessageToFanpagesBase : ModuleSettingsUserControl<MessageToFanpagesViewModel, MessageToFanpagesModel>
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
    public partial class MessageToFanpages
    {
        public MessageToFanpages()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: MessageToFanpageFooter,
                queryControl: MessageToFanpagesSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.MessageToFanpages,
                moduleName: FdMainModule.Messanger.ToString()
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.MassageToFanPagesVideoTutorialLink;
            KnowledgeBaseLink = "https://help.socinator.com/support/solutions/articles/42000088686-facebook-auto-send-message-to-fan-pages";
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            base.SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static MessageToFanpages CurrentFanapgeLiker { get; set; }

        public static MessageToFanpages GetSingeltonObjectMessageToFanpages()
        {
            return CurrentFanapgeLiker ?? (CurrentFanapgeLiker = new MessageToFanpages());
        }
    }
}