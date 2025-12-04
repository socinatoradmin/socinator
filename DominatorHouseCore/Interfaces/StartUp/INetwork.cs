#region

using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.StartupActivity;
using DominatorHouseCore.StartupActivity.Instagram;
using DominatorHouseCore.StartupActivity.Linkedin;
using DominatorHouseCore.StartupActivity.Pinterest;
using DominatorHouseCore.StartupActivity.Quora;
using DominatorHouseCore.StartupActivity.Reddit;
using DominatorHouseCore.StartupActivity.Tumblr;
using DominatorHouseCore.StartupActivity.Twitter;
using DominatorHouseCore.StartupActivity.Youtube;

#endregion

namespace DominatorHouseCore.Interfaces.StartUp
{
    internal class SocialNetworkActivity
    {
        private static Dictionary<string, INetworkActivity> Networks { get; } =
            new Dictionary<string, INetworkActivity>();

        public static INetworkActivity GetNetworkActivity(string networks)
        {
            return Networks.ContainsKey(networks) ? Networks[networks] : null;
        }

        public static void RegisterNetwork()
        {
            try
            {
                Networks.Add(SocialNetworks.Quora.ToString(), new QuoraActivity());
                //Networks.Add(SocialNetworks.Facebook.ToString(), new FacebookActivity());
                Networks.Add(SocialNetworks.Instagram.ToString(), new InstagramActivity());
                Networks.Add(SocialNetworks.LinkedIn.ToString(), new LinkedinActivity());
                Networks.Add(SocialNetworks.Pinterest.ToString(), new PinterestActivity());
                Networks.Add(SocialNetworks.Reddit.ToString(), new RedditActivity());
                Networks.Add(SocialNetworks.Tumblr.ToString(), new TumblrActivity());
                Networks.Add(SocialNetworks.Twitter.ToString(), new TwitterActivity());
                Networks.Add(SocialNetworks.YouTube.ToString(), new YoutubeActivity());
            }
            catch (Exception exc)
            {
                exc.DebugLog();
            }
        }
    }

    public interface INetworkActivity
    {
        BaseActivity GetActivity(string activity);
    }
}