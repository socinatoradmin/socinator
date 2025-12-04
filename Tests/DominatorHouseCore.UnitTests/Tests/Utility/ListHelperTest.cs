using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class ListHelperTest
    {
        [TestMethod]
        public void should_return_lists_with_Shuffle_if_input_list_have_more_than_one_items()
        {
            var lstname = new List<string>
            {
                "kumar",
                "Harsh",
                "Choudhary"
            };
            var result = new List<string>(lstname);
            result.Shuffle();
            result.Should().NotBeEmpty().And.IntersectWith(lstname);
            result.Should().BeEquivalentTo(lstname);
            Assert.AreNotSame(lstname, result);
        }
        [TestMethod]
        public void should_return_lists_after_Shuffle_in_same_order_if_list_contain_only_one_item()
        {
            var lstname = new List<string> { "kumar" };
            var result = new List<string>(lstname);
            result.Shuffle();
            result.Should().ContainInOrder(lstname);
        }
        
        [TestMethod]
        public void should_return_reverse_of_list_after_swaping_list_element()
        {
            var lstname = new List<string> { "kumar", "harsh" };
            var result = new List<string>(lstname);
            result.Swap(0, 1);
            lstname.Reverse();
            result.Should().ContainInOrder(lstname);
        }
        [TestMethod]
        public void after_swapping_actual_list_and_swapped_list_should_not_be_same_in_order()
        {
            var lstname = new List<string> { "kumar", "harsh" };
            var result = new List<string>(lstname);
            result.Swap(0, 1);
            Assert.AreNotSame(lstname, result);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void should_throw_ArgumentOutOfRangeException_if_list_is_empty()
        {
            List<string> lstname = new List<string>();
            var result = new List<string>(lstname);
            result.Swap(0, 1);
           
        }
    }
}
