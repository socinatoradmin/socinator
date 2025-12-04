using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManagePostsViewModel : BindableBase
    {
        private readonly IGenericFileManager _genericFileManager;

        public PublisherManagePostsViewModel()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            TabChangeCommand = new BaseCommand<object>(TabChangeCanExecute, TabChangeExecute);
            SelectionChangedCommand = new BaseCommand<object>(SelectionChangedCanExecute, SelectionChangedExecute);
            InitializeTabs();
        }

        private void InitializeTabs()
        {
            ManagePostTabItems.Add(Application.Current.FindResource("LangKeyDraft")?.ToString());
            ManagePostTabItems.Add(Application.Current.FindResource("LangKeyPending")?.ToString());
            ManagePostTabItems.Add(Application.Current.FindResource("LangKeyPublished")?.ToString());

            var campaignDetails =
                _genericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable
                    .GetPublisherCampaignFile());

            campaignDetails.ForEach(campaign =>
                CampaignList.Add(new IdNameBinderModel {Id = campaign.CampaignId, Name = campaign.CampaignName}));


            if (campaignDetails.Count > 0)
                SelectedCampaignDetails = CampaignList[0];
        }

        private bool NavigationCanExecute(object sender)
        {
            return true;
        }

        private void NavigationExecute(object sender)
        {
            var module = sender.ToString();
            switch (module)
            {
                case "Back":
                {
                    // stop processes by cancellationToken if any process running  
                    PostLoadingCancellation();
                    // call the default Publisher Homepage
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherDefaultPage.Instance();
                }
                    break;
            }
        }

        private bool TabChangeCanExecute(object sender)
        {
            return true;
        }

        public void TabChangeExecute(object sender)
        {
            var selectedButton = sender as string;
            var cancellationToken = PostLoadingCancellation();

            if (selectedButton == ConstantVariable.DraftPostList)
            {
                var draftView = PublisherManagePostDrafts.GetPublisherManagePostDrafts();
                SelectedTabsUserControls = draftView;
                try
                {
                    if (SelectedCampaignDetails?.Id == null)
                    {
                        draftView.PublisherManagePostDraftsViewModel.IsProgressRingActive = false;
                        return;
                    }

                    SelectedTabs = ConstantVariable.DraftPostList;
                    ThreadFactory.Instance.Start(
                        () => draftView.PublisherManagePostDraftsViewModel.ReadPostList(SelectedCampaignDetails.Id,
                            cancellationToken), cancellationToken.Token);
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Request Cancelled!");
                }
                catch (Exception ex)
                {
                    draftView.PublisherManagePostDraftsViewModel.IsProgressRingActive = false;
                    ex.DebugLog();
                }
            }
            else if (selectedButton == ConstantVariable.PendingPostList)
            {
                var pendingView = PublisherManagePostPending.GetPublisherManagePostPending();
                SelectedTabsUserControls = pendingView;
                try
                {
                    if (SelectedCampaignDetails?.Id == null)
                    {
                        pendingView.PublisherManagePostPendingViewModel.IsProgressRingActive = false;
                        return;
                    }

                    SelectedTabs = ConstantVariable.PendingPostList;
                    ThreadFactory.Instance.Start(
                        () => pendingView.PublisherManagePostPendingViewModel.ReadPostList(SelectedCampaignDetails.Id,
                            cancellationToken, PostQueuedStatus.Pending), cancellationToken.Token);
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Request Cancelled!");
                }
                catch (Exception ex)
                {
                    pendingView.PublisherManagePostPendingViewModel.IsProgressRingActive = false;
                    ex.DebugLog();
                }
            }
            else
            {
                var publishedView = PublisherManagePostPublished.GetPublisherManagePostPublished();
                SelectedTabsUserControls = publishedView;
                try
                {
                    if (SelectedCampaignDetails?.Id == null)
                    {
                        publishedView.PublisherManagePostPublishedViewModel.IsProgressRingActive = false;
                        return;
                    }

                    SelectedTabs = ConstantVariable.PublishedPostList;
                    ThreadFactory.Instance.Start(
                        () => publishedView.PublisherManagePostPublishedViewModel.ReadPostList(
                            SelectedCampaignDetails.Id, cancellationToken, PostQueuedStatus.Published),
                        cancellationToken.Token);
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Request Cancelled!");
                }
                catch (Exception ex)
                {
                    publishedView.PublisherManagePostPublishedViewModel.IsProgressRingActive = false;
                    ex.DebugLog();
                }
            }
        }

        private CancellationTokenSource PostLoadingCancellation()
        {
            CancelRunningTask();
            var cancellationToken = new CancellationTokenSource();
            QueueCancellationTokenSources.Enqueue(cancellationToken);
            return cancellationToken;
        }

        private bool SelectionChangedCanExecute(object sender)
        {
            return true;
        }

        private void SelectionChangedExecute(object sender)
        {
            var cancellationToken = PostLoadingCancellation();

            switch (SelectedTabs)
            {
                case "Draft":
                    try
                    {
                        var draftView = PublisherManagePostDrafts.GetPublisherManagePostDrafts();
                        ThreadFactory.Instance.Start(
                            () => draftView.PublisherManagePostDraftsViewModel.ReadPostList(SelectedCampaignDetails.Id,
                                cancellationToken), cancellationToken.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        ex.DebugLog("Request Cancelled!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }

                    break;
                case "Pending":
                    try
                    {
                        var pendingView = PublisherManagePostPending.GetPublisherManagePostPending();
                        ThreadFactory.Instance.Start(
                            () => pendingView.PublisherManagePostPendingViewModel.ReadPostList(
                                SelectedCampaignDetails.Id, cancellationToken, PostQueuedStatus.Pending),
                            cancellationToken.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        ex.DebugLog("Request Cancelled!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }

                    break;
                case "Published":
                    try
                    {
                        var publishedView = PublisherManagePostPublished.GetPublisherManagePostPublished();
                        ThreadFactory.Instance.Start(
                            () => publishedView.PublisherManagePostPublishedViewModel.ReadPostList(
                                SelectedCampaignDetails.Id, cancellationToken, PostQueuedStatus.Published),
                            cancellationToken.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        ex.DebugLog("Request Cancelled!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }

                    break;
            }
        }

        public void CancelRunningTask()
        {
            while (QueueCancellationTokenSources.Count > 0)
                QueueCancellationTokenSources.Dequeue().Cancel();
        }


        #region Properties

        public ICommand NavigationCommand { get; set; }

        public ICommand TabChangeCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }

        public List<string> ManagePostTabItems { get; set; } = new List<string>();

        private string _selectedTabs = string.Empty;

        public string SelectedTabs
        {
            get => _selectedTabs;
            set
            {
                if (_selectedTabs == value)
                    return;
                _selectedTabs = value;
                OnPropertyChanged(nameof(SelectedTabs));
            }
        }


        private IdNameBinderModel _selectedCampaignDetails;

        public IdNameBinderModel SelectedCampaignDetails
        {
            get => _selectedCampaignDetails;
            set
            {
                if (_selectedCampaignDetails == value)
                    return;
                _selectedCampaignDetails = value;
                OnPropertyChanged(nameof(SelectedCampaignDetails));
            }
        }


        private List<IdNameBinderModel> _campaignList = new List<IdNameBinderModel>();

        public List<IdNameBinderModel> CampaignList
        {
            get => _campaignList;
            set
            {
                if (_campaignList == value)
                    return;
                _campaignList = value;
                OnPropertyChanged(nameof(CampaignList));
            }
        }

        private UserControl _selectedTabsUserControls = PublisherManagePostDrafts.GetPublisherManagePostDrafts();

        public UserControl SelectedTabsUserControls
        {
            get => _selectedTabsUserControls;
            set
            {
                if (Equals(_selectedTabsUserControls, value))
                    return;
                _selectedTabsUserControls = value;
                OnPropertyChanged(nameof(SelectedTabsUserControls));
            }
        }

        public Queue<CancellationTokenSource> QueueCancellationTokenSources { get; set; } =
            new Queue<CancellationTokenSource>();

        #endregion
    }
}