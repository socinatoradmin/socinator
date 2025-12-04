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
using YoutubeDominatorCore.YoutubeModels.EngageModel;
using YoutubeDominatorCore.YoutubeViewModel.EngageViewModel;
using static YoutubeDominatorCore.YDEnums.Enums;

namespace YoutubeDominatorUI.YDViews.Tools.Engage
{
    public class LikeConfigurationBase : ModuleSettingsUserControl<LikeViewModel, LikeModel>
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
                    ObjViewModel.LikeModel =
                        JsonConvert.DeserializeObject<LikeModel>(templateModel.ActivitySettings);
                else if (ObjViewModel == null)
                    ObjViewModel = new LikeViewModel();

                ObjViewModel.LikeModel.IsAccountGrowthActive = isToggleActive;
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
    public partial class LikeConfiguration
    {
        private static LikeConfiguration _currentLikeConfiguration;

        public LikeConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Like,
                YdMainModule.Like.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: LikeSearchControl
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.LikeVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.LikeKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ContactSupportLink;

            var accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            AccountGrowthHeader.AccountItemSource =
                new ObservableCollectionBase<string>(accountsFileManager.GetUsers());
            MainGrid.DataContext = ObjViewModel.LikeModel;
        }

        public static LikeConfiguration GetSingeltonObjectLikeConfiguration()
        {
            return _currentLikeConfiguration ?? (_currentLikeConfiguration = new LikeConfiguration());
        }
    }
}