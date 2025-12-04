using Dominator.Tests.Utils;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Process
{
    [TestClass, Ignore("rewrite static class for unit testing")]
    public class PublishSchedulerTest : UnityInitializationTests
    {
        string campaignId;
        IGenericFileManager _genericFileManager;
        IAccountsFileManager _accountFileManager;
        IAccountScopeFactory _accountScopeFactory;
     
        [TestInitialize]
        public void StartUp()
        {
            base.SetUp();
            _genericFileManager = Substitute.For<IGenericFileManager>();
            Container.RegisterInstance(_genericFileManager);

            campaignId = Guid.NewGuid().ToString();
        }
        [TestMethod]
        public void should_increase_running_count_by_one_if_campaignid_is_present()
        {
            var actualRunningCount = 2;
            var expectedRunningCount = 3;
            PublishScheduler.AttachedActionCounts.GetOrAdd(campaignId, actualRunningCount);
            PublishScheduler.IncreasePublishingCount(campaignId);
            PublishScheduler.AttachedActionCounts[campaignId].Should().Be(expectedRunningCount);
        }
        [TestMethod]
        public void should_add_campaignid_and_runningcount_should_be_1_if_campaignid_is_not_present()
        {
            var expectedRunningCount = 1;
            PublishScheduler.IncreasePublishingCount(campaignId);
            PublishScheduler.AttachedActionCounts[campaignId].Should().Be(expectedRunningCount);
        }
        [TestMethod]
        public void should_decrease_running_count_by_one_if_campaignid_is_present()
        {
            var actualRunningCount = 2;
            var expectedRunningCount = 1;
            PublishScheduler.AttachedActionCounts.GetOrAdd(campaignId, actualRunningCount);
            PublishScheduler.DecreasePublishingCount(campaignId);
            PublishScheduler.AttachedActionCounts[campaignId].Should().Be(expectedRunningCount);
        }
        [TestMethod]
        public void should_Run_And_Remove_Action_if_action_is_present_in_campaign_PublisherActionList()
        {
            var input = new LinkedList<Action>();
            input.AddFirst(() => PublishScheduler.DecreasePublishingCount(campaignId));
            PublishScheduler.PublisherActionList.GetOrAdd(campaignId, input);
            PublishScheduler.RunAndRemovePublisherAction(campaignId);
            PublishScheduler.PublisherActionList[campaignId].Should().BeEmpty();
        }

        [TestMethod]
        public void should_start_publishing_post()
        {
            var publisherPostFetchModel = new PublisherPostFetchModel();
            _genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).ReturnsForAnyArgs(new List<PublisherPostFetchModel>() { publisherPostFetchModel });

            var publishedPostDetailsModel = new PublishedPostDetailsModel();
            _genericFileManager.GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails)
                .ReturnsForAnyArgs(new List<PublishedPostDetailsModel>() { publishedPostDetailsModel });

            var generalmodel = new GeneralModel();
            _genericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
            .ReturnsForAnyArgs(new List<GeneralModel> { generalmodel });

            var input = new PublisherCampaignStatusModel();
            PublishScheduler.StartPublishingPosts(input);
            _genericFileManager.Received(2).GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social));
            _genericFileManager.Received(1).GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails);
            _genericFileManager.Received(1).GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile);
        }
        [TestMethod]
        public void should_Schedule_PublishNow_ByCampaign()
        {
            var publisherPostFetchModel = new PublisherPostFetchModel();
            _genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).ReturnsForAnyArgs(new List<PublisherPostFetchModel>() { publisherPostFetchModel });

            var publishedPostDetailsModel = new PublishedPostDetailsModel();
            _genericFileManager.GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails)
                .ReturnsForAnyArgs(new List<PublishedPostDetailsModel>() { publishedPostDetailsModel });

            var generalmodel = new GeneralModel();
            _genericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
            .ReturnsForAnyArgs(new List<GeneralModel> { generalmodel });

            var publisherCampaignStatusModel = new PublisherCampaignStatusModel
            {
                CampaignId = campaignId,
                ScheduledWeekday = new List<ContentSelectGroup>
                    {
                        new ContentSelectGroup
                        {
                            Content= DateTime.Now.DayOfWeek.ToString(),
                            IsContentSelected=true
                        }
                    }
            };
            PublisherInitialize.GetInstance.ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
            PublisherInitialize.GetInstance.GetSavedCampaigns().FirstOrDefault(x => x.CampaignId == campaignId).Should().Be(publisherCampaignStatusModel);
            var specificCampaign = new PublisherCampaignStatusModel { CampaignId = campaignId };

            PublishScheduler.ValidateCampaignsTime(specificCampaign).Should().Be(true);

            PublishScheduler.SchedulePublishNowByCampaign(campaignId);

            _genericFileManager.Received(2).GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social));
            _genericFileManager.Received(1).GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails);
            _genericFileManager.Received(1).GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile);

        }


        [TestMethod]
        public void should_stop_Publishing_Campaign()
        {
            var actualRunningCount = 2;
            var expectedRunningCount = 1;
            PublishScheduler.AttachedActionCounts.GetOrAdd(campaignId, actualRunningCount);
            PublishScheduler.PublisherScheduledList.Add(campaignId);
            PublishScheduler.CampaignsCancellationTokens.Add(campaignId, new System.Threading.CancellationTokenSource());
            PublishScheduler.StopPublishingPosts(campaignId);
            PublishScheduler.AttachedActionCounts[campaignId].Should().Be(expectedRunningCount);
            PublishScheduler.CampaignsCancellationTokens.Should().BeEmpty();
        }


        [TestMethod]
        public void should_enable_delete_post()
        {
            _accountFileManager = Substitute.For<IAccountsFileManager>();
            Container.RegisterInstance(_accountFileManager);
            _accountScopeFactory = Substitute.For<IAccountScopeFactory>();
            Container.RegisterInstance(_accountFileManager);

            var postDeletionModel = new PostDeletionModel()
            {
                Networks = SocialNetworks.Social,
                CampaignId = campaignId,
                AccountId = Guid.NewGuid().ToString(),
                DeletionTime = DateTime.Now
            };
            _genericFileManager.AddModule(postDeletionModel,
              ConstantVariable.GetDeletePublisherPostModel).ReturnsForAnyArgs(true);
            var generalmodel = new GeneralModel();
            _genericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
            .ReturnsForAnyArgs(new List<GeneralModel> { generalmodel });

            var publisherCreateCampaignModel = new PublisherCreateCampaignModel
            {
                CampaignId = campaignId,
                JobConfigurations = new DominatorHouseCore.Models.Publisher.JobConfigurationModel(),
                OtherConfiguration = new DominatorHouseCore.Models.Publisher.OtherConfigurationModel()
            };
            _genericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile())
            .ReturnsForAnyArgs(new List<PublisherCreateCampaignModel> { publisherCreateCampaignModel });

            PublishScheduler.EnableDeletePost(postDeletionModel);
            _genericFileManager.Received(1).AddModule(postDeletionModel,
              ConstantVariable.GetDeletePublisherPostModel);
            _genericFileManager.Received(0).GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile());

        }

        [TestMethod]
        public void should_Schedule_Todays_Publisher()
        {
            var publisherCampaignStatusModel = new PublisherCampaignStatusModel
            {
                CampaignId = campaignId,
                ScheduledWeekday = new List<ContentSelectGroup>
                    {
                        new ContentSelectGroup
                        {
                            Content= DateTime.Now.DayOfWeek.ToString(),
                            IsContentSelected=true
                        }
                    },
                Status = PublisherCampaignStatus.Active
            };

            var lstpublisherCampaignStatusModel = new List<PublisherCampaignStatusModel> { publisherCampaignStatusModel };
            PublisherInitialize.GetInstance.ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
            PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => x.Status == PublisherCampaignStatus.Active).Should().BeEquivalentTo(lstpublisherCampaignStatusModel);
            var specificCampaign = new PublisherCampaignStatusModel { CampaignId = campaignId };

            PublishScheduler.ValidateCampaignsTime(specificCampaign).Should().Be(true);

            PublishScheduler.ScheduleTodaysPublisher();
        }

        [TestMethod]
        public void should_Schedule_Todays_Publisher_By_Campaign()
        {
            var publisherCampaignStatusModel = new PublisherCampaignStatusModel
            {
                CampaignId = campaignId,
                ScheduledWeekday = new List<ContentSelectGroup>
                    {
                        new ContentSelectGroup
                        {
                            Content= DateTime.Now.DayOfWeek.ToString(),
                            IsContentSelected=true
                        }
                    },
                SpecificRunningTime = new List<TimeSpan>
                {
                    new TimeSpan(18,20,30)
                }
            };
            var generalmodel = new GeneralModel();
            _genericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
            .ReturnsForAnyArgs(new List<GeneralModel> { generalmodel });
            PublisherInitialize.GetInstance.ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
            PublisherInitialize.GetInstance.GetSavedCampaigns().FirstOrDefault(x => x.CampaignId == campaignId).Should().Be(publisherCampaignStatusModel);
            var specificCampaign = new PublisherCampaignStatusModel { CampaignId = campaignId };

            PublishScheduler.ValidateCampaignsTime(specificCampaign).Should().Be(true);

            PublishScheduler.ScheduleTodaysPublisherByCampaign(campaignId);
            _genericFileManager.Received(1).GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social));
        }
    }
}
