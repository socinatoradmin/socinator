using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FaceDominatorCore.FDViewModel.LikerCommentorViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.FanpageLiker
{
    public class FanpageLikerToolsBase : ModuleSettingsUserControl<FanpageLikerViewModel, FanpageLikerModel>
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

            if (Model.ChkCommentOnFanPageLatestPostsChecked && Model.ListComments.Count == 0 && Model.ListComments.Count == 0)
            {
                Dialog.ShowDialog(this,
                    "LangKeyError".FromResourceDictionary(), "LangKeyAddSomeComments".FromResourceDictionary());
                return false;
            }
            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel.ActivitySettings))
                    ObjViewModel.FanpageLikerModel
                        = templateModel.ActivitySettings.GetActivityModel<FanpageLikerModel>(ObjViewModel.Model);
                else
                    ObjViewModel = new FanpageLikerViewModel();
                ObjViewModel.FanpageLikerModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for FanpageLikerTools.xaml
    /// </summary>
    public partial class FanpageLikerTools
    {
        public FanpageLikerTools()
        {
            InitializeComponent();

            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.FanpageLiker,
                FdMainModule.Friends.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader,
                queryControl: LikerSearchControl
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.FanpageLikerVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.FanpageLikerKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;

            //var accounts = new ObservableCollectionBase<string>(AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook).Select(x => x.UserName));
            //accountGrowthHeader.AccountItemSource = accounts;
            //accountGrowthHeader.SelectedItem = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? (!string.IsNullOrEmpty(accounts[0]) ? accounts[0] : "") : SelectedDominatorAccounts.FdAccounts;
            //SelectedDominatorAccounts.FdAccounts = string.IsNullOrEmpty(SelectedDominatorAccounts.FdAccounts) ? accountGrowthHeader.SelectedItem : SelectedDominatorAccounts.FdAccounts;
        }


        private static FanpageLikerTools CurrentFanpageLikerTools { get; set; }

        public static FanpageLikerTools GetSingeltonObjectFanpageLikerTools()
        {
            return CurrentFanpageLikerTools ?? (CurrentFanpageLikerTools = new FanpageLikerTools());
        }
    }
}