using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using GramDominatorCore.GDLibrary.Response;
using GramDominatorCore.GDModel;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace GramDominatorCore.Response
{
    [Localizable(false)]
    public class ActivityNewsResponse : IGResponseHandler
    {
        public ActivityNewsResponse(IResponseParameter response) : base(response)
        {
            if (!Success)
                return;
            foreach (var token in handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "new_stories")))
                NewStories.Add(GetActivityItem(token));
            foreach (var token in handler.GetJArrayElement(handler.GetJTokenValue(RespJ, "old_stories")))
                OldStories.Add(GetActivityItem(token));
        }

        public List<ActivityItem> NewStories { get; } = new List<ActivityItem>();

        public List<ActivityItem> OldStories { get; } = new List<ActivityItem>();

        private ActivityItem GetActivityItem(JToken token)
        {
            int.TryParse(handler.GetJTokenValue(token, "type"),out int type);
            int.TryParse(handler.GetJTokenValue(token, "args", "timestamp"), out int time);
            switch (type)
            {
                case 1:
                    
                    return new ActivityItem()
                    {
                        Text = handler.GetJTokenValue(token, "args", "text"),
                        Image = handler.GetJTokenValue(token, "args", "media",0, "image"),
                        Time = time,
                        User = new BaseUser(handler.GetJTokenValue(token, "args", "profile_id"), string.Empty)
                    };
                case 3:
                    return new ActivityItem()
                    {
                        Text = handler.GetJTokenValue(token, "args", "text"),
                        Image = handler.GetJTokenValue(token, "args", "profile_image"),
                        Time = time,
                        User = new BaseUser(handler.GetJTokenValue(token, "args", "profile_id"), handler.GetJTokenValue(token, "args", "inline_follow", "user_info", "username"))
                    };
                case 4:
                    return new ActivityItem()
                    {
                        Text = handler.GetJTokenValue(token, "args", "text"),
                        Image = handler.GetJTokenValue(token, "args", "profile_image"),
                        Time = time,
                        User = new BaseUser(handler.GetJTokenValue(token, "args", "profile_id"), string.Empty)
                    };
                case 13:
                    return new ActivityItem()
                    {
                        Text = handler.GetJTokenValue(token, "args", "text"),
                        Time = time,
                        Url = $"https://www.instagram.com{handler.GetJTokenValue(token, "args", "action_url")}"
                    };
                case 14:
                    return new ActivityItem()
                    {
                        Text = handler.GetJTokenValue(token, "args", "text"),
                        Image = handler.GetJTokenValue(token, "args", "profile_image"),
                        Time = time,
                        User = new BaseUser(handler.GetJTokenValue(token, "args", "profile_id"), string.Empty)
                    };
                case 15:
                    return new ActivityItem()
                    {
                        Text = handler.GetJTokenValue(token, "args", "text"),
                        Image = handler.GetJTokenValue(token, "args", "media",0, "image"),
                        Url = handler.GetJTokenValue(token, "args", "media", 0, "image"),
                        Time = time
                    };
                default:
                    GlobusLogHelper.log.Error($"Unsupported activity item - {token}");
                    throw new InstagramException("Not handled type");
            }
        }
    }
}
