using System;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDModel.LikerCommentorModel;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class DownloadPhotosProcessTest : BaseFacebookProcessTest
    {

        private DownloadPhotosProcess _sut;
        private ActivityType _activityType;
        private string _userName;
        private string _accountId;
        private string _templateId;
        private ScrapeResultNew _scrapeResultNew;
        private PostLikerModel _postLikerModel;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _userName = "User Name";
            _accountId = "ID123";
            _templateId = "T123";

            _activityType = ActivityType.DownloadScraper;
            ProcessScopeModel = Substitute.For<IProcessScopeModel>();
            ProcessScopeModel.Account.Returns(new DominatorAccountModel
            {
                AccountId = _accountId,
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    AccountNetwork = SocialNetworks.Facebook,
                    UserName = _userName,
                }
            });

            ProcessScopeModel.ActivityType.Returns(_activityType);
            Container.RegisterInstance(BinFileHelper);
            var moduleSetting = new ModuleSetting();
            ProcessScopeModel.GetActivitySettingsAs<ModuleSetting>().Returns(moduleSetting);
            _postLikerModel = new PostLikerModel();
            ProcessScopeModel.GetActivitySettingsAs<PostLikerModel>().Returns(_postLikerModel);

            _sut = new DownloadPhotosProcess(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary,DbCampaignServiceScoped);

        }

        [TestMethod]
        public void Should_DownloadPhotos_And_Save_Result_In_Database()
        {

            FacebookPostDetails facebookPostDetails = new FacebookPostDetails();
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = facebookPostDetails,
                QueryInfo = QueryInfo.NoQuery
            };

            ProcessScopeModel.TemplateId.Returns(_templateId);


            var moduleConfigurations = new ModuleConfiguration
            {
                ActivityType = ActivityType.DownloadScraper,
                IsTemplateMadeByCampaignMode = true,
                TemplateId = _templateId
            };

            JobActivityConfigurationManager[_accountId, ActivityType.DownloadScraper]
                .Returns(moduleConfigurations);

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            DbAccountServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts>());
            DbCampaignServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts>());
            jpr.IsProcessSuceessfull.Should().Be(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void should_throw_null_ref_exception_if_scrapeResultNew_is_null()
        {
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            jpr.IsProcessSuceessfull.Should().Be(false);
        }

    }
}
