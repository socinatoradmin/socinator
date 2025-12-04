#region

using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.ViewModel;

#endregion

namespace DominatorHouseCore.Utility
{
    public interface IAccountGrowthPropertiesProvider
    {
        IList<GrowthProperty> this[SocialNetworks network] { get; }
    }

    public class AccountGrowthPropertiesProvider : IAccountGrowthPropertiesProvider
    {
        private readonly IDictionary<SocialNetworks, List<GrowthProperty>> _growthProperties;

        public AccountGrowthPropertiesProvider()
        {
            _growthProperties = new Dictionary<SocialNetworks, List<GrowthProperty>>
            {
                {
                    SocialNetworks.Twitter, new List<GrowthProperty>
                    {
                        new GrowthProperty
                            {PropertyName = "LangKeyFollowers".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyFollowings".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty {PropertyName = "LangKeyTweets".FromResourceDictionary(), PropertyValue = 0}
                    }
                },
                {
                    SocialNetworks.Facebook, new List<GrowthProperty>
                    {
                        new GrowthProperty
                            {PropertyName = "LangKeyFriends".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyJoinedGroups".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyOwnPages".FromResourceDictionary(), PropertyValue = 0}
                    }
                },
                {
                    SocialNetworks.Quora, new List<GrowthProperty>
                    {
                        new GrowthProperty
                            {PropertyName = "LangKeyFollowers".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyFollowings".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty {PropertyName = "LangKeyPost".FromResourceDictionary(), PropertyValue = 0}
                    }
                },
                {
                    SocialNetworks.Reddit, new List<GrowthProperty>
                    {
                        new GrowthProperty {PropertyName = "LangKeyScore".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyCommunities".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyPostKarma".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyCommentKarma".FromResourceDictionary(), PropertyValue = 0}
                    }
                }
                // {
                //SocialNetworks.Gplus, new List<GrowthProperty>
                //{
                //    new GrowthProperty {PropertyName = "Followers", PropertyValue = 0},
                //    new GrowthProperty {PropertyName = "Followings", PropertyValue = 0},
                //    new GrowthProperty {PropertyName = "Communities", PropertyValue = 0},
                //}
                // }
                ,
                {
                    SocialNetworks.YouTube, new List<GrowthProperty>
                    {
                        new GrowthProperty
                            {PropertyName = "LangKeyChannelsCount".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyViewsCount".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeySupportLinksCount".FromResourceDictionary(), PropertyValue = 0}
                    }
                },
                {
                    SocialNetworks.Instagram, new List<GrowthProperty>
                    {
                        new GrowthProperty
                            {PropertyName = "LangKeyFollowers".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyFollowings".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty {PropertyName = "LangKeyUploads".FromResourceDictionary(), PropertyValue = 0}
                    }
                },
                {
                    SocialNetworks.LinkedIn, new List<GrowthProperty>
                    {
                        new GrowthProperty
                            {PropertyName = "LangKeyConnections".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty {PropertyName = "LangKeyGroups".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyInvitation".FromResourceDictionary(), PropertyValue = 0}
                    }
                },
                {
                    SocialNetworks.Tumblr, new List<GrowthProperty>
                    {
                        new GrowthProperty
                            {PropertyName = "LangKeyFollowers".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyFollowings".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty {PropertyName = "LangKeyPosts".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyChannels".FromResourceDictionary(), PropertyValue = 0}
                    }
                },
                {
                    SocialNetworks.Pinterest, new List<GrowthProperty>
                    {
                        new GrowthProperty
                            {PropertyName = "LangKeyFollowers".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty
                            {PropertyName = "LangKeyFollowings".FromResourceDictionary(), PropertyValue = 0},
                        new GrowthProperty {PropertyName = "LangKeyPosts".FromResourceDictionary(), PropertyValue = 0}
                    }
                },
                {
                    SocialNetworks.Social, new List<GrowthProperty>()
                },
                {
                    SocialNetworks.TikTok, new List<GrowthProperty>()
                }
            };
        }

        public IList<GrowthProperty> this[SocialNetworks network] => _growthProperties[network];
    }
}