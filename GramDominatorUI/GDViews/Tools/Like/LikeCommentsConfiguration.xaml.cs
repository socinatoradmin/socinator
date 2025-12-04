using System;
using System.Windows;
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
    public class LikeCommentsConfigurationBase : ModuleSettingsUserControl<LikeCommentViewModel, LikeCommentModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.LikeCommentModel =
                        templateModel.ActivitySettings.GetActivityModel<LikeCommentModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new LikeCommentViewModel();
                ObjViewModel.LikeCommentModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for LikeCommentsConfiguration.xaml
    /// </summary>
    public partial class LikeCommentsConfiguration : LikeCommentsConfigurationBase
    {
        public LikeCommentsConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);
            InitializeBaseClass
            (
                MainGrid,
                ActivityType.LikeComment,
                Enums.GdMainModule.LikeComment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: LikeCommentSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.LikeCommentVideoTutorialsLink;
        }

        private static LikeCommentsConfiguration CurrentLikeCommentsConfiguration { get; set; }

        /// <summary>
        ///     GetSingeltonObjectLikeConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static LikeCommentsConfiguration GetSingeltonObjectLikeConfiguration()
        {
            return CurrentLikeCommentsConfiguration ??
                   (CurrentLikeCommentsConfiguration = new LikeCommentsConfiguration());
        }

        private void AccountGrowthHeader_OnSaveClick(object sender, RoutedEventArgs e)
        {
            SaveConfigurations();
        }

        private void LikeCommentsConfiguration_OnLoaded_(object sender, RoutedEventArgs e)
        {
            SetSelectedAccounts(SocialNetworks.Instagram);
        }
    }
}