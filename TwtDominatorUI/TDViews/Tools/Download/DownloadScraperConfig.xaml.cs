using System;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.TdQuery;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using TwtDominatorCore.TDEnums;
using TwtDominatorCore.TDModels;
using TwtDominatorCore.TDUtility;
using TwtDominatorCore.TDViewModel.Scraper;
using TwtDominatorUI.CustomControl;

namespace TwtDominatorUI.TDViews.Tools.Download
{
    public class DownloadScraperBase : ModuleSettingsUserControl<DownloadViewModel, ScrapeTweetModel>
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

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.ScrapeTweetModel =
                        JsonConvert.DeserializeObject<ScrapeTweetModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new DownloadViewModel();

                ObjViewModel.ScrapeTweetModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for DownloadScraperConfig.xaml
    /// </summary>
    public partial class DownloadScraperConfig : DownloadScraperBase
    {
        public DownloadScraperConfig()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);


            InitializeBaseClass
            (
                MainGrid,
                ActivityType.DownloadScraper,
                Enums.TdMainModule.Scraper.ToString(),
                accountGrowthModeHeader: accountgrothHeader,
                queryControl: DownloadConfigurationSearchControl
            );


            // Help control links. 
            VideoTutorialLink = TDHelpDetails.TwtLikerVideoTutorialsLink;
            KnowledgeBaseLink = TDHelpDetails.TwtLikerVideoTutorialsLink;
            ContactSupportLink = TDHelpDetails.ContactLink;
        }


        private void DownloadConfigurationSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        {
            SearchQueryControl_OnAddQuery(sender, e, typeof(TdTweetInteractionQueryEnum));
        }
        //private void TglStatus_OnIsCheckedChanged(object sender, EventArgs e)
        //   => base.ScheduleJobFromGrowthMode(ObjViewModel.Model.IsAccountGrowthActive, accountgrothHeader.SelectedItem, SocialNetworks.Twitter);

        private void DownloadSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        {
            var objUserFiltersControl = new UserFiltersControl();
            var objDialog = new Dialog();

            var FilterWindow = objDialog.GetMetroWindow(objUserFiltersControl, "Filter");

            objUserFiltersControl.SaveButton.Click += (senders, Events) =>
            {
                var UserFilter = objUserFiltersControl.UserFilter;
                var SerializeCustomFilter = JsonConvert.SerializeObject(UserFilter);
                DownloadConfigurationSearchControl.CurrentQuery.CustomFilters = SerializeCustomFilter;
                FilterWindow.Close();
            };

            FilterWindow.ShowDialog();
        }

        //private void AccountgrothHeader_OnSaveClick(object sender, RoutedEventArgs e)
        //{
        //    if (base.SaveAccountGrowthSettings())
        //        ObjViewModel.ScrapeTweetModel.IsAccountGrowthActive = true;
        //}

        private void accountgrothHeader_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SetAccountModeDataContext(SocialNetworks.Twitter);
        }

        private void DownloadScraperConfiguration_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetSelectedAccounts(SocialNetworks.Twitter);
        }

        private void BtnDownloadedPath_OnClick(object sender, RoutedEventArgs e)
        {
            var downloadedFolderPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(downloadedFolderPath))
                downloadedFolderPath = ConstantVariable.GetDownloadedMediaFolderPath;

            ObjViewModel.ScrapeTweetModel.DownloadFolderPath = downloadedFolderPath;
        }

        private void TglStatus_Click(object sender, RoutedEventArgs e)
        {
            if (!ChangeAccountsModuleStatus(ObjViewModel.ScrapeTweetModel.IsAccountGrowthActive,
                accountgrothHeader.SelectedItem, SocialNetworks.Twitter))
                ObjViewModel.ScrapeTweetModel.IsAccountGrowthActive = false;
        }

        private void AccountgrothHeader_OnSaveClick(object sender, RoutedEventArgs e)
        {
            SaveConfigurations();
        }
    }
}