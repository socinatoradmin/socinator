using System;
using System.Linq;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using LinkedDominatorCore.Enums;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDViewModel.Group;
using LinkedDominatorUI.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace LinkedDominatorUI.LDViews.Group
{
    public class GroupInviterBase : ModuleSettingsUserControl<GroupInviterViewModel, GroupInviterModel>
    {
        //public override void AddNewCampaign(List<string> lstSelectedAccounts, ActivityType moduleType)
        //    => GlobalMethods.AddNewCampaign(lstSelectedAccounts, moduleType);

        //public override void SaveDetails(List<string> lstSelectedAccounts, ActivityType moduleType)
        //    => GlobalMethods.SaveDetails(lstSelectedAccounts, moduleType);
    }

    /// <summary>
    ///     Interaction logic for GroupInviter.xaml
    /// </summary>
    public partial class GroupInviter : GroupInviterBase
    {
        private Window _window;

        public GroupInviter()
        {
            InitializeComponent();
            InitializeBaseClass
            (
                header: GroupInviterHeader,
                footer: GroupInviterFooter,
                queryControl: GroupInviterSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.GroupInviter,
                moduleName: LdMainModules.Group.ToString()
            );

            // Help control links. 
            VideoTutorialLink = ConstantHelpDetails.GroupInviterVideoTutorialsLink;

            KnowledgeBaseLink = ConstantHelpDetails.GroupInviterKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            DialogParticipation.SetRegister(this, this);
            SetDataContext();
        }

        private static GroupInviter CurrentGroupInviter { get; set; }
        public GroupInviterSelectAccounts GroupInviterSelectAccounts { get; set; }

        /// <summary>
        ///     GetSingeltonObjectGroupInviter is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static GroupInviter GetSingeltonObjectGroupInviter()
        {
            return CurrentGroupInviter ?? (CurrentGroupInviter = new GroupInviter());
        }

        private void GroupInviterSearchControl_OnAddQuery(object sender, RoutedEventArgs e)
        {
            SearchQueryControl_OnAddQuery(sender, e, typeof(LDGroupQueryParameters));
        }


        public override void SelectAccount()
        {
            try
            {
                GroupInviterSelectAccounts = new GroupInviterSelectAccounts(ObjViewModel);
                GroupInviterSelectAccounts.InitializeProperties();
                GroupInviterSelectAccounts.BtnSave.Click += BtnSaveEvent;

                GroupInviterSelectAccounts.GroupInviterSelectAccountsViewModel.PublisherCreateDestinationModel =
                    new GroupInviterSelectAccountsModel();
                _window = new Dialog().GetMetroWindow(GroupInviterSelectAccounts, "Select Group");
                _window.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BtnSaveEvent(object sender, EventArgs e)
        {
            try
            {
                GroupInviterSelectAccounts.BtnSaveEvent(sender, e);
                var listOfSelectedAccounts = GroupInviterSelectAccounts.GroupInviterSelectAccountsViewModel
                    .PublisherCreateDestinationModel
                    .ListSelectDestination
                    .Where(x => x.IsAccountSelected && x.SocialNetworks == SocialNetworks.LinkedIn)
                    .Select(x => x.AccountName).ToList();
                FooterControl_OnSelectAccountChanged(listOfSelectedAccounts);
                _window.Close();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        //private void GroupInviterSearchControl_OnAddQuery(object sender, RoutedEventArgs e) => SearchQueryControl_OnAddQuery(sender, e, typeof(LDGroupQueryParameters));

        //private void GroupInviterSearchControl_OnCustomFilterChanged(object sender, RoutedEventArgs e) => SearchQueryControl_OnCustomFilterChanged(sender, e);

        //private void GroupInviterFooter_OnSelectAccountChanged(object sender, RoutedEventArgs e) => FooterControl_OnSelectAccountChanged(sender, e);

        //private void GroupInviterFooter_OnCreateCampaignChanged(object sender, RoutedEventArgs e) => CreateCampaign();

        //private void GroupInviterFooter_OnUpdateCampaignChanged(object sender, RoutedEventArgs e) => UpdateCampaign();
    }
}