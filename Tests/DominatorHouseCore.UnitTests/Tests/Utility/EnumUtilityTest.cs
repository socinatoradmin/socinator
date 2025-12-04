using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class EnumUtilityTest
    {
        ActivityType _activityType;
        [TestInitialize]
        public void SetUp()
        {
            _activityType = ActivityType.Follow;
        }
        [TestMethod]
        public void should_return_all_quora_activity()
        {
            var result = EnumUtility.GetEnums("Quora");
            result.Count.Should().Be(14);
        }
        [TestMethod]
        public void should_return_empty_if_string_is_not_socialnetwork()
        {
            var result = EnumUtility.GetEnums("abc");
            result.Count.Should().Be(0);
        }
        [TestMethod]
        public void should_return_description_of_enum()
        {
            var result = EnumUtility.GetDescriptionAttr(_activityType);
            result.Should().Be("Twitter,Instagram,Gplus,Quora,Tumblr,Pinterest,Reddit,TikTok");
        }
        [TestMethod]
        public void should_return_null_if_enum_dont_have_description()
        {
            var result = EnumUtility.GetDescriptionAttr(SocialNetworks.Facebook);
            result.Should().Be(null);
        }
       
    }
}
