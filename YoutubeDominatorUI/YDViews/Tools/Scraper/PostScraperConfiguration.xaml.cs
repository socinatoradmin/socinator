using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Linq;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels.ScraperModel;
using YoutubeDominatorCore.YoutubeViewModel.Scraper_ViewModel;
using static YoutubeDominatorCore.YDEnums.Enums;

namespace YoutubeDominatorUI.YDViews.Tools.Scraper
{
    public class PostScraperConfigurationBase : ModuleSettingsUserControl<PostScraperViewModel, PostScraperModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!ValidateSavedQueries())
                return false;
            if (!ObjViewModel.Model.SavedQueries.Any(x => x.QueryTypeEnum == "Keywords") &&
                Model.VideoFilterModel.SearchVideoFilterModel != null)
            {
                Model.VideoFilterModel.IsCheckedSearchVideoFilter = false;
                Model.VideoFilterModel.SearchVideoFilterModel = null;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.PostScraperModel =
                        JsonConvert.DeserializeObject<PostScraperModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new PostScraperViewModel();

                ObjViewModel.PostScraperModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for PostScraperConfiguration.xaml
    /// </summary>
    public partial class PostScraperConfiguration
    {
        private static PostScraperConfiguration _currentPostScraperConfiguration;

        public PostScraperConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.PostScraper,
                YdMainModule.PostScraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: PostScraperSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ScrapVideosVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ScrapVideosKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ContactSupportLink;

            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            AccountGrowthHeader.AccountItemSource = new ObservableCollectionBase<string>
                (accountsFileManager.GetUsers());
            MainGrid.DataContext = ObjViewModel.PostScraperModel;
        }

        public static PostScraperConfiguration GetSingeltonObjectPostScraperConfiguration()
        {
            return _currentPostScraperConfiguration ??
                   (_currentPostScraperConfiguration = new PostScraperConfiguration());
        }
    }
}