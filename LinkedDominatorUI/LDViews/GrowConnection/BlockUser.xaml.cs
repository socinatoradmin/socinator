using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.GrowConnection;
using LinkedDominatorCore.LDViewModel.GrowConnection;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.GrowConnection
{
    public class BlockUserBase : ModuleSettingsUserControl<BlockUserViewModel, BlockUserModel>
    {
        protected override bool ValidateCampaign()
        {
            if (!string.IsNullOrEmpty(ObjViewModel.BlockUserModel.UrlInput?.Trim()))
                return base.ValidateCampaign();
            Dialog.ShowDialog("LangKeyError".FromResourceDictionary(),
                "LangKeyPleaseAddProfileUrls".FromResourceDictionary());
            return false;
        }
    }

    /// <summary>
    ///     Interaction logic for BlockUser.xaml
    /// </summary>
    public partial class BlockUser : BlockUserBase
    {
        public BlockUser()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: BlockUserHeader,
                footer: BlockUserFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.BlockUser,
                moduleName: LdMainModules.GrowConnection.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.BlockUserVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.BlockUserKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static BlockUser CurrentBlockUser { get; set; }

        public static BlockUser GetSingletonObjectBlockUser()
        {
            return CurrentBlockUser ?? (CurrentBlockUser = new BlockUser());
        }
    }
}