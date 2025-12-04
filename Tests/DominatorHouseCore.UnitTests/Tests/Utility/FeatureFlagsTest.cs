using DominatorHouseCore.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using FluentAssertions;
using System.Windows;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class FeatureFlagsTest
    {

        [TestInitialize]
        public void SetUp()
        {

            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Quora);
            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Facebook);
            FeatureFlags.UpdateFeatures();
        }
        [TestMethod]
        public void should_return_true_for_quora_network_as_string()
        {
            var result = FeatureFlags.Check(SocialNetworks.Quora.ToString());
            result.Should().BeTrue();
        }
        [TestMethod]
        public void should_return_true_for_Intagram_network_as_string()
        {
            var result = FeatureFlags.Check(SocialNetworks.Instagram.ToString());
            result.Should().BeFalse();
        }
        [TestMethod]
        public void should_return_Visible_for_quora_network_as_SocialNetworks()
        {
            var result = FeatureFlags.Check(SocialNetworks.Quora);
            result.Should().Be(Visibility.Visible);
        }
        [TestMethod]
        public void should_return_Collapsed_for_Intagram_network_as_SocialNetworks()
        {
            var result = FeatureFlags.Check(SocialNetworks.Instagram);
            result.Should().Be(Visibility.Collapsed);
        }
        [TestMethod]
        public void should_return_true_for_available_network()
        {
            var result = FeatureFlags.IsNetworkAvailable(SocialNetworks.Quora);
            result.Should().BeTrue();
        }
        [TestMethod]
        public void should_return_true_for_not_available_network()
        {
            var result = FeatureFlags.IsNetworkAvailable(SocialNetworks.Instagram);
            result.Should().BeFalse();
        }
    }
}
