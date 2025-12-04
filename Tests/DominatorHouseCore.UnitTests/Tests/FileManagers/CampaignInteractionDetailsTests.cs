using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class CampaignInteractionDetailsTests
    {
        private IBinFileHelper _binFileHelper;
        private ICampaignInteractionDetails _sut;

        [TestInitialize]
        public void SetUp()
        {
            _binFileHelper = Substitute.For<IBinFileHelper>();
            _sut = new CampaignInteractionDetails(_binFileHelper);
        }

        [TestMethod]
        public void should_add_campaign_if_not_exist()
        {
            // arrange
            var interactedData = "some data";
            var network = SocialNetworks.Reddit;
            var campaignId = "someCampaignId";
            _binFileHelper.GetCampaignInteractedDetails(network).Returns(new List<CampaignInteractionViewModel>());

            // act
            _sut.AddInteractedData(network, campaignId, interactedData);

            // assert
            _sut[network, campaignId].InteractedData.ContainsKey(interactedData).Should().BeTrue();
            _binFileHelper.Received(1).UpdateCampaignInteractedDetails(
                Arg.Is((List<CampaignInteractionViewModel> a) => a[0].CampaignInteractedCollections[campaignId]
                    .InteractedData.ContainsKey(interactedData)), network);
        }

        [TestMethod]
        public void should_add_interacted_data_to_the_existing_campaign()
        {
            // arrange
            var interactedDataOld = "OldData";
            var interactedDataNew = "newData";
            var network = SocialNetworks.Reddit;
            var campaignId = "someCampaignId";
            _binFileHelper.GetCampaignInteractedDetails(network).Returns(new List<CampaignInteractionViewModel>
            {
                new CampaignInteractionViewModel
                {
                    CampaignInteractedCollections = new Dictionary<string, CampaignInteractionDataModel>
                    {
                        {
                            campaignId,
                            new CampaignInteractionDataModel
                            {
                                CampaignId = campaignId,
                                InteractedData = new SortedList<string, DateTime> {{interactedDataOld, DateTime.Now}}
                            }
                        }
                    }
                }
            });

            // act
            _sut.AddInteractedData(network, campaignId, interactedDataNew);

            // assert
            _sut[network, campaignId].InteractedData.Count.Should().Be(2);
            _binFileHelper.Received(1).UpdateCampaignInteractedDetails(
                Arg.Is((List<CampaignInteractionViewModel> a) => a[0].CampaignInteractedCollections[campaignId]
                                                                     .InteractedData.ContainsKey(interactedDataOld) && a[0]
                                                                     .CampaignInteractedCollections[campaignId]
                                                                     .InteractedData.ContainsKey(interactedDataNew)),
                network);
        }

        [TestMethod]
        public void should_NOT_save_anything_if_removing_does_not_happen()
        {
            // arrange
            var interactedData = "some data";
            var network = SocialNetworks.Reddit;
            var campaignId = "someCampaignId";
            _binFileHelper.GetCampaignInteractedDetails(network).Returns(new List<CampaignInteractionViewModel>());

            // act
            _sut.RemoveIfExist(network, campaignId, interactedData);

            // assert
            _binFileHelper.DidNotReceive().UpdateCampaignInteractedDetails(Arg.Any<List<CampaignInteractionViewModel>>(), Arg.Any<SocialNetworks>());
        }

        [TestMethod]
        public void should_remove_interacted_data()
        {

            // arrange
            var dataToDelete = "OldData";
            var network = SocialNetworks.Reddit;
            var campaignId = "someCampaignId";
            _binFileHelper.GetCampaignInteractedDetails(network).Returns(new List<CampaignInteractionViewModel>
            {
                new CampaignInteractionViewModel
                {
                    CampaignInteractedCollections = new Dictionary<string, CampaignInteractionDataModel>
                    {
                        {
                            campaignId,
                            new CampaignInteractionDataModel
                            {
                                CampaignId = campaignId,
                                InteractedData = new SortedList<string, DateTime> {{dataToDelete, DateTime.Now}}
                            }
                        }
                    }
                }
            });

            // act
            _sut.RemoveIfExist(network, campaignId, dataToDelete);

            // assert
            _sut[network, campaignId].InteractedData.Count.Should().Be(0);
            _binFileHelper.Received(1).UpdateCampaignInteractedDetails(
                Arg.Is((List<CampaignInteractionViewModel> a) => a[0].CampaignInteractedCollections[campaignId]
                                                                     .InteractedData.Count == 0),
                network);
        }
    }
}
