using Dominator.Tests.Utils;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class ConfigFileManagerTest : UnityInitializationTests
    {
        IBinFileHelper _binFileHelper;
        Configuration input;

        [TestInitialize]
        public void Setup()
        {
            base.SetUp();
            _binFileHelper = Substitute.For<IBinFileHelper>();
            Container.RegisterInstance(_binFileHelper);
        }
        [TestMethod]
        public void should_return_true_if_input_is_successfully_saved()
        {
            input = new Configuration();
            var expected = true;
            var output = ConfigFileManager.SaveConfig(input);
            output.Should().Be(expected);
        }
       
        [TestMethod]
        public void should_return_all_configuration()
        {
            var inputs = new List<Configuration>
            {
                 new Configuration(), new Configuration(), new Configuration()
            };
            _binFileHelper.GetConfigDetails<Configuration>().ReturnsForAnyArgs(inputs);
            var output = ConfigFileManager.GetAllConfig();
            output.Should().NotBeEmpty().And.HaveCount(3);
        }
        [TestMethod]
        public void should_return_empty_configuration()
        {
            var inputs = new List<Configuration>();
            _binFileHelper.GetConfigDetails<Configuration>().ReturnsForAnyArgs(inputs);
            var output = ConfigFileManager.GetAllConfig();
            output.Should().BeEmpty();
        }
    }
}
