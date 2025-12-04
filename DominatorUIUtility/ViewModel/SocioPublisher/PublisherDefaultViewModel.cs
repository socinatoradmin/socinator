using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using AdvanceSettingModel = DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherDefaultViewModel : BindableBase
    {
        private readonly IGenericFileManager _genericFileManager;

        public PublisherDefaultViewModel(IGenericFileManager genericFileManager)
        {
            _genericFileManager = genericFileManager;
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            SelectionCommand = new BaseCommand<object>(SelectionCanExecute, SelectionExecute);
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            CampaignCloneCommand = new BaseCommand<object>(CampaignCloneCanExecute, CampaignCloneExecute);
            DeleteCampaignCommand = new BaseCommand<object>(DeleteCampaignCanExecute, DeleteCampaignExecute);
            ActiveSelectedCampaignCommand =
                new BaseCommand<object>(ActiveSelectedCampaignCanExecute, ActiveSelectedCampaignExecute);
            PauseSelectedCampaignCommand =
                new BaseCommand<object>(PauseSelectedCampaignCanExecute, PauseSelectedCampaignExecute);
            PublishNowSelectedCampaignCommand = new BaseCommand<object>(PublishNowSelectedCampaignCanExecute,
                PublishNowSelectedCampaignExecute);
            CopyCampaignId = new BaseCommand<object>(CopyCampaignIdCanExecute, CopyCampaignIdExecute);
            TutorialsCommand = new DelegateCommand(() => IsTutorialsOpen = true);
            WatchTutorialCommand = new DelegateCommand<string>(url => Process.Start(url));
            InitializeDefaultCampaignStatus();
        }


        private bool PublishNowSelectedCampaignCanExecute(object sender)
        {
            return true;
        }

        private void PublishNowSelectedCampaignExecute(object sender)
        {
            ThreadFactory.Instance.Start(() =>
            {
                var selectedCampaigns = GetSelectedCampaigns();
                // Call to publish now options
                selectedCampaigns.ForEach(x =>
                {
                    PublishScheduler.SchedulePublishNowByCampaign(x.CampaignId);
                    //PublisherInitialize.GetInstance.UpdateCampaignStatus(x.CampaignId, PublisherCampaignStatus.Active);
                    UpdateCampaignStatus(x.CampaignId, PublisherCampaignStatus.Active);
                    // Set the default settings
                    InitializeDefaultCampaignStatus();
                });
            });
            // Get all selected campaigns
        }

        public void UpdateCampaignStatus(string campaignId, PublisherCampaignStatus status)
        {
            // Get campaign Details
            var campaignItem = ListPublisherCampaignStatusModels.FirstOrDefault(x => x.CampaignId == campaignId);

            if (campaignItem == null)
                return;

            // get the index of current item
            var currentCampaignIndex = ListPublisherCampaignStatusModels.IndexOf(campaignItem);

            // Update the status
            ListPublisherCampaignStatusModels[currentCampaignIndex].Status = status;

            // Get campaign model
            var allCampaign = _genericFileManager
                .GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile());

            // Get the particular campaign
            var currentCampaign = allCampaign.FirstOrDefault(x => x.CampaignId == campaignId);

            if (currentCampaign == null)
                return;
            // Finding index
            var campaignIndex = allCampaign.IndexOf(currentCampaign);

            // Update status 
            currentCampaign.CampaignStatus = status;
            allCampaign[campaignIndex] = currentCampaign;

            //Save into bin file 
            _genericFileManager.UpdateModuleDetails(allCampaign, ConstantVariable.GetPublisherCampaignFile());
        }

        private bool PauseSelectedCampaignCanExecute(object sender)
        {
            return true;
        }

        private void PauseSelectedCampaignExecute(object sender)
        {
            // Get all selected campaigns
            var selectedCampaigns = GetSelectedCampaigns();
            selectedCampaigns.ForEach(x =>
            {
                // Stop publishing posts
                PublishScheduler.StopPublishingPosts(x.CampaignId);
                PublisherPostFetcher.StopFetchingPostsByCampaignId(x.CampaignId, false);
                // Update the campaign status to paused
                UpdateCampaignStatus(x.CampaignId, PublisherCampaignStatus.Paused);
                // Call to set the default settings
                InitializeDefaultCampaignStatus();

                Thread.Sleep(1000);
                //GlobusLogHelper.log.Info(Log.PublisherCampaignPaused, x.CampaignName);
            });

            if (selectedCampaigns.Count > 0)
                GlobusLogHelper.log.Info("LangKeyCampaignStatusChangedPause".FromResourceDictionary());
        }


        private bool ActiveSelectedCampaignCanExecute(object sender)
        {
            return true;
        }

        private void ActiveSelectedCampaignExecute(object sender)
        {
            // Get the selected campaigns
            var selectedCampaigns = GetSelectedCampaigns();
            selectedCampaigns.ForEach(x =>
            {
                // Call to schedule todays campaign
                var publisherPostFetcher = new PublisherPostFetcher();
                // Start fetching the post for the campaign
                publisherPostFetcher.FetchPostsForCampaign(x.CampaignId);
                PublishScheduler.ScheduleTodaysPublisherByCampaign(x.CampaignId);
                // Make the status to active
                UpdateCampaignStatus(x.CampaignId, PublisherCampaignStatus.Active);
                // Set the default settings
                InitializeDefaultCampaignStatus();
            });
            // Validate whether campaign is active is possible
            if (selectedCampaigns.Count > 0)
                GlobusLogHelper.log.Info("LangKeyCampaignStatusChangedActive".FromResourceDictionary());
        }

        private bool CopyCampaignIdCanExecute(object sender)
        {
            return true;
        }

        private void CopyCampaignIdExecute(object sender)
        {
            if (sender is PublisherCampaignStatusModel)
            {
                // Get the campaign status object
                var campaignDetails = sender as PublisherCampaignStatusModel;
                // Copy to clipboard
                Clipboard.SetText(campaignDetails.CampaignId);
                ToasterNotification.ShowSuccess("LangKeyCampaignIdCopied".FromResourceDictionary());
            }
            else
            {
                // Get all selected campaign Id
                var selectedCampaigns = GetSelectedCampaigns().Select(x => x.CampaignId).ToList();


                if (selectedCampaigns.Count != 0)
                {
                    // Get the export path
                    var exportPath = FileUtilities.GetExportPath();

                    if (!string.IsNullOrEmpty(exportPath))
                    {
                        var header = "Campaign Id";

                        var filename = $"{exportPath}\\{ConstantVariable.GetDateTime()}.csv";

                        FileUtilities.AddHeaderToCsv(filename, header);

                        // Iterate and export the campaign id to csv file
                        selectedCampaigns.ForEach(campaignId =>
                        {
                            // var csvData = campaignId;
                            using (var streamWriter = new StreamWriter(filename, true))
                            {
                                streamWriter.WriteLine(campaignId);
                            }
                        });
                    }
                    else
                    {
                        Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                            "LangKeySelectPathToExport".FromResourceDictionary());
                    }
                }
                else
                {
                    Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                        "LangKeySelectCampaignToExport".FromResourceDictionary());
                }
            }
        }


        private bool NavigationCanExecute(object sender)
        {
            return true;
        }

        private void NavigationExecute(object sender)
        {
            // switch to edit campaign
            if (sender is PublisherCampaignStatusModel)
                try
                {
                    // Get the campaign details object
                    var createCampign = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns();

                    // collect the binding model
                    var currentCampaign = _genericFileManager
                        .GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile())
                        .FirstOrDefault(campaign =>
                            campaign.CampaignId == (sender as PublisherCampaignStatusModel).CampaignId);

                    // Clear already saved model
                    createCampign.PublisherCreateCampaignViewModel.ClearCurrentCampaigns();

                    // Assign the selected campaign's name to combo box
                    createCampign.PublisherCreateCampaignViewModel.SelectedItem = currentCampaign?.CampaignName;

                    // Navigate to campaign UI
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl =
                        createCampign;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            else
                try
                {
                    var moduleName = sender.ToString();

                    switch (moduleName)
                    {
                        // Navigate to manage destination
                        case "ManageDestinations":
                            PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl =
                                PublisherManageDestinations.Instance();
                            break;
                        // Navigate to manage posts
                        case "ManagePosts":
                            PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl =
                                new PublisherManagePosts();
                            break;
                        // Navigate to create campaign
                        case "CreateCampaigns":
                            var createCampign = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns();
                            createCampign.PublisherCreateCampaignViewModel.ClearCurrentCampaigns();
                            PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl =
                                createCampign;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
        }

        private bool OpenContextMenuCanExecute(object sender)
        {
            return true;
        }

        private void OpenContextMenuExecute(object sender)
        {
            try
            {
                var contextMenu = ((Button) sender).ContextMenu;
                if (contextMenu == null) return;
                contextMenu.DataContext = ((Button) sender).DataContext;
                contextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool SelectionCanExecute(object sender)
        {
            return true;
        }

        private void SelectionExecute(object sender)
        {
            var moduleName = sender.ToString();
            switch (moduleName)
            {
                // Deselect all campaigns
                case "MenuSelectNone":
                case "SelectNone":
                    IsAllCampaignSelected = false;
                    break;

                // Select all campaign
                case "SelectAll":
                case "MenuSelectAll":
                    IsAllCampaignSelected = true;
                    break;

                // Enable the select all option while in selecting all campaign
                case "SelectManually":

                    if (ListPublisherCampaignStatusModels.All(x => x.IsSelected))
                    {
                        IsAllCampaignSelected = true;
                    }
                    else
                    {
                        if (IsAllCampaignSelected)
                            _isUncheckedFromList = true;
                        IsAllCampaignSelected = false;
                    }

                    break;
            }
        }

        private bool CampaignCloneCanExecute(object sender)
        {
            return true;
        }

        private void CampaignCloneExecute(object sender)
        {
            try
            {
                // get the source campaign for cloning 
                var isSingleDuplicate = sender is PublisherCampaignStatusModel;

                if (isSingleDuplicate)
                {
                    // selected campaign
                    var campaignStatus = (PublisherCampaignStatusModel) sender;

                    // Get the deep clone
                    var clonedCampaignStatus = GetCampaginDeepClone(campaignStatus);

                    // Generate new campaignId and campaign Name
                    clonedCampaignStatus.GenerateCloneCampaign(campaignStatus.CampaignName);
                    clonedCampaignStatus.Status = PublisherCampaignStatus.Active;
                    SaveClonedCampaign(clonedCampaignStatus, campaignStatus.CampaignId);

                    if (campaignStatus.IsSelected)
                        clonedCampaignStatus.IsSelected = true;

                    GlobusLogHelper.log.Info(campaignStatus.CampaignName +
                                             $" {"LangKeySuccessfullyDuplicated".FromResourceDictionary()}");

                    // Update in default page
                    PublisherInitialize.GetInstance.AddCampaignDetails(clonedCampaignStatus);

                    // Collect all postlists
                    var allSavedPosts = PostlistFileManager.GetAll(campaignStatus.CampaignId);

                    var clonedPostlist = new List<PublisherPostlistModel>();
                    allSavedPosts.ForEach(x =>
                    {
                        // Generate the post id
                        x.GenerateClonePostId();

                        x.CampaignId = clonedCampaignStatus.CampaignId;

                        // Move all published posts to pending posts while cloning
                        x.PostQueuedStatus = x.PostQueuedStatus == PostQueuedStatus.Published
                            ? PostQueuedStatus.Pending
                            : x.PostQueuedStatus;
                        x.LstPublishedPostDetailsModels = new ObservableCollection<PublishedPostDetailsModel>();
                        x.PostRunningStatus = PostRunningStatus.Active;
                        // Add to clone options
                        clonedPostlist.Add(x);
                    });

                    // Update the new post lists
                    PostlistFileManager.UpdatePostlists(clonedCampaignStatus.CampaignId, clonedPostlist);


                    PublisherInitialize.GetInstance.UpdatePostStatus(clonedCampaignStatus.CampaignId);

                    // Save advanced setting of all network

                    #region Saving Advance setting

                    var file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social);
                    var generalModels = _genericFileManager.GetModuleDetails<GeneralModel>(file)
                        .FirstOrDefault(x => x.CampaignId == campaignStatus.CampaignId);
                    if (generalModels != null)
                    {
                        generalModels.CampaignId = clonedCampaignStatus.CampaignId;
                        _genericFileManager.AddModule(generalModels, file);
                    }

                    file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Facebook);
                    var facebookModel = _genericFileManager.GetModuleDetails<FacebookModel>(file)
                        .FirstOrDefault(x => x.CampaignId == campaignStatus.CampaignId);
                    if (facebookModel != null)
                    {
                        facebookModel.CampaignId = clonedCampaignStatus.CampaignId;
                        _genericFileManager.AddModule(facebookModel, file);
                    }


                    //file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Gplus);
                    //var googlePlusModel = _genericFileManager.GetModuleDetails<GooglePlusModel>(file).FirstOrDefault(x => x.CampaignId == campaignStatus.CampaignId);
                    //if (googlePlusModel != null)
                    //{
                    //    googlePlusModel.CampaignId = clonedCampaignStatus.CampaignId;
                    //    _genericFileManager.AddModule(googlePlusModel, file);
                    //}


                    file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Instagram);
                    var instagramModel = _genericFileManager.GetModuleDetails<AdvanceSettingModel.InstagramModel>(file)
                        .FirstOrDefault(x => x.CampaignId == campaignStatus.CampaignId);
                    if (instagramModel != null)
                    {
                        instagramModel.CampaignId = clonedCampaignStatus.CampaignId;
                        _genericFileManager.AddModule(instagramModel, file);
                    }


                    file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Pinterest);
                    var pinterestModel = _genericFileManager.GetModuleDetails<AdvanceSettingModel.PinterestModel>(file)
                        .FirstOrDefault(x => x.CampaignId == campaignStatus.CampaignId);
                    if (pinterestModel != null)
                    {
                        pinterestModel.CampaignId = clonedCampaignStatus.CampaignId;
                        _genericFileManager.AddModule(pinterestModel, file);
                    }


                    file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Tumblr);
                    var tumblrModel = _genericFileManager.GetModuleDetails<AdvanceSettingModel.TumblrModel>(file)
                        .FirstOrDefault(x => x.CampaignId == campaignStatus.CampaignId);
                    if (tumblrModel != null)
                    {
                        tumblrModel.CampaignId = clonedCampaignStatus.CampaignId;
                        _genericFileManager.AddModule(tumblrModel, file);
                    }

                    file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Twitter);
                    var twitterModel = _genericFileManager.GetModuleDetails<AdvanceSettingModel.TwitterModel>(file)
                        .FirstOrDefault(x => x.CampaignId == campaignStatus.CampaignId);
                    if (twitterModel != null)
                    {
                        twitterModel.CampaignId = clonedCampaignStatus.CampaignId;
                        _genericFileManager.AddModule(twitterModel, file);
                    }


                    file = ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Reddit);
                    var redditModel = _genericFileManager.GetModuleDetails<RedditModel>(file)
                        .FirstOrDefault(x => x.CampaignId == campaignStatus.CampaignId);
                    if (redditModel != null)
                    {
                        redditModel.CampaignId = clonedCampaignStatus.CampaignId;
                        _genericFileManager.AddModule(redditModel, file);
                    }

                    #endregion

                    // get post fetcher model
                    var publisherPostFetchModel =
                        _genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                            .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == campaignStatus.CampaignId);

                    // Save all manage destinations
                    PublisherManageDestinationModel.AddCampaignToDestinationList(
                        publisherPostFetchModel?.SelectedDestinations, clonedCampaignStatus.CampaignId);

                    // Enable the post fetcher options
                    var allFetchDetails = _genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).Where(x => x.CampaignId == campaignStatus.CampaignId);

                    var currentCampaignsFetchDetails = new List<PublisherPostFetchModel>();

                    allFetchDetails.ForEach(x =>
                    {
                        x.CampaignId = clonedCampaignStatus.CampaignId;
                        x.CampaignName = clonedCampaignStatus.CampaignName;
                        currentCampaignsFetchDetails.Add(x);
                    });

                    // Add to bin file for post fetcher
                    _genericFileManager.AddRangeModule(currentCampaignsFetchDetails,
                        ConstantVariable.GetPublisherPostFetchFile);

                    var publisherPostFetcher = new PublisherPostFetcher();
                    publisherPostFetcher.FetchPostsForCampaign(clonedCampaignStatus.CampaignId);

                    // Schedule the today job process if campaign is active
                    if (clonedCampaignStatus.Status == PublisherCampaignStatus.Active)
                        PublishScheduler.ScheduleTodaysPublisherByCampaign(clonedCampaignStatus.CampaignId);
                }
                else
                {
                    GetSelectedCampaigns().ForEach(campaign =>
                    {
                        // Get the deep clone
                        var clonedCampaignStatus = GetCampaginDeepClone(campaign);

                        // Generate new campaignId and campaign Name
                        clonedCampaignStatus.GenerateCloneCampaign(campaign.CampaignName);
                        clonedCampaignStatus.Status = PublisherCampaignStatus.Active;
                        SaveClonedCampaign(clonedCampaignStatus, campaign.CampaignId);

                        // Update in default page
                        PublisherInitialize.GetInstance.AddCampaignDetails(clonedCampaignStatus);

                        if (campaign.IsSelected)
                            clonedCampaignStatus.IsSelected = true;

                        var allSavedPosts = PostlistFileManager.GetAll(campaign.CampaignId);

                        var clonedPostlist = new List<PublisherPostlistModel>();
                        allSavedPosts.ForEach(x =>
                        {
                            // Generate the post id
                            x.GenerateClonePostId();
                            x.CampaignId = clonedCampaignStatus.CampaignId;
                            // Move all published posts to pending posts while cloning
                            x.PostQueuedStatus = x.PostQueuedStatus == PostQueuedStatus.Published
                                ? PostQueuedStatus.Pending
                                : x.PostQueuedStatus;
                            x.LstPublishedPostDetailsModels = new ObservableCollection<PublishedPostDetailsModel>();
                            x.PostRunningStatus = PostRunningStatus.Active;
                            // Add to clone options
                            clonedPostlist.Add(x);
                        });
                        // Update the new post lists
                        PostlistFileManager.UpdatePostlists(clonedCampaignStatus.CampaignId, clonedPostlist);

                        PublisherInitialize.GetInstance.UpdatePostStatus(clonedCampaignStatus.CampaignId);

                        var publisherPostFetchModel =
                            _genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                                .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == campaign.CampaignId);

                        // Save all manage destinations
                        PublisherManageDestinationModel.AddCampaignToDestinationList(
                            publisherPostFetchModel?.SelectedDestinations, clonedCampaignStatus.CampaignId);

                        // get post fetcher model
                        var allFetchDetails = _genericFileManager.GetModuleDetails<PublisherPostFetchModel>(
                            ConstantVariable
                                .GetPublisherPostFetchFile).Where(x => x.CampaignId == campaign.CampaignId);

                        var currentCampaignsFetchDetails = new List<PublisherPostFetchModel>();

                        // Enable the post fetcher options
                        allFetchDetails.ForEach(x =>
                        {
                            x.CampaignId = clonedCampaignStatus.CampaignId;
                            x.CampaignName = clonedCampaignStatus.CampaignName;
                            currentCampaignsFetchDetails.Add(x);
                        });

                        // Add to bin file for post fetcher
                        _genericFileManager.AddRangeModule(currentCampaignsFetchDetails,
                            ConstantVariable.GetPublisherPostFetchFile);

                        var publisherPostFetcher = new PublisherPostFetcher();

                        // Start fetching the post for the campaign
                        publisherPostFetcher.FetchPostsForCampaign(clonedCampaignStatus.CampaignId);

                        // Schedule the today job process if campaign is active
                        if (clonedCampaignStatus.Status == PublisherCampaignStatus.Active)
                            PublishScheduler.ScheduleTodaysPublisherByCampaign(clonedCampaignStatus.CampaignId);
                    });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static void SaveClonedCampaign(PublisherCampaignStatusModel clonedCampaignStatus, string campaignId)
        {
            try
            {
                // Get the saved campaign detail
                var genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
                var duplicatedCampaign = genericFileManager
                    .GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile())
                    .FirstOrDefault(campaign => campaign.CampaignId == campaignId);

                if (duplicatedCampaign == null)
                    return;

                duplicatedCampaign.CampaignName = clonedCampaignStatus.CampaignName;
                duplicatedCampaign.CampaignStatus = clonedCampaignStatus.Status;
                duplicatedCampaign.CampaignId = clonedCampaignStatus.CampaignId;
                duplicatedCampaign.CreatedDate = clonedCampaignStatus.CreatedDate;
                duplicatedCampaign.UpdatedDate = clonedCampaignStatus.UpdatedTime;
                duplicatedCampaign.JobConfigurations.CampaignStartDate = clonedCampaignStatus.StartDate;

                // Add to bin file
                genericFileManager.AddModule(duplicatedCampaign, ConstantVariable.GetPublisherCampaignFile());

                // Add the campaign name to campaign list 
                PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .CampaignList.Add(duplicatedCampaign.CampaignName);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SelectAllCampaign()
        {
            // select all campaign
            ListPublisherCampaignStatusModels.Select(x =>
            {
                x.IsSelected = true;
                return x;
            }).ToList();
        }

        public void SelectNoneCampaign()
        {
            // deselect all campaign
            ListPublisherCampaignStatusModels.Select(x =>
            {
                x.IsSelected = false;
                return x;
            }).ToList();
        }

        public bool DeleteCampaignCanExecute(object sender)
        {
            return true;
        }

        public void DeleteCampaignExecute(object sender)
        {
            var isIndividualDelete = sender is PublisherCampaignStatusModel;
            var campaignList = PublisherManagePosts.Instance.PublisherManagePostsViewModel.CampaignList;
            // Single delete campaign
            if (isIndividualDelete)
            {
                var campaign = (PublisherCampaignStatusModel) sender;

                var dialogResult = Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(),
                    "LangKeyConfirmOnIfDeletedCantRecoverBack".FromResourceDictionary(),
                    "LangKeyDeleteAnyway".FromResourceDictionary(), "LangKeyDontDelete".FromResourceDictionary());

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;
                // remove from campaign Ui
                PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .CampaignList.Remove(campaign.CampaignName);
                Application.Current.Dispatcher.Invoke(delegate
                {
                    ListPublisherCampaignStatusModels.RemoveAt(ListPublisherCampaignStatusModels.IndexOf(campaign));
                });
                // Stop publishing the post
                PublishScheduler.StopPublishingPosts(campaign.CampaignId);

                // Disconnect the relationship from remove destinations
                PublisherManageDestinationModel.RemoveDestinationFromCampaign(campaign.CampaignId);

                // Stop publishing delete model
                _genericFileManager.Delete<PostDeletionModel>(y => campaign.CampaignId == y.CampaignId,
                    ConstantVariable.GetDeletePublisherPostModel);

                // stop fetching posts for a campaign
                PublisherPostFetcher.StopFetchingPostsByCampaignId(campaign.CampaignId);

                // Delete the post list bin file for the campaign
                _genericFileManager.DeleteBinFiles(
                    $"{ConstantVariable.GetPublisherCreatePostlistFolder()}\\{campaign.CampaignId}.bin");
                // Delete the campaign from bin file
                _genericFileManager.Delete<PublisherCreateCampaignModel>(y => campaign.CampaignId == y.CampaignId,
                    ConstantVariable.GetPublisherCampaignFile());
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, campaign.CampaignName,
                    "Publisher Campaign",
                    $"{campaign.CampaignName} {"LangKeyDeletedSuccessfully".FromResourceDictionary()}");

                //update campaign list in managepost
                campaignList.Remove(campaignList.FirstOrDefault(x => x.Id == campaign.CampaignId));
            }
            else
            {
                // Get all selected campaign
                var publisherCampaignStatusModels = GetSelectedCampaigns();

                if (publisherCampaignStatusModels.Count == 0)
                {
                    Dialog.ShowDialog("LangKeyAlert".FromResourceDictionary(),
                        "LangKeySelectAtleastOneCampaign".FromResourceDictionary());
                    return;
                }

                var dialogResult = Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(),
                    "LangKeyConfirmOnIfDeletedWillDeleteAllSelectedCampaign".FromResourceDictionary(),
                    "LangKeyDeleteAnyway".FromResourceDictionary(), "LangKeyDontDelete".FromResourceDictionary());

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;
                publisherCampaignStatusModels.ForEach(x =>
                {
                    // remove from campaign Ui
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        ListPublisherCampaignStatusModels.RemoveAt(ListPublisherCampaignStatusModels.IndexOf(x));
                    });
                    PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                        .CampaignList.Remove(x.CampaignName);

                    // Stop publishing the post
                    PublishScheduler.StopPublishingPosts(x.CampaignId);

                    // Disconnect the relationship from remove destinations
                    PublisherManageDestinationModel.RemoveDestinationFromCampaign(x.CampaignId);

                    // Stop publishing delete model
                    _genericFileManager.Delete<PostDeletionModel>(y => x.CampaignId == y.CampaignId,
                        ConstantVariable.GetDeletePublisherPostModel);

                    // stop fetching posts for a campaign
                    PublisherPostFetcher.StopFetchingPostsByCampaignId(x.CampaignId);

                    // Delete the post list bin file for the campaign
                    _genericFileManager.DeleteBinFiles(
                        $"{ConstantVariable.GetPublisherCreatePostlistFolder()}\\{x.CampaignId}.bin");
                    // Delete the campaign from bin file
                    _genericFileManager.Delete<PublisherCreateCampaignModel>(y => x.CampaignId == y.CampaignId, ConstantVariable
                    .GetPublisherCampaignFile());
                    //update campaign list in managepost
                    campaignList.Remove(campaignList.FirstOrDefault(y => y.Id == x.CampaignId));
                });
                _genericFileManager.Delete<PublisherPostFetchModel>(
                    x => publisherCampaignStatusModels.FirstOrDefault(a => a.CampaignId == x.CampaignId) != null,
                    ConstantVariable.GetPublisherPostFetchFile);
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, "", "Publisher Campaign",
                    "LangKeyCampaignDeletionCompleted".FromResourceDictionary());
            }

            if (ListPublisherCampaignStatusModels.Count == 0 ||
                !ListPublisherCampaignStatusModels.Any(x => x.IsSelected))
                IsAllCampaignSelected = false;
        }

        /// <summary>
        ///     Get all selected campaign
        /// </summary>
        /// <returns></returns>
        private List<PublisherCampaignStatusModel> GetSelectedCampaigns()
        {
            return ListPublisherCampaignStatusModels.Where(x => x.IsSelected).ToList();
        }

        public void InitializeDefaultCampaignStatus()
        {
            ListPublisherCampaignStatusModels = PublisherInitialize.GetInstance.GetSavedCampaigns();
            PublisherCampaignStatusModelView = null;
            PublisherCampaignStatusModelView = CollectionViewSource.GetDefaultView(ListPublisherCampaignStatusModels);
        }

        // Get the deep clone
        public PublisherCampaignStatusModel GetCampaginDeepClone(
            PublisherCampaignStatusModel publisherCampaignStatusModel)
        {
            return publisherCampaignStatusModel.DeepClone();
        }

        #region Command

        /// <summary>
        ///     Copy Campaign Id is used to get the campaign Id
        /// </summary>
        public ICommand CopyCampaignId { get; set; }

        /// <summary>
        ///     To Navigate the user control to manage destination, Manage posts, Create campaign, and Edit Campaign
        /// </summary>
        public ICommand NavigationCommand { get; set; }

        /// <summary>
        ///     To Enable the button's context menu on left click also
        /// </summary>
        public ICommand OpenContextMenuCommand { get; set; }

        /// <summary>
        ///     To Select the campaign's details
        /// </summary>
        public ICommand SelectionCommand { get; set; }

        /// <summary>
        ///     To make the clone of the campaigns
        /// </summary>
        public ICommand CampaignCloneCommand { get; set; }

        /// <summary>
        ///     To Delete the campaign from publisher
        /// </summary>
        public ICommand DeleteCampaignCommand { get; set; }

        /// <summary>
        ///     To make the campaign to active state
        /// </summary>
        public ICommand ActiveSelectedCampaignCommand { get; set; }

        /// <summary>
        ///     To make the campaign to pause state
        /// </summary>
        public ICommand PauseSelectedCampaignCommand { get; set; }

        /// <summary>
        ///     To publish the campaign's functions now without any time specific
        /// </summary>
        public ICommand PublishNowSelectedCampaignCommand { get; set; }

        public ICommand TutorialsCommand { get; set; }
        public ICommand WatchTutorialCommand { get; set; }

        #endregion

        #region Properties

        private bool _isTutorialsOpen;

        public bool IsTutorialsOpen
        {
            get => _isTutorialsOpen;
            set => SetProperty(ref _isTutorialsOpen, value);
        }


        /// <summary>
        ///     To holds the default pages details
        /// </summary>
        public ObservableCollection<PublisherCampaignStatusModel> ListPublisherCampaignStatusModels
        {
            get => _listPublisherCampaignStatusModels;
            set
            {
                _listPublisherCampaignStatusModels = value;
                OnPropertyChanged(nameof(ListPublisherCampaignStatusModels));
                if (IsAllCampaignSelected)
                    ListPublisherCampaignStatusModels.Select(x =>
                    {
                        x.IsSelected = true;
                        return x;
                    }).ToList();
            }
        }

        private ICollectionView _publisherCampaignStatusModelView;

        /// <summary>
        ///     To specify the collection view for binding
        /// </summary>
        public ICollectionView PublisherCampaignStatusModelView
        {
            get => _publisherCampaignStatusModelView;
            set
            {
                if (_publisherCampaignStatusModelView != null && _publisherCampaignStatusModelView == value)
                    return;
                SetProperty(ref _publisherCampaignStatusModelView, value);
            }
        }

        private bool _isAllCampaignSelected;

        private ObservableCollection<PublisherCampaignStatusModel> _listPublisherCampaignStatusModels =
            new ObservableCollection<PublisherCampaignStatusModel>();

        /// <summary>
        ///     To specify all campaign is selected or not
        /// </summary>
        public bool IsAllCampaignSelected
        {
            get => _isAllCampaignSelected;
            set
            {
                if (_isAllCampaignSelected == value)
                    return;
                SetProperty(ref _isAllCampaignSelected, value);

                SelectAll(_isAllCampaignSelected);
                _isUncheckedFromList = false;
            }
        }

        private bool _isUncheckedFromList { get; set; }

        /// <summary>
        ///     To change the selection status of the campaign
        /// </summary>
        /// <param name="isAllSelected"></param>
        public void SelectAll(bool isAllSelected)
        {
            if (_isUncheckedFromList)
                return;
            ListPublisherCampaignStatusModels.Select(x =>
            {
                x.IsSelected = isAllSelected;
                return x;
            }).ToList();
        }

        #endregion
    }
}