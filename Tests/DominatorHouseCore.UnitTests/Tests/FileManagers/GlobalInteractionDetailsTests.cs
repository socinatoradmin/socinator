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
    public class GlobalInteractionDetailsTests
    {
        private IBinFileHelper _binFileHelper;
        private IGlobalInteractionDetails _sut;

        [TestInitialize]
        public void SetUp()
        {
            _binFileHelper = Substitute.For<IBinFileHelper>();
            _sut = new GlobalInteractionDetails(_binFileHelper);
        }

        [TestMethod]
        public void should_add_campaign_if_not_exist()
        {
            // arrange
            var interactedData = "some data";
            var network = SocialNetworks.Reddit;
            var activityType = ActivityType.BoardScraper;
            _binFileHelper.GetGlobalInteractedDetails(network).Returns(new List<GlobalInteractionViewModel>());

            // act
            _sut.AddInteractedData(network, activityType, interactedData);

            // assert
            _sut[network, activityType].InteractedData.ContainsKey(interactedData).Should().BeTrue();
            _binFileHelper.Received(1).UpdateGlobalInteractedDetails(
                Arg.Is((List<GlobalInteractionViewModel> a) => a[0].GlobalInteractedCollections[activityType]
                    .InteractedData.ContainsKey(interactedData)), network);
        }

        [TestMethod]
        public void should_add_interacted_data_to_the_existing_campaign()
        {
            // arrange
            var interactedDataOld = "OldData";
            var interactedDataNew = "newData";
            var network = SocialNetworks.Reddit;
            var activityType = ActivityType.BoardScraper;
            _binFileHelper.GetGlobalInteractedDetails(network).Returns(new List<GlobalInteractionViewModel>
            {
                new GlobalInteractionViewModel
                {
                    GlobalInteractedCollections = new Dictionary<ActivityType, GlobalInteractionDataModel>
                    {
                        {
                            activityType,
                            new GlobalInteractionDataModel
                            {
                                InteractedData = new SortedList<string, DateTime> {{interactedDataOld, DateTime.Now}}
                            }
                        }
                    }
                }
            });

            // act
            _sut.AddInteractedData(network, activityType, interactedDataNew);

            // assert
            _sut[network, activityType].InteractedData.Count.Should().Be(2);
            _binFileHelper.Received(1).UpdateGlobalInteractedDetails(
                Arg.Is((List<GlobalInteractionViewModel> a) => a[0].GlobalInteractedCollections[activityType]
                                                                     .InteractedData.ContainsKey(interactedDataOld) && a[0]
                                                                     .GlobalInteractedCollections[activityType]
                                                                     .InteractedData.ContainsKey(interactedDataNew)),
                network);
        }

        [TestMethod]
        public void should_NOT_save_anything_if_removing_does_not_happen()
        {
            // arrange
            var interactedData = "some data";
            var network = SocialNetworks.Reddit;
            var activityType = ActivityType.BoardScraper;
            _binFileHelper.GetGlobalInteractedDetails(network).Returns(new List<GlobalInteractionViewModel>());

            // act
            _sut.RemoveIfExist(network, activityType, interactedData);

            // assert
            _binFileHelper.DidNotReceive().UpdateGlobalInteractedDetails(Arg.Any<List<GlobalInteractionViewModel>>(), Arg.Any<SocialNetworks>());
        }

        [TestMethod]
        public void should_remove_interacted_data()
        {

            // arrange
            var dataToDelete = "OldData";
            var network = SocialNetworks.Reddit;
            var activityType = ActivityType.BoardScraper;
            _binFileHelper.GetGlobalInteractedDetails(network).Returns(new List<GlobalInteractionViewModel>
            {
                new GlobalInteractionViewModel
                {
                    GlobalInteractedCollections = new Dictionary<ActivityType, GlobalInteractionDataModel>
                    {
                        {
                            activityType,
                            new GlobalInteractionDataModel
                            {
                                InteractedData = new SortedList<string, DateTime> {{dataToDelete, DateTime.Now}}
                            }
                        }
                    }
                }
            });

            // act
            _sut.RemoveIfExist(network, activityType, dataToDelete);

            // assert
            _sut[network, activityType].InteractedData.Count.Should().Be(0);
            _binFileHelper.Received(1).UpdateGlobalInteractedDetails(
                Arg.Is((List<GlobalInteractionViewModel> a) => a[0].GlobalInteractedCollections[activityType]
                                                                     .InteractedData.Count == 0),
                network);
        }
    }
}
