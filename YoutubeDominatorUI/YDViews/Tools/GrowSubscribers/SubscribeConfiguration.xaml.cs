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
using YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel;
using YoutubeDominatorCore.YoutubeViewModel.GrowSubscribers_ViewModel;
using static YoutubeDominatorCore.YDEnums.Enums;

namespace YoutubeDominatorUI.YDViews.Tools.GrowSubscribers
{
    public class SubscribeConfigurationBase : ModuleSettingsUserControl<SubscribeViewModel, SubscribeModel>
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

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.SubscribeModel =
                        JsonConvert.DeserializeObject<SubscribeModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new SubscribeViewModel();

                ObjViewModel.SubscribeModel.IsAccountGrowthActive = isToggleActive;

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
    ///     Interaction logic for EngageConfiguration.xaml
    /// </summary>
    public partial class SubscribeConfiguration
    {
        private static SubscribeConfiguration _currentSubscribeConfiguration;

        public SubscribeConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Subscribe,
                YdMainModule.Subscribe.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: SubscribeSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.SubscribeVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.SubscribeKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ContactSupportLink;

            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            AccountGrowthHeader.AccountItemSource =
                new ObservableCollectionBase<string>(accountsFileManager.GetUsers());
            MainGrid.DataContext = ObjViewModel.SubscribeModel;
        }

        public static SubscribeConfiguration GetSingeltonObjectSubscribeConfiguration()
        {
            return _currentSubscribeConfiguration ?? (_currentSubscribeConfiguration = new SubscribeConfiguration());
        }
    }
}