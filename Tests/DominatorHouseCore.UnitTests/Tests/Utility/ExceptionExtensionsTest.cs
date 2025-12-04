using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DominatorHouseCore.UnitTests.Tests.Utility
{
    [TestClass]
    public class ExceptionExtensionsTest
    {
        [TestMethod]
        public void should_ToUserString_method_return_expactedresult_if_input_is_null()
        {
            string result = string.Empty;
            string expactedresult = "Exception of type 'System.NullReferenceException' has been thrown\r\n\r\nMessage: Object reference not set to an instance of an object.\r\n\r\nMessage Details: null string";
            try
            {
                string data = null;
                data.ToString();
            }
            catch (Exception ex)
            {
                result=ex.ToUserString("null string");
            }
            result.Should().Be(expactedresult);
        }
        [TestMethod]
        public void should_ToUserStringWithStack_method_return_expactedresult_if_input_is_null()
        {
            string result = string.Empty;
            string expactedresult = "Exception of type 'System.NullReferenceException' has been thrown\r\n\r\nMessage: Object reference not set to an instance of an object.\r\n\r\nMessage Details: null string";
            try
            {
                string data = null;
                data.ToString();
            }
            catch (Exception ex)
            {
                result = ex.ToUserString("null string");
            }
            result.Should().Be(expactedresult);
        }
        [TestMethod]
        public void should_ToUserStringWithStack_method_return_empty_string_if_input_is_not_null()
        {
            string result = string.Empty;
           
            try
            {
                string data = "kumar";
                data.ToString();
            }
            catch (Exception ex)
            {
                result = ex.ToUserString("null string");
            }
            result.Should().Be(string.Empty);
        }
    }
}
