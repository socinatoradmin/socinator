using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GramDominatorCore.Response
{
    public class HashTagFeedIgResponseHandler : IGResponseHandler
    {
        public HashTagFeedIgResponseHandler(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            
            if (response.Response.Contains(",\"data\":{\"id\":"))
            {
                GetPostList(response.Response);
                GetPaginationData(response?.Response);
                if (RankedItems.Count <= 0)
                    Success = false;
                return;
            }
            else if (response.Response.Contains("\"media_grid\":{\"refinements\""))
            {
                GetPostList(response.Response);
                GetPaginationData(response?.Response);
                if (RankedItems.Count <= 0)
                    Success = false;
                return;
            }
        }

        private void GetPaginationData(string response)
        {
            {
                var obj = handler.ParseJsonToJObject(response);
                MaxId = handler.GetJTokenValue(obj, "media_grid", "next_max_id");
                NextMediaId = handler.GetJTokenValue(obj, "media_grid", "rank_token");
                bool.TryParse(handler.GetJTokenValue(obj, "media_grid", "has_more"), out bool has_more);
                MoreAvailable = has_more;
            }
        }

        public List<InstagramPost> Items { get; set; } = new List<InstagramPost>();

        public string MaxId { get; set; }

        public bool MoreAvailable { get; set; }

        public List<InstagramPost> RankedItems { get; set; } = new List<InstagramPost>();

        public string NextMediaId { get; set; }

        public string message { get; set; }

        public List<InstagramPost> GetMedia(JToken jtoken1)
        {
            var instagramPostList = new List<InstagramPost>();
            var instagramPost1 = new InstagramPost();
            var User = handler.GetJTokenOfJToken(jtoken1, "media", "user");
            bool.TryParse(handler.GetJTokenValue(User, "has_anonymous_profile_picture"), out bool hasProfilePic);
            var instagramUser =
                new InstagramUser(handler.GetJTokenValue(User, "pk"),
                    handler.GetJTokenValue(User, "username"))
                {
                    HasAnonymousProfilePicture = hasProfilePic,
                    ProfilePicUrl = handler.GetJTokenValue(User, "profile_pic_url"),
                    FullName = handler.GetJTokenValue(User, "full_name")
                };
            bool.TryParse(handler.GetJTokenValue(User, "is_private"), out bool isPrivate);
            bool.TryParse(handler.GetJTokenValue(User, "is_verified"), out bool IsVerified);
            instagramUser.IsVerified = IsVerified;
            instagramUser.IsPrivate = isPrivate;
            instagramPost1.User = instagramUser;
            int.TryParse(handler.GetJTokenValue(jtoken1, "media", "taken_at"),out int takenAt);
            instagramPost1.TakenAt = takenAt;
            instagramPost1.Pk = handler.GetJTokenValue(jtoken1,"media","pk");
            instagramPost1.Id = handler.GetJTokenValue(jtoken1,"media","id");
            int.TryParse(handler.GetJTokenValue(jtoken1, "media", "media_type"), out int mediaType);
            instagramPost1.MediaType = (MediaType)mediaType;
            try
            {
                bool.TryParse(handler.GetJTokenValue(jtoken1, "data", "photo_of_you"), out bool photoOfYou);
                if(!photoOfYou)
                    bool.TryParse(handler.GetJTokenValue(jtoken1, "media", "photo_of_you"), out photoOfYou);
                instagramPost1.PhotoOfYou = photoOfYou;
            }
            catch
            {
                
            }
            int.TryParse(handler.GetJTokenValue(jtoken1, "media", "comment_count"), out int commentCount);
            instagramPost1.CommentCount = commentCount;
            var instagramPost2 = instagramPost1;
            var usertagJtoken = handler.GetJTokenOfJToken(jtoken1,"media", "usertags");
            if (usertagJtoken != null && usertagJtoken.HasValues)
            {

                foreach (var jtoken2 in handler.GetJTokenOfJToken(usertagJtoken, "in"))
                {
                    var userstag = new InstagramUser();
                    var IgUser = handler.GetJTokenOfJToken(jtoken2, "user");
                    userstag.Pk = handler.GetJTokenValue(IgUser,"pk");
                    userstag.UserId = handler.GetJTokenValue(IgUser,"pk");
                    userstag.Username = handler.GetJTokenValue(IgUser,"username");
                    var jtokenFullName = handler.GetJTokenValue(IgUser, "full_name");
                    userstag.FullName = !string.IsNullOrEmpty(jtokenFullName) ? jtokenFullName : string.Empty;
                    var jtokenProfilePic = handler.GetJTokenValue(IgUser, "profile_pic_url");
                    userstag.ProfilePicUrl = !string.IsNullOrEmpty(jtokenProfilePic) ? jtokenProfilePic : string.Empty;
                    bool.TryParse(handler.GetJTokenValue(IgUser, "is_private"), out isPrivate);
                    userstag.IsPrivate = isPrivate;
                    instagramPost1.UserTags.Add(userstag);
                }
            }
            instagramPost2.DeviceTimestamp = handler.GetJTokenValue(jtoken1, "media", "device_timestamp");
            instagramPost2.Code = handler.GetJTokenValue(jtoken1, "media","code");
            instagramPost2.Caption = handler.GetJTokenValue(jtoken1, "media", "caption", "text");
            int.TryParse(handler.GetJTokenValue(jtoken1, "media", "like_count"), out int likeCount);
            instagramPost2.LikeCount = likeCount;
            bool.TryParse(handler.GetJTokenValue(jtoken1, "media", "has_liked"), out bool hasLiked);
            instagramPost2.HasLiked = hasLiked;
            if (instagramPost2.MediaType == MediaType.Album)
            {
                foreach (var jtoken2 in handler.GetJArrayElement(handler.GetJTokenValue(jtoken1,"media", "carousel_media")))
                {
                    int.TryParse(handler.GetJTokenValue(jtoken2, "original_width"), out int width);
                    int.TryParse(handler.GetJTokenValue(jtoken2, "original_height"), out int height);
                    int.TryParse(handler.GetJTokenValue(jtoken2, "media_type"), out mediaType);
                    CarouselMedia carouselMedia = new CarouselMedia()
                    {
                        Width = width,
                        Height = height,
                        MediaType = mediaType,
                        Id = handler.GetJTokenValue(jtoken2,"id")
                    };
                    foreach (var jtoken3 in handler.GetJArrayElement(handler.GetJTokenValue(jtoken2, "image_versions2", "candidates")))
                    {
                        int.TryParse(handler.GetJTokenValue(jtoken3, "width"), out width);
                        int.TryParse(handler.GetJTokenValue(jtoken3, "height"), out height);
                        carouselMedia.Images.Add(new InstaGramImage()
                        {
                            Width = width,
                            Height = height,
                            Url = handler.GetJTokenValue(jtoken3,"url")
                        });
                    }
                    if (carouselMedia.MediaType == 2)
                    {
                        double.TryParse(handler.GetJTokenValue(jtoken2, "video_duration"), out double videoDuration);
                        carouselMedia.VideoDuration = videoDuration;
                        int.TryParse(handler.GetJTokenValue(jtoken2, "video_versions",0, "width"), out width);
                        int.TryParse(handler.GetJTokenValue(jtoken2, "video_versions",0, "height"), out height);
                        carouselMedia.Video = new InstaGramImage()
                        {
                            Width = width,
                            Height = height,
                            Url = handler.GetJTokenValue(jtoken2, "video_versions",0,"url")
                        };
                    }
                    instagramPost2.Album.Add(carouselMedia);
                }
            }
            else
            {
                foreach (JToken jtoken2 in handler.GetJArrayElement(handler.GetJTokenValue(jtoken1,"media", "image_versions2", "candidates")))
                {
                    int.TryParse(handler.GetJTokenValue(jtoken2, "width"), out int width);
                    int.TryParse(handler.GetJTokenValue(jtoken2, "height"), out int height);
                    instagramPost2.Images.Add(new InstaGramImage()
                    {
                        Width = width,
                        Height = height,
                        Url = handler.GetJTokenValue(jtoken2,"url")
                    });
                }
                    
                if (instagramPost2.MediaType == MediaType.Video)
                {
                    int.TryParse(handler.GetJTokenValue(jtoken1, "media", "view_count"), out int viewCount);
                    instagramPost2.ViewCount = viewCount;
                    double.TryParse(handler.GetJTokenValue(jtoken1, "media", "video_duration"), out double videoDuration);
                    instagramPost2.VideoDuration = videoDuration;
                    int.TryParse(handler.GetJTokenValue(jtoken1, "media", "video_versions",0, "width"), out int width);
                    int.TryParse(handler.GetJTokenValue(jtoken1, "media", "video_versions", 0, "height"), out int height);
                    instagramPost2.Video = new InstaGramImage()
                    {
                        Width = width,
                        Height = height,
                        Url = handler.GetJTokenValue(jtoken1,"media", "video_versions",0,"url")
                    };
                }
            }
            bool.TryParse(handler.GetJTokenValue(jtoken1,"media", "comments_disabled"), out bool commentDisabled);
            instagramPost2.CommentsDisabled = commentDisabled;
            int.TryParse(handler.GetJTokenValue(jtoken1, "media", "comment_count"), out commentCount);
            instagramPost2.CommentCount = commentCount;
            bool.TryParse(handler.GetJTokenValue(jtoken1, "media", "comment_likes_enabled"), out bool commentLikeEnabled);
            instagramPost2.CommentLikesEnabled = commentLikeEnabled;
            var locationToken = handler.GetJTokenOfJToken(jtoken1, "media", "location");
            if (locationToken != null && locationToken.HasValues)
            {
                instagramPost2.HasLocation = true;
                instagramPost2.Location = new Location()
                {
                    Name = handler.GetJTokenValue(locationToken,"name")
                };
                instagramPost2.Location.City = handler.GetJTokenValue(locationToken, "city");
                var Latitude = handler.GetJTokenValue(locationToken, "lat");
                if (!string.IsNullOrEmpty(Latitude))
                {
                    instagramPost2.HasDetailedLocation = true;
                    float.TryParse(Latitude, out float lat);
                    float.TryParse(handler.GetJTokenValue(locationToken, "lng"), out float lang);
                    instagramPost2.Location.Lat = lat;
                    instagramPost2.Location.Lng = lang;
                }
            }
            instagramPost2.IsAd = !string.IsNullOrEmpty(handler.GetJTokenValue(jtoken1, "media", "ad_header_style"));
            instagramPostList.Add(instagramPost2);
            return instagramPostList;
        }

        public void GetPostList(string response)
        {
            try
            {
                var jObject = handler.ParseJsonToJObject(response);
                if (response.Contains("\"media_grid\":{\"refinements\""))
                {
                        var typeSections = handler.GetJTokenOfJToken(jObject, "media_grid", "sections");
                        foreach (var typeSection in typeSections)
                        {
                            var medias = handler.GetJTokenOfJToken(typeSection, "layout_content", "medias");
                            medias = medias is null || !medias.HasValues ? handler.GetJTokenOfJToken(typeSection, "layout_content", "fill_items") : medias;
                            foreach (var media in medias)
                            {
                                var mediaDetails = GetMedia(media);
                                RankedItems.AddRange(mediaDetails);
                            }
                        }
                }
                else
                {
                    var sections = new string[] { "top", "recent" };
                    foreach (var section in sections)
                    {
                        var typeSections = handler.GetJTokenOfJToken(jObject, "data", section, "sections");
                        foreach (var typeSection in typeSections)
                        {
                            var medias = handler.GetJTokenOfJToken(typeSection, "layout_content", "medias");
                            medias = medias is null || !medias.HasValues ? handler.GetJTokenOfJToken(typeSection, "layout_content", "fill_items") : medias;
                            foreach (var media in medias)
                            {
                                var mediaDetails = GetMedia(media);
                                RankedItems.AddRange(mediaDetails);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
