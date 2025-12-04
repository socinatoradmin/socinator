using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using QuoraDominatorCore.Enums;
using QuoraDominatorCore.Models;
using QuoraDominatorCore.QdUtility;
using QuoraDominatorCore.ViewModel.GrowFollower;

namespace QuoraDominatorUI.QDViews.GrowFollowers
{
    public class FollowerBase : ModuleSettingsUserControl<FollowerViewModel, FollowerModel>
    {
        protected override bool ValidateCampaign()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for Follower.xaml
    /// </summary>
    public partial class Follower
    {
        /// Constructor
        private Follower()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: FollowHeader,
                footer: FollowFooter,
                queryControl: FollowerSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Follow,
                moduleName: QdMainModule.GrowFollower.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.FollowKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static Follower CurrentFollower { get; set; }

        /// <summary>
        ///     GetSingeltonObjectFollower is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Follower GetSingeltonObjectFollower()
        {
            return CurrentFollower ?? (CurrentFollower = new Follower());
        }


        //private void UploadCommentInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        //{
        //    //ObjViewModel.FollowerModel.UploadComment = UploadCommentInputBox.InputText;
        //    //ObjViewModel.FollowerModel.LstComments = Regex.Split(UploadCommentInputBox.InputText, "\r\n").ToList();
        //}

        //private void AddMessageInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        //{
        //    //ObjViewModel.FollowerModel.Message = AddMessageInputBox.InputText;
        //    //ObjViewModel.FollowerModel.LstMessages = Regex.Split(AddMessageInputBox.InputText, "\r\n").ToList();
        //}

        //private void FollowerSearchControl_CustomFilterChanged(object sender, RoutedEventArgs e) => base.SearchQueryControl_OnCustomFilterChanged(sender, e);
    }
}