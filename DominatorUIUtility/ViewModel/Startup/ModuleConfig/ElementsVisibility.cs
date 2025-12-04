using System;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.ViewModel.Startup.ModuleConfig
{
    public interface ITwitterVisibilityModel
    {
        Visibility TwitterElementsVisibility { get; set; }
    }

    public interface IInstagramVisibilityModel
    {
        Visibility InstagramElementsVisibility { get; set; }
    }

    public interface IFacebookModel
    {
        Visibility FacebookElementsVisibility { get; set; }
    }

    public interface ILinkedInModel
    {
        Visibility LinkedInElementsVisibility { get; set; }
    }

    public interface IYoutubeModel
    {
        Visibility YoutubeElementsVisibility { get; set; }
    }

    public interface IRedditModel
    {
        Visibility RedditElementsVisibility { get; set; }
    }


    public class ElementsVisibility
    {
        public static void NetworkElementsVisibilty(dynamic visibilityModel)
        {
            try
            {
                var nw = InstanceProvider.GetInstance<ISelectActivityViewModel>().SelectedNetwork;
                var network = (SocialNetworks) Enum.Parse(typeof(SocialNetworks), nw);
                switch (network)
                {
                    case SocialNetworks.Twitter:
                        visibilityModel.TwitterElementsVisibility = Visibility.Visible;
                        break;
                    case SocialNetworks.Instagram:
                        visibilityModel.InstagramElementsVisibility = Visibility.Visible;
                        break;
                    case SocialNetworks.LinkedIn:
                        visibilityModel.LinkedInElementsVisibility = Visibility.Visible;
                        break;
                    case SocialNetworks.Facebook:
                        visibilityModel.FacebookElementsVisibility = Visibility.Visible;
                        break;
                    case SocialNetworks.YouTube:
                        visibilityModel.YoutubeElementsVisibility = Visibility.Visible;
                        break;
                    case SocialNetworks.Reddit:
                        visibilityModel.RedditElementsVisibility = Visibility.Visible;
                        break;
                    case SocialNetworks.Quora:
                        visibilityModel.QuoraElementsVisibility = Visibility.Visible;
                        break;
                }
            }
            catch
            {
            }
        }
    }
}