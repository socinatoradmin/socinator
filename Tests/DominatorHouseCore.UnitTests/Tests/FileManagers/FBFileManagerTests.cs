using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using DominatorHouseCore.UnitTests.Tests.FileManagers;
using FluentAssertions;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class FbFileManagerTests
    {
        private IProtoBuffBase _protoBuffBase;
        private ILockFileConfigProvider _lockFileConfigProvider;
        private IFileSystemProvider _fileSystemProvider;
        private FBFileManager _sut;

        [TestInitialize]
        public void SetUp()
        {
            _protoBuffBase = Substitute.For<IProtoBuffBase>();
            _lockFileConfigProvider = Substitute.For<ILockFileConfigProvider>();
            _fileSystemProvider = Substitute.For<IFileSystemProvider>();
            _sut = new FBFileManager(_protoBuffBase, _lockFileConfigProvider, _fileSystemProvider);
        }
        [TestMethod]
        public void GetFacebookConfig_should_return_data_if_file_exist()
        {
            var data = new ConfigFacebookModel();
            var path = "path to a FacebookConfig file";
            _fileSystemProvider.Exists(path).Returns(true);
            _protoBuffBase.Deserialize<ConfigFacebookModel>(path).Returns(data);
           
            // act
            var result = _sut.GetFacebookConfig();

            // assert
            result.Should().BeEquivalentTo(data);
        }

     
    }
}
