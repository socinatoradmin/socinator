using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorCore.FDModel.CommonSettings;
using FaceDominatorCore.FDResponse.LikeCommentsResponse;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Reflection;
using Castle.Components.DictionaryAdapter;
using Castle.DynamicProxy.Generators;
using CommonServiceLocator;
using FaceDominatorCore.FDModel.LikerCommentorModel;

namespace FaceDominatorCore.UnitTests.Tests.Process
{
    [TestClass]
    public class PostCommentorProcessTest : BaseFacebookProcessTest
    {
        private PostCommentorProcess _sut;
        private ActivityType _activityType;
        private string _userId;
        private string _accountId;
        private string _templateId;
        private ScrapeResultNew _scrapeResultNew;
        private PostCommentorModel _postCommentorModel;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            _userId = "User ID";
            _accountId = "ID123";
            _templateId = "T123";

            _activityType = ActivityType.PostCommentor;
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
            _postCommentorModel = new PostCommentorModel();
            ProcessScopeModel.GetActivitySettingsAs<PostCommentorModel>().Returns(_postCommentorModel);

            _sut = new PostCommentorProcess(ProcessScopeModel, DbAccountServiceScoped, FdQueryScraperFactory, FdHttpHelper, FdLoginProcess, FdRequestLibrary, DbCampaignServiceScoped, DelayService);

        }

        [TestMethod]
        public void Should_CommentOnPost_And_Save_Result_In_Database()
        {
            var response = TestUtils.ReadFileFromResources
                 ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.CommentOnPostResponse.html", Assembly.GetExecutingAssembly());

            var responseParameter = new ResponseParameter
            {
                Response = response,
                Exception = null,
                HasError = false
            };

            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new FacebookPostDetails(),
                ResultUser = new FacebookUser(),
                QueryInfo = new QueryInfo()
            };

            ProcessScopeModel.TemplateId.Returns(_templateId);
          
            ProcessScopeModel.GetActivitySettingsAs<CampaignDetails>().Returns(new CampaignDetails
            {
                CampaignId = "45765",
            });

            var moduleConfigurations = new ModuleConfiguration
            {
                ActivityType = _activityType,
                IsTemplateMadeByCampaignMode = true,
                TemplateId = _templateId
            };

            JobActivityConfigurationManager[_accountId, _activityType]
                .Returns(moduleConfigurations);

            var commentOnPostResponseHandler = new CommentOnPostResponseHandler(responseParameter);

            FdRequestLibrary.CommentOnPost
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns(commentOnPostResponseHandler);

            var demosList = new List<CampaignDetails>
                {
                    new CampaignDetails {CampaignId = ""}
                };

            _postCommentorModel.LikerCommentorConfigModel.LstManageCommentModel.Add(new ManageCommentModel
            {
                CommentText = "hai",
                SelectedQuery = new ObservableCollection<QueryContent>
                    {
                        new QueryContent
                        { IsContentSelected = true,
                            Content = new QueryInfo()
                            {
                                QueryValue = "",
                                QueryType = ""
                            } }
                    }
            }
            );

            var binFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
            binFileHelper.GetCampaignDetail().Returns(demosList);

            // act
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            // assert
            FdRequestLibrary.Received(1).CommentOnPost
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            DbAccountServiceScoped.Received(1).Add((Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Accounts.InteractedPosts>()));
            DbCampaignServiceScoped.Received(1).Add(Arg.Any<DominatorHouseCore.DatabaseHandler.FdTables.Campaigns.InteractedPosts>());
            jpr.IsProcessSuceessfull.Should().Be(true);
        }

        [TestMethod]
        public void should_return_False_IsProcessSucessfull_is_False()
        {
            _scrapeResultNew = new ScrapeResultNew
            {
                ResultPost = new FacebookPostDetails(),
                ResultUser = new FacebookUser(),
                QueryInfo = new QueryInfo()
            };

            var binFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
            binFileHelper.GetCampaignDetail().Returns(new EditableList<CampaignDetails>());

            FdRequestLibrary.CommentOnPost
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()).Returns((CommentOnPostResponseHandler)null);

            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            FdRequestLibrary.Received(0).CommentOnPost
                (Arg.Any<DominatorAccountModel>(), Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>());
            jpr.IsProcessSuceessfull.Should().Be(false);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void should_throw_null_ref_exception_if_scrapeResultNew_is_null()
        {
            var jpr = _sut.PostScrapeProcess(_scrapeResultNew);

            jpr.IsProcessSuceessfull.Should().Be(false);
        }

    }
}
