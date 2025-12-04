using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
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
    public class HashtagsScraperConfigBase : ModuleSettingsUserControl<HashtagsScraperViewModel, HashtagsScraperModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (string.IsNullOrEmpty(Model.Keyword))
            {
                Dialog.ShowDialog(this, "Input Error",
                    "Please enter atleast one keyword");
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.HashtagsScraperModel =
                        JsonConvert.DeserializeObject<HashtagsScraperModel>(templateModel.ActivitySettings);
                else
                    ObjViewModel = new HashtagsScraperViewModel();
                ObjViewModel.HashtagsScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for HashtagsScraperConfig.xaml
    /// </summary>
    public partial class HashtagsScraperConfig : HashtagsScraperConfigBase
    {
        private HashtagsScraperConfig()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.HashtagsScraper,
                Enums.GdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );
            VideoTutorialLink = ConstantHelpDetails.HashtagVideoTutorialsLink;
        }

        private static HashtagsScraperConfig CurrentHashtagsScraperConfig { get; set; }

        /// <summary>
        ///     GetSingeltonObjectHashtagsScraperConfig is used to get the object of the current user control,
        ///     if object is already created then its wont create a new object object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static HashtagsScraperConfig GetSingeltonObjectHashtagsScraperConfig()
        {
            return CurrentHashtagsScraperConfig ?? (CurrentHashtagsScraperConfig = new HashtagsScraperConfig());
        }

        private void Keywords_OnGetInputClick(object sender, RoutedEventArgs e)
        {
            if (ObjViewModel.HashtagsScraperModel.Keyword != null)
                ObjViewModel.HashtagsScraperModel.LstKeyword = Regex
                    .Split(ObjViewModel.HashtagsScraperModel.Keyword, "\r\n").Where(x => !string.IsNullOrEmpty(x))
                    .ToList();

            GlobusLogHelper.log.Info(
                $"{ObjViewModel.HashtagsScraperModel.LstKeyword.Count} keyword{(ObjViewModel.HashtagsScraperModel.LstKeyword.Count > 1 ? "s" : "")} added successfully");
        }
    }
}