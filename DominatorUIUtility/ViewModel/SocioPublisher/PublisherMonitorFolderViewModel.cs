using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherMonitorFolderViewModel : BindableBase
    {
        private readonly PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl;

        public PublisherMonitorFolderViewModel()
        {
            #region Command Initilization

            ClearCommand = new BaseCommand<object>(ClearCanExecute, ClearExecute);
            SaveCommand = new BaseCommand<object>(SaveCanExecute, SaveExecute);
            EditCommand = new BaseCommand<object>(EditCanExecute, EditExecute);
            DeleteCommand = new BaseCommand<object>(DeleteCanExecute, DeleteExecute);
            BrowseFolderCommand = new BaseCommand<object>(BrowseFolderCanExecute, BrowseFolderExecute);

            #endregion
        }

        public PublisherMonitorFolderViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) :
            this()
        {
            this.tabItemsControl = tabItemsControl;
            LstFolderPath = tabItemsControl.LstFolderPath;
        }

        #region Command

        public ICommand ClearCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand BrowseFolderCommand { get; set; }

        #endregion

        #region Properties

        private PublisherMonitorFolderModel _publisherMonitorFolderModel = new PublisherMonitorFolderModel();

        public PublisherMonitorFolderModel PublisherMonitorFolderModel
        {
            get => _publisherMonitorFolderModel;
            set
            {
                if (value == _publisherMonitorFolderModel)
                    return;
                SetProperty(ref _publisherMonitorFolderModel, value);
            }
        }

        private ObservableCollection<PublisherMonitorFolderModel> _lstFolderPath =
            new ObservableCollection<PublisherMonitorFolderModel>();

        public ObservableCollection<PublisherMonitorFolderModel> LstFolderPath
        {
            get => _lstFolderPath;
            set
            {
                if (value == _lstFolderPath)
                    return;
                SetProperty(ref _lstFolderPath, value);
            }
        }

        #endregion

        #region Methods

        private bool DeleteCanExecute(object sender)
        {
            return true;
        }

        private void DeleteExecute(object sender)
        {
            try
            {
                var itemTodelete = ((FrameworkElement) sender).DataContext as PublisherMonitorFolderModel;
                LstFolderPath.Remove(LstFolderPath.FirstOrDefault(x => x.FolderPath == itemTodelete.FolderPath));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool EditCanExecute(object sender)
        {
            return true;
        }

        private void EditExecute(object sender)
        {
            try
            {
                var itemToEdit = ((FrameworkElement) sender).DataContext as PublisherMonitorFolderModel;
                itemToEdit.ButtonContent = "LangKeyUpdateFolderPath".FromResourceDictionary();
                PublisherMonitorFolderModel = itemToEdit;
                PublisherMonitorFolder.GetPublisherMonitorFolder(tabItemsControl).PostContentControl.SetMedia();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool SaveCanExecute(object sender)
        {
            return true;
        }

        private void SaveExecute(object sender)
        {
            if (!string.IsNullOrEmpty(PublisherMonitorFolderModel.FolderPath))
                try
                {
                    if (PublisherMonitorFolderModel.ButtonContent == "LangKeySaveFolderPath".FromResourceDictionary())
                    {
                        if (LstFolderPath.All(x => string.Compare(x.FolderPath, PublisherMonitorFolderModel.FolderPath,
                                                       StringComparison.CurrentCultureIgnoreCase) != 0))
                        {
                            PublisherMonitorFolderModel.PostDetailsModel.PostDetailsId = Utilities.GetGuid();
                            PublisherMonitorFolderModel.PostDetailsModel.CreatedDateTime = DateTime.Now;
                            LstFolderPath.Add(new PublisherMonitorFolderModel
                            {
                                FolderPath = PublisherMonitorFolderModel.FolderPath.Trim(),
                                FolderTemplate = PublisherMonitorFolderModel.FolderTemplate,
                                PostDetailsModel = PublisherMonitorFolderModel.PostDetailsModel
                            });
                        }
                    }
                    else
                    {
                        var itemToUpdate =
                            LstFolderPath.FirstOrDefault(x => x.FolderPath == PublisherMonitorFolderModel.FolderPath);
                        if (itemToUpdate == null)
                            LstFolderPath.Add(new PublisherMonitorFolderModel
                            {
                                FolderPath = PublisherMonitorFolderModel.FolderPath.Trim(),
                                FolderTemplate = PublisherMonitorFolderModel.FolderTemplate,
                                PostDetailsModel = PublisherMonitorFolderModel.PostDetailsModel
                            });
                        else
                            // ReSharper disable once RedundantAssignment
                            itemToUpdate = PublisherMonitorFolderModel;
                    }

                    PublisherMonitorFolderModel = new PublisherMonitorFolderModel();
                    PublisherMonitorFolder.GetPublisherMonitorFolder(tabItemsControl).PostContentControl.SetMedia();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            else
                GlobusLogHelper.log.Info("LangKeyPleaseEnterFolderPath".FromResourceDictionary());
        }

        private bool ClearCanExecute(object sender)
        {
            return true;
        }

        private void ClearExecute(object sender)
        {
            try
            {
                PublisherMonitorFolderModel = new PublisherMonitorFolderModel();
                PublisherMonitorFolder.GetPublisherMonitorFolder(tabItemsControl).PostContentControl.SetMedia();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private bool BrowseFolderCanExecute(object arg)
        {
            return true;
        }

        private void BrowseFolderExecute(object obj)
        {
            var folderPath = FileUtilities.GetExportPath(true);
            if (string.IsNullOrEmpty(folderPath))
                return;
            if (!LstFolderPath.Any(x =>
                string.Compare(x.FolderPath, folderPath, StringComparison.CurrentCultureIgnoreCase) == 0))
                PublisherMonitorFolderModel.FolderPath = folderPath.Trim();
            else
                Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                    "LangKeyFolderPathAlreadyExist".FromResourceDictionary());
        }

        #endregion
    }
}