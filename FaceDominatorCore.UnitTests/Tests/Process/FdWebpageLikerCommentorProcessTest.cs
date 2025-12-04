using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class FdWebpageLikerCommentorProcessTest : BaseFacebookProcessTest
    {
        private FdWebpageLikerCommentorProcess _sut;
        private ActivityType _activityType;
        private string _userId;
        private string _accountId;
        private string _templateId;
        private ScrapeResultNew _scrapeResultNew;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _userId = "User ID";
            _accountId = "ID123";
            _templateId = "T123";

            _activityType = ActivityType.LikeComment;
            ProcessScopeModel = Substitute.For<IProcessScopeModel>();
            ProcessScopeModel.Account.Returns(new DominatorAccountModel
            {
                AccountId = _accountId,
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    AccountNetwork = SocialNetworks.Facebook,

                }
            });

            ProcessScopeModel.ActivityType.Returns(_activityType);
            var moduleSetting = new ModuleSetting();
            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);

            _sut = new FdWebpageLikerCommentorProcess(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, DbCampaignServiceScoped);

        }

        [TestMethod]
        public void Should_FdWebpageLikerCommentor_And_Save_Result_In_Database()
        {
        
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultUser = new FacebookUser(),
                QueryInfo = QueryInfo.NoQuery
            };

            ProcessScopeModel.TemplateId.Returns(_templateId);


            var moduleConfigurations = new ModuleConfiguration
            {
                ActivityType = ActivityType.AutoReplyToNewMessage,
                IsTemplateMadeByCampaignMode = true,
                TemplateId = _templateId
            };

            JobActivityConfigurationManager[_accountId, ActivityType.AutoReplyToNewMessage]
                .Returns(moduleConfigurations);

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            jpr.IsProcessSuceessfull.Should().Be(false);
        }
    }
}
