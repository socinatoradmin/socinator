using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class SpinTexHelperTest
    {
        [TestMethod]
        public void should_return_list_of_spintext()
        {
            string spintext = "(hello|hi) how are (you|guys)";
            var result = SpinTexHelper.GetSpinMessageCollection(spintext);
            result.Count.Should().Be(4);
            result.Should().Contain("hello how are you");
            result.Should().Contain("hello how are guys");
            result.Should().Contain("hi how are you");
            result.Should().Contain("hi how are guys");
        }
        [TestMethod]
        public void should_return_empty_list_if_spintext_is_null()
        {
            string spintext = null;
            var result = SpinTexHelper.GetSpinMessageCollection(spintext);
            result.Count.Should().Be(0);
        }
        [TestMethod]
        public void should_return_spintext_if_text_present_in_list_of_spintext()
        {
            string spintext = "(hello|hi) how are (you|guys)";
            var lstspintext = SpinTexHelper.GetSpinMessageCollection(spintext);
            var result = SpinTexHelper.GetSpinText(spintext);
            lstspintext.Should().Contain(result);

        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void should_throw_ArgumentNullException_if_spintext_is_null()
        {
            string spintext = null;
            var result = SpinTexHelper.GetSpinText(spintext);
        }
    }
}
