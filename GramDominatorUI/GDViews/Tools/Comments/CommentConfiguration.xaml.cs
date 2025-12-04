using System;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.InstaLikerCommenter;
using MahApps.Metro.Controls.Dialogs;

namespace GramDominatorUI.GDViews.Tools.Comments
{
    public class CommentConfigurationBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
    {
        protected override bool ValidateExtraProperty()
        {
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

            if (Model.IsChkAfterCommentAction &&
                (!Model.IsChkLikePostAfterComment && !Model.IsChkFollowUserAfterComment))
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please check atleast one option inside After Comment Action category");
                return false;
            }

            if (Model.IsChkMentionRandomUsers && string.IsNullOrEmpty(Model.MentionUsers))
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please enter user(s) to mention in comment");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.CommentModel =
                        templateModel.ActivitySettings.GetActivityModel<CommentModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new CommentViewModel();
                ObjViewModel.CommentModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for CommentConfiguration.xaml
    /// </summary>
    public partial class CommentConfiguration : CommentConfigurationBase
    {
        private CommentConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Comment,
                Enums.GdMainModule.LikeComment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: CommentConfigSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
        }


        #region Object creation and INotifyPropertyChanged Implementation

        private static CommentConfiguration CurrentCommentConfiguration { get; set; }

        /// <summary>
        ///     GetSingeltonObjectCommentConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static CommentConfiguration GetSingeltonObjectCommentConfiguration()
        {
            return CurrentCommentConfiguration ?? (CurrentCommentConfiguration = new CommentConfiguration());
        }

        #endregion
    }
}