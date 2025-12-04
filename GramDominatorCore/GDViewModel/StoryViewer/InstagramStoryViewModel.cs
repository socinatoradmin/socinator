using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using GramDominatorCore.Response;
using GramDominatorCore.Utility;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GramDominatorCore.GDViewModel.StoryViewer
{
    public class InstagramStoryViewModel : BindableBase
    {
        #region Properties
        public List<SessionModel> Params { get; set; } = new List<SessionModel>();
        private bool _IsProcessing;
        public bool IsProcessing
        {
            get => _IsProcessing;
            set => SetProperty(ref _IsProcessing, value, nameof(IsProcessing));
        }
        private string _downloadPath = GramStatic.GetStoryDownloadPath();
        public string DownloadPath
        {
            get => _downloadPath;
            set => SetProperty(ref _downloadPath, value, nameof(DownloadPath));
        }
        private string _status;
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value, nameof(Status));
        }
        private bool _autoDownload;
        public bool AutoDownload
        {
            get => _autoDownload;
            set => SetProperty(ref _autoDownload, value, nameof(AutoDownload));
        }
        private IgHttpHelper helper { get; set; }
        private string _InstaUsername;
        public string InstaUsername
        {
            get => _InstaUsername;
            set => SetProperty(ref _InstaUsername, value);
        }
        private ObservableCollection<StoryCollection> _UserProfileCollections
            = new ObservableCollection<StoryCollection>();
        public ObservableCollection<StoryCollection> UserProfileCollections
        {
            get => _UserProfileCollections;
            set => SetProperty(ref _UserProfileCollections, value);
        }
        public StoryFetcher fetcher { get; set; }
        public ICommand SearchUserID { get; set; }
        public ICommand DownloadMedia { get; set; }
        public ICommand DownloadHighlight { get; set; }
        public ICommand DownloadProfile { get; set; }
        public ICommand OpenInBrowser { get; set; }
        public ICommand SelectFolder { get; set; }
        #endregion

        #region Constructor

        public InstagramStoryViewModel()
        {
            SearchUserID = new BaseCommand<object>(SearchUserIDCanExecute, SearchUserIDExecute);
            DownloadProfile = new BaseCommand<object>(sender => true, DownloadProfileExecute);
            DownloadMedia = new BaseCommand<object>(DownloadMediaCanExecute, DownloadMediaExecute);
            DownloadHighlight = new BaseCommand<object>(sender => true, DownloadHightlightsExecute);
            OpenInBrowser = new BaseCommand<object>(sender => true, OpenInBrowserExecute);
            SelectFolder = new BaseCommand<object>(sender => true, SelectFolderExecute);
            helper = new IgHttpHelper();
            fetcher = new StoryFetcher();
        }
        #endregion

        #region Command Methods

        private async void DownloadProfileExecute(object obj)
        {
            try
            {
                //Download Media
                var model = obj as StoryCollection;
                if (model != null && !string.IsNullOrEmpty(model.ProfilePic))
                {
                    var fileName = $"{GramStatic.SanitizeFileName(model.Username)}.jpg";
                    var path = Path.Combine(DownloadPath, "InstaProfilePic");
                    DirectoryUtilities.CreateDirectory(path);
                    var filePath = Path.Combine(path, fileName);
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                    await DownloadFileAsync(model.ProfilePic, filePath, "ProfilePic");
                }
            }
            catch { }
            finally
            {
                IsProcessing = false;
            }
        }
        private void SelectFolderExecute(object obj)
        {
            var path = FileUtilities.GetExportPath();
            if (!string.IsNullOrEmpty(path))
                DownloadPath = path;
        }
        private void OpenInBrowserExecute(object obj)
        {
            try
            {
                var model = obj as StoryCollection;
                if (model != null && !string.IsNullOrEmpty(model.Username))
                {
                    var url = $"https://www.instagram.com/{model.Username}/";
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
            }
            catch { }
        }
        private async void DownloadMediaExecute(object obj)
        {
            try
            {
                var counter = 0;
                //Download Media
                var model = obj as StoryCollection;
                if (model != null && model.Stories.Count > 0)
                {

                    foreach (var story in model.Stories)
                    {
                        if (string.IsNullOrEmpty(story?.StoryUrl))
                            continue;
                        await ThreadFactory.Instance.Start(async () =>
                        {
                            var isVideo = false;
                            var extension = "jpg";
                            if (story.Type == "video")
                            {
                                extension = "mp4";
                                isVideo = true;
                            }
                            var fileName = $"{GramStatic.SanitizeFileName(model.Username)}_Story_{counter}.{extension}";
                            var path = Path.Combine(DownloadPath, "InstaStories");
                            DirectoryUtilities.CreateDirectory(path);
                            var filePath = Path.Combine(path, fileName);
                            if (File.Exists(filePath))
                                File.Delete(filePath);
                            await DownloadFileAsync(isVideo ? story.VideoUrl : story.StoryUrl, filePath, "Story");
                        });
                        counter++;
                    }
                }
                else
                {
                    var model1 = obj as StoriesMedia;
                    model = UserProfileCollections.FirstOrDefault(x => x.Username == model1.Username);
                    counter = 0;
                    if (model != null)
                        counter = UserProfileCollections.IndexOf(model);
                    if (string.IsNullOrEmpty(model1?.StoryUrl))
                        return;
                    await ThreadFactory.Instance.Start(async () =>
                    {
                        var isVideo = false;
                        var extension = "png";
                        if (model1.Type == "video")
                        {
                            extension = "mp4";
                            isVideo = true;
                        }
                        var fileName = $"{GramStatic.SanitizeFileName(model1.Username)}_Story_{counter}.{extension}";
                        var path = Path.Combine(DownloadPath, "InstaStories");
                        DirectoryUtilities.CreateDirectory(path);
                        var filePath = Path.Combine(path, fileName);
                        if (File.Exists(filePath))
                            File.Delete(filePath);
                        await DownloadFileAsync(isVideo ? model1.VideoUrl : model1.StoryUrl, filePath, "Story");
                    });

                }
            }
            catch { }
            finally
            {
                IsProcessing = false;
            }
        }
        public async Task DownloadFileAsync(string url, string outputPath, string type)
        {
            try
            {
                IsProcessing = true;
                Status = $"Downloading {type}";
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        response.EnsureSuccessStatusCode();

                        Stream contentStream = await response.Content.ReadAsStreamAsync(),
                            fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None);
                        await contentStream.CopyToAsync(fileStream);
                    }
                    Status = "Download Completed";
                    IsProcessing = false;
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                                    "Instagram",
                                    "Instagram User", $"{type} Download", $"Successfully Downloaded At ==> {outputPath}");
                }
            }
            catch (Exception)
            {
                IsProcessing = false;
                Status = string.Empty;
            }
        }
        private bool DownloadMediaCanExecute(object arg)
        {
            return true;
        }
        private bool SearchUserIDCanExecute(object arg)
        {
            return !string.IsNullOrEmpty(InstaUsername);
        }
        private async void SearchUserIDExecute(object obj)
        {
            try
            {
                #region API's
                //var userAPI = "https://api-wh.storiesig.info/api/v1/instagram/userInfo";
                //var url = "https://api-wh.storiesig.info/api/v1/instagram/stories";
                //var userResponse = await GetResponse(InstaUsername, userAPI);
                //var responseBody = await GetResponse(InstaUsername, url);
                #endregion
                IsProcessing = true;
                Status = "Fetching Stories";
                await GetUsersStories(InstaUsername);
                InstaUsername = string.Empty;
                Status = "Fetched Stories";
            }
            catch { }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task GetUsersStories(string instaUsername)
        {
            var Media = await FetchMedia(InstaUsername);
            UpdateUI(Media);
        }

        private async void UpdateUI(StoryCollection story)
        {
            var isValid = await fetcher.IsImageVisibleAsync(story.ProfilePic);
            if (!isValid && !string.IsNullOrEmpty(story.OtherProfile))
                story.ProfilePic = story.OtherProfile;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (!UserProfileCollections.Any(x => x.Username == story.Username))
                {
                    UserProfileCollections.Add(story);
                    if (AutoDownload)
                    {
                        ThreadFactory.Instance.Start(async () =>
                        {
                            DownloadProfileExecute(story);
                            DownloadMediaExecute(story);
                            DownloadHightlightsExecute(story);
                        });
                    }
                }
                else
                {
                    var data = UserProfileCollections.FirstOrDefault(x => x.Username == story.Username);
                    if (data != null)
                    {
                        var index = UserProfileCollections.IndexOf(data);
                        if (index >= 0)
                            UserProfileCollections[index] = story;
                        if (AutoDownload)
                        {
                            ThreadFactory.Instance.Start(async () =>
                            {
                                DownloadProfileExecute(story);
                                DownloadMediaExecute(story);
                                DownloadHightlightsExecute(story);
                            });
                        }
                    }
                }
            });
        }
        public async Task<List<HeighlightDetails>> GetHighlightDetails(InstaHightlight hightlight)
        {
            var heighlightDetails = new List<HeighlightDetails>();
            try
            {
                if (Params is null || Params.Count == 0)
                {
                    var paramDict = new Dictionary<string, string>()
                    {
                        { "XSRF-TOKEN", ".storynavigation.com" },
                        { "laravel_session", ".storynavigation.com" }
                    };
                    var param = await fetcher.GetParam("https://storynavigation.com/", paramDict);
                    Params = param;
                }
                var heighlightAPI = "https://storynavigation.com/get-highlight-stories";
                var jsonBody = $"{{\"highlightId\":\"{hightlight?.Id}\"}}";
                var origin = "https://storynavigation.com";
                var referer = $"https://storynavigation.com/user/{hightlight?.Username}";
                var heighlightResponse = await fetcher.HitRequest(heighlightAPI, Params, jsonBody, origin, referer);
                return new MediaDetailsResponseHandler(heighlightResponse).HeighlightDetails;
            }
            catch { }
            return heighlightDetails;
        }
        private async void DownloadHightlightsExecute(object obj)
        {
            try
            {
                var story = obj as StoryCollection;
                if (story != null && story.Highlights.Count > 0)
                {
                    var counter = 0;
                    foreach (var data in story.Highlights)
                    {
                        if (string.IsNullOrEmpty(data?.CoverUrl) || string.IsNullOrEmpty(data?.Id))
                            continue;
                        var heighlightDetails = await GetHighlightDetails(data);
                        foreach (var data1 in heighlightDetails)
                        {
                            await ThreadFactory.Instance.Start(async () =>
                            {
                                var IsVideo = false;
                                var extension = "jpg";
                                if (data1.Type == "video")
                                {
                                    extension = "mp4";
                                    IsVideo = true;
                                }
                                var id = string.IsNullOrEmpty(data.Id) ? counter.ToString() : data.Id;
                                var fileName = $"{GramStatic.SanitizeFileName(data.Username)}_{id}_{DateTime.Now.Ticks.ToString()}.{extension}";
                                var path = Path.Combine(DownloadPath, "InstaHighlights");
                                DirectoryUtilities.CreateDirectory(path);
                                var filePath = Path.Combine(path, fileName);
                                if (File.Exists(filePath))
                                    File.Delete(filePath);
                                await DownloadFileAsync(IsVideo ? data1.VideoUrl : data1.MediaUrl, filePath, "Highlight");
                            });
                            counter++;
                        }
                    }
                }
                else
                {
                    var data = obj as InstaHightlight;
                    if (string.IsNullOrEmpty(data?.CoverUrl) || string.IsNullOrEmpty(data?.Id))
                        return;
                    var heighlightDetails = await GetHighlightDetails(data);
                    foreach (var data1 in heighlightDetails)
                    {
                        await ThreadFactory.Instance.Start(async () =>
                        {
                            var IsVideo = false;
                            var extension = "jpg";
                            if (data1.Type == "video")
                            {
                                extension = "mp4";
                                IsVideo = true;
                            }
                            var fileName = $"{GramStatic.SanitizeFileName(data.Username)}_{data.Id}_{DateTime.Now.Ticks.ToString()}.{extension}";
                            var path = Path.Combine(DownloadPath, "InstaHighlights");
                            DirectoryUtilities.CreateDirectory(path);
                            var filePath = Path.Combine(path, fileName);
                            if (File.Exists(filePath))
                                File.Delete(filePath);
                            await DownloadFileAsync(IsVideo ? data1.VideoUrl : data1.MediaUrl, filePath, "Highlight");
                        });
                    }
                }
            }
            catch { }
        }

        private async Task<StoryCollection> FetchMedia(string instaUsername)
        {
            var model = new StoryCollection();
            try
            {
                var username = fetcher.GetInstaUsername(instaUsername);
                var paramDict = new Dictionary<string, string>()
                {
                    { "XSRF-TOKEN", ".storynavigation.com" },
                    { "laravel_session", ".storynavigation.com" }
                };
                var param = await fetcher.GetParam("https://storynavigation.com/", paramDict);
                Params = param;
                var userInfoAPI = "https://storynavigation.com/get-user-profile";
                var storyAPI = "https://storynavigation.com/get-user-last-stories";
                var jsonBody = $"{{\"userName\":\"{username}\"}}";
                var origin = "https://storynavigation.com";
                var referer = $"https://storynavigation.com/user/{username}";
                var userInfo = await fetcher.HitRequest(userInfoAPI, param, jsonBody, origin, referer);
                paramDict = new Dictionary<string, string>()
                {
                    { "XSRF-TOKEN", "i.theasmn.com" },
                    { "hacking_panel_session", "i.theasmn.com" }
                };
                var param1 = await fetcher.GetParam("https://i.theasmn.com/", paramDict);
                var userAPI = "https://i.theasmn.com/api/user";
                var userInfoBody = $"{{\"username\":\"{username}\"}}";
                //var userInfoResponse = await fetcher.HitRequest(userAPI, param1, userInfoBody, "https://i.theasmn.com", "https://i.theasmn.com/processing");
                var userInfoResponse = string.Empty;
                var isPrivate = Utilities.GetBetween(userInfo, "\"isPrivate\":", ",");
                var userID = Utilities.GetBetween(userInfo, "\"id\":", ",")?.Replace("\"", "");
                var userBody = $"{{\"userName\":\"{username}\",\"isPrivate\":{isPrivate},\"instagramUserId\":{userID}}}";
                var storyNavigation = await fetcher.HitRequest(storyAPI, param, userBody, origin, referer);
                if (string.IsNullOrEmpty(storyNavigation)||storyNavigation.Contains("\"Server Error\""))
                    storyNavigation = await fetcher.HitStoryOrHighlights("https://anonstories.com/api/v1/story", username);
                var hightlightBody = $"{{\"userName\":\"{username}\",\"userId\":{userID}}}";
                var hightlights = await fetcher.HitRequest("https://storynavigation.com/get-user-highlights", param, hightlightBody, origin, referer);
                if(string.IsNullOrEmpty(hightlights) || hightlights.Contains("\"Server Error\""))
                    hightlights = await fetcher.HitStoryOrHighlights("https://anonstories.com/api/v1/highlights", username);
                var stories = new InstagramStoriesResponseHandler(userInfo, storyNavigation, userInfoResponse, _downloadMediaCommand: DownloadMedia, DownloadHighlight: DownloadHighlight, Highlights: hightlights);
                if (stories?.model?.Stories?.Count == 0 && !stories.IsPrivate)
                {
                    stories.model = await fetcher.GetStoryByBrowser(username, stories.model, DownloadMedia, DownloadHighlight);
                }
                model = stories?.model;

                return model;
            }
            catch (Exception)
            {
            }
            return model;
        }
        #endregion
    }
}