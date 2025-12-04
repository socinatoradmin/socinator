using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using YoutubeDominatorCore.YDUtility;
using YoutubeDominatorCore.YoutubeModels;
using YoutubeDominatorCore.YoutubeModels.GrowSubscribersModel;
using YoutubeDominatorCore.YoutubeViewModel.GrowSubscribers_ViewModel;
using YoutubeDominatorUI.CustomControl;
using static YoutubeDominatorCore.YDEnums.Enums;

namespace YoutubeDominatorUI.YDViews.GrowSubscribers
{
    public class UnsubscribeBase : ModuleSettingsUserControl<UnsubscribeViewModel, UnsubscribeModel>
    {
        protected override bool ValidateExtraProperty()
        {
            if (!Model.IsChkChannelSubscribedBySoftwareChecked && !Model.IsChkChannelSubscribedOutsideSoftwareChecked
                                                               && !Model.IsChkCustomChannelsListChecked)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeySelectAtleastOneUnsubscribeSource".FromResourceDictionary());
                return false;
            }

            if (Model.IsChkCustomChannelsListChecked &&
                (string.IsNullOrWhiteSpace(Model.CustomChannelsList) || Model.ListCustomChannels?.Count == 0))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyCustomUserListEmpty".FromResourceDictionary());
                return false;
            }

            return true;
        }
    }

    public partial class Unsubscribe
    {
        private bool _enteredHere;
        private UnsubscribeViewModel _objUnsubscribeViewModel;
        private Window _window;

        public Unsubscribe()
        {
            InitializeComponent();

            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: UnSubscribeFooter,
                //queryControl: UnsubscribeSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.UnSubscribe,
                moduleName: YdMainModule.UnSubscribe.ToString()
            );

            VideoTutorialLink = ConstantHelpDetails.UnsubscribeVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.UnsubscribeKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ContactSupportLink;

            CurrentUnsubscribe = this;
            try
            {
                DialogParticipation.SetRegister(this, this);
                SetDataContext();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            SelectChannel = SelectChannel.Instance;
        }

        private static Unsubscribe CurrentUnsubscribe { get; set; }

        public SelectChannel SelectChannel { get; set; }

        public UnsubscribeViewModel ObjUnSubscribeViewModel
        {
            get => _objUnsubscribeViewModel;
            set
            {
                _objUnsubscribeViewModel = value;
                OnPropertyChanged(nameof(ObjUnSubscribeViewModel));
            }
        }

        public static Unsubscribe GetSingeltonObjectUnsubscribe()
        {
            return CurrentUnsubscribe ?? (CurrentUnsubscribe = new Unsubscribe());
        }

        public override void SelectAccount()
        {
            try
            {
                if (SelectChannel.SelectChannelViewModel.ChannelSelectModel.ListSelectDestination.Count !=
                    SelectChannel.SelectChannelViewModel.GetYdSuccessAccounts.Count)
                {
                    var lastViewModel = SelectChannel.SelectChannelViewModel;
                    SelectChannel = SelectChannel.Instance = new SelectChannel();
                    SelectChannel.SelectChannelViewModel = lastViewModel;
                }
                else
                {
                    SelectChannel = SelectChannel.Instance;
                }

                SelectChannel.BtnSave.Click += BtnSaveEvent;

                ObjViewModel.Model.ListSelectDestination.Where(x => !x.IsAccountSelected)
                    .ForEach(y => y.IsAccountSelected = true);
                var lastSelected = ObjViewModel.Model.ListSelectDestination.Where(x => x.IsAccountSelected);

                if (ObjViewModel.Model.ListSelectDestination.Count == 0)
                {
                    SelectChannel.SelectChannelViewModel.Groups.ForEach(x => x.IsContentSelected = false);
                    var temp = SelectChannel.SelectChannelViewModel.ChannelSelectModel.ListSelectDestination?.Where(y =>
                        lastSelected.Any(z => z.AccountId == y.AccountId) && !y.IsAccountSelected);
                    temp.ForEach(x => x.IsAccountSelected = true);
                    SelectChannel.SelectChannelViewModel.ChannelSelectModel.ListSelectDestination
                        ?.Where(y => y.IsAccountSelected && lastSelected.All(z => z.AccountId != y.AccountId))
                        .ForEach(x => x.IsAccountSelected = false);
                }

                if (SelectChannel.SelectChannelViewModel.GetYdSuccessAccounts.Count == 0)
                {
                    ObjViewModel.Model.ListSelectDestination.Clear();
                    SelectChannel.SelectChannelViewModel.ChannelSelectModel.ListSelectDestination.Clear();
                }

                if (ObjViewModel.Model.ListSelectDestination.Count > 0)
                {
                    SelectChannel.SelectChannelViewModel.CheckDeletedAccounts(
                        SelectChannel.SelectChannelViewModel.GetYdSuccessAccounts,
                        ObjViewModel.Model.ListSelectDestination);
                    var temp = SelectChannel.SelectChannelViewModel.ChannelSelectModel.ListSelectDestination?.Where(y =>
                        lastSelected.Any(z => z.AccountId == y.AccountId) && !y.IsAccountSelected);
                    temp.ForEach(x => x.IsAccountSelected = true);
                    SelectChannel.SelectChannelViewModel.ChannelSelectModel.ListSelectDestination
                        ?.Where(y => y.IsAccountSelected && lastSelected.All(z => z.AccountId != y.AccountId))
                        .ForEach(x => x.IsAccountSelected = false);
                    SelectChannel.SelectChannelViewModel.DestinationCollectionView =
                        CollectionViewSource.GetDefaultView(
                            SelectChannel.SelectChannelViewModel.ChannelSelectModel.ListSelectDestination ??
                            new ObservableCollection<ChannelDestinationSelectModel>());

                    if (SelectChannel.SelectChannelViewModel.ChannelSelectModel.ListSelectDestination.Count !=
                        SelectChannel.SelectChannelViewModel.GetYdSuccessAccounts.Count)
                        SelectChannel.SelectChannelViewModel.InitializeDestinationList();
                }
                else
                {
                    SelectChannel.SelectChannelViewModel.InitializeDestinationList();
                }

                _enteredHere = true;
                _window = new Dialog().GetMetroWindow(SelectChannel, "LangKeySelectChannels".FromResourceDictionary());
                _window.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                SelectChannel.BtnSave.Click -= BtnSaveEvent;
            }

            _enteredHere = false;
        }

        private void BtnSaveEvent(object sender, EventArgs e)
        {
            try
            {
                if (!_enteredHere) return;

                ObjViewModel.Model.ListSelectDestination.Clear();
                ObjViewModel.Model.ListSelectDestination.AddRange(
                    SelectChannel.SelectChannelViewModel.ChannelSelectModel.ListSelectDestination.Where(x =>
                        x.IsAccountSelected));
                var listOfSelectedAccounts =
                    ObjViewModel.Model.ListSelectDestination.Select(x => x.AccountName).ToList();

                FooterControl_OnSelectAccountChanged(listOfSelectedAccounts);
                _window.Close();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}