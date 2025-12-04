using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManageDestinationViewModel : BindableBase
    {
        private readonly IGenericFileManager _genericFileManager;

        #region Constructor

        public PublisherManageDestinationViewModel()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            SelectionCommand = new BaseCommand<object>(SelectionCanExecute, SelectionExecute);
            DeleteDestinationCommand = new BaseCommand<object>(DeleteDestinationCanExecute, DeleteDestinationExecute);
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
        }

        #endregion

        private List<PublisherManageDestinationModel> GetSelectedDestinations()
        {
            return ListPublisherManageDestinationModels.Where(x => x.IsSelected).ToList();
        }

        /// <summary>
        ///     To initialize with default destinations settings
        /// </summary>
        public void InitializeDefaultDestinations()
        {
            // clearing the destination collections
            ListPublisherManageDestinationModels = new ObservableCollection<PublisherManageDestinationModel>();
            PublisherManageDestinationModelView = null;
            // Updating the collection view source
            PublisherManageDestinationModelView =
                CollectionViewSource.GetDefaultView(ListPublisherManageDestinationModels);

            // Get updated destination
            var savedDestinations = ManageDestinationFileManager.GetAll();

            // Call to add to list
            savedDestinations.ForEach(x => { AddDestinations(x, false); });
        }

        /// <summary>
        ///     To add the destinations to UI binded object with dispatcher
        /// </summary>
        /// <param name="publisherManageDestinationModel">
        ///     Model which going to add! <see cref="PublisherManageDestinationModel" />
        /// </param>
        /// <param name="isNewDestination">Specify given destination is new or just to update already available destinations</param>
        /// <returns></returns>
        public bool AddDestinations(PublisherManageDestinationModel publisherManageDestinationModel,
            bool isNewDestination)
        {
            if (ListPublisherManageDestinationModels.Any(x =>
                x.DestinationName == publisherManageDestinationModel.DestinationName))
            {
                GlobusLogHelper.log.Info("LangKeyDestinationNameAlreadyPresent".FromResourceDictionary());
                return false;
            }

            try
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        // get the used campaign Ids
                        publisherManageDestinationModel.AddUsedCampaignId =
                            publisherManageDestinationModel.AddUsedCampaignId.Distinct().ToList();

                        // Calculate the campaigns
                        publisherManageDestinationModel.CampaignsCount =
                            publisherManageDestinationModel.AddUsedCampaignId.Count;

                        // Add to the list
                        ListPublisherManageDestinationModels.Add(publisherManageDestinationModel);
                    });
                }
                else
                {
                    // get the used campaign Ids
                    publisherManageDestinationModel.AddUsedCampaignId =
                        publisherManageDestinationModel.AddUsedCampaignId.Distinct().ToList();
                    // Calculate the campaigns
                    publisherManageDestinationModel.CampaignsCount =
                        publisherManageDestinationModel.AddUsedCampaignId.Count;
                    // Add to the list
                    ListPublisherManageDestinationModels.Add(publisherManageDestinationModel);
                }

                // Update to bin file if its not present
                if (isNewDestination)
                    ManageDestinationFileManager.Add(publisherManageDestinationModel);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Update the destinations
        /// </summary>
        /// <param name="publisherManageDestinationModel">destinations which is going to update</param>
        /// <returns></returns>
        public bool UpdateDestinations(PublisherManageDestinationModel publisherManageDestinationModel)
        {
            try
            {
                // Dispatcer call
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        // get the updating destinations
                        var destination = ListPublisherManageDestinationModels.FirstOrDefault(x =>
                            x.DestinationId == publisherManageDestinationModel.DestinationId);

                        // void checker
                        if (destination == null)
                            return;

                        // Assgin the valid fields
                        destination.AccountCount = publisherManageDestinationModel.AccountCount;
                        destination.CampaignsCount = publisherManageDestinationModel.CampaignsCount;
                        destination.CreatedDate = publisherManageDestinationModel.CreatedDate;
                        destination.DestinationId = publisherManageDestinationModel.DestinationId;
                        destination.DestinationName = publisherManageDestinationModel.DestinationName;
                        destination.GroupsCount = publisherManageDestinationModel.GroupsCount;
                        destination.IsSelected = publisherManageDestinationModel.IsSelected;
                        destination.PagesOrBoardsCount = publisherManageDestinationModel.PagesOrBoardsCount;
                        destination.WallsOrProfilesCount = publisherManageDestinationModel.WallsOrProfilesCount;
                        destination.CustomDestinationsCount = publisherManageDestinationModel.CustomDestinationsCount;
                        destination.IsAddNewGroups = publisherManageDestinationModel.IsAddNewGroups;
                        destination.IsRemoveGroupsRequiresValidation =
                            publisherManageDestinationModel.IsRemoveGroupsRequiresValidation;
                    });
                }
                else
                {
                    // get the updating destinations
                    var destination = ListPublisherManageDestinationModels.FirstOrDefault(x =>
                        x.DestinationId == publisherManageDestinationModel.DestinationId);

                    // void checker
                    if (destination != null)
                    {
                        // Assgin the valid fields
                        destination.AccountCount = publisherManageDestinationModel.AccountCount;
                        destination.CampaignsCount = publisherManageDestinationModel.CampaignsCount;
                        destination.CreatedDate = publisherManageDestinationModel.CreatedDate;
                        destination.DestinationId = publisherManageDestinationModel.DestinationId;
                        destination.DestinationName = publisherManageDestinationModel.DestinationName;
                        destination.GroupsCount = publisherManageDestinationModel.GroupsCount;
                        destination.IsSelected = publisherManageDestinationModel.IsSelected;
                        destination.PagesOrBoardsCount = publisherManageDestinationModel.PagesOrBoardsCount;
                        destination.WallsOrProfilesCount = publisherManageDestinationModel.WallsOrProfilesCount;
                        destination.CustomDestinationsCount = publisherManageDestinationModel.CustomDestinationsCount;
                        destination.IsAddNewGroups = publisherManageDestinationModel.IsAddNewGroups;
                        destination.IsRemoveGroupsRequiresValidation =
                            publisherManageDestinationModel.IsRemoveGroupsRequiresValidation;
                    }
                }

                // Update the destiantions
                ManageDestinationFileManager.UpdateDestinations(ListPublisherManageDestinationModels);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Get the destiantion object with help of the destination Id
        /// </summary>
        /// <param name="destinationId">Required destination's Id</param>
        /// <returns></returns>
        public PublisherManageDestinationModel GetManageDestination(string destinationId)
        {
            // Find and return matched elements
            return ListPublisherManageDestinationModels.FirstOrDefault(x => x.DestinationId == destinationId);
        }

        #region Properties

        /// <summary>
        ///     To Go to Publisher Home page and Create new destinations pages
        /// </summary>
        public ICommand NavigationCommand { get; set; }

        /// <summary>
        ///     To select the destinations
        /// </summary>
        public ICommand SelectionCommand { get; set; }

        /// <summary>
        ///     To delete the unwanted destination
        /// </summary>
        public ICommand DeleteDestinationCommand { get; set; }

        /// <summary>
        ///     To show up the context menu options
        /// </summary>
        public ICommand OpenContextMenuCommand { get; set; }

        /// <summary>
        ///     To store all destinations
        /// </summary>
        public ObservableCollection<PublisherManageDestinationModel> ListPublisherManageDestinationModels
        {
            get => _listPublisherManageDestinationModels;
            set
            {
                if (_listPublisherManageDestinationModels == value)
                    return;

                SetProperty(ref _listPublisherManageDestinationModels, value);
            }
        }

        /// <summary>
        ///     To Bind the collection view to UI
        /// </summary>
        private ICollectionView _publisherManageDestinationModelView;

        public ICollectionView PublisherManageDestinationModelView
        {
            get => _publisherManageDestinationModelView;
            set => SetProperty(ref _publisherManageDestinationModelView, value);
        }


        private bool _isAllDestinationSelected;

        private ObservableCollection<PublisherManageDestinationModel> _listPublisherManageDestinationModels =
            new ObservableCollection<PublisherManageDestinationModel>();

        private bool _isUncheckedFromList { get; set; }

        /// <summary>
        ///     To specify all destinations
        /// </summary>
        public bool IsAllDestinationSelected
        {
            get => _isAllDestinationSelected;
            set
            {
                if (_isAllDestinationSelected == value)
                    return;
                SetProperty(ref _isAllDestinationSelected, value);


                SelectAllDestination(_isAllDestinationSelected);
                _isUncheckedFromList = false;
            }
        }

        public void SelectAllDestination(bool isAllSelected)
        {
            if (_isUncheckedFromList)
                return;
            ListPublisherManageDestinationModels.Select(x =>
            {
                x.IsSelected = isAllSelected;
                return x;
            }).ToList();
        }

        #endregion

        #region Commands Definitions

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
                    // To go back to home page
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherDefaultPage.Instance();
                    break;
                case "CreateDestination":
                    var createDestiantion = PublisherCreateDestination.Instance;
                    createDestiantion.PublisherCreateDestinationsViewModel.ClearCurrentDestination();
                    // To go for creating new destinations
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = createDestiantion;
                    break;
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
                case "MenuSelectNone":
                case "SelectNone":
                    // To deselect the destinations
                    IsAllDestinationSelected = false;
                    break;

                case "SelectAll":
                case "MenuSelectAll":
                    // To select the destinations
                    IsAllDestinationSelected = true;
                    break;
                case "SelectManually":
                    // To check whether all destinations are selected, then make the tick mark on column header
                    if (ListPublisherManageDestinationModels.All(x => x.IsSelected))
                    {
                        IsAllDestinationSelected = true;
                    }
                    else
                    {
                        if (IsAllDestinationSelected)
                            _isUncheckedFromList = true;
                        // If not so, dont tick the column header 
                        IsAllDestinationSelected = false;
                    }

                    break;
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
                // To display the context menu of the button
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


        public bool DeleteDestinationCanExecute(object sender)
        {
            return true;
        }

        public void DeleteDestinationExecute(object sender)
        {
            var isIndividualDelete = sender is PublisherManageDestinationModel;

            // To delete the single destinations
            if (isIndividualDelete)
            {
                var destination = (PublisherManageDestinationModel) sender;

                // Warning message
                var dialogResult = Dialog.ShowCustomDialog(
                    "LangKeyConfirmation".FromResourceDictionary(),
                    "LangKeyConfirmOnIfDeletedCantRecoverBack".FromResourceDictionary(),
                    "LangKeyDeleteAnyway".FromResourceDictionary(),
                    "LangKeyDontDelete".FromResourceDictionary());

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;
                // Remove from the destination list
                ListPublisherManageDestinationModels.Remove(destination);

                // Update to bin file
                ManageDestinationFileManager.Delete(d => d.DestinationId == destination.DestinationId);
                _genericFileManager.DeleteBinFiles(
                    $"{ConstantVariable.GetPublisherCreateDestinationsFolder()}\\{destination.DestinationId}.bin");
            }
            else
            {
                // Get all selected destinations
                var publisherManageDestinationModel = GetSelectedDestinations();

                // check void selections
                if (publisherManageDestinationModel.Count == 0)
                {
                    Dialog.ShowDialog("LangKeyAlert".FromResourceDictionary(),
                        "LangKeyWarningSelectDestination".FromResourceDictionary());
                    return;
                }

                // Warning message
                var dialogResult = Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(),
                    "LangKeyConfirmOnIfDeletedWillDeleteAllSelectedCampaign".FromResourceDictionary(),
                    "LangKeyDeleteAnyway".FromResourceDictionary(),
                    "LangKeyDontDelete".FromResourceDictionary());

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                publisherManageDestinationModel.ForEach(x =>
                {
                    // To update to bin files
                    ListPublisherManageDestinationModels.Remove(x);
                    _genericFileManager.DeleteBinFiles(
                        $"{ConstantVariable.GetPublisherCreateDestinationsFolder()}\\{x.DestinationId}.bin");
                });

                ManageDestinationFileManager.DeleteSelected(publisherManageDestinationModel);
            }
        }

        #endregion
    }
}