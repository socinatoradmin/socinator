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
using YoutubeDominatorCore.YoutubeModels.EngageModel;
using YoutubeDominatorCore.YoutubeViewModel.EngageViewModel;
using YoutubeDominatorUI.CustomControl;
using static YoutubeDominatorCore.YDEnums.Enums;

namespace YoutubeDominatorUI.YDViews.Engage
{
    public class CommentBase : ModuleSettingsUserControl<CommentViewModel, CommentModel>
    {
        protected override bool ValidateCampaign()
        {
            // Check queries
            if (Model.SavedQueries.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyErrorAddAtLeastOneQuery".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.CommentModel.CommentAsReplyOnlyChecked)
                if (ObjViewModel.CommentModel.SavedQueries.Any(x =>
                    x.QueryTypeEnum == "CustomUrls" &&
                    (x.QueryValue.ToLower().EndsWith("&lc=") || !x.QueryValue.ToLower().Contains("&lc="))))
                {
                    var dialog = Dialog.ShowCustomDialog("LangKeyWarning".FromResourceDictionary(),
                        "LangKeyUrlsDontHaveCommentId".FromResourceDictionary(),
                        "LangKeyContinue".FromResourceDictionary(), "LangKeyEdit".FromResourceDictionary());
                    if (dialog == MessageDialogResult.Negative)
                        return false;
                }

            if (ObjViewModel.CommentModel.LstDisplayManageCommentModel.Count == 0)
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyAddAtLeastOneComment".FromResourceDictionary());
                return false;
            }

            if (ObjViewModel.CommentModel.IsCheckFixCommentCountForPost &&
                string.IsNullOrWhiteSpace(ObjViewModel.CommentModel.CommentIdForReply))
            {
                Dialog.ShowDialog(this, "LangKeyError".FromResourceDictionary(),
                    "LangKeyCheckedReplyToCommentWithTextButNotGivenText".FromResourceDictionary());
                return false;
            }

            if (!ObjViewModel.Model.SavedQueries.Any(x => x.QueryTypeEnum == "Keywords") &&
                Model.VideoFilterModel.SearchVideoFilterModel != null)
            {
                Model.VideoFilterModel.IsCheckedSearchVideoFilter = false;
                Model.VideoFilterModel.SearchVideoFilterModel = null;
            }

            return base.ValidateCampaign();
        }
    }

    public partial class Comment
    {
        private bool _enteredHere;
        private Window _window;

        public Comment()
        {
            InitializeComponent();
            CurrentComment = this;
            InitializeBaseClass
            (
                header: HeaderGrid,
                footer: CommentFooter,
                queryControl: CommentSearchControl,
                MainGrid: MainGrid,
                activityType: ActivityType.Comment,
                moduleName: YdMainModule.Comment.ToString()
            );

            VideoTutorialLink = ConstantHelpDetails.CommentVideoTutorialsLink;
            KnowledgeBaseLink = ConstantHelpDetails.CommentKnowledgeBaseLink;
            ContactSupportLink = ConstantHelpDetails.ContactSupportLink;
            try
            {
                SetDataContext();
                DialogParticipation.SetRegister(this, this);
                //ObjViewModel.commentModel.ManageCommentModel.LstQueries.Add(QueryContent);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            SelectChannel = SelectChannel.Instance;
        }

        private static Comment CurrentComment { get; set; }

        public SelectChannel SelectChannel { get; set; }

        public static Comment GetSingeltonObjectComment()
        {
            return CurrentComment ?? (CurrentComment = new Comment());
        }

        public override void SelectAccount()
        {
            try
            {
                SelectChannel.SelectChannelViewModel.IsAllDestinationSelected = false;
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