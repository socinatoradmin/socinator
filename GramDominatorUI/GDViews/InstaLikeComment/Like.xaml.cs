using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaLikerCommenter;
using MahApps.Metro.Controls.Dialogs;
using static GramDominatorCore.GDEnums.Enums;

namespace GramDominatorUI.GDViews.InstaLikeComment
{
    public class LikeBase : ModuleSettingsUserControl<LikeViewModel, LikeModel>
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

            if (Model.PostFilterModel.PostCategory.FilterPostCategory)
                if (Model.PostFilterModel.PostCategory.IgnorePostImages &&
                    Model.PostFilterModel.PostCategory.IgnorePostVideos &&
                    Model.PostFilterModel.PostCategory.IgnorePostAlbums)
                {
                    Dialog.ShowDialog(this, "Input Error",
                        "Please check maximum two options for Post Type filteration inside Post Filter category.");
                    return false;
                }

            return base.ValidateCampaign();
        }
    }

    public partial class Like : LikeBase
    {
        private Like()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: LikeHeader,
                footer: LikeFooter,
                queryControl: LikeSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Like,
                moduleName: GdMainModule.LikeComment.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.LikeVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.LikeKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            SetDataContext();
            DialogParticipation.SetRegister(this, this);
        }

        #region Object creation 

        private static Like CurrentLike { get; set; }

        /// <summary>
        ///     GetSingeltonObjectLike is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static Like GetSingeltonObjectLike()
        {
            return CurrentLike ?? (CurrentLike = new Like());
        }

        #endregion
    }
}