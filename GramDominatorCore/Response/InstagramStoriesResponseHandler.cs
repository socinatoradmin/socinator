using DominatorHouseCore.Utility;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GramDominatorCore.Response
{
    public class SessionModel
    {
        public string key { get; set; }
        public string value { get; set; }
        public string domain { get; set; }
        public string expires { get; set; }
    }
    public class InstagramStoriesResponseHandler: IGResponseHandler
    {
        public StoryCollection model = new StoryCollection();
        public PageInfo pageInfo { get; set; } = new PageInfo();
        public bool IsPrivate { get; set; } = false;
        public InstagramStoriesResponseHandler(string ProfileResponse, string StoryResponse,string OtherProfile, ICommand _downloadMediaCommand,ICommand DownloadHighlight, StorySource storySource=StorySource.Storynavigation
            ,bool IsBrowser=false,string Highlights="")
        {
            Success = false;
            if(string.IsNullOrEmpty(ProfileResponse))
            {
                return;
            }
            try
            {
                if(storySource == StorySource.Storynavigation)
                {
                    if (IsBrowser)
                    {
                        try
                        {
                            var profileJson = Utilities.GetBetween(ProfileResponse, "v-bind:user-info-prop=\"", "v-bind:highlights-prop=\"");
                            profileJson = profileJson?.Replace(" ", "")?.Replace("\n", "")?.Replace("\r\n", "").TrimEnd('\"');
                            var obj = handler.ParseJsonToJObject(profileJson);
                            model.Id = handler.GetJTokenValue(obj, "id");
                            model.Username = handler.GetJTokenValue(obj, "username");
                            if(!string.IsNullOrEmpty(model.Username))
                            {
                                model.ProfileUrl = $"https://www.instagram.com/{model.Username}/";
                            }
                            model.FullName = handler.GetJTokenValue(obj, "fullName");
                            model.Caption = handler.GetJTokenValue(obj, "biography");
                            int.TryParse(handler.GetJTokenValue(obj, "mediaCount"), out int postCount);
                            model.PostCount = postCount;
                            int.TryParse(handler.GetJTokenValue(obj, "followedByCount"), out int followerCount);
                            model.FollowerCount = followerCount;
                            int.TryParse(handler.GetJTokenValue(obj, "followsCount"), out int followingCount);
                            model.FollowingCount = followingCount;
                            bool.TryParse(handler.GetJTokenValue(obj, "isPrivate"), out bool isPrivate);
                            model.IsPrivate = IsPrivate = isPrivate;
                            var profilePic = handler.GetJTokenValue(obj, "profilePicUrl");
                            if (!string.IsNullOrEmpty(profilePic))
                                model.ProfilePic = FromBase64(profilePic);
                        }
                        catch { }
                        try
                        {
                            var storyJson = Utilities.GetBetween(ProfileResponse, "v-bind:last-stories-prop=\"", "v-bind:posts-prop=\"");
                            storyJson = storyJson?.Replace(" ", "")?.Replace("\n", "")?.Replace("\r\n", "").TrimEnd('\"');
                            var stories = handler.GetJArrayElement(storyJson);
                            if (stories != null && stories.HasValues)
                            {
                                var media = new ObservableCollection<StoriesMedia>();
                                foreach (var story in stories)
                                {
                                    var mediaModel = new StoriesMedia();
                                    mediaModel.Username = model.Username;
                                    mediaModel.Type = handler.GetJTokenValue(story, "type");
                                    mediaModel.StoryUrl = FromBase64(handler.GetJTokenValue(story, "thumbnailUrl"));
                                    mediaModel.VideoUrl = FromBase64(handler.GetJTokenValue(story, "videoUrl"));
                                    var created = handler.GetJTokenValue(story, "createdTime")?.Trim();
                                    if (!string.IsNullOrEmpty(created))
                                    {
                                        var splitted = created.Split(':');
                                        int.TryParse(splitted[0], out int h);
                                        int.TryParse(splitted[1], out int m);
                                        int.TryParse(splitted[2], out int s);
                                        var dateTime = DateTime.Now.AddHours(-h).AddMinutes(-m).AddSeconds(-s);
                                        mediaModel.CreatedAt = dateTime.ToString("dd-MM-yyyy hh:mm:ss tt");
                                    }
                                    mediaModel.DownloadStoryCommand = _downloadMediaCommand;
                                    media.Add(mediaModel);
                                }
                                model.Stories = media;
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            var node = "accountInfo";
                            if (!string.IsNullOrEmpty(ProfileResponse) && ProfileResponse.Contains("\"found\":false"))
                            {
                                ProfileResponse = StoryResponse;
                                if(!string.IsNullOrEmpty(ProfileResponse))
                                    node = "user_info";
                            }
                            var obj = handler.ParseJsonToJObject(ProfileResponse);
                            model.Id = handler.GetJTokenValue(obj, node, "id");
                            model.Username = handler.GetJTokenValue(obj, node, "username");
                            if (!string.IsNullOrEmpty(model.Username))
                            {
                                model.ProfileUrl = $"https://www.instagram.com/{model.Username}/";
                            }
                            model.FullName = handler.GetJTokenValue(obj, node, "fullName");
                            model.Caption = handler.GetJTokenValue(obj, node, "biography");
                            int.TryParse(handler.GetJTokenValue(obj, node, "mediaCount"), out int postCount);
                            model.PostCount = postCount;
                            int.TryParse(handler.GetJTokenValue(obj, node, "followedByCount"), out int followerCount);
                            model.FollowerCount = followerCount;
                            int.TryParse(handler.GetJTokenValue(obj, node, "followsCount"), out int followingCount);
                            model.FollowingCount = followingCount;
                            bool.TryParse(handler.GetJTokenValue(obj, node, "isPrivate"), out bool isPrivate);
                            model.IsPrivate = IsPrivate = isPrivate;
                            var profilePic = handler.GetJTokenValue(obj, node, "profilePicUrl");
                            if (!string.IsNullOrEmpty(profilePic))
                                model.ProfilePic = FromBase64(profilePic);
                            UpdateMissingDetails(OtherProfile, model);
                        }
                        catch { }

                        try
                        {
                            var obj = handler.ParseJsonToJObject(StoryResponse);
                            var stories = handler.GetJArrayElement(handler.GetJTokenValue(obj, "lastStories"));
                            stories = stories is null || !stories.HasValues ? handler.GetJArrayElement(handler.GetJTokenValue(obj, "stories")) : stories;
                            if (stories != null && stories.HasValues)
                            {
                                var media = new ObservableCollection<StoriesMedia>();
                                foreach (var story in stories)
                                {
                                    var mediaModel = new StoriesMedia();
                                    mediaModel.Username = model.Username;
                                    mediaModel.Type = handler.GetJTokenValue(story, "type");
                                    if (string.IsNullOrEmpty(mediaModel.Type))
                                        mediaModel.Type = handler.GetJTokenValue(story, "media_type");
                                    mediaModel.StoryUrl = FromBase64(handler.GetJTokenValue(story, "thumbnailUrl"));
                                    if (string.IsNullOrEmpty(mediaModel.StoryUrl))
                                        mediaModel.StoryUrl = handler.GetJTokenValue(story, "thumbnail");
                                    mediaModel.VideoUrl = FromBase64(handler.GetJTokenValue(story, "videoUrl"));
                                    if (string.IsNullOrEmpty(mediaModel.VideoUrl))
                                        mediaModel.VideoUrl = handler.GetJTokenValue(story, "source");
                                    var created = handler.GetJTokenValue(story, "createdTime")?.Trim();
                                    if (!string.IsNullOrEmpty(created))
                                    {
                                        var splitted = created.Split(':');
                                        int.TryParse(splitted[0], out int h);
                                        int.TryParse(splitted[1], out int m);
                                        int.TryParse(splitted[2], out int s);
                                        var dateTime = DateTime.Now.AddHours(-h).AddMinutes(-m).AddSeconds(-s);
                                        mediaModel.CreatedAt = mediaModel.StoryDate = dateTime.ToString("dd-MM-yyyy hh:mm:ss tt");
                                    }
                                    else
                                    {
                                        created = handler.GetJTokenValue(story, "taken_at");
                                        mediaModel.CreatedAt = mediaModel.StoryDate = created;
                                    }
                                    mediaModel.DownloadStoryCommand = _downloadMediaCommand;
                                    media.Add(mediaModel);
                                }
                                model.Stories = media;
                            }
                            var highlights = handler.GetJArrayElement(Highlights);
                            highlights = highlights is null || !highlights.HasValues ? handler.GetJArrayElement(handler.GetJTokenValue(handler.ParseJsonToJObject(Highlights), "highlights")): highlights;
                            if(highlights != null && highlights.HasValues)
                            {
                                var highlightList = new ObservableCollection<InstaHightlight>();
                                foreach (var highlight in highlights)
                                {
                                    var highlightModel = new InstaHightlight();
                                    highlightModel.Username = model.Username;
                                    var id = handler.GetJTokenValue(highlight, "id");
                                    if(string.IsNullOrEmpty(id))
                                        id = handler.GetJTokenValue(highlight, "node", "id");
                                    highlightModel.Id = id;
                                    var title = handler.GetJTokenValue(highlight, "title");
                                    if (string.IsNullOrEmpty(title))
                                        title = handler.GetJTokenValue(highlight, "node", "title");
                                    highlightModel.Title = title;
                                    var mediaUrl = FromBase64(handler.GetJTokenValue(highlight, "imageThumbnail"));
                                    if (string.IsNullOrEmpty(mediaUrl))
                                        mediaUrl = handler.GetJTokenValue(highlight,"node", "cover_media", "thumbnail_src");
                                    highlightModel.CoverUrl = mediaUrl;
                                    highlightModel.DownloadStoryCommand = DownloadHighlight;
                                    if (!highlightList.Any(x=>x.Id == highlightModel.Id))
                                        highlightList.Add(highlightModel);
                                }
                                model.Highlights = highlightList;
                            }
                        }
                        catch { }
                    }
                    Success = model!= null &&  model.Stories.Count > 0;
                }
            }
            catch
            {

            }
        }

        private void UpdateMissingDetails(string otherProfile, StoryCollection model)
        {
            try
            {
                var obj = handler.ParseJsonToJObject(otherProfile);
                model.OtherProfile = handler.GetJTokenValue(obj, "data", "image");
                if (string.IsNullOrEmpty(model.ProfilePic))
                    model.ProfilePic = model.OtherProfile;
                if (string.IsNullOrEmpty(model.Username))
                    model.Username = handler.GetJTokenValue(obj, "data", "username");
                if (!string.IsNullOrEmpty(model.Username))
                {
                    model.ProfileUrl = $"https://www.instagram.com/{model.Username}/";
                }
                if (string.IsNullOrEmpty(model.Caption))
                    model.Caption = handler.GetJTokenValue(obj, "data", "bio");
                if (string.IsNullOrEmpty(model.FullName))
                    model.FullName = handler.GetJTokenValue(obj, "data", "first_name") + " "+ handler.GetJTokenValue(obj, "data", "last_name");
                if(model.PostCount == 0)
                {
                    int.TryParse(handler.GetJTokenValue(obj, "data", "media_count"), out int media_count);
                    model.PostCount = media_count;
                }
                if (model.FollowerCount == 0)
                {
                    int.TryParse(handler.GetJTokenValue(obj, "data", "followers"), out int followers);
                    model.FollowerCount = followers;
                }
                if (model.FollowingCount == 0)
                {
                    int.TryParse(handler.GetJTokenValue(obj, "data", "following"), out int following);
                    model.FollowingCount = following;
                }

            }
            catch { }
        }

        private string FromBase64(string base64string)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(base64string));
            }
            catch { return null; }
        }
    }
    public class PageInfo
    {
        public string EndCursor { get; set; }
        public bool HasNextPage { get; set; }
        public string StartCursor { get; set; }
        public bool HasPreviousPage { get; set; }
    }
    public class MediaDetailsResponseHandler : IGResponseHandler
    {
        public List<HeighlightDetails> HeighlightDetails = new List<HeighlightDetails>();
        public MediaDetailsResponseHandler(string Response)
        {
            try
            {
                var list = handler.GetJArrayElement(Response);
                if (list != null && list.HasValues)
                {
                    foreach (var item in list)
                    {
                        var model = new HeighlightDetails
                        {
                            VideoUrl = FromBase64(handler.GetJTokenValue(item, "videoUrl")),
                            MediaUrl = FromBase64(handler.GetJTokenValue(item, "thumbnailUrl")),
                            Type = handler.GetJTokenValue(item, "type"),
                            Created = handler.GetJTokenValue(item, "createdTime")
                        };
                        if (!string.IsNullOrEmpty(model.MediaUrl) && !HeighlightDetails.Any(x => x.MediaUrl != model.MediaUrl))
                        {
                            HeighlightDetails.Add(model);
                        }
                    }
                }
            }
            catch { }
        }
        private string FromBase64(string base64string)
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(base64string));
            }
            catch { return null; }
        }
    }
}
