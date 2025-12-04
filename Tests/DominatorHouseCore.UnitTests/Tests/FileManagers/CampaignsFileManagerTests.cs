using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class CampaignsFileManagerTests
    {
        private ICampaignsFileManager _sut;
        private IBinFileHelper _binFileHelper;

        [TestInitialize]
        public void SetUp()
        {
            _binFileHelper = Substitute.For<IBinFileHelper>();
            _sut = new CampaignsFileManager(_binFileHelper);
        }

        [TestMethod]
        public void should_return_campaigns_by_network()
        {
            // arrange
            var campaigns = new List<CampaignDetails> { new CampaignDetails { SocialNetworks = SocialNetworks.Twitter } };

            _binFileHelper.GetCampaignDetail().Returns(campaigns);

            // act
            //var result = _sut.GetCampaignByNetwork(SocialNetworks.Twitter);

            //// assert
            //result.Count.Should().Be(1);
            //result.Single().Should().Be(campaigns.First());
        }

        [TestMethod]
        public void should_return_campaigns_by_id()
        {
            // arrange
            var campaigns = new List<CampaignDetails> { new CampaignDetails { CampaignId = "123" } };

            _binFileHelper.GetCampaignDetail().Returns(campaigns);

            // act
            var result = _sut.GetCampaignById("123");

            // assert
            result.Should().Be(campaigns.FirstOrDefault());
        }

        [TestMethod]
        public void should_add_new_campaign()
        {
            // arrange
            var campaigns = new List<CampaignDetails> { new CampaignDetails { CampaignId = "123" } };
            var newCampaign = new CampaignDetails { CampaignId = "1234" };
            _binFileHelper.GetCampaignDetail().Returns(campaigns);

            // act
            _sut.Add(newCampaign);
            var result = _sut.GetCampaignById("1234");

            // assert
            result.Should().Be(newCampaign);
            _binFileHelper.Received(1).Append(newCampaign);
            _sut.Count().Should().Be(2);
        }

        [TestMethod]
        public void should_delete_campaign()
        {
            // arrange
            var campaigns = new List<CampaignDetails> { new CampaignDetails { CampaignId = "123" } };
            _binFileHelper.GetCampaignDetail().Returns(campaigns);

            // act
            _sut.Delete(campaigns.FirstOrDefault());
            var result = _sut.GetCampaignById("123");

            // assert
            result.Should().Be(null);
            _binFileHelper.Received(1).UpdateCampaigns(campaigns);
            _sut.Should().BeEmpty();
        }

        [TestMethod]
        public void should_delete_account_from_all_the_campaigns_by_template_id()
        {
            // arrange
            var campaign1 = new CampaignDetails
            {
                CampaignId = "123",
                TemplateId = "123",
                SelectedAccountList = new List<string> { "accountNameToDelete" }
            };
            var campaign2 = new CampaignDetails
            {
                CampaignId = "321",
                TemplateId = "321",
                SelectedAccountList = new List<string> { "accountNameToDelete" }
            };
            var campaigns = new List<CampaignDetails>
            {
                campaign1,campaign2
            };
            _binFileHelper.GetCampaignDetail().Returns(campaigns);

            // act
            _sut.DeleteSelectedAccount("321", "accountNameToDelete");

            // assert
            campaign1.SelectedAccountList.Should().NotBeEmpty();
            campaign2.SelectedAccountList.Should().BeEmpty();
        }

        [TestMethod]
        public void should_update_campaign()
        {
            // arrange
            var campaign1 = new CampaignDetails
            {
                CampaignId = "123",
                TemplateId = "123",
            };
            var campaign2 = new CampaignDetails
            {
                CampaignId = "123",
                TemplateId = "321",
            };
            var campaigns = new List<CampaignDetails>
            {
                campaign1
            };
            _binFileHelper.GetCampaignDetail().Returns(campaigns);

            // act
            _sut.Edit(campaign2);

            // assert
            _sut.Count().Should().Be(1);
            _sut.Single().Should().Be(campaign2);
        }

        [TestMethod]
        public void should_add_and_update_campaigns()
        {
            // arrange
            var campaign1 = new CampaignDetails
            {
                CampaignId = "campaign1",
                TemplateId = "123",
            };
            var campaign1New = new CampaignDetails
            {
                CampaignId = "campaign1",
                TemplateId = "323124",
            };

            var campaign2 = new CampaignDetails
            {
                CampaignId = "campaign2",
                TemplateId = "321",
            };

            var campaigns = new List<CampaignDetails>
            {
                campaign1
            };
            var campaignsNew = new List<CampaignDetails>
            {
                campaign1New,campaign2
            };
            _binFileHelper.GetCampaignDetail().Returns(campaigns);

            // act
            _sut.UpdateCampaigns(campaignsNew);

            // assert
            _sut.Count().Should().Be(2);
            _sut.Should().BeEquivalentTo(campaign1New, campaign2);
        }
    }
}
