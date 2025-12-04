using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using DominatorUIUtility.Views.SocioPublisher.CustomControl;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherDirectPostsViewModel : BindableBase
    {
        #region Constructor

        public PublisherDirectPostsViewModel()
        {
            MultiplePostCommand = new BaseCommand<object>(CanExecuteMultiPost, ExecuteMultiPost);
            ImportFromCsvCommand = new BaseCommand<object>(ImportFromCsvCanExecute, ImportFromCsvExecute);
            SearchCommand = new BaseCommand<object>(SearchCanExecute, SearchExecute);
            SaveCurrentPostCommand = new BaseCommand<object>(CanExecuteSaveSinglePost, SavePost);
            UploadDescriptionCommand = new BaseCommand<object>(sender => true, UploadDescription);
            MultipleImageUrlCommand = new BaseCommand<object>(sender => true, MultipleImageUrlExecute);
        }

        private void MultipleImageUrlExecute(object sender)
        {
            try
            {
                PostDetailsModel.ListMultipleImageUrl = Regex.Split(PostDetailsModel.MultipleImageUrl, "\r\n")
                    .Where(x => !string.IsNullOrEmpty(x)).ToList();
            }
            catch (Exception)
            {
            }
        }

        private void UploadDescription(object obj)
        {
            var createCampaignModel = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                .PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel;
            createCampaignModel.LstUploadPostDescription = FileUtilities.FileBrowseAndReader();
        }

        public PublisherDirectPostsViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {
            this.tabItemsControl = tabItemsControl;
            PostDetailsModel = tabItemsControl.PostDetailsModel;
        }

        #endregion

        #region Properties

        private static object _lock = new object();
        //private ObservableCollection<PostDetailsModel> _lstPostDetailsModels;

        //public ObservableCollection<PostDetailsModel> LstPostDetailsModels
        //{
        //    get
        //    {
        //        return _lstPostDetailsModels;
        //    }
        //    set
        //    {
        //        if (_lstPostDetailsModels == value)
        //            return;
        //        SetProperty(ref _lstPostDetailsModels, value);
        //    }
        //}

        /// <summary>
        ///     Post source details
        /// </summary>
        private readonly PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl;

        private PostDetailsModel _postDetailsModel = new PostDetailsModel();

        /// <summary>
        ///     Keep post details, which holds all needed data about post
        /// </summary>
        public PostDetailsModel PostDetailsModel
        {
            get => _postDetailsModel;
            set
            {
                if (_postDetailsModel == value)
                    return;
                _postDetailsModel = value;
                OnPropertyChanged(nameof(PostDetailsModel));
            }
        }

        private PublisherMediaViewerModel _publisherMultipleImagesMediaViewerModel = new PublisherMediaViewerModel();

        /// <summary>
        ///     To specify the media viewer details
        /// </summary>
        public PublisherMediaViewerModel PublisherMultipleImagesMediaViewerModel
        {
            get => _publisherMultipleImagesMediaViewerModel;
            set
            {
                if (_publisherMultipleImagesMediaViewerModel == value)
                    return;
                _publisherMultipleImagesMediaViewerModel = value;
                OnPropertyChanged(nameof(PublisherMultipleImagesMediaViewerModel));
            }
        }

        public List<string> MediaList { get; set; } = new List<string>();

        #endregion

        #region Command

        /// <summary>
        ///     Opening mulitple post window
        /// </summary>
        public ICommand MultiplePostCommand { get; set; }

        /// <summary>
        ///     Importing Posts from Csv
        /// </summary>
        public ICommand ImportFromCsvCommand { get; set; }

        /// <summary>
        ///     Getting multiple image posts
        /// </summary>
        public ICommand SearchCommand { get; set; }

        /// <summary>
        ///     Saving posts to create campaign view model
        /// </summary>
        public ICommand SaveCurrentPostCommand { get; set; }

        public ICommand UploadDescriptionCommand { get; set; }

        public ICommand MultipleImageUrlCommand { get; set; }

        #endregion

        #region Methods

        public bool CanExecuteSaveSinglePost(object sender)
        {
            return true;
        }

        /// <summary>
        ///     Save the post list
        /// </summary>
        /// <param name="sender"></param>
        public void SavePost(object sender)
        {
            var saveLocation = sender as string;
            var status = saveLocation == "SaveToPending" ? PostQueuedStatus.Pending : PostQueuedStatus.Draft;
            try
            {
                // Get the create campaign Post list model
                var createCampaignModel = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                    .PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel;

                // get all post collection 
                var postCollectionDetails = createCampaignModel.PostCollection;

                // Is need to display logger message
                var isLoggerNeeded = false;

                #region Single Post

                // Single Posts save post
                if (PostDetailsModel.IsSinglePost && (!string.IsNullOrEmpty(PostDetailsModel.PostDescription) ||
                                                      PostDetailsModel.MediaViewer.MediaList.Count > 0 ||
                                                      !string.IsNullOrEmpty(PostDetailsModel.PublisherInstagramTitle) ||
                                                      !string.IsNullOrEmpty(PostDetailsModel.PdSourceUrl)))
                {
                    isLoggerNeeded = true;

                    // Getting deep clone
                    var cloneObject = PostDetailsModel.DeepClone();

                    // Assign created date time
                    cloneObject.CreatedDateTime = DateTime.Now;

                    // Post Id
                    cloneObject.PostDetailsId = Utilities.GetGuid();

                    // Post Queued status
                    cloneObject.PostQueuedStatus = status;

                    // Adding to post collections
                    postCollectionDetails.Add(cloneObject);
                }

                #endregion

                #region Multiple Post

                // Check whether multiple posts and image posts contains posts or not
                if (PostDetailsModel.IsMultiPost && createCampaignModel.LstPostDetailsModels.Count > 0)
                {
                    // Iterate the Multiple posts 
                    createCampaignModel.LstPostDetailsModels.ForEach(post =>
                    {
                        post.PostQueuedStatus = status;
                        // Add to Post Collections 
                        postCollectionDetails.Add(post);
                    });
                    isLoggerNeeded = true;
                }

                #endregion

                #region Multiple Images post

                if (PostDetailsModel.IsMultipleImagePost &&
                    createCampaignModel.LstMultipleImagePostCollection.Count > 0)
                {
                    var LstTitle = new List<string>();
                    if (!string.IsNullOrEmpty(PostDetailsModel.PublisherInstagramTitle))
                        LstTitle = PostDetailsModel.PublisherInstagramTitle.Split('\n').ToList();
                    // Iterate the Multiple image posts
                    createCampaignModel.LstMultipleImagePostCollection.ForEach(post =>
                    {
                        post.PostQueuedStatus = saveLocation == "SaveToPending"
                            ? PostQueuedStatus.Pending
                            : PostQueuedStatus.Draft;

                        post.IsUseFileNameAsDescription = PostDetailsModel.IsUseFileNameAsDescription;
                        post.IsUniquePost = PostDetailsModel.IsUniquePost;
                        if (LstTitle != null && LstTitle.Count > 0)
                            post.PublisherInstagramTitle = LstTitle?.GetRandomItem();

                        // Add to Post Collections 
                        postCollectionDetails.Add(post);
                    });
                    isLoggerNeeded = true;
                }

                #endregion

                if (isLoggerNeeded)
                {
                    // Clearing current post objects
                    PostDetailsModel = new PostDetailsModel();
                    var publisherDirectPosts = PublisherDirectPosts.GetPublisherDirectPosts(tabItemsControl);
                    publisherDirectPosts.PostContentControl.SetMedia();
                    publisherDirectPosts.ImageMediaViewer.Initialize();

                    var createCampaign = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns();
                    createCampaign.PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.PostDetailsModel =
                        new PostDetailsModel();
                    tabItemsControl.PostDetailsModel = new PostDetailsModel();

                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, createCampaignModel.CampaignName,
                        "Publisher Campaign", $"{"LangKeyPostSaved".FromResourceDictionary()} to {status} list");
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, createCampaignModel.CampaignName,
                        "Publisher Campaign",
                        $"{"LangKeyFailedToSave".FromResourceDictionary()} to {status} list. {"LangKeyAddAtleastOnePost".FromResourceDictionary()}");
                }

                _multipostWindow?.Close();
                _multipostWindow = null;
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        //Clear the current object values
                        createCampaignModel.LstMultipleImagePostCollection =
                            new ObservableCollection<PostDetailsModel>();
                        createCampaignModel.LstPostDetailsModels = new ObservableCollection<PostDetailsModel>();
                    });
                }
                else
                {
                    //Clear the current object values
                    createCampaignModel.LstMultipleImagePostCollection =
                        new ObservableCollection<PostDetailsModel>();
                    createCampaignModel.LstPostDetailsModels = new ObservableCollection<PostDetailsModel>();
                }

                PublisherMultiplePost.GetMultiplePost(createCampaignModel.LstPostDetailsModels).Dispose();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static Window _multipostWindow;

        private static Window GetMultiPost(ObservableCollection<PostDetailsModel> lstPostDetailsModels)
        {
            if (_multipostWindow == null)
            {
                var publisherMultiplePost = PublisherMultiplePost.GetMultiplePost(lstPostDetailsModels);
                // Get the core dialog object
                var dialog = new Dialog();

                // Pass the object with Title
                _multipostWindow =
                    dialog.GetMetroWindow(publisherMultiplePost, "LangKeyMultiplePost".FromResourceDictionary());
                _multipostWindow.Closing += (s, e) =>
                {
                    e.Cancel = true;
                    _multipostWindow.Visibility = Visibility.Collapsed;
                };
                return _multipostWindow;
            }

            return _multipostWindow;
        }

        public bool CanExecuteMultiPost(object sender)
        {
            return true;
        }

        /// <summary>
        ///     Open Multiple post list window
        /// </summary>
        /// <param name="sender"></param>
        public void ExecuteMultiPost(object sender)
        {
            // Get the object of multiple post UI
            //var publisherMultiplePost = new PublisherMultiplePost();
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                try
                {
                    var window = GetMultiPost(PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                        .PublisherCreateCampaignViewModel
                        .PublisherCreateCampaignModel.LstPostDetailsModels);
                    window.ShowDialog();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private bool SearchCanExecute(object sender)
        {
            return true;
        }

        /// <summary>
        ///     SearchExecute is used for fetching the image posts from the url
        /// </summary>
        /// <param name="sender"></param>
        private void SearchExecute(object sender)
        {
            try
            {
                var pageTitle = string.Empty;

                var mediaViewer = (MediaViewer)sender;

                // check whether Image url is empty or not
                if (string.IsNullOrEmpty(PostDetailsModel.ImagesUrl))
                {
                    mediaViewer.MediaList.Clear();
                    PostDetailsModel.MediaList.Clear();
                    MediaList.Clear();
                    PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                        .PublisherCreateCampaignModel.LstMultipleImagePostCollection.Clear();
                    // Re intialize post lists
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    mediaViewer?.Initialize();
                    return;
                }

                // Start scraping the image url from ImageExtracter.ExtractImageUrls
                PostDetailsModel.MediaList =
                    new ObservableCollection<string>(ImageExtracter.ExtractImageUrls(PostDetailsModel.ImagesUrl,
                        ref pageTitle));

                // Add the scraped medias to postdetails collection
                PostDetailsModel.MediaList.ForEach(x => MediaList.Add(x));

                // Get the Create campaign View model object for multiple image post
                var postDetails = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                    .PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstMultipleImagePostCollection;

                // Iterate all the media and add into post detail model
                foreach (var image in PostDetailsModel.MediaList)
                {
                    // Assign the image url
                    var publisherMediaViewerModel = new PublisherMediaViewerModel
                    { MediaList = new ObservableCollection<string> { image } };

                    // Create a new object with fetched image urls
                    var postDetailsModel = new PostDetailsModel
                    {
                        MediaList = new ObservableCollection<string> { image },
                        PostDetailsId = Utilities.GetGuid(),
                        PostDescription = new Uri(image).Segments.Last(),
                        CreatedDateTime = DateTime.Now,
                        MediaViewer = publisherMediaViewerModel,
                        IsMultipleImagePost = true
                    };

                    //Added post lists
                    postDetails.Add(postDetailsModel);
                }

                mediaViewer.MediaList = PostDetailsModel.MediaList;
                // Re intialize post lists
                // ReSharper disable once ConstantConditionalAccessQualifier
                mediaViewer?.Initialize();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool ImportFromCsvCanExecute(object sender)
        {
            return true;
        }

        /// <summary>
        ///     Import the posts from csv file
        /// </summary>
        /// <param name="sender"></param>
        private void ImportFromCsvExecute(object sender)
        {
            // select the file path
            var listPostDetailsModel = FileUtilities.FileBrowseAndReader();

            if (listPostDetailsModel.Count == 0)
                return;
            var publisherMultiplePost = PublisherMultiplePost.GetMultiplePost(PublisherCreateCampaigns
                .GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                .PublisherCreateCampaignModel.LstPostDetailsModels);

            // Get all post details from campaign View model
            //var LstPostDetailsModels = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()?.PublisherCreateCampaignViewModel?
            //     .PublisherCreateCampaignModel?.LstPostDetailsModels;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var window = GetMultiPost(PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns()
                    .PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstPostDetailsModels);
                window?.ShowDialog();
            }));
            Task.Factory.StartNew(() =>
            {
                publisherMultiplePost.PublisherMultiplePostViewModel.IsProgressVisibile = Visibility.Visible;
                publisherMultiplePost.PublisherMultiplePostViewModel.IsProgressActive = true;
                Thread.Sleep(1000);

                // Split with separator
                var separator = ConstantVariable.Separator;

                var mediaUtilites = new MediaUtilites();
                // Iterate selected file name
                listPostDetailsModel.ForEach(x =>
                {
                    if (publisherMultiplePost.PublisherMultiplePostViewModel.IsStopLoadingPost)
                        return;

                    try
                    {
                        using (var postDetailsModel = new PostDetailsModel())
                        {
                            // PostDetailsModel postDetailsModel = new PostDetailsModel();

                            // Split the file details
                            var allData = x.Split('\t');

                            postDetailsModel.PostDescription = allData[0];

                            // Media list

                            #region Medialist

                            var mediaDetails = allData.Length > 1 ? allData[1] : string.Empty;

                            var mediaUrl = Regex.Split(mediaDetails, separator).ToList();
                            mediaUrl.ForEach(media =>
                            {
                                var path = media.Trim();
                                if (File.Exists(path))
                                {
                                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                        postDetailsModel.MediaViewer.MediaList.Add(mediaUtilites.GetThumbnail(path))));
                                    Thread.Sleep(2);
                                }
                                var extension = Path.GetExtension(path)?.Replace(".", "");
                                if (ConstantVariable.SupportedVideoFormat.Contains(extension))
                                    postDetailsModel.MediaList.Add(path);
                            });

                            #endregion

                            // Title
                            postDetailsModel.PublisherInstagramTitle = allData.Length > 2 ? allData[2] : string.Empty;

                            // Source url
                            postDetailsModel.PdSourceUrl = allData.Length > 3 ? allData[3] : string.Empty;

                            // Facebook Sell post details

                            #region FdSell

                            var FdSellDetails = allData.Length > 4 ? allData[4] : string.Empty;

                            var Fdsell = Regex.Split(FdSellDetails, separator);

                            if (string.Compare(Fdsell[0], "Yes", StringComparison.CurrentCultureIgnoreCase) == 0 ||
                                string.Compare(Fdsell[0], "Y", StringComparison.CurrentCultureIgnoreCase) == 0 ||
                                string.Compare(Fdsell[0], "True", StringComparison.CurrentCultureIgnoreCase) == 0)
                            {
                                postDetailsModel.IsFdSellPost = true;
                                postDetailsModel.FdSellProductTitle = Fdsell[1];
                                postDetailsModel.FdSellPrice = double.Parse(Fdsell[2]);
                                postDetailsModel.FdSellLocation = Fdsell[3];
                                postDetailsModel.FdCondition = Fdsell[4];
                            }

                            #endregion

                            // Created date
                            postDetailsModel.CreatedDateTime = DateTime.Now;

                            // Post id
                            postDetailsModel.PostDetailsId = Utilities.GetGuid();

                            // Add to Collections
                            //postDetails.Add(postDetailsModel);

                            Application.Current.Dispatcher.BeginInvoke(new Action(() => PublisherCreateCampaigns
                                .GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                                .PublisherCreateCampaignModel.LstPostDetailsModels.Add(postDetailsModel)));
                            Thread.Sleep(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });

                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    publisherMultiplePost.PublisherMultiplePostViewModel.PostListsCollectionView =
                        CollectionViewSource.GetDefaultView(PublisherCreateCampaigns
                            .GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                            .PublisherCreateCampaignModel.LstPostDetailsModels)));
                publisherMultiplePost.PublisherMultiplePostViewModel.IsProgressVisibile = Visibility.Collapsed;
                publisherMultiplePost.PublisherMultiplePostViewModel.IsProgressActive = false;
                publisherMultiplePost.PublisherMultiplePostViewModel.IsStopLoadingPost = false;
            });
        }

        #endregion
    }
}