using Microsoft.VisualStudio.TestTools.UnitTesting;
using DominatorHouseCore.Models;
using System.Windows;
using FluentAssertions;
using System;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class AllignmentConvertorTest
    {
        AllignmentConvertor _allignmentConvertor;
        [TestInitialize]
        public void Initialize()
        {
            _allignmentConvertor = new AllignmentConvertor();
        }
        [TestMethod]
        public void should_Convert_method_return_Horizontal_Right_Alignment_if_input_is_true()
        {
            var input = true;
            var expected = HorizontalAlignment.Right;
            var result = _allignmentConvertor.Convert(input, null, null, null);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_Convert_method_return_Horizontal_Left_Alignment_if_input_is_false()
        {
            var input = false;
            var expected = HorizontalAlignment.Left;
            var result = _allignmentConvertor.Convert(input, null, null, null);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_Convert_method_throw_NullReferenceException_if_input_is_null()
        {
            bool? input = null;
            _allignmentConvertor.Convert(input, null, null, null).Should().Be(HorizontalAlignment.Left);
        }



        [TestMethod]
        public void should_ConvertBack_method_return_true_if_input_is_Horizontal_Right_Alignment()
        {
            var input = HorizontalAlignment.Right;
            var expected = true ;
            var result = _allignmentConvertor.ConvertBack(input, null, null, null);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_ConvertBack_method_return_false_if_input_is_Horizontal_Left_Alignment()
        {
            var input = HorizontalAlignment.Left;
            var expected = false;
            var result = _allignmentConvertor.ConvertBack(input, null, null, null);
            result.Should().Be(expected);
        }
        [TestMethod]
        public void should_ConvertBack_method_throw_NullReferenceException_if_input_is_null()
        {
            bool? input = null;
            _allignmentConvertor.ConvertBack(input, null, null, null).Should().Be(false);
        }
    }
}
