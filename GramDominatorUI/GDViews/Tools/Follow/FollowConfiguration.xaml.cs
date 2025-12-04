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
using GramDominatorCore.GDViewModel.GrowFollower;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;

namespace GramDominatorUI.GDViews.Tools.Follow
{
    public class FollowerConfigurationBase : ModuleSettingsUserControl<FollowerViewModel, FollowerModel>
    {
        protected override bool ValidateExtraProperty()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            // Check AutoFollow.Unfollow
            if (Model.IsChkEnableAutoFollowUnfollowChecked)
                if (!Model.IsChkStopFollowToolWhenReachedSpecifiedFollowings &&
                    !Model.IsChkWhenFollowerFollowingsIsSmallerThanChecked)
                {
                    Dialog.ShowDialog(this, "Input Error",
                        "Please select atleast one checkbox option inside AutoFollow/Unfollow feature to  Stat/Stop Unfollow/Follow process.");
                    return false;
                }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.FollowerModel =
                        templateModel.ActivitySettings.GetActivityModel<FollowerModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new FollowerViewModel();
                ObjViewModel.FollowerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for FollowConfiguration.xaml
    /// </summary>
    public partial class FollowConfiguration : FollowerConfigurationBase
    {
        private FollowConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Follow,
                Enums.GdMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: FollowConfigurationSearchControl
            );
            VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;
        }


        private static FollowConfiguration CurrentFollowConfiguration { get; set; }

        public static FollowConfiguration GetSingeltonObjectFollowConfiguration()
        {
            return CurrentFollowConfiguration ?? (CurrentFollowConfiguration = new FollowConfiguration());
        }

        private void btnPhotos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opf = new OpenFileDialog();
                opf.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";
                if (opf.ShowDialog().Value) ObjViewModel.Model.MediaPath = opf.FileName;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void DeleteMedia_Click(object sender, RoutedEventArgs e)
        {
            Model.MediaPath = "";
        }

        #region Commented    

        private void AddMessageInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            //    //  ObjViewModel.FollowerModel.Message = AddMessageInputBox.InputText;
            //    //  ObjViewModel.FollowerModel.LstMessages = Regex.Split(AddMessageInputBox.InputText, "\r\n").ToList();
        }

        private void UploadCommentInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            //    //  ObjViewModel.FollowerModel.UploadComment = UploadCommentInputBox.InputText;
            //    //  ObjViewModel.FollowerModel.LstComments = Regex.Split(UploadCommentInputBox.InputText, "\r\n").ToList();
        }

        #endregion
    }
}