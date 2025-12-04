using System;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using Prism.Commands;
using DominatorHouseCore.LogHelper;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherScrapePostViewModel : BindableBase
    {
        private ScrapePostModel _scrapePostModel = new ScrapePostModel();

        public PublisherScrapePostViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl)
        {
            ScrapePostModel = tabItemsControl.ScrapePostModel;
            UploadPostDetailsCommand = new DelegateCommand(UploadPostDetails);
            UploadPostDescriptionCommand = new DelegateCommand(UploadPostDescriptionExecute);
            DeleteUploadedPostCommand = new DelegateCommand(ExecuteDeleteAllPosts);
        }

        public ICommand UploadPostDetailsCommand { get; }
        public ICommand UploadPostDescriptionCommand { get; }
        public ICommand DeleteUploadedPostCommand { get; }

        public ScrapePostModel ScrapePostModel
        {
            get => _scrapePostModel;
            set
            {
                if (value == _scrapePostModel)
                    return;
                SetProperty(ref _scrapePostModel, value);
            }
        }

        private void UploadPostDescriptionExecute()
        {
            try
            {
                ScrapePostModel.LstUploadPostDescription = FileUtilities.FileBrowseAndReader();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UploadPostDetails()
        {
            try
            {
                var postDetails = FileUtilities.FileBrowseAndReader();
                if (postDetails.Count > 0)
                {
                    if (postDetails[0].Contains("Title") && postDetails[0].Contains("Description") && postDetails[0].Contains("Link"))
                    {
                        postDetails.RemoveAt(0);
                    }
                    postDetails.ForEach(x => ScrapePostModel.LstScrapedPostDetails.Add(x.Trim()));
                    ToasterNotification.ShowSuccess("LangKeyPostDetailsUploaded".FromResourceDictionary());
                }
                else
                {
                    GlobusLogHelper.log.Info("You did not upload any Posts details !!");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Info("There is error in uploading Posts details !!");
            }
        }
        /// <summary>
        /// This method is use for clear all post which uploaded from csv
        /// </summary>
        private void ExecuteDeleteAllPosts()
        {
            if (ScrapePostModel.LstScrapedPostDetails.Count > 0)
            {
                ScrapePostModel.LstScrapedPostDetails.Clear();
                ToasterNotification.ShowSuccess("LangKeyPostDetailsDeleted".FromResourceDictionary());
            }
        }
    }
}