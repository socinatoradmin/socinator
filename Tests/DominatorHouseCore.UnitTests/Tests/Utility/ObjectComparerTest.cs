using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class ObjectComparerTest
    {
        [TestMethod]
        public void should_CompareAndGetChangedObject_method_return_newName_lists_if_old_and_new_list_are_not_same()
        {
            var old = new List<string>
            {
                "kumar",
                "Harsh",
                "Choudhary"
            };
            var newName = new List<string>
            {
                "Choudhary"
            };
            var result = ObjectComparer.CompareAndGetChangedObject(old, newName);
            result.Should().NotBeEmpty().And.Equals(newName);
        }
        [TestMethod]
        public void should_CompareAndGetChangedObject_method_return_null_if_old_and_new_list_are_same()
        {
            var old = new List<string>
            {
                "kumar",
                "Harsh",
                "Choudhary"
            };
            var newName = new List<string>
            {
                 "kumar",
                "Harsh",
                "Choudhary"
            };
            var result = ObjectComparer.CompareAndGetChangedObject(old, newName);
            result.Should().BeNull();
          
        }
        [TestMethod]
        public void should_Compare_method_return_false_if_old_and_new_list_are_not_same()
        {
            var old = new List<string>
            {
                "kumar",
                "Harsh",
                "Choudhary"
            };
            var newName = new List<string>
            {
                "Choudhary"
            };
            var result = ObjectComparer.Compare(old, newName);
            result.Should().BeFalse();
        }
        [TestMethod]
        public void should_Compare_method_return_true_if_old_and_new_list_are_same()
        {
            var old = new List<string>
            {
                "kumar",
                "Harsh",
                "Choudhary"
            };
            var newName = new List<string>
            {
                 "kumar",
                "Harsh",
                "Choudhary"
            };
            var result = ObjectComparer.Compare(old, newName);
            result.Should().BeTrue();

        }
    }
}
