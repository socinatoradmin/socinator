using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.YdQuery;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Windows;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels.ScraperModel;
using YoutubeDominatorCore.YoutubeViewModel.Scraper_ViewModel;
using static YoutubeDominatorCore.YDEnums.Enums;

namespace YoutubeDominatorUI.YDViews.Tools.Scraper
{
    public class
        ChannelScraperConfigurationBase : ModuleSettingsUserControl<ChannelScraperViewModel, ChannelScraperModel>
    {
        protected override bool ValidateExtraProperty()
        {
            return ValidateSavedQueries();
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.ChannelScraperModel =
                        JsonConvert.DeserializeObject<ChannelScraperModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new ChannelScraperViewModel();

                ObjViewModel.ChannelScraperModel.IsAccountGrowthActive = isToggleActive;

                #region Remove this code before next 2nd-3rd release

                if (ObjViewModel.Model.ListQueryType.Count == 2)
                    Enum.GetValues(typeof(YdScraperParameters)).Cast<YdScraperParameters>().ToList().ForEach(query =>
                    {
                        if (query == YdScraperParameters.YTVideoCommenters)
                            ObjViewModel.Model.ListQueryType.Add(Application.Current
                                .FindResource(query.GetDescriptionAttr()).ToString());
                    });

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    /// <summary>
    ///     Interaction logic for ChannelScraperConfiguration.xaml
    /// </summary>
    public partial class ChannelScraperConfiguration
    {
        private static ChannelScraperConfiguration _currentChannelScraperConfiguration;

        public ChannelScraperConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.ChannelScraper,
                YdMainModule.ChannelScraper.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ChannelScraperSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ScrapChannelsVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ScrapChannelsKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ContactSupportLink;

            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            AccountGrowthHeader.AccountItemSource = new ObservableCollectionBase<string>
                (accountsFileManager.GetUsers());
            MainGrid.DataContext = ObjViewModel.ChannelScraperModel;
        }

        public static ChannelScraperConfiguration GetSingeltonObjectChannelScraperConfiguration()
        {
            return _currentChannelScraperConfiguration ??
                   (_currentChannelScraperConfiguration = new ChannelScraperConfiguration());
        }
    }
}