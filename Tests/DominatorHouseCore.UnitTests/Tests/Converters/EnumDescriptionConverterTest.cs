using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class EnumDescriptionConverterTest
    {
        EnumDescriptionConverter _sut;
        object value;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new EnumDescriptionConverter();
        }
        [TestMethod, Ignore("returns the value from resources, but not actual")]
        public void should_return_Twitter_Instagram_Gplus_Quora_Tumblr_Pinterest_Reddit_sepereted_by_comma_if_input_is_Follow_ActivityType_enum()
        {
            value = ActivityType.Follow;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be("Twitter,Instagram,Gplus,Quora,Tumblr,Pinterest,Reddit");
        }
        [TestMethod]
        public void should_return_null_if_input_is_not_enum()
        {
            value = 13232;
            var result = _sut.Convert(value, value.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(null);
        }
    }
}
