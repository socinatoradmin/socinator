using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using GramDominatorCore.GDLibrary.Response;
using Newtonsoft.Json.Linq;

namespace GramDominatorCore.Response
{
    public class VisualThreadResponse : IGResponseHandler
    {
        public VisualThreadResponse(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;

            try
            {
                string username = RespJ["thread"]["users"][0]["username"].ToString();
                int moreAvailableMinValue = Convert.ToBoolean(RespJ["thread"]["more_available_min"]) ? 1 : 0;
                MoreAvailableMin = moreAvailableMinValue != 0;
                int moreAvailableMaxValue = Convert.ToBoolean(RespJ["thread"]["more_available_max"]) ? 1 : 0;
               MoreAvailableMax = moreAvailableMaxValue != 0;
               NextMaxId = (RespJ["thread"]["next_max_id"] != null) ? RespJ["thread"]["next_max_id"].ToString() : string.Empty;
               NextMinId = (RespJ["thread"]["next_min_id"] != null) ? RespJ["thread"]["next_min_id"].ToString() : string.Empty;
                oldestCursor = (RespJ["thread"]["oldest_cursor"] != null) ? RespJ["thread"]["oldest_cursor"].ToString() : string.Empty;
                foreach (JToken jtoken in RespJ["thread"]["items"])
                {
                    try
                    {
                        ChatDetails chatDetail = new ChatDetails();
                        chatDetail.ListMediaUrls = new ObservableCollection<string>();
                       var messageType= jtoken["item_type"].ToString(); 
                        if (messageType.Contains("text"))
                        {
                             chatDetail = new ChatDetails
                            {
                                Messeges = (jtoken["text"] != null) ? jtoken["text"].ToString() : string.Empty,
                                MessegeType = ChatMessageType.Text
                            };
                        }
                        else if(messageType.Contains("link"))
                        {
                            chatDetail = new ChatDetails
                            {
                                Messeges = jtoken["link"]["text"].ToString(),
                                MessegeType = ChatMessageType.Text
                            };
                            //chatDetail.Messeges = (jtoken["link"]["text"] != null) ? jtoken["text"]["text"].ToString() : string.Empty;
                            //chatDetail.MessegeType = ChatMessageType.Link;
                        }
                        else if(messageType.Contains("reel_share"))
                        {
                            chatDetail.Messeges = (jtoken["reel_share"]["text"] != null) ? jtoken["reel_share"]["text"].ToString() : string.Empty;
                            chatDetail.MessegeType = ChatMessageType.ReelShare;
                        }
                        else if(messageType.Equals("media"))
                        {
                            string mediaUrl = jtoken["media"]["image_versions2"]["candidates"][0]["url"].ToString();
                            chatDetail.ListMediaUrls.Add(mediaUrl);
                            chatDetail.MessegeType = ChatMessageType.Media;
                        }
                        else if(messageType.Equals("media_share"))
                        {
                            try
                            {
                                chatDetail.Messeges = jtoken["media_share"]["image_versions2"]["candidates"][0]["url"].ToString();
                                chatDetail.MessegeType = ChatMessageType.Mediashare;
                            }
                            catch (Exception)
                            {
                                chatDetail.Messeges = jtoken["media_share"]["carousel_media"][0]["image_versions2"]["candidates"][0]["url"].ToString();
                                chatDetail.MessegeType = ChatMessageType.Mediashare;
                            }
                        }
                        else if(messageType.Equals("raven_media"))
                        {
                            chatDetail.Messeges = jtoken["visual_media"]["media"]["image_versions2"]["candidates"][0]["url"].ToString();
                            chatDetail.MessegeType = ChatMessageType.RavenMedia;
                        }
                        else if (messageType.Equals("live_viewer_invite"))
                        {
                            chatDetail.Messeges = jtoken["live_viewer_invite"]["message"].ToString();
                            chatDetail.MessegeType = ChatMessageType.LiveViewerInvite;
                        }
                        else if (messageType.Equals("story_share"))
                        {
                            chatDetail.Messeges = jtoken["story_share"]["media"]["image_versions2"]["candidates"][0]["url"].ToString();
                            chatDetail.Messeges = (jtoken["story_share"]["text"] != null) ? jtoken["story_share"]["text"].ToString() : string.Empty;
                            chatDetail.MessegeType = ChatMessageType.StoryShare;
                        }
                        else
                        {
                            chatDetail.MessegeType = ChatMessageType.Null;
                        }
                        chatDetail.Sender = username;
                        DateTime dateTime = double.Parse(jtoken["timestamp"].ToString().Substring(0, 10)).EpochToDateTimeUtc().ToLocalTime();
                        chatDetail.Time = dateTime.ToString(CultureInfo.InvariantCulture);
                        chatDetail.MessegesId = jtoken["item_id"].ToString();
                        chatDetail.SenderId = jtoken["user_id"].ToString();
                        JToken jtoken1 = jtoken["client_context"];
                        chatDetail.ClientContext = jtoken1?.ToString();

                        LstChatDetails.Add(chatDetail);
                    }
                    catch (Exception)
                    {
                       // ex.DebugLog();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public List<ChatDetails> LstChatDetails { get; set; } = new List<ChatDetails>();

        public bool MoreAvailableMin { get; set; }

        public bool MoreAvailableMax { get; set; }

        public string NextMaxId { get; set; }

        public string NextMinId { get; set; }

        public string Client_Context { get; set; }
        public string oldestCursor { get; set; }
    }
}
