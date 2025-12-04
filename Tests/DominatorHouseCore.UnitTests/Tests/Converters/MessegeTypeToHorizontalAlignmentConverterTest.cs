
using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class MessegeTypeToHorizontalAlignmentConverterTest
    {
        MessegeTypeToHorizontalAlignmentConverter _sut;
        object input;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new MessegeTypeToHorizontalAlignmentConverter();
        }
        [TestMethod]
        public void should_return_Right_HorizontalAlignment_if_input_sent_ChatMessage()
        {
            input = ChatMessage.Sent.ToString();
            var expected = HorizontalAlignment.Right;
            var result = _sut.Convert(input, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_Left_HorizontalAlignment_if_input_Received_ChatMessage()
        {
            input = ChatMessage.Received.ToString();
            var expected = HorizontalAlignment.Left;
            var result = _sut.Convert(input, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_return_Left_HorizontalAlignment_if_input_null()
        {
            input = null;
            var expected = HorizontalAlignment.Left;
            var result = _sut.Convert(input, null, null, CultureInfo.CurrentUICulture);
            result.Should().Be(expected);
        }
    }
}
