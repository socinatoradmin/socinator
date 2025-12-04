using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Windows.Media;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class SocialNetworkToColorBrushConverterTests
    {
        private SocialNetworkToColorBrushConverter _sut;
        object value;
        [TestInitialize]
        public void SetUp()
        {
            _sut = new SocialNetworkToColorBrushConverter();
        }

        /// <summary>
        /// this test will failed if a new network will be added but converter isn't changed for this network
        /// </summary>
        [TestMethod]
        public void should_return_value_for_all_the_types()
        {
            // arrange
            var allEnumValues = Enum.GetValues(typeof(SocialNetworks));

            // act
            foreach (var socialNetwork in allEnumValues)
            {
                if (socialNetwork.ToString() == SocialNetworks.Gplus.ToString())
                    continue;

                var val = _sut.Convert(socialNetwork, typeof(SocialNetworks), new object(),
                    CultureInfo.CurrentUICulture);
                // assert
                if (((SocialNetworks)socialNetwork) == SocialNetworks.Social)
                {
                    val.Should().BeNull();
                }
                else
                {
                    val.Should().NotBeNull();
                }
            }
        }

        [TestMethod]
        public void should_return_Red_brush_if_value_is_Quora_network()
        {
            value = SocialNetworks.Quora;
            var expected = (SolidColorBrush)new BrushConverter().ConvertFrom("#b92b27");
            var result = (SolidColorBrush)_sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.ToString().Should().Be(expected.ToString());
        }
        [TestMethod]
        public void should_return_null_if_value_is_Social_network()
        {
            value = SocialNetworks.Social;
            object expected = null;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_null_if_value_is_null()
        {
            value = null;
            object expected = null;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_null_if_value_is_not_a_SocialNetwork()
        {
            value = "Harsh";
            object expected = null;
            var result = _sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
    }
}
