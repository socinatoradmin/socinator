using Dominator.Tests.Utils;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class PostlistFileManagerTest : UnityInitializationTests
    {

        private IBinFileHelper _binFileHelper;
        string _campaignId;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _binFileHelper = Substitute.For<IBinFileHelper>();
            Container.RegisterInstance(_binFileHelper);
            _campaignId = Guid.NewGuid().ToString();
        }

        [TestMethod]
        public void should_Update_Post_lists_details()
        {
            var postlist = new List<PublisherPostlistModel>()
            {
                new PublisherPostlistModel() {CampaignId="123", PostId = "1" },
                new PublisherPostlistModel() {CampaignId="1234", PostId = "2" },
                new PublisherPostlistModel() {CampaignId="12345", PostId = "3" },
            };
            var postlistToupdate = new List<PublisherPostlistModel>()
            {
                new PublisherPostlistModel() {CampaignId="123", PostId = "1",FdSellPrice=100 },
                new PublisherPostlistModel() {CampaignId="1234", PostId = "2",FdSellPrice=1000 },
                new PublisherPostlistModel() {CampaignId="12345", PostId = "3" ,FdSellPrice=10000},
            };
            _binFileHelper.GetPublisherPostListModels(_campaignId).ReturnsForAnyArgs(postlist);
            _binFileHelper.UpdateAllPostlists(_campaignId, postlist).ReturnsForAnyArgs(true);
            PostlistFileManager.UpdatePostlists(_campaignId, postlistToupdate);

            _binFileHelper.GetPublisherPostListModels(_campaignId).Should().NotBeEmpty().And.HaveCount(3);


        }
        [TestMethod]
        public void should_Update_single_Post_details()
        {
            var postlist = new List<PublisherPostlistModel>()
            {
                new PublisherPostlistModel() {CampaignId="123",PostId="1" },
                new PublisherPostlistModel() {CampaignId="1234" },
                new PublisherPostlistModel() {CampaignId="12345" },
            };
            var postToupdate = new PublisherPostlistModel() { CampaignId = "123", PostId = "1", FdSellPrice = 100 };

            _binFileHelper.GetPublisherPostListModels(_campaignId).ReturnsForAnyArgs(postlist);
            _binFileHelper.UpdateAllPostlists(_campaignId, postlist).ReturnsForAnyArgs(true);
            PostlistFileManager.UpdatePost(_campaignId, postToupdate);

            _binFileHelper.GetPublisherPostListModels(_campaignId).Should().NotBeEmpty().And.HaveCount(3);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void should_throw_ArgumentOutOfRangeException_if_post_isnot_present()
        {
            var postlist = new List<PublisherPostlistModel>()
            {
                new PublisherPostlistModel() {CampaignId="123",PostId="1" },
           };
            var postToupdate = new PublisherPostlistModel() { CampaignId = "123", PostId = Guid.NewGuid().ToString(), FdSellPrice = 100 };

            _binFileHelper.GetPublisherPostListModels(_campaignId).ReturnsForAnyArgs(postlist);
            _binFileHelper.UpdateAllPostlists(_campaignId, postlist).ReturnsForAnyArgs(true);
            PostlistFileManager.UpdatePost(_campaignId, postToupdate);
        }

    }
}
