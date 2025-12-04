#region

using System;
using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums
{
    public enum QueryType
    {
        //[Description("Facebook, Instagram,Twitter, Pinterest, LinkedIn, Reddit, Quora,YouTube,Tumblr")]
        [Facebook]
        [Instagram]
        [Twitter]
        [Pinterest]
        [LinkedIn]
        [Reddit]
        [Youtube]
        [Tumblr]
        [Quora]
        [Description("Follow")]
        Keyword,

        // ReSharper disable once UnusedMember.Global
        [Quora] [Description("Follow")] SomeOneFollower
    }

    internal class FacebookAttribute : Attribute
    {
    }

    internal class InstagramAttribute : Attribute
    {
    }

    internal class TwitterAttribute : Attribute
    {
    }

    internal class QuoraAttribute : Attribute
    {
    }

    internal class PinterestAttribute : Attribute
    {
    }

    internal class LinkedInAttribute : Attribute
    {
    }

    internal class RedditAttribute : Attribute
    {
    }

    internal class YoutubeAttribute : Attribute
    {
    }

    internal class TumblrAttribute : Attribute
    {
    }
}