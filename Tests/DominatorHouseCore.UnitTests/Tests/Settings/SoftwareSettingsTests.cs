using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace DominatorHouseCore.UnitTests.Tests.Settings
{
    [TestClass]
    public class SoftwareSettingsTests
    {
        private ISoftwareSettingsFileManager _softwareSettingsFileManager;
        private IFileSystemProvider _fileSystemProvider;
        private IGenericFileManager _genericFileManager;
        private  IAccountsFileManager _accountsFileManager;
        private ISoftwareSettings _sut;

        [TestInitialize]
        public void SetUp()
        {
            _softwareSettingsFileManager = Substitute.For<ISoftwareSettingsFileManager>();
            _fileSystemProvider = Substitute.For<IFileSystemProvider>();
            _genericFileManager = Substitute.For<IGenericFileManager>();
            _accountsFileManager = Substitute.For<IAccountsFileManager>();
            _sut = new SoftwareSettings(_softwareSettingsFileManager, _fileSystemProvider, _genericFileManager,_accountsFileManager);
        }

        [TestMethod]
        public void should_return_settings_if_it_exist()
        {
            // arrange
            var model = new SoftwareSettingsModel();
            _fileSystemProvider.Exists(ConstantVariable.GetOtherSoftwareSettingsFile()).Returns(true);
            _softwareSettingsFileManager.GetSoftwareSettings().Returns(model);

            // act
            _sut.InitializeOnLoadConfigurations();

            // assert
            _sut.Settings.Should().Be(model);
            _softwareSettingsFileManager.DidNotReceive().SaveSoftwareSettings(Arg.Any<SoftwareSettingsModel>());
        }

        [TestMethod]
        public void should_return_default_settings_and_save_if_not_exist()
        {
            // arrange
            _fileSystemProvider.Exists(ConstantVariable.GetOtherSoftwareSettingsFile()).Returns(false);

            // act
            _sut.InitializeOnLoadConfigurations();

            // assert
            _sut.Settings.IsEnableAdvancedUserMode.Should().Be(true);
            _softwareSettingsFileManager.Received(1).SaveSoftwareSettings(Arg.Any<SoftwareSettingsModel>());
        }
    }
}
