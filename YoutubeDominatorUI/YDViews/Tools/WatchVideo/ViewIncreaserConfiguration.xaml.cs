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
using YoutubeDominatorCore.YDEnums;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels.WatchVideoModel;
using YoutubeDominatorCore.YoutubeViewModel.WatchVideo_ViewModel;

namespace YoutubeDominatorUI.YDViews.Tools.WatchVideo
{
    public class ViewIncreaserConfigurationBase : ModuleSettingsUserControl<ViewIncreaserViewModel, ViewIncreaserModel>
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
                    ObjViewModel.ViewIncreaserModel =
                        JsonConvert.DeserializeObject<ViewIncreaserModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new ViewIncreaserViewModel();

                ObjViewModel.ViewIncreaserModel.IsAccountGrowthActive = isToggleActive;
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
    public partial class ViewIncreaserConfiguration
    {
        private static ViewIncreaserConfiguration _currentViewIncreaserConfiguration;

        public ViewIncreaserConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.ViewIncreaser,
                Enums.YdMainModule.ViewIncreaser.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: ViewIncreaserSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ViewIncreaserVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ViewIncreaserKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ContactSupportLink;

            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            AccountGrowthHeader.AccountItemSource =
                new ObservableCollectionBase<string>(accountsFileManager.GetUsers());
            MainGrid.DataContext = ObjViewModel.ViewIncreaserModel;
        }

        public static ViewIncreaserConfiguration GetSingeltonObjectViewIncreaserConfiguration()
        {
            return _currentViewIncreaserConfiguration ??
                   (_currentViewIncreaserConfiguration = new ViewIncreaserConfiguration());
        }
    }
}