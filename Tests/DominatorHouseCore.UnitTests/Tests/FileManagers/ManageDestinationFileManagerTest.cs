using Dominator.Tests.Utils;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass, Ignore("Issue with resolving inside static constructor, make ManageDestinationFileManager non static")]
    public class ManageDestinationFileManagerTest : UnityInitializationTests
    {

        private IBinFileHelper _binFileHelper;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _binFileHelper = Substitute.For<IBinFileHelper>();
            Container.RegisterInstance<IBinFileHelper>(_binFileHelper);
        }

        [TestMethod]
        public void should_Update_Destination_lists_if_present()
        {
            var postlist = new List<PublisherManageDestinationModel>()
            {
                new PublisherManageDestinationModel() {DestinationId="123" },
                new PublisherManageDestinationModel() {DestinationId="1234", },
                new PublisherManageDestinationModel() {DestinationId="12345", },
            };
            var postlistToupdate = new List<PublisherManageDestinationModel>()
            {
                new PublisherManageDestinationModel() {DestinationId="123" ,DestinationName="First"},
                new PublisherManageDestinationModel() {DestinationId="1234"  ,DestinationName="Second"},
                new PublisherManageDestinationModel() {DestinationId="12345"},
            };
            _binFileHelper.GetPublisherManageDestinationModels().ReturnsForAnyArgs(postlist);
            _binFileHelper.UpdateAllManageDestination(postlist).ReturnsForAnyArgs(true);
            ManageDestinationFileManager.UpdateDestinations(postlistToupdate);

            _binFileHelper.GetPublisherManageDestinationModels().Should().NotBeEmpty().And.HaveCount(3);


        }
        [TestMethod]
        public void should_add_destination_if_destination_is_not_present()
        {
            var postlist = new List<PublisherManageDestinationModel>() {
             new PublisherManageDestinationModel() {DestinationId="3" ,DestinationName="test"},};
            var postlistToupdate = new List<PublisherManageDestinationModel>()
            {
                new PublisherManageDestinationModel() {DestinationId="1" ,DestinationName="First"},
                new PublisherManageDestinationModel() {DestinationId="4"  ,DestinationName="Second"},
                new PublisherManageDestinationModel() {DestinationId="5"},
            };
            _binFileHelper.GetPublisherManageDestinationModels().Returns(postlist);
            _binFileHelper.UpdateAllManageDestination(postlist).Returns(true);
            ManageDestinationFileManager.UpdateDestinations(postlistToupdate);

            _binFileHelper.Received(1).UpdateAllManageDestination(Arg.Any<List<PublisherManageDestinationModel>>());
        }
        [TestMethod]
        public void should_return_destination_if_destinationID_is_matched()
        {
            var postlist = new List<PublisherManageDestinationModel>() {
             new PublisherManageDestinationModel() {DestinationId="3" ,DestinationName="test"},
              new PublisherManageDestinationModel() {DestinationId="4" ,DestinationName="test2"},};

            ManageDestinationFileManager.GetAll().ReturnsForAnyArgs(postlist);
            var result = ManageDestinationFileManager.GetByDestinationId("3");
            result.Should().Be(postlist[0]);
        }
        [TestMethod]
        public void should_return_null_if_destinationID_is_not_matched()
        {
            var postlist = new List<PublisherManageDestinationModel>() {
             new PublisherManageDestinationModel() {DestinationId="3" ,DestinationName="test"},
              new PublisherManageDestinationModel() {DestinationId="4" ,DestinationName="test2"},};

            ManageDestinationFileManager.GetAll().ReturnsForAnyArgs(postlist);
            var result = ManageDestinationFileManager.GetByDestinationId("5");
            result.Should().Be(null);
        }
        [TestMethod]
        public void should_return_true_if_destination_is_Successfully_added()
        {
            var post = new PublisherManageDestinationModel() { DestinationId = "3", DestinationName = "test" };
            var result = ManageDestinationFileManager.Add(post);
            result.Should().Be(true);
        }
        [TestMethod]
        public void should_return_true_if_destination_lists_are_Successfully_added()
        {
            var postlist = new List<PublisherManageDestinationModel>() {
             new PublisherManageDestinationModel() {DestinationId="3" ,DestinationName="test"},
              new PublisherManageDestinationModel() {DestinationId="4" ,DestinationName="test2"},};
            var result = ManageDestinationFileManager.AddRange(postlist);
            result.Should().Be(true);
        }
        [TestMethod]
        public void should_delete_destinations_if_matched_predicate()
        {
            var postlist = new List<PublisherManageDestinationModel>() {
             new PublisherManageDestinationModel() {DestinationId="3" ,DestinationName="test"},
              new PublisherManageDestinationModel() {DestinationId="4" ,DestinationName="test2"},};
            var result = ManageDestinationFileManager.AddRange(postlist);
            ManageDestinationFileManager.GetAll().Returns(postlist);
            ManageDestinationFileManager.Delete(x => x.DestinationId == "3");
            //GetAll() return remaning destination after deletion
            ManageDestinationFileManager.GetAll().Should().NotBeEmpty().And.HaveCount(1);
        }
        [TestMethod]
        public void should_not_delete_destinations_if_not_matched_predicate()
        {
            var postlist = new List<PublisherManageDestinationModel>() {
             new PublisherManageDestinationModel() {DestinationId="3" ,DestinationName="test"},
              new PublisherManageDestinationModel() {DestinationId="4" ,DestinationName="test2"},};
            var result = ManageDestinationFileManager.AddRange(postlist);
            ManageDestinationFileManager.GetAll().Returns(postlist);
            ManageDestinationFileManager.Delete(x => x.DestinationId == "13");
            //GetAll() return all destinations if its not deleted
            ManageDestinationFileManager.GetAll().Should().NotBeEmpty().And.HaveCount(2);
        }
    }
}
