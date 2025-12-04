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
    public class PublisherRssFeedViewModel : BindableBase
    {
        private readonly PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl;

        public PublisherRssFeedViewModel()
        {
            #region Command Initilization

            ClearCommand = new BaseCommand<object>(ClearCanExecute, ClearExecute);
            SaveCommand = new BaseCommand<object>(SaveCanExecute, SaveExecute);
            EditCommand = new BaseCommand<object>(EditCanExecute, EditExecute);
            DeleteCommand = new BaseCommand<object>(DeleteCanExecute, DeleteExecute);

            #endregion
        }

        public PublisherRssFeedViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {
            this.tabItemsControl = tabItemsControl;
            LstFeedUrl = tabItemsControl.LstFeedUrl;
        }

        #region Command

        public ICommand ClearCommand { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        #endregion

        #region Properties

        private PublisherRssFeedModel _publisherRssFeedModel = new PublisherRssFeedModel();

        public PublisherRssFeedModel PublisherRssFeedModel
        {
            get => _publisherRssFeedModel;
            set
            {
                if (value == _publisherRssFeedModel)
                    return;
                SetProperty(ref _publisherRssFeedModel, value);
            }
        }

        private ObservableCollection<PublisherRssFeedModel> _lstFeedUrl =
            new ObservableCollection<PublisherRssFeedModel>();

        public ObservableCollection<PublisherRssFeedModel> LstFeedUrl
        {
            get => _lstFeedUrl;
            set
            {
                if (value == _lstFeedUrl)
                    return;
                SetProperty(ref _lstFeedUrl, value);
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
                var itemTodelete = ((FrameworkElement) sender).DataContext as PublisherRssFeedModel;
                LstFeedUrl.Remove(LstFeedUrl.FirstOrDefault(x => x.FeedUrl == itemTodelete.FeedUrl));
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
                var itemToEdit = ((FrameworkElement) sender).DataContext as PublisherRssFeedModel;
                itemToEdit.ButtonContent = "LangKeyUpdateFeedUrl".FromResourceDictionary();
                PublisherRssFeedModel = itemToEdit;
                PublisherRssFeed.GetPublisherRssFeed(tabItemsControl).PostContentControl.SetMedia();
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
            if (!string.IsNullOrEmpty(PublisherRssFeedModel.FeedUrl))
                try
                {
                    if (PublisherRssFeedModel.ButtonContent == "LangKeySaveFeedUrl".FromResourceDictionary())
                    {
                        if (LstFeedUrl.All(x => string.Compare(x.FeedUrl, PublisherRssFeedModel.FeedUrl,
                                                    StringComparison.CurrentCultureIgnoreCase) != 0))
                        {
                            PublisherRssFeedModel.PostDetailsModel.PostDetailsId = Utilities.GetGuid();
                            PublisherRssFeedModel.PostDetailsModel.CreatedDateTime = DateTime.Now;
                            LstFeedUrl.Add(new PublisherRssFeedModel
                            {
                                FeedUrl = PublisherRssFeedModel.FeedUrl.Trim(),
                                FeedTemplate = PublisherRssFeedModel.FeedTemplate,
                                PostDetailsModel = PublisherRssFeedModel.PostDetailsModel
                            });
                        }
                    }
                    else
                    {
                        var itemToUpdate = LstFeedUrl.FirstOrDefault(x => x.FeedUrl == PublisherRssFeedModel.FeedUrl);
                        if (itemToUpdate == null)
                            LstFeedUrl.Add(new PublisherRssFeedModel
                            {
                                FeedUrl = PublisherRssFeedModel.FeedUrl.Trim(),
                                FeedTemplate = PublisherRssFeedModel.FeedTemplate,
                                PostDetailsModel = PublisherRssFeedModel.PostDetailsModel
                            });
                        else
                            // ReSharper disable once RedundantAssignment
                            itemToUpdate = PublisherRssFeedModel;
                    }

                    PublisherRssFeedModel = new PublisherRssFeedModel();
                    PublisherRssFeed.GetPublisherRssFeed(tabItemsControl).PostContentControl.SetMedia();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            else
                GlobusLogHelper.log.Info("LangKeyPleaseEnterFeedUrl".FromResourceDictionary());
        }

        private bool ClearCanExecute(object sender)
        {
            return true;
        }

        private void ClearExecute(object sender)
        {
            try
            {
                PublisherRssFeedModel = new PublisherRssFeedModel();
                PublisherRssFeed.GetPublisherRssFeed(tabItemsControl).PostContentControl.SetMedia();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion
    }
}