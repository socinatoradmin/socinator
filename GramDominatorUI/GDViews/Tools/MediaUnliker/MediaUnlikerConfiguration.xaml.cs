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

namespace GramDominatorUI.GDViews.Tools.MediaUnliker
{
    public class MediaUnlikerConfigurationBase : ModuleSettingsUserControl<MediaUnlikerViewModel, MediaUnlikerModel>
    {
        protected override bool ValidateExtraProperty()
        {
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
                    ObjViewModel.MediaUnlikerModel =
                        templateModel.ActivitySettings.GetActivityModel<MediaUnlikerModel>(ObjViewModel.Model, true);
                else
                    ObjViewModel = new MediaUnlikerViewModel();
                ObjViewModel.MediaUnlikerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for MediaUnlikerConfiguration.xaml
    /// </summary>
    public partial class MediaUnlikerConfiguration : MediaUnlikerConfigurationBase
    {
        private MediaUnlikerConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Unlike,
                Enums.GdMainModule.LikeComment.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            VideoTutorialLink = ConstantHelpDetails.MediaUnlikerVideoTutorialsLink;
        }

        private static MediaUnlikerConfiguration CurrentMediaUnlikerConfiguration { get; set; }

        /// <summary>
        ///     USING THIS METHOD WE WILL GET SINGELTON OBJECTT OF DeletePostConfiguration
        /// </summary>
        /// <returns></returns>
        public static MediaUnlikerConfiguration GetSingeltonObjectMediaUnlikerConfiguration()
        {
            return CurrentMediaUnlikerConfiguration ??
                   (CurrentMediaUnlikerConfiguration = new MediaUnlikerConfiguration());
        }
    }
}