using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class GenericFileManagerTests
    {
        private IProtoBuffBase _protoBuffBase;
        private ILockFileConfigProvider _lockFileConfigProvider;
        private IFileSystemProvider _fileSystemProvider;
        private IGenericFileManager _sut;

        [TestInitialize]
        public void SetUp()
        {
            _protoBuffBase = Substitute.For<IProtoBuffBase>();
            _lockFileConfigProvider = Substitute.For<ILockFileConfigProvider>();
            _fileSystemProvider = Substitute.For<IFileSystemProvider>();
            _sut = new GenericFileManager(_protoBuffBase, _lockFileConfigProvider, _fileSystemProvider);
        }

        [TestMethod]
        public void GetModuleDetails_should_return_data_if_file_exist()
        {
            // arrange
            var data = new PostDeletionModel();
            var path = "some path to a file";
            _fileSystemProvider.Exists(path).Returns(true);
            _protoBuffBase.DeserializeList<PostDeletionModel>(path).Returns(new List<PostDeletionModel> { data });

            // act
            var result = _sut.GetModuleDetails<PostDeletionModel>(path);

            // assert
            result.Single().Should().Be(data);
        }

        [TestMethod]
        public void GetModuleDetails_should_return_empty_list_if_file_does_not_exist()
        {
            // arrange
            var path = "some path to a file";
            _fileSystemProvider.Exists(path).Returns(false);

            // act
            var result = _sut.GetModuleDetails<PostDeletionModel>(path);

            // assert
            result.Should().BeEmpty();
        }
    }
}
