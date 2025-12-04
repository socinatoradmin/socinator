using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using RedditDominatorCore.RDModel;
using RedditDominatorCore.RDViewModel;
using RedditDominatorUI.RDViews.Tools;
using System.Linq;
using System.Windows;
using static RedditDominatorCore.RDEnums.Enums;

namespace RedditDominatorUI.RDViews.Engage
{
    public class EditCommentBase : ModuleSettingsUserControl<EditCommentViewModel, EditCommentModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.CommentDetails.Count == 0)
            {
                Dialog.ShowDialog(this, "Input Error", "Please Add Comments Details");
                return false;
            }

            if (Model.CommentDetails.Count != 0) return base.ValidateCampaign();
            Dialog.ShowDialog(this, "Error", "Please Add Comments Details");
            return false;
        }
    }


    /// <summary>
    ///     Interaction logic for EditComment.xaml
    /// </summary>
    public partial class EditComment : EditCommentBase
    {
        public EditComment()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: EditCommentHeader,
                footer: EditCommentFooter,
                MainGrid: MainGrid,
                activityType: ActivityType.EditComment,
                moduleName: RdMainModule.GrowComment.ToString()
            );
            // Help control links. 
            KnowledgeBaseLink = ConstantHelpDetails.EditCommentKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
            ;
            VideoTutorialLink = ConstantHelpDetails.EditCommentVideoTutorialsLink;
            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static EditComment CurrentEditComment { get; set; }

        public static EditComment GetSingletonObjectEditComment()
        {
            return CurrentEditComment ?? (CurrentEditComment = new EditComment());
        }

        private void EditComments_Loaded(object sender, RoutedEventArgs e)
        {
            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accounts = accountsFileManager.GetAll(SocialNetworks.Reddit)
                .Where(account => account.AccountBaseModel.Status == AccountStatus.Success);

            ObjViewModel.EditCommentModel.LstAccounts =
                accounts.Select(account => account.AccountBaseModel.UserName).ToList();
        }
    }
}