using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.JobConfigurations;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NSubstitute;
using System.Collections.ObjectModel;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Process
{
    [TestClass]
    public class JobConfigurationProviderTests : UnityInitializationTests
    {
        private IJobConfigurationProvider _sut;
        private IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private ITemplatesFileManager _templatesFileManager;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _jobActivityConfigurationManager = Substitute.For<IJobActivityConfigurationManager>();
            _templatesFileManager = Substitute.For<ITemplatesFileManager>();
            _sut = new JobConfigurationProvider(_jobActivityConfigurationManager, _templatesFileManager);
            Container.RegisterInstance(Substitute.For<IDateProvider>());
        }

        [TestMethod]
        public void should_return_common_elements_of_configuration()
        {
            // arrange
            var accountId = "a account";
            var activityType = ActivityType.Delete;
            var moduleConfiguration = new ModuleConfiguration { TemplateId = "a temnplate" };
            var jc = new JobConfiguration
            {
                ActivitiesPerJob = new RangeUtilities(1, 5),
                ActivitiesPerHour = new RangeUtilities(1, 5),
                ActivitiesPerDay = new RangeUtilities(1, 5),
                ActivitiesPerWeek = new RangeUtilities(1, 5)
            };
            var sq = new ObservableCollection<QueryInfo>() { QueryInfo.NoQuery };
            var temmplateModel = new TemplateModel
            {
                ActivitySettings = JsonConvert.SerializeObject(new
                {
                    JobConfiguration = jc,
                    SavedQueries = sq,
                    IsNeedToStart = true
                })
            };
            _jobActivityConfigurationManager[accountId, activityType].Returns(moduleConfiguration);
            _templatesFileManager.GetTemplateById(moduleConfiguration.TemplateId).Returns(temmplateModel);

            // act
            var result = _sut.GetJobConfiguration(accountId, activityType);

            // assert
            result.JobConfiguration.Should().NotBeNull();
            result.SavedQueries.Should().NotBeNullOrEmpty();
            result.IsNeedToSchedule.Should().BeTrue();
        }
    }
}
