using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.DeletePost;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.InstaPoster
{
    public class DeletePostsBase : ModuleSettingsUserControl<DeletePostViewModel, DeletePostModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check AutoFollow.Unfollow
            if (!Model.ChkDeletePostWhichIsPostedBySoftware && !Model.ChkDeletePostWhichIsPostedByOutsideSoftware)
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please select atleast one option");
                return false;
            }

            return true;
            // return base.ValidateCampaign();
        }
    }


    /// <summary>
    ///     Interaction logic for DeletePosts.xaml
    /// </summary>
    public partial class DeletePosts : DeletePostsBase
    {
        private DeletePosts()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: DeletePostsHeader,
                footer: DeletePostsFooter,
                queryControl: null,
                MainGrid: MainGrid,
                activityType: ActivityType.DeletePost,
                moduleName: Enums.GdMainModule.Poster.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.DeletePostVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.DeletePostKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        private static DeletePosts CurrentDeletePosts { get; set; }

        /// <summary>
        ///     GetSingeltonObjectDeletePosts is used to get the object of the current user control,
        ///     if object is already created then it won't create a new object, simply it returns already created object,
        ///     otherwise will return a new created object.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static DeletePosts GetSingeltonObjectDeletePosts()
        {
            return CurrentDeletePosts ?? (CurrentDeletePosts = new DeletePosts());
        }
    }
}