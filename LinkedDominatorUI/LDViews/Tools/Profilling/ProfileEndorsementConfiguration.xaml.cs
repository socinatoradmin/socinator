using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel.Profilling;
using LinkedDominatorCore.LDViewModel.Profilling;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Tools.Profilling
{
    public class
        ProfileEndorsementConfigurationBase : ModuleSettingsUserControl<ProfileEndorsementViewModel,
            ProfileEndorsementModel>
    {
        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.ProfileEndorsementModel =
                        templateModel.ActivitySettings.GetActivityModel<ProfileEndorsementModel>(ObjViewModel.Model,
                            true);
                else
                    ObjViewModel = new ProfileEndorsementViewModel();
                ObjViewModel.ProfileEndorsementModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        protected override bool ValidateExtraProperty()
        {
            if (!ObjViewModel.ProfileEndorsementModel.IsCheckedBySoftware
                && !ObjViewModel.ProfileEndorsementModel.IsCheckedOutSideSoftware
                && !ObjViewModel.ProfileEndorsementModel.IsCheckedLangKeyCustomUserList
            )

            {
                Dialog.ShowDialog("Error", "select at least once of the connection sources");
                return false;
            }

            return true;
        }
    }

    /// <summary>
    ///     Interaction logic for ProfileEndorsementConfiguration.xaml
    /// </summary>
    public partial class ProfileEndorsementConfiguration : ProfileEndorsementConfigurationBase
    {
        private ProfileEndorsementConfiguration()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.ProfileEndorsement,
                LdMainModules.Profilling.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.ProfileEndorsementVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.ProfileEndorsementKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static ProfileEndorsementConfiguration CurrentProfileEndorsementConfiguration { get; set; }

        public static ProfileEndorsementConfiguration GetSingeltonObjectProfileEndorsementConfiguration()
        {
            return CurrentProfileEndorsementConfiguration ??
                   (CurrentProfileEndorsementConfiguration = new ProfileEndorsementConfiguration());
        }

        private void AccountGrowthHeader_SelectionChanged(object sender, RoutedEventArgs e)
        {
            SocinatorInitialize.GetSocialLibrary(SocialNetworks.LinkedIn).GetNetworkCoreFactory()
                .AccountUserControlTools.RecentlySelectedAccount = AccountGrowthHeader.SelectedItem;
            base.SetAccountModeDataContext(SocialNetworks.LinkedIn);
        }

        private void ProfileEndorsementConfigurationBase_Loaded(object sender, RoutedEventArgs e)
        {
            SetSelectedAccounts(SocialNetworks.LinkedIn);
            base.SetAccountModeDataContext(SocialNetworks.LinkedIn);
        }


        private void AccountGrowthHeader_SaveClick(object sender, RoutedEventArgs e)
        {
            SaveConfigurations();
        }

        private void ProfileEndorsementSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        {
            SearchQueryControl_OnAddQuery(sender, e, typeof(LDGrowConnectionUserQueryParameters));
        }

        private void ProfileEndorsementSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e)
        {
            SearchQueryControl_OnCustomFilterChanged(sender, e);
        }

        private void TglStatus_OnClick(object sender, RoutedEventArgs e)
        {
            if (!ChangeAccountsModuleStatus(ObjViewModel.ProfileEndorsementModel.IsAccountGrowthActive,
                AccountGrowthHeader.SelectedItem, SocialNetworks.LinkedIn))
                ObjViewModel.ProfileEndorsementModel.IsAccountGrowthActive = false;
        }

        private void CustomUserListInputBox_GetInputClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CustomUserListInputBox.InputText.Contains("\r\n"))
                {
                    ObjViewModel.ProfileEndorsementModel.UrlList =
                        Regex.Split(CustomUserListInputBox.InputText, "\r\n").ToList();
                    GlobusLogHelper.log.Info("" + ObjViewModel.ProfileEndorsementModel.UrlList.Count +
                                             " profile urls saved sucessfully");
                }
                else
                {
                    try
                    {
                        ObjViewModel.ProfileEndorsementModel.UrlList = new List<string>();
                        ObjViewModel.ProfileEndorsementModel.UrlList.Add(CustomUserListInputBox.InputText);
                        GlobusLogHelper.log.Info("On profile url saved sucessfully");
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}