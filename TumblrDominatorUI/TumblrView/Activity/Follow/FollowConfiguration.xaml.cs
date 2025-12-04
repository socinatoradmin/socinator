using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using System;
using TumblrDominatorCore.Enums;
using TumblrDominatorCore.Models;
using TumblrDominatorCore.TmblrUtility;
using TumblrDominatorCore.ViewModels.GrowFollower;

namespace TumblrDominatorUI.TumblrView.Activity.Follow
{
    /// <summary>
    ///     Interaction logic for FollowConfiguration.xaml
    /// </summary>
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

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.FollowerModel =
                        JsonConvert.DeserializeObject<FollowerModel>(templateModel.ActivitySettings);
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
    public partial class FollowConfiguration
    {
        private FollowConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.Follow,
                Enums.TmbMainModule.GrowFollower.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: FollowConfigurationSearchControl
            );

            VideoTutorialLink = ConstantHelpDetails.FollowVideoTutorialsLink;

            // base.SetAccountModeDataContext();            

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Tumblr).Select(x => x.UserName));
            //AccountGrowthHeader.AccountItemSource = accounts;
            //AccountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.TumblrAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.TumblrAccounts;
            //SelectedDominatorAccounts.TumblrAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.TumblrAccounts) ? AccountGrowthHeader.SelectedItem : SelectedDominatorAccounts.TumblrAccounts;
        }


        private static FollowConfiguration CurrentFollowConfiguration { get; set; }

        public static FollowConfiguration GetSingeltonObjectFollowConfiguration()
        {
            return CurrentFollowConfiguration ?? (CurrentFollowConfiguration = new FollowConfiguration());
        }


        //private void UploadCommentInputBox_OnGetInputClick(object sender, RoutedEventArgs e)
        //    => ObjViewModel.FollowerModel.LstComments = Regex.Split(UploadCommentInputBox.InputText, "\r\n").ToList();
    }
}