using System;
using System.Collections.Generic;
using System.Windows.Controls;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher.Settings;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl.Settings
{
    /// <summary>
    ///     Interaction logic for PostAdvancedSettings.xaml
    /// </summary>
    public partial class PostAdvancedSettings : UserControl
    {
        private PostAdvancedSettings()
        {
            InitializeComponent();
        }

        public PostAdvancedSettings(PublisherPostSettings publisherPostSettings) : this()
        {
            Initialize(publisherPostSettings);
        }

        public PostGeneralSettings PostGeneralSettings { get; set; }

        public PostFacebookSettings PostFacebookSettings { get; set; }

        public PostInstagramSettings PostInstagramSettings { get; set; }

        public PostRedditSettings PostRedditSettings { get; set; }
        public PostYoutubeSettings PostYoutubeSettings { get; set; }

        public PostTwitterSettings PostTwitterSettings { get; set; }

        public PostLinkedInSettings PostLinkedInSettings { get; set; }

        public PostTumblrSettings PostTumblrSettings { get; set; }


        public void Initialize(PublisherPostSettings publisherPostSettings)
        {
            PostGeneralSettings = new PostGeneralSettings(publisherPostSettings);

            var items = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyGeneralSettings").ToString(),
                    Content = new Lazy<UserControl>(() => PostGeneralSettings)
                }
            };


            var availableNetworks = SocinatorInitialize.AvailableNetworks;
            foreach (var network in availableNetworks)
                switch (network)
                {
                    case SocialNetworks.Facebook:
                        PostFacebookSettings = new PostFacebookSettings(publisherPostSettings);
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyFacebook").ToString(),
                            Content = new Lazy<UserControl>(() => PostFacebookSettings)
                        });
                        break;
                    case SocialNetworks.Instagram:
                        PostInstagramSettings = new PostInstagramSettings(publisherPostSettings);
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyInstagram").ToString(),
                            Content = new Lazy<UserControl>(() => PostInstagramSettings)
                        });
                        break;
                    case SocialNetworks.Twitter:
                        PostTwitterSettings = new PostTwitterSettings(publisherPostSettings);
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyTwitter").ToString(),
                            Content = new Lazy<UserControl>(() => PostTwitterSettings)
                        });
                        break;
                    case SocialNetworks.LinkedIn:

                        PostLinkedInSettings = new PostLinkedInSettings(publisherPostSettings);
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyLinkedIn").ToString(),
                            Content = new Lazy<UserControl>(() => PostLinkedInSettings)
                        });
                        break;
                    case SocialNetworks.Tumblr:
                        PostTumblrSettings = new PostTumblrSettings(publisherPostSettings);
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyTumblr").ToString(),
                            Content = new Lazy<UserControl>(() => PostTumblrSettings)
                        });
                        break;
                    case SocialNetworks.Reddit:
                        PostRedditSettings = new PostRedditSettings(publisherPostSettings);
                        items.Add(new TabItemTemplates
                        {
                            Title = FindResource("LangKeyReddit").ToString(),
                            Content = new Lazy<UserControl>(() => PostRedditSettings)
                        });
                        break;
                    //case SocialNetworks.Pinterest:
                    //case SocialNetworks.Quora:
                    //case SocialNetworks.Gplus:
                    //case SocialNetworks.YouTube:
                    //    PostYoutubeSettings = new PostYoutubeSettings(publisherPostSettings);
                    //    items.Add(new TabItemTemplates
                    //    {
                    //        Title = FindResource("LangKeyYoutube").ToString(),
                    //        Content = new Lazy<UserControl>(() => PostYoutubeSettings)
                    //    });
                    //    break;
                }

            AdvancedPostSettings.ItemsSource = items;
        }
    }
}