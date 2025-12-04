using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDModel.FriendsModel;
using FaceDominatorCore.FDViewModel.FriendsViewModel;
using FaceDominatorCore.Utility;
using MahApps.Metro.Controls.Dialogs;
using System;

namespace FaceDominatorUI.FDViews.Tools.IncommingFriendRequest
{
    public class IncommingFriendRequestToolsBase : ModuleSettingsUserControl<IncommingFriendRequestViewModel,
        IncommingFriendRequestModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.ManageFriendsModel.IsAcceptRequest &&
                !Model.ManageFriendsModel.IsCancelReceivedRequest)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneOption".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationFilter.IsFilterByGender && !Model.GenderAndLocationFilter.SelectMaleUser &&
                !Model.GenderAndLocationFilter.SelectFemaleUser)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAcceptGenderFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationCancelFilter.IsFilterByGender &&
                !Model.GenderAndLocationCancelFilter.SelectMaleUser &&
                !Model.GenderAndLocationCancelFilter.SelectFemaleUser)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectCancelGenderFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationFilter.IsLocationFilterChecked &&
                Model.GenderAndLocationFilter.ListLocationUrl.Count == 0
            )
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterCancelLocationFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationCancelFilter.IsLocationFilterChecked &&
                Model.GenderAndLocationCancelFilter.ListLocationUrl.Count == 0
            )
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterCancelLocationFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationFilter.IsNoOfMutualFriend &&
                Model.GenderAndLocationFilter.TotalNoOfMutualFriend == 0
            )
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterNoOfMutualFriendFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationCancelFilter.IsNoOfMutualFriend &&
                Model.GenderAndLocationCancelFilter.TotalNoOfMutualFriend == 0
            )
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyEnterNoOfMutualFriendFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationFilter.IsFriendOfFriend && Model.GenderAndLocationFilter.ListFriends.Count == 0
            )
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LAngKeyEnterFriendOfFriendFilter".FromResourceDictionary());
                return false;
            }

            if (Model.GenderAndLocationCancelFilter.IsFriendOfFriend &&
                Model.GenderAndLocationCancelFilter.ListFriends.Count == 0
            )
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LAngKeyEnterFriendOfFriendFilter".FromResourceDictionary());
                return false;
            }

            return true;
        }

        protected override void SetModuleValues(bool isToggleActive, TemplateModel templateModel)
        {
            try
            {
                if (!string.IsNullOrEmpty(templateModel?.ActivitySettings))
                    ObjViewModel.IncommingFriendRequestModel =
                        templateModel.ActivitySettings.GetActivityModelNonQueryList<IncommingFriendRequestModel>(
                            ObjViewModel.Model);
                else
                    ObjViewModel = new IncommingFriendRequestViewModel();
                ObjViewModel.IncommingFriendRequestModel.IsAccountGrowthActive = isToggleActive;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }


    /// <summary>
    ///     Interaction logic for IncommingFriendRequestTools.xaml
    /// </summary>
    public partial class IncommingFriendRequestTools
    {
        public IncommingFriendRequestTools()
        {
            InitializeComponent();
            DialogParticipation.SetRegister(this, this);

            InitializeBaseClass
            (
                MainGrid,
                ActivityType.IncommingFriendRequest,
                FdMainModule.Friends.ToString(),
                accountGrowthModeHeader: AccountGrowthHeader
            );

            // Help control links. 
            VideoTutorialLink = FdConstants.IncommingFriendsVideoTutorialsLink;
            KnowledgeBaseLink = FdConstants.IncommingFriendsKnowledgeBaseLink;
            ContactSupportLink = ConstantVariable.ContactSupportLink;
        }

        private static IncommingFriendRequestTools CurrentIncommingFriendRequestTools { get; set; }

        public static IncommingFriendRequestTools GetSingeltonObjectIncommingFriendRequestTools()
        {
            return CurrentIncommingFriendRequestTools ??
                   (CurrentIncommingFriendRequestTools = new IncommingFriendRequestTools());
        }
    }
}