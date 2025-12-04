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

namespace GramDominatorUI.GDViews.Tools.Like
{
    public class LikeConfigurationBase : ModuleSettingsUserControl<LikeViewModel, LikeModel>
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

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.LikeModel =
                        templateModel.ActivitySettings.GetActivityModel<LikeModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new LikeViewModel();
                ObjViewModel.LikeModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for LikeConfiguration.xaml
    /// </summary>
    public partial class LikeConfiguration : LikeConfigurationBase
    {
        private LikeConfiguration()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Like,
                Enums.GdMainModule.LikeComment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: LikeSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.LikeVideoTutorialsLink;
        }

        #region Object creation

        private static LikeConfiguration CurrentLike { get; set; }

        /// <summary>
        ///     GetSingeltonObjectLike is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static LikeConfiguration GetSingeltonObjectLikeConfiguration()
        {
            return CurrentLike ?? (CurrentLike = new LikeConfiguration());
        }

        #endregion
    }
}