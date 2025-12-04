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
using GramDominatorCore.GDViewModel.InstaScraper;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;

namespace GramDominatorUI.GDViews.Tools.Scraper
{
    public class
        DownloadPhotosConfigurationBase : ModuleSettingsUserControl<DownloadPhotosViewModel, DownloadPhotosModel>
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
                    ObjViewModel.DownloadPhotosModel =
                        JsonConvert.DeserializeObject<DownloadPhotosModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new DownloadPhotosViewModel();
                ObjViewModel.DownloadPhotosModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for DownloadPhotosConfiguration.xaml
    /// </summary>
    public partial class DownloadPhotosConfiguration : DownloadPhotosConfigurationBase
    {
        public DownloadPhotosConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.PostScraper,
                Enums.GdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: DownloadPhotosSearchQueryControl
            );
            VideoTutorialLink = ConstantHelpDetails.DownloadPhotosVideoTutorialsLink;
        }

        private static DownloadPhotosConfiguration CurrentDownloadPhotosConfiguration { get; set; }

        /// <summary>
        ///     GetSingeltonObjectDownloadPhotosConfiguration is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static DownloadPhotosConfiguration GetSingeltonObjectDownloadPhotosConfiguration()
        {
            return CurrentDownloadPhotosConfiguration ??
                   (CurrentDownloadPhotosConfiguration = new DownloadPhotosConfiguration());
        }

        private void Chk_RequiredData(object sender, RoutedEventArgs e)
        {
            ObjViewModel.CheckAllReqData(sender);
        }
    }
}