using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDEnums;
using GramDominatorCore.GDModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GramDominatorCore.GDUtility
{
    [Localizable(false)]
    public static class InstagramHelper
    {
        private static readonly JsonJArrayHandler handler = JsonJArrayHandler.GetInstance;
        //public static string GenerateDeviceId()
        //{
        //    return "android-" + RandomUtilties.RandomString(5).GetMd5().Substring(0, 16);
        //}

        //public static string ConvertInstagramNewline(this string value)
        //{
        //    return value?.Replace("\r\n", "\n") ?? string.Empty;
        //}

        //public static string GenerateUploadId()
        //{
        //    string source = Math.Round(DateTimeUtilities.GetEpochTimeMicroSecs() - DateTime.UtcNow.Date.GetStartOfWeek(DayOfWeek.Monday).ConvertToEpoch(), 6).ToString(CultureInfo.InvariantCulture);
        //    return
        //        $"{(object) string.Join(string.Empty, source.Where(char.IsDigit))}{(object) RandomUtilties.GetRandomNumber(999, 1).ToString().PadLeft(3, '0')}";
        //}

        public static string GenerateUserBreadcrumb(int size)
        {
            byte[] bytes = Encoding.UTF8.GetBytes("iN4$aGr0m");
            long num1 = (long)(DateTimeUtilities.GetEpochTimeMicroSecs() * 1000.0);
            int num2 = RandomUtilties.GetRandomNumber(3, 2) * 1000 + size * RandomUtilties.GetRandomNumber(20, 15) * 100;
            int num3 = (int)Math.Round(size / (double)RandomUtilties.GetRandomNumber(3, 2));
            if (num3 == 0)
                num3 = 1;
            string str = $"{(object)size} {(object)num2} {(object)num3} {(object)num1}";
            return
                $"{(object)Convert.ToBase64String(StringHelper.GetSha256Raw(str, bytes))}\n{(object)Convert.ToBase64String(Encoding.UTF8.GetBytes(str))}\n";
        }

        public static string GetCodeFromId(this string mediaId)
        {
            ulong num = Convert.ToUInt64(mediaId);
            char[] charArray = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_".ToCharArray();
            string str = string.Empty;
            while (num > 0UL)
            {
                ulong index = num % 64UL;
                num = (num - index) / 64UL;
                str = charArray[index].ToString() + str;
            }
            return str;
        }

        public static string GetCodeFromUrl(this string url)
        {
            string urls = StringHelper.GetRegexPatern("(?<=(https:\\/\\/www.instagram.com\\/p\\/)).{10,11}", url);
            return urls;
        }

        public static string GetIdFromCode(this string mediaCode)
        {
            Dictionary<char, int> alphabet = new Dictionary<char, int>()
            {
                {
                    '-',
                    62
                },
                {
                    '1',
                    53
                },
                {
                    '0',
                    52
                },
                {
                    '3',
                    55
                },
                {
                    '2',
                    54
                },
                {
                    '5',
                    57
                },
                {
                    '4',
                    56
                },
                {
                    '7',
                    59
                },
                {
                    '6',
                    58
                },
                {
                    '9',
                    61
                },
                {
                    '8',
                    60
                },
                {
                    'A',
                    0
                },
                {
                    'C',
                    2
                },
                {
                    'B',
                    1
                },
                {
                    'E',
                    4
                },
                {
                    'D',
                    3
                },
                {
                    'G',
                    6
                },
                {
                    'F',
                    5
                },
                {
                    'I',
                    8
                },
                {
                    'H',
                    7
                },
                {
                    'K',
                    10
                },
                {
                    'J',
                    9
                },
                {
                    'M',
                    12
                },
                {
                    'L',
                    11
                },
                {
                    'O',
                    14
                },
                {
                    'N',
                    13
                },
                {
                    'Q',
                    16
                },
                {
                    'P',
                    15
                },
                {
                    'S',
                    18
                },
                {
                    'R',
                    17
                },
                {
                    'U',
                    20
                },
                {
                    'T',
                    19
                },
                {
                    'Y',
                    24
                },
                {
                    'W',
                    22
                },
                {
                    'V',
                    21
                },
                {
                    'X',
                    23
                },
                {
                    'Z',
                    25
                },
                {
                    '_',
                    63
                },
                {
                    'a',
                    26
                },
                {
                    'c',
                    28
                },
                {
                    'b',
                    27
                },
                {
                    'e',
                    30
                },
                {
                    'd',
                    29
                },
                {
                    'g',
                    32
                },
                {
                    'f',
                    31
                },
                {
                    'i',
                    34
                },
                {
                    'h',
                    33
                },
                {
                    'k',
                    36
                },
                {
                    'j',
                    35
                },
                {
                    'm',
                    38
                },
                {
                    'l',
                    37
                },
                {
                    'o',
                    40
                },
                {
                    'n',
                    39
                },
                {
                    'q',
                    42
                },
                {
                    'p',
                    41
                },
                {
                    's',
                    44
                },
                {
                    'r',
                    43
                },
                {
                    'u',
                    46
                },
                {
                    't',
                    45
                },
                {
                    'w',
                    48
                },
                {
                    'v',
                    47
                },
                {
                    'y',
                    50
                },
                {
                    'x',
                    49
                },
                {
                    'z',
                    51
                }
            };
            return mediaCode.Aggregate(0UL,
                (current, c) => Convert.ToUInt64(alphabet[c]) + current * 64UL).ToString();
        }

        private static DirectMessageType GetMessageType(this string messageType)
        {
            DirectMessageType directMessageType;

            switch (messageType)
            {
                case "text":
                    directMessageType = DirectMessageType.Text;
                    break;
                case "media":
                    directMessageType = DirectMessageType.Media;
                    break;
                case "link":
                    directMessageType = DirectMessageType.Link;
                    break;
                case "like":
                    directMessageType = DirectMessageType.Like;
                    break;
                default:
                    throw new ArgumentException(messageType);
            }

            return directMessageType;
        }

        public static MessageDetails GetMessageDetails(this JToken jToken)
        {
            try
            {
                MessageDetails messageDetails = new MessageDetails
                {
                    ItemId = jToken["item_id"].ToString(),
                    Timestamp = jToken["timestamp"].ToString(),
                    UserId = jToken["user_id"].ToString(),
                    ClientContext = jToken["client_context"]?.ToString(),
                    MessageType = jToken["item_type"].ToString().GetMessageType()
                };


                switch (messageDetails.MessageType)
                {
                    case DirectMessageType.Text:
                        messageDetails.MessageText = jToken["text"].ToString();
                        break;
                    case DirectMessageType.Media:
                        messageDetails.MediaMessageDetails = jToken["media"].GetMediaMessageDetails();
                        break;
                    case DirectMessageType.Link:
                        messageDetails.LinkMessageDetails = jToken["link"].GetLinkMessageDetails();
                        break;
                    case DirectMessageType.Like:
                        messageDetails.LikeMessageDetails = GetLikeMessageDetails();
                        break;
                    default:
                        throw new ArgumentException("Error in fetching proper MessageType");
                }

                return messageDetails;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                throw new Exception("Error in getting MessageDetails");
            }
        }

        private static MediaMessageDetails GetMediaMessageDetails(this JToken jTokenMedia)
        {
            try
            {
                MediaMessageDetails mediaMessageDetails = new MediaMessageDetails();
                List<MediaCandidate> lstMediaCandidates = new List<MediaCandidate>();

                foreach (var candidates in jTokenMedia["image_versions2"]["candidates"].ToArray())
                {
                    try
                    {
                        MediaCandidate mediaCandidate = new MediaCandidate
                        {
                            Width = Convert.ToInt32(candidates["width"]),
                            Height = Convert.ToInt32(candidates["height"]),
                            ImageUrl = candidates["url"].ToString()
                        };

                        lstMediaCandidates.Add(mediaCandidate);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                mediaMessageDetails.LstMediaCandidates = lstMediaCandidates;
                mediaMessageDetails.OriginalWidth = Convert.ToInt32(jTokenMedia["original_width"]);
                mediaMessageDetails.OriginalHeight = Convert.ToInt32(jTokenMedia["original_height"]);
                mediaMessageDetails.MediaType = (MediaType)Convert.ToInt32(jTokenMedia["media_type"].ToString());

                return mediaMessageDetails;
            }
            catch (Exception)
            {
                throw new Exception("Error in getting MediaMessageDetails");
            }
        }

        private static LinkMessageDetails GetLinkMessageDetails(this JToken jTokenLink)
        {
            try
            {
                LinkMessageDetails linkMessageDetails = new LinkMessageDetails
                {
                    MessageText = jTokenLink["text"].ToString()
                };

                LinkMessageContext linkMessageContext = new LinkMessageContext();
                JToken tokenLinkContext = jTokenLink["link_context"];

                linkMessageContext.LinkUrl = tokenLinkContext["link_url"].ToString();
                linkMessageContext.LinkTitle = tokenLinkContext["link_title"].ToString();
                linkMessageContext.LinkSummary = tokenLinkContext["link_summary"].ToString();
                linkMessageContext.LinkImageUrl = tokenLinkContext["link_image_url"].ToString();

                linkMessageDetails.LinkMessageContext = linkMessageContext;

                return linkMessageDetails;
            }
            catch (Exception)
            {
                throw new Exception("Error in getting LinkMessageDetails");
            }
        }

        private static LikeMessageDetails GetLikeMessageDetails()
        {
            try
            {
                LikeMessageDetails likeMessageDetails = new LikeMessageDetails { IsLiked = true, Like = "❤️" };

                return likeMessageDetails;
            }
            catch (Exception)
            {
                throw new Exception("Error in getting LikeMessageDetails");
            }
        }

        public static InstagramUser GetInstagramUser(this JToken jToken)
        {
            try
            {
                InstagramUser instagramUser = new InstagramUser(jToken["pk"].ToString(),
                    jToken["username"].ToString())
                {
                    HasAnonymousProfilePicture =
                        Convert.ToBoolean(jToken["has_anonymous_profile_picture"].ToString()),
                    ProfilePicUrl = jToken["profile_pic_url"].ToString(),
                    FullName = jToken["full_name"].ToString()
                };

                int isPrivateIndex = Convert.ToBoolean(jToken["is_private"].ToString()) ? 1 : 0;
                instagramUser.IsPrivate = isPrivateIndex != 0;
                int isVerifiedIndex = Convert.ToBoolean(jToken["is_private"].ToString()) ? 1 : 0;
                instagramUser.IsVerified = isVerifiedIndex != 0;

                return instagramUser;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Failed to retrieve Instagram User data");
                ex.DebugLog();
                throw;
            }
        }

        public static IEnumerable<InstagramPost> GetImageData(this JArray array, bool IsUnique = false, bool IsLocationPost = false)
        {
            try
            {
                List<InstagramPost> instagramPostList = new List<InstagramPost>();
                foreach (JToken token in array)
                {
                    try
                    {
                        JToken jtoken1;
                        if (IsUnique)
                            jtoken1 = handler.GetJTokenOfJToken(token, "node");
                        else
                            jtoken1 = token;
                        if (jtoken1.HasValues)
                        {
                            InstagramPost instagramPost1 = new InstagramPost();
                            var fullname = handler.GetJTokenValue(jtoken1, "owner", "full_name");
                            fullname = string.IsNullOrEmpty(fullname) ? handler.GetJTokenValue(jtoken1, "user", "full_name") : fullname;
                            InstagramUser instagramUser =
                                new InstagramUser(handler.GetJTokenValue(jtoken1, "user", "pk"),
                                   handler.GetJTokenValue(jtoken1, "user", "username"))
                                {
                                    ProfilePicUrl = handler.GetJTokenValue(jtoken1, "user", "profile_pic_url"),
                                    FullName = string.IsNullOrEmpty(fullname) ? handler.GetJTokenValue(jtoken1, "owner", "username") : fullname
                                };
                            bool.TryParse(handler.GetJTokenValue(jtoken1, "user", "is_private"), out bool Isprivate);
                            instagramUser.IsPrivate = Isprivate;
                            instagramPost1.User = instagramUser;
                            int.TryParse(handler.GetJTokenValue(jtoken1, "taken_at"), out int takenAt);
                            instagramPost1.TakenAt = takenAt;
                            instagramPost1.Pk = handler.GetJTokenValue(jtoken1, "pk");
                            instagramPost1.Id = handler.GetJTokenValue(jtoken1, "id");
                            int.TryParse(handler.GetJTokenValue(jtoken1, "media_type"), out int mediaType);
                            instagramPost1.MediaType = (MediaType)mediaType;
                            instagramPost1.ProductType = handler.GetJTokenValue(jtoken1, "product_type");
                            var usertagJtoken = handler.GetJTokenOfJToken(jtoken1, "usertags");
                            if (usertagJtoken != null && usertagJtoken.HasValues)
                            {

                                foreach (var jtoken2 in handler.GetJArrayElement(handler.GetJTokenValue(usertagJtoken, "in")))
                                {
                                    InstagramUser userstag = new InstagramUser();
                                    userstag.Pk = handler.GetJTokenValue(jtoken2, "user", "pk");
                                    userstag.UserId = handler.GetJTokenValue(jtoken2, "user", "pk");
                                    userstag.Username = handler.GetJTokenValue(jtoken2, "user", "username");
                                    userstag.FullName = handler.GetJTokenValue(jtoken2, "user", "full_name");
                                    userstag.ProfilePicUrl = handler.GetJTokenValue(jtoken2, "user", "profile_pic_url");
                                    bool.TryParse(handler.GetJTokenValue(jtoken2, "user", "is_private"), out Isprivate);
                                    userstag.IsPrivate = Isprivate;
                                    instagramPost1.UserTags.Add(userstag);
                                }
                            }

                            InstagramPost instagramPost2 = instagramPost1;
                            instagramPost2.DeviceTimestamp = handler.GetJTokenValue(jtoken1, "device_timestamp");
                            instagramPost2.Code = handler.GetJTokenValue(jtoken1, "code");
                            instagramPost2.Caption = handler.GetJTokenValue(jtoken1, "caption", "text");
                            int.TryParse(handler.GetJTokenValue(jtoken1, "like_count"), out int likeCount);
                            bool.TryParse(handler.GetJTokenValue(jtoken1, "has_liked"), out bool hasLiked);
                            instagramPost2.LikeCount = likeCount;
                            instagramPost2.HasLiked = hasLiked;
                            if (instagramPost2.MediaType == MediaType.Album)
                            {
                                foreach (JToken jtoken2 in handler.GetJArrayElement(handler.GetJTokenValue(jtoken1, "carousel_media")))
                                {
                                    int.TryParse(handler.GetJTokenValue(jtoken2, "original_width"), out int width);
                                    int.TryParse(handler.GetJTokenValue(jtoken2, "original_height"), out int height);
                                    int.TryParse(handler.GetJTokenValue(jtoken2, "media_type"), out int media_Type);
                                    CarouselMedia carouselMedia = new CarouselMedia()
                                    {
                                        Width = width,
                                        Height = height,
                                        MediaType = media_Type,
                                        Id = handler.GetJTokenValue(jtoken2, "id")
                                    };
                                    foreach (JToken jtoken3 in handler.GetJArrayElement(handler.GetJTokenValue(jtoken2, "image_versions2", "candidates")))
                                    {
                                        int.TryParse(handler.GetJTokenValue(jtoken3, "width"), out width);
                                        int.TryParse(handler.GetJTokenValue(jtoken3, "height"), out height);
                                        carouselMedia.Images.Add(new InstaGramImage()
                                        {
                                            Width = width,
                                            Height = height,
                                            Url = handler.GetJTokenValue(jtoken3, "url")
                                        });
                                    }
                                    if (carouselMedia.MediaType == 2)
                                    {
                                        var VideoDashManifest = handler.GetJTokenValue(jtoken2, "video_dash_manifest");
                                        var durationString = Utilities.GetBetween(VideoDashManifest, "mediaPresentationDuration=\"", "\"")?.Replace("PT", "")?.Replace("S", "");
                                        double.TryParse(durationString, out double duration);
                                        carouselMedia.VideoDuration = duration;
                                        int.TryParse(handler.GetJTokenValue(jtoken2, "video_versions", 0, "width"), out width);
                                        int.TryParse(handler.GetJTokenValue(jtoken2, "video_versions", 0, "height"), out height);
                                        carouselMedia.Video = new InstaGramImage()
                                        {
                                            Width = width,
                                            Height = height,
                                            Url = handler.GetJTokenValue(jtoken2, "video_versions", 0, "url")
                                        };
                                    }
                                    instagramPost2.Album.Add(carouselMedia);
                                }
                            }
                            else
                            {
                                foreach (JToken jtoken2 in handler.GetJArrayElement(handler.GetJTokenValue(jtoken1, "image_versions2", "candidates")))
                                {
                                    int.TryParse(handler.GetJTokenValue(jtoken2, "width"), out int width);
                                    int.TryParse(handler.GetJTokenValue(jtoken2, "height"), out int height);
                                    instagramPost2.Images.Add(new InstaGramImage()
                                    {
                                        Width = width,
                                        Height = height,
                                        Url = handler.GetJTokenValue(jtoken2, "url")
                                    });
                                }
                                if (instagramPost2.MediaType == MediaType.Video)
                                {
                                    int.TryParse(handler.GetJTokenValue(jtoken1, "view_count"), out int viewCount);
                                    int.TryParse(handler.GetJTokenValue(jtoken1, "video_duration"), out int videoDuration);
                                    instagramPost2.ViewCount = viewCount;
                                    instagramPost2.VideoDuration = videoDuration;
                                    int.TryParse(handler.GetJTokenValue(jtoken1, "video_versions", 0, "width"), out int width);
                                    int.TryParse(handler.GetJTokenValue(jtoken1, "video_versions", 0, "height"), out int height);
                                    instagramPost2.Video = new InstaGramImage()
                                    {
                                        Width = width,
                                        Height = height,
                                        Url = handler.GetJTokenValue(jtoken1, "video_versions", 0, "url")
                                    };
                                }
                            }
                            bool.TryParse(handler.GetJTokenValue(jtoken1, "comments_disabled"), out bool commentDisabled);
                            instagramPost2.CommentsDisabled = commentDisabled;
                            bool.TryParse(handler.GetJTokenValue(jtoken1, "comment_likes_enabled"), out bool commentLikesEnable);
                            int.TryParse(handler.GetJTokenValue(jtoken1, "comment_count"), out int commentCount);
                            instagramPost2.CommentCount = commentCount;
                            instagramPost2.CommentLikesEnabled = commentLikesEnable;
                            instagramPost2.IsLocationPost = IsLocationPost;
                            var locationToken = handler.GetJTokenOfJToken(jtoken1, "location");
                            if (locationToken != null && locationToken.HasValues)
                            {
                                instagramPost2.HasLocation = true;
                                instagramPost2.Location = new Location()
                                {
                                    Name = handler.GetJTokenValue(locationToken, "name")
                                };
                                instagramPost2.Location.City = handler.GetJTokenValue(locationToken, "city");
                                instagramPost2.Location.Id = handler.GetJTokenValue(locationToken, "facebook_places_id");
                                var latitude = handler.GetJTokenOfJToken(locationToken, "lat");
                                if (latitude != null)
                                {
                                    instagramPost2.HasDetailedLocation = true;
                                    float.TryParse(latitude.ToString(), out float lat);
                                    float.TryParse(handler.GetJTokenValue(locationToken, "lng"), out float lang);
                                    instagramPost2.Location.Lat = lat;
                                    instagramPost2.Location.Lng = lang;
                                }
                            }
                            instagramPostList.Add(instagramPost2);
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                return instagramPostList;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                GlobusLogHelper.log.Error("Failed to retrieve image data");
                throw;
            }
        }
        public static string GetUrlFromCode(this string code)
        {
            return $"https://www.instagram.com/p/{(object)code}/";
        }
    }
}
