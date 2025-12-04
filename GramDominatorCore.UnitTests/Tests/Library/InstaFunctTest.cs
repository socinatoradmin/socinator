using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using FluentAssertions;
using NSubstitute;
using Unity;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DominatorHouseCore.Request;
using System.Reflection;
using System.Threading;
using System.Text;
using System.Net;
using System.Collections.Generic;
using DominatorHouseCore.Utility;
using GramDominatorCore.Response;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using DominatorHouse.ThreadUtils;
using System.IO;
using System.Drawing;
using System;
using System.Threading.Tasks;

namespace GramDominatorCore.UnitTests.Test_Data.Library
{
    [TestClass]
    public class InstaFunctTest : UnityInitializationTests
    {
        private IInstaFunction _sut;
        private IAccountsFileManager _accountsFileManager;
        private IAccountsCacheService _accountsCacheService;
        private DominatorAccountModel _dominatorAccountModel;
        private IDateProvider _dateProvider;
        private IGdLogInProcess gdloginProcess;
        private CancellationToken CancellationToken { get; }
        private IGdHttpHelper httpHelper;
        private IResponseParameter responseParameter;
        private IgRequestParameters igRequestParameter;
        private IGdBrowserManager gdBrowserManager;
        private IDelayService delayService;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _dominatorAccountModel = new DominatorAccountModel
            {
                AccountBaseModel = new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333" },
            };
            httpHelper = Substitute.For<IGdHttpHelper>();
            igRequestParameter = Substitute.For<IgRequestParameters>();
            responseParameter = Substitute.For<IResponseParameter>();
            _accountsFileManager = Substitute.For<IAccountsFileManager>();
            _accountsCacheService = Substitute.For<IAccountsCacheService>();
            gdBrowserManager = Substitute.For<IGdBrowserManager>();
            _dateProvider = Substitute.For<IDateProvider>();
            gdloginProcess = Substitute.For<IGdLogInProcess>();
            delayService = Substitute.For<IDelayService>();
            Container.RegisterInstance(delayService);
            Container.RegisterInstance<IGdBrowserManager>(gdBrowserManager);
            Container.RegisterInstance<IAccountsCacheService>(_accountsCacheService);
            Container.RegisterInstance<IGdLogInProcess>(gdloginProcess);
            Container.RegisterInstance<IHttpHelper>(SocialNetworks.Instagram.ToString(), httpHelper);
            Container.RegisterInstance<IAccountsFileManager>(SocialNetworks.Instagram.ToString(), _accountsFileManager);
            Container.RegisterInstance<IDateProvider>(_dateProvider);
            Container.RegisterInstance<IHttpHelper>(SocialNetworks.Instagram.ToString(), httpHelper);
            _sut = new InstaFunct(_dominatorAccountModel, httpHelper, gdBrowserManager, delayService, _dateProvider);
        }

        public void LoginCommonData(string url, ref DominatorAccountModel account)
        {
            var requestParameter = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameter);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.LoginResponse.json", Assembly.GetExecutingAssembly());
            JsonElements jsonElements = new JsonElements()
            {
                CountryCodes = "[{\"country_code\":\"1\",\"source\":[\"default\"]}]",
                PhoneId = "1cb96047-85dd-4b49-b1a0-c501cf88f50f",
                Csrftoken = "KXenOeCP42mU63MhUgVj3eVCyyXqEg4e",
                Username = "metcalfe_penelope",
                Adid = "a9538f41-6e38-48fe-a1ee-7836a5fd26a1",
                Guid = "a4675f1b-ee87-45da-88d9-f193273fff88",
                DeviceId = "android-5a841e4e3eff8897",
                Password = "kuFuha4",
                GoogleTokens = "[]",
                LoginAttemptCount = "0"
            };
            requestParameter.Body = jsonElements;
            byte[] postData = requestParameter.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=SIGNATURE.%7B%22adid%22%3A%22a9538f41-6e38-48fe-a1ee-7836a5fd26a1%22%2C%22_csrftoken%22%3A%22KXenOeCP42mU63MhUgVj3eVCyyXqEg4e%22%2C%22device_id%22%3A%22android-5a841e4e3eff8897%22%2C%22guid%22%3A%22a4675f1b-ee87-45da-88d9-f193273fff88%22%2C%22login_attempt_count%22%3A%220%22%2C%22password%22%3A%22kuFuha4%22%2C%22phone_id%22%3A%221cb96047-85dd-4b49-b1a0-c501cf88f50f%22%2C%22username%22%3A%22metcalfe_penelope%22%2C%22country_codes%22%3A%22%5B%7B%5C%22country_code%5C%22%3A%5C%221%5C%22%2C%5C%22source%5C%22%3A%5B%5C%22default%5C%22%5D%7D%5D%22%2C%22google_tokens%22%3A%22%5B%5D%22%7D");
                // a.Should().BeSameAs(postData);
            }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram, UserId = "7465184286", UserName = "metcalfe_penelope", Password = "kuFuha4" },
               };
        }
        [DataTestMethod]
        [DataRow("https://i.instagram.com/api/v1/accounts/login/")]
        public void Login_should_be_true_if_LoginAsync_success(string url)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            LoginCommonData(url, ref account);
            AccountModel accountModel = new AccountModel(account);
            accountModel.Device_Id = "android-5a841e4e3eff8897";
            accountModel.PhoneId = "1cb96047-85dd-4b49-b1a0-c501cf88f50f";
            accountModel.AdId = "a9538f41-6e38-48fe-a1ee-7836a5fd26a1";
            accountModel.Guid = "a4675f1b-ee87-45da-88d9-f193273fff88";
            accountModel.CsrfToken = "KXenOeCP42mU63MhUgVj3eVCyyXqEg4e";
            var result = _sut.LoginAsync(account, accountModel);
            result.Result.Success.Should().Be(true);
        }

        [TestMethod]
        [DataRow("https://i.instagram.com/api/v1/accounts/loginn/")]
        public void Login_should_be_false_if_Url_Will_be_Wrong(string url)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            LoginCommonData(url, ref account);
            AccountModel accountModel = new AccountModel(account);
            var result = _sut.LoginAsync(account, accountModel);
            result.Status.Should().Be(System.Threading.Tasks.TaskStatus.Faulted);

        }

        public void SendSecurityCode(string url, ref DominatorAccountModel account)
        {
            var requestParameter = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameter);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SendCodeResponse.json", Assembly.GetExecutingAssembly());
            JsonElements jsonElements = new JsonElements()
            {
                Csrftoken = null,
                Uid = null,
                Uuid = "ca280d5d-ad04-4e4e-a595-8d01c21f08d2",
                DeviceId = "android-4b9ffe5010d6b992",
                Choice = "1"
            };
            requestParameter.Body = jsonElements;
            byte[] postData = requestParameter.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=SIGNATURE.%7B%22device_id%22%3A%22android-4b9ffe5010d6b992%22%2C%22_uuid%22%3A%22ca280d5d-ad04-4e4e-a595-8d01c21f08d2%22%2C%22choice%22%3A%221%22%7D");
            }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram },
               };
            // account.DeviceDetails.DeviceId = "android-4b9ffe5010d6b992";          
        }

        [DataTestMethod]
        [DataRow("https://i.instagram.com/api/v1/challenge/5514264613/CGN8gsQdSy/")]
        public void Login_should_be_true_if_SendSecurityCodeAsync(string url)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            SendSecurityCode(url, ref account);
            AccountModel accountModel = new AccountModel(account);
            string challengeUrl = "/challenge/5514264613/CGN8gsQdSy/";
            var result = _sut.SendSecurityCodeAsync(challengeUrl,"", "1", account, accountModel);
            result.Result.Success.Should().Be(true);
        }


        [TestMethod]
        [DataRow("https://i.instagram.com/api/v1/challenge/5514264613//")]
        public void Login_should_be_false_if_Challange_code_Will_be_empty_SendSecurityCodeAsync(string url)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            SendSecurityCode(url, ref account);
            AccountModel accountModel = new AccountModel(account);
            accountModel.Uuid = "ca280d5d-ad04-4e4e-a595-8d01c21f08d2";
            accountModel.Device_Id = "android-4b9ffe5010d6b992";
            var result = _sut.SendSecurityCodeAsync("challenge/5514264613/", "1","", account, accountModel);
            result.Status.Should().Be(System.Threading.Tasks.TaskStatus.RanToCompletion);
        }

        public void VerificationAccount(string securityCode, ref DominatorAccountModel account)
        {
            var requestParameter = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameter);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.VerificationResponse.json", Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/challenge/5514264613/CGN8gsQdSy/";
            JsonElements jsonElements = new JsonElements()
            {
                DeviceId = "android-f6c453c385f228e7",
                SecurityCode = securityCode,
                Csrftoken = "",
                Uid = "5854658120",
                // Uuid = "2d038e2b-fc28-432b-9a7a-481e51fed4da",
                Guid = "8f0c9a02-49c7-43f7-8f64-3097ac243831"
            };
            requestParameter.Body = jsonElements;
            byte[] postData = requestParameter.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22%22%2C%22device_id%22%3A%22android-f6c453c385f228e7%22%2C%22guid%22%3A%228f0c9a02-49c7-43f7-8f64-3097ac243831%22%2C%22_uid%22%3A%225854658120%22%2C%22security_code%22%3A%22817432%22%7D");
            }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram, UserId = "5854658120" },
               };
            //account.DeviceDetails.DeviceId = "android-f6c453c385f228e7";
            // var accountModel = new AccountModel(account);
        }
        [DataTestMethod]
        [DataRow("817432")]
        public void Login_should_be_true_if_VerifyAccountAsync(string securityCode)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            VerificationAccount(securityCode, ref account);
            AccountModel accountModel = new AccountModel(account);
            //accountModel.Uuid = "2d038e2b-fc28-432b-9a7a-481e51fed4da";
            accountModel.Device_Id = "android-f6c453c385f228e7";
            accountModel.Guid = "8f0c9a02-49c7-43f7-8f64-3097ac243831";
            string challengeUrl = "/challenge/5514264613/CGN8gsQdSy/";
            var result = _sut.SubmitChallengeAsync(account, accountModel,"", challengeUrl, securityCode);
            result.Result.Success.Should().Be(true);
        }

        [DataTestMethod]
        [DataRow("")]
        public void Login_Should_be_false_if_Security_Code_Will_Be_Empty_VerifyAccountAsync(string securityCode)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            VerificationAccount(securityCode, ref account);
            AccountModel accountModel = new AccountModel(account);
            string challengeUrl = "/challenge/5514264613/CGN8gsQdSy/";
            var result = _sut.SubmitChallengeAsync(account, accountModel,"", challengeUrl, securityCode);
            result.Status.Should().Be(System.Threading.Tasks.TaskStatus.Faulted);
        }

        public void Follow(string userId, ref DominatorAccountModel account)
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            //string userId = "9925711872";
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.FollowResponse.json", Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/friendships/create/{userId}/";
            JsonElements jsonElements = new JsonElements()
            {
                Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07",
                Uid = "7150086983",
                Csrftoken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz",
                DeviceId = "android-7cb0d1049eeb764a",
                UserId = userId,
                RadioType = "wifi-none"
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22t9Wwm8geusRml2Wl7fOP4objbZyR1dpz%22%2C%22device_id%22%3A%22android-7cb0d1049eeb764a%22%2C%22radio_type%22%3A%22wifi-none%22%2C%22_uid%22%3A%227150086983%22%2C%22user_id%22%3A%229925711872%22%2C%22_uuid%22%3A%221ecd8d17-dbf0-47b6-88c3-e6177c7e0b07%22%7D");
                // a.Should().BeSameAs(postData);
            }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
               };
            // httpHelper = CommonServiceLocator.ServiceLocator.Current.GetInstance<IGdHttpHelper>();
            account.DeviceDetails.DeviceId = "android-7cb0d1049eeb764a";
            var accountModel = new AccountModel(account);

        }

        [TestMethod]
        public void follow_should_return_true_if_follow_success()
        {
            string userId = "9925711872";
            Follow(userId, ref _dominatorAccountModel);
            AccountModel account = new AccountModel(_dominatorAccountModel);
            account.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            account.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            account.Device_Id = "android-7cb0d1049eeb764a";
            var result = _sut.Follow(_dominatorAccountModel, account, CancellationToken, userId);

            result.Success.Should().Be(true);

        }


        public void Like(string mediaId, ref DominatorAccountModel account)
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            // string mediaId = "1947724516487055651";
            var pageresponse = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.LikeResponse.json",
                Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/media/{mediaId}_7713704103/like/?d=0";
            account = new DominatorAccountModel
            {
                AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "7713704103" },
            };

            JsonElements jsonElements = new JsonElements()
            {
                Uuid = "318cc395-bc23-4a7d-8cd6-e4fb9c13a7cc",
                Uid = "7713704103",
                Csrftoken = "2zCsSQXQGI79KLelVJxQ4bX9FuPLSGPo",
                MediaId = mediaId + "_" + account.AccountBaseModel.UserId,
                RadioType = "wifi-none",
                ContainerModuleForLike = "feed_contextual_location",
                DeviceId = "android-cf507250321b1420",
                IsCarouselBumpedPost = "false",
                FeedPosition = "4",
                LocationId = "8832400"
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%222zCsSQXQGI79KLelVJxQ4bX9FuPLSGPo%22%2C%22device_id%22%3A%22android-cf507250321b1420%22%2C%22media_id%22%3A%222108960387480037566_7713704103%22%2C%22container_module%22%3A%22feed_contextual_location%22%2C%22radio_type%22%3A%22wifi-none%22%2C%22_uid%22%3A%227713704103%22%2C%22_uuid%22%3A%22318cc395-bc23-4a7d-8cd6-e4fb9c13a7cc%22%2C%22is_carousel_bumped_post%22%3A%22false%22%2C%22feed_position%22%3A%224%22%2C%22location_id%22%3A%228832400%22%7D");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
        }
        [TestMethod]
        public void Like_should_return_true_if_Like_success()
        {
            string mediaId = "2108960387480037566";
            DominatorAccountModel dominatorAccountModel = new DominatorAccountModel();
            QueryInfo query = new QueryInfo();
            query.QueryType = "Location Posts";
            query.QueryValue = "8832400";
            Like(mediaId, ref dominatorAccountModel);
            var accountModel = new AccountModel(dominatorAccountModel);
            accountModel.CsrfToken = "2zCsSQXQGI79KLelVJxQ4bX9FuPLSGPo";
            accountModel.Uuid = "318cc395-bc23-4a7d-8cd6-e4fb9c13a7cc";
            accountModel.Device_Id = "android-cf507250321b1420";
            var result = _sut.Like(dominatorAccountModel, accountModel, CancellationToken, mediaId, "", "", QueryInfo: query, isUnitTest: true);
            result.Success.Should().Be(true);

        }

        [TestMethod]
        [DataRow("1947724516487")]
        public void Like_should_return_false_if_MediaId_Will_Be_Wrond_Or_Empty_Like_Failed(string mediaId)
        {
            DominatorAccountModel dominatorAccountModel = new DominatorAccountModel();
            Like(mediaId, ref dominatorAccountModel);
            var accountModel = new AccountModel(dominatorAccountModel);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            var result = _sut.Like(dominatorAccountModel, accountModel, CancellationToken, mediaId, "", "", null, isUnitTest: true);
            result.Should().BeNull();

        }

        public void Unfollow(string userId, ref DominatorAccountModel account)
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            // string userId = "24931858";
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.UnfollowResponse.json", Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/friendships/destroy/{userId}/";
            JsonElements jsonElements = new JsonElements()
            {
                Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07",
                Uid = "7150086983",
                Csrftoken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz",
                UserId = userId,
                RadioType = "wifi-none"
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22t9Wwm8geusRml2Wl7fOP4objbZyR1dpz%22%2C%22radio_type%22%3A%22wifi-none%22%2C%22_uid%22%3A%227150086983%22%2C%22user_id%22%3A%2224931858%22%2C%22_uuid%22%3A%221ecd8d17-dbf0-47b6-88c3-e6177c7e0b07%22%7D");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
               };
        }

        [TestMethod]
        public void Unfollow_should_return_true_if_Unfollow_Success()
        {
            string userid = "24931858";
            DominatorAccountModel account = new DominatorAccountModel();
            Unfollow(userid, ref account);
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            var result = _sut.Unfollow(account, accountModel, CancellationToken, userid);
            result.Success.Should().Be(true);
        }

        public void BlockFollower(string userId, ref DominatorAccountModel account)
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            //string userId = "4651924329";
            var pageresponse =
                TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.BlockFollower.json",
                    Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/friendships/block/{userId}/";
            JsonElements jsonElements = new JsonElements()
            {
                Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07",
                Uid = "7150086983",
                Csrftoken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz",
                UserId = userId,
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22t9Wwm8geusRml2Wl7fOP4objbZyR1dpz%22%2C%22_uid%22%3A%227150086983%22%2C%22user_id%22%3A%224651924329%22%2C%22_uuid%22%3A%221ecd8d17-dbf0-47b6-88c3-e6177c7e0b07%22%7D");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
               };
        }

        [TestMethod]
        public void BlockFollower_Should_return_true_if_BlockFollower_Scucess()
        {
            string UserId = "4651924329";
            DominatorAccountModel account = new DominatorAccountModel();
            BlockFollower(UserId, ref account);
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            var result = _sut.Block(account, accountModel, CancellationToken, UserId);
            result.Success.Should().BeTrue();
        }


        public void SendMessage(string userId, string message, ref DominatorAccountModel account)
        {
            var requestParameters = new IgRequestParameters();

            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.SendMessage.json",
                Assembly.GetExecutingAssembly());
            // string userId = "4580718406";
            // string message = "wow";
            var url = "https://i.instagram.com/api/v1/direct_v2/threads/broadcast/text/";
            string postdata =
                $"recipient_users=%5B%5B%22{userId}%22%5D%5D&client_context=%221680c464-12ca-4e0a-960d-1caadc079c93%22&text={message}";
            var postData = Encoding.UTF8.GetBytes(postdata);
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should()
                    .Be(
                        "recipient_users=%5B%5B%224580718406%22%5D%5D&client_context=%221680c464-12ca-4e0a-960d-1caadc079c93%22&text=wow");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
               };
        }
        [TestMethod]
        public void SendMessage_Should_return_true_if_SendMessage_Sucess()
        {
            string userId = "4580718406";
            string message = "wow";
            DominatorAccountModel account = new DominatorAccountModel();
            AccountModel accountModel = new AccountModel(account);
            SendMessage(userId, message, ref account);

            //Need to work on it
            var result = _sut.SendMessage(account, accountModel, userId, message, "", CancellationToken);
            result.Success.Should().Be(true);
        }
        [TestMethod]
        [DataRow("", "wow")]
        public void SendMessage_Should_return_False_if_UserId_Will_Be_Empty_OR_Wrong_SendMessage_Failed(string userId, string message)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            AccountModel accountModel = new AccountModel(account);
            SendMessage(userId, message, ref account);
            //Need to work on it
            var result = _sut.SendMessage(account, accountModel, userId, message, "", CancellationToken);
            result.Should().BeNull();
        }
        [TestMethod]
        [DataRow("4580718406", "")]
        public void SendMessage_Should_return_False_if_Message_Will_Be_Empty_SendMessage_Failed(string userId, string message)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            AccountModel accountModel = new AccountModel(account);
            SendMessage(userId, message, ref account);
            //Need to work on it
            var result = _sut.SendMessage(account, accountModel, userId, message, "", CancellationToken);
            result.Should().BeNull();
        }


        public void SendMessageWithLink(string userId, string message, List<string> LinkText, ref DominatorAccountModel account)
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SendMessageWithLink.json", Assembly.GetExecutingAssembly());
            // string userId = "4580718406";
            //string message = "wow www.javatpoint.com";
            string linkUrl = $"\"{LinkText[0]}\"";
            var url = "https://i.instagram.com/api/v1/direct_v2/threads/broadcast/link/";
            var jsonElements = new JsonElements()
            {
                LinkText = WebUtility.UrlEncode(message),
                LinkUrls = $"[{linkUrl}]",
                Action = "send_item",
                RecipientUsers = $"[[{userId}]]",
                ClientContext = "6579c2dd-bbdb-4954-94a5-841a848c6ccc",
                Csrftoken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz",
                Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07"
            };

            requestParameters.Body = jsonElements;
            requestParameters.DontSign();
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(
                    "action=send_item&_csrftoken=t9Wwm8geusRml2Wl7fOP4objbZyR1dpz&recipient_users=[[4580718406]]&_uuid=1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07&client_context=6579c2dd-bbdb-4954-94a5-841a848c6ccc&link_text=wow+www.javatpoint.com&link_urls=[\"www.javatpoint.com\"]");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
               };

        }

        [TestMethod]
        public void SendMessageWithLink_should_return_true_if_SendmessageWithLink_success()
        {
            string userId = "4580718406"; string message = "wow www.javatpoint.com";
            DominatorAccountModel account = new DominatorAccountModel();
            List<string> LinkText = new List<string>();
            LinkText.Add("www.javatpoint.com");
            SendMessageWithLink(userId, message, LinkText, ref account);
            AccountModel accountModel = new AccountModel(account);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            //Need to work on it
            var result = _sut.SendMessageWithLink(account, CancellationToken, userId, message, LinkText, "", accountModel, true);
            result.Success.Should().Be(true);
        }
        public void DeleteMedia(string mediaId, ref DominatorAccountModel account)
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.DeleteMedia.json",
                Assembly.GetExecutingAssembly());
            // mediaId = "1878286034182173977";
            var url = $"https://i.instagram.com/api/v1/media/{mediaId}/delete/";
            JsonElements jsonElements = new JsonElements()
            {
                Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07",
                Uid = "7150086983",
                Csrftoken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz",
                MediaId = mediaId
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(
                    "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22t9Wwm8geusRml2Wl7fOP4objbZyR1dpz%22%2C%22media_id%22%3A%221878286034182173977%22%2C%22_uid%22%3A%227150086983%22%2C%22_uuid%22%3A%221ecd8d17-dbf0-47b6-88c3-e6177c7e0b07%22%7D");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
               };
        }

        [TestMethod]
        [DataRow("1878286034182173977")]
        public void DeleteMedia_should_return_true_if_DeleteMedia_success(string mediaId)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            DeleteMedia(mediaId, ref account);
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            var result = _sut.DeleteMedia(account, accountModel, CancellationToken, mediaId);
            result.Success.Should().Be(true);
        }



        public void Comment(string mediaId, string comment, ref DominatorAccountModel account, IgRequestParameters IGRequestParameter)
        {
            IgRequestParameters requestParameters;
            if (igRequestParameter == null)
            {
                requestParameters = IGRequestParameter;
            }
            else
            {
                requestParameters = igRequestParameter;
            }
            httpHelper.GetRequestParameter().Returns(requestParameters);
            requestParameters.Cookies.Returns(new CookieCollection());
            var pageresponse = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.CommentResponse.json",
                Assembly.GetExecutingAssembly());
            var url =
                $"https://i.instagram.com/api/v1/media/{mediaId}_2071648333/comment/";
            JsonElements jsonElements = new JsonElements()
            {
                UserBreadcrumb = "u7uNCq6IUi7SIg8zWIgQCEVeQh+6Ms1m2jnyCcFNLPM=\nOCAxNTgwMCAzIDE1NTMwNzUwODkyNzY=\n",
                IdempotenceToken = "a1004b74-5697-4624-a6c7-8d21ddfcc632",
                Uuid = "9b44dc5e-2584-41fd-bc0d-b4956e1f80fa",
                Uid = "2071648333",
                Csrftoken = "EkHp7FnH37sO1USDwEnw7gxYNssQwVbZ",
                CommentText = comment,
                Containermodule = "comments_v2_feed_contextual_chain",
                RadioType = "wifi-none",
                DeviceId = "android-1ab478a3243d1360",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=SIGNATURE.%7B%22comment_text%22%3A%22gorgeous%22%2C%22containermodule%22%3A%22comments_v2_feed_contextual_chain%22%2C%22_csrftoken%22%3A%22EkHp7FnH37sO1USDwEnw7gxYNssQwVbZ%22%2C%22device_id%22%3A%22android-1ab478a3243d1360%22%2C%22idempotence_token%22%3A%22a1004b74-5697-4624-a6c7-8d21ddfcc632%22%2C%22radio_type%22%3A%22wifi-none%22%2C%22_uid%22%3A%222071648333%22%2C%22user_breadcrumb%22%3A%22u7uNCq6IUi7SIg8zWIgQCEVeQh%2B6Ms1m2jnyCcFNLPM%3D%5CnOCAxNTgwMCAzIDE1NTMwNzUwODkyNzY%3D%5Cn%22%2C%22_uuid%22%3A%229b44dc5e-2584-41fd-bc0d-b4956e1f80fa%22%7D");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333", UserName = "jacksmith729" },
               };
            //account.DeviceDetails.DeviceId = "android-1ab478a3243d1360";
        }
        [TestMethod]
        [DataRow("2003661316507544321", "gorgeous", null)]
        public void Comment_should_return_true_if_Comment_success(string mediaId, string comment, IgRequestParameters igRequestParameter)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            Comment(mediaId, comment, ref account, igRequestParameter);
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "EkHp7FnH37sO1USDwEnw7gxYNssQwVbZ";
            accountModel.Uuid = "9b44dc5e-2584-41fd-bc0d-b4956e1f80fa";
            accountModel.Device_Id = "android-1ab478a3243d1360";
            var result = _sut.Comment(account, accountModel, CancellationToken, mediaId, comment, null, "comments_v2_feed_contextual_chain", "u7uNCq6IUi7SIg8zWIgQCEVeQh+6Ms1m2jnyCcFNLPM=\nOCAxNTgwMCAzIDE1NTMwNzUwODkyNzY=\n", "a1004b74-5697-4624-a6c7-8d21ddfcc632");
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void UnlikeMedia_should_return_true_if_Unlike_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.UnlikeMediaResponse.json", Assembly.GetExecutingAssembly());
            string mediaId = "1857051003963384707_1535597752";
            var url = "https://i.instagram.com/api/v1/media/1857051003963384707_1535597752/unlike/";
            JsonElements jsonElements = new JsonElements()
            {
                ModuleName = "photo_view_other",
                MediaId = "1857051003963384707_1535597752",
                Csrftoken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz",
                RadioType = "mobile-hspa+",
                Uid = "7150086983",
                Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var finalpostdata = Encoding.UTF8.GetString(postData);
                    finalpostdata.Should().Be(
                        "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22t9Wwm8geusRml2Wl7fOP4objbZyR1dpz%22%2C%22media_id%22%3A%221857051003963384707_1535597752%22%2C%22module_name%22%3A%22photo_view_other%22%2C%22radio_type%22%3A%22mobile-hspa%2B%22%2C%22_uid%22%3A%227150086983%22%2C%22_uuid%22%3A%221ecd8d17-dbf0-47b6-88c3-e6177c7e0b07%22%7D");
                }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            var result = _sut.UnlikeMedia(account, accountModel, CancellationToken, mediaId);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void UnlikeMedia_should_return_Null_if_Unlike_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.UnlikeMediaResponse.json", Assembly.GetExecutingAssembly());
            string mediaId = "1857051003963384707_1535597752";
            var url = "https://i.instagram.com/api/v1/media//unlike/";
            JsonElements jsonElements = new JsonElements()
            {
                ModuleName = "photo_view_other",
                MediaId = "1857051003963384707_1535597752",
                Csrftoken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz",
                RadioType = "mobile-hspa+",
                Uid = "7150086983",
                Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(
                    "signed_body=7e4c47c3126dd43605bd974380313fe71d04e34fbf5a551864252ee1b028cc0c.%7B%22_csrftoken%22%3A%22t9Wwm8geusRml2Wl7fOP4objbZyR1dpz%22%2C%22media_id%22%3A%221857051003963384707_1535597752%22%2C%22module_name%22%3A%22photo_view_other%22%2C%22radio_type%22%3A%22mobile-hspa%2B%22%2C%22_uid%22%3A%227150086983%22%2C%22_uuid%22%3A%221ecd8d17-dbf0-47b6-88c3-e6177c7e0b07%22%7D&ig_sig_key_version=4");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            var result = _sut.UnlikeMedia(account, accountModel, CancellationToken, mediaId);
            result.Should().BeNull();
        }

        [TestMethod]
        public void MediaInfo_should_return_true_if_MediaInfo_success()
        {
            var requestParameters = new IgRequestParameters();

            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.MediaInfoResponse.json", Assembly.GetExecutingAssembly());
            string mediaId = "1937953844155939956";
            var url = "https://i.instagram.com/api/v1/media/1937953844155939956/info/";
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.GetRequest(url)
                 .Returns(new ResponseParameter
                 {
                     Response = pageresponse,
                     Exception = null,
                     HasError = false
                 });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
                };
            var accountModel = new AccountModel(account);
            var result = _sut.MediaInfo(account, accountModel, mediaId, CancellationToken);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void MediaInfo_should_return_Null_if_MediaInfo_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.MediaInfoResponse.json", Assembly.GetExecutingAssembly());
            string mediaId = "1937953844155939956";
            var url = "https://i.instagram.com/api/v1/media/6/info/";
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.GetRequest(url)
                 .Returns(new ResponseParameter
                 {
                     Response = pageresponse,
                     Exception = null,
                     HasError = false
                 });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
                };
            var accountModel = new AccountModel(account);
            var result = _sut.MediaInfo(account, accountModel, mediaId, CancellationToken);
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetVisualThread_should_return_true_if_GetVisualThread_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetVisualThreadResponse.json", Assembly.GetExecutingAssembly());
            string threadId = "340282366841710300949128194088822507172";
            var url =
                "https://i.instagram.com/api/v1/direct_v2/threads/340282366841710300949128194088822507172/?use_unified_inbox=true";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetVisualThread(account, threadId);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetVisualThread_should_return_Null_if_GetVisualThread_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetVisualThreadResponse.json", Assembly.GetExecutingAssembly());
            string threadId = "340282366841710300949128194088822507172";
            var url =
                "https://i.instagram.com/api/v1/direct_v2/threads//?use_unified_inbox=true";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetVisualThread(account, threadId);
            result.Should().BeNull();
        }

        [TestMethod]
        public void UserFriendship_should_return_true_if_UserFriendship_success()
        {
            var requestParameters = new IgRequestParameters();

            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.UserFriendshipResponse.json", Assembly.GetExecutingAssembly());
            string userId = "4580718406";
            var url = "https://i.instagram.com/api/v1/friendships/show/4580718406/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            var result = _sut.UserFriendship(account, accountModel, userId);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void UserFriendship_should_return_Null_if_UserFriendship_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.UserFriendshipResponse.json", Assembly.GetExecutingAssembly());
            string userId = "4580718406";
            var url = "https://i.instagram.com/api/v1/friendships/show//";
            JsonElements jsonElements = new JsonElements()
            {
                UserId = userId,
                Csrftoken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz",
                Uid = "7150086983",
                Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(
                    "signed_body=30de1f0c8309b4bd69440836520d8c1b624336d6a876300f3b8a51de0ad3df72.%7B%22_csrftoken%22%3A%22t9Wwm8geusRml2Wl7fOP4objbZyR1dpz%22%2C%22_uid%22%3A%227150086983%22%2C%22user_id%22%3A%224580718406%22%2C%22_uuid%22%3A%221ecd8d17-dbf0-47b6-88c3-e6177c7e0b07%22%7D&ig_sig_key_version=4");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            var result = _sut.UserFriendship(account, accountModel, userId);
            result.Should().BeNull();
        }


        [TestMethod]
        public void GetInstaProfile_should_return_true_if_EditInstaProfile_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetInstaProfileResponse.json", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/accounts/current_user/?edit=true";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetProfileDetails(account);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetInstaProfile_should_return_Null_if_EditInstaProfile_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetInstaProfileResponse.json", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/accounts/current_user/?edit=";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetProfileDetails(account);
            result.Should().BeNull();
        }


        [TestMethod]
        public void SetBiography_should_return_if_SetBiography_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SetBiographyResponse.json", Assembly.GetExecutingAssembly());
            string bioText = "nice day ";
            var url = "https://i.instagram.com/api/v1/accounts/set_biography/";
            JsonElements jsonElements = new JsonElements()
            {
                RawText = bioText,
                Csrftoken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz",
                Uid = "7150086983",
                Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
                {
                    var finalpostdata = Encoding.UTF8.GetString(postData);
                    finalpostdata.Should().Be(
                        "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22t9Wwm8geusRml2Wl7fOP4objbZyR1dpz%22%2C%22_uid%22%3A%227150086983%22%2C%22_uuid%22%3A%221ecd8d17-dbf0-47b6-88c3-e6177c7e0b07%22%2C%22raw_text%22%3A%22nice+day+%22%7D");
                }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            var result = _sut.SetBiographyAsync(account, accountModel, bioText);
            result.Result.Success.Should().Be(true);
        }

        [TestMethod]
        public async Task EditProfile_should_return_if_EditProfile_success()
        {

            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.EditProfileResponse.json", Assembly.GetExecutingAssembly());
            string bioText = "what a lovely day";
            string phoneNo = null;
            string fullName = "jack smith";
            string email = "jacksmith0174 @gmail.com";
            int gender = 3;
            string username = "jacksmith000174";
            string externalUrl = "http://www.facebook.com/profile.php?id=100001882353236";
            var url = "https://i.instagram.com/api/v1/accounts/edit_profile/";
            JsonElements jsonElements = new JsonElements()
            {
                ExternalUrl = externalUrl,
                Gender = gender,
                PhoneNumber = phoneNo,
                Csrftoken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO",
                Username = username,
                FirstName = fullName,
                Uid = "2071648333",
                Biography = bioText,
                Uuid = "676480d2-b925-4bf7-a94d-5077b5681388",
                Email = email
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
                {
                    var finalpostdata = Encoding.UTF8.GetString(postData);
                    finalpostdata.Should().Be(
                        "signed_body=SIGNATURE.%7B%22biography%22%3A%22what+a+lovely+day%22%2C%22_csrftoken%22%3A%22xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO%22%2C%22email%22%3A%22jacksmith0174+%40gmail.com%22%2C%22external_url%22%3A%22http%3A%2F%2Fwww.facebook.com%2Fprofile.php%3Fid%3D100001882353236%22%2C%22gender%22%3A3%2C%22_uid%22%3A%222071648333%22%2C%22username%22%3A%22jacksmith000174%22%2C%22_uuid%22%3A%22676480d2-b925-4bf7-a94d-5077b5681388%22%2C%22first_name%22%3A%22jack+smith%22%7D");
                }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO";
            accountModel.Uuid = "676480d2-b925-4bf7-a94d-5077b5681388";
            var result = await _sut.EditProfileAsync(account, accountModel, externalUrl, phoneNo, fullName, bioText, email,
                gender, username);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void CheckUsername_should_return_if_CheckUsername_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.CheckUserNameAvailableResponse.json",
                Assembly.GetExecutingAssembly());
            string userName = "jacksmith_729";
            var url = "https://i.instagram.com/api/v1/users/check_username/";
            JsonElements jsonElements = new JsonElements()
            {
                Username = userName,
                Csrftoken = "iHfB9vVxod2GtXbWVo6XtQsKHLGl5nxd",
                Uid = "2071648333",// 2071648333
                Uuid = "44819636-82b5-4191-b7d6-bdb958149571",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
                {
                    var finalpostdata = Encoding.UTF8.GetString(postData);
                    finalpostdata.Should().Be(
                        "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22iHfB9vVxod2GtXbWVo6XtQsKHLGl5nxd%22%2C%22_uid%22%3A%222071648333%22%2C%22username%22%3A%22jacksmith_729%22%2C%22_uuid%22%3A%2244819636-82b5-4191-b7d6-bdb958149571%22%7D");
                }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "iHfB9vVxod2GtXbWVo6XtQsKHLGl5nxd";
            accountModel.Uuid = "44819636-82b5-4191-b7d6-bdb958149571";
            var result = _sut.CheckUsernameAsync(account, accountModel, userName);
            result.Result.Success.Should().Be(true);
        }

        [TestMethod]
        public void CheckUsername_should_return_Null_if_CheckUsername_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.CheckUserNameAvailableResponse.json",
                Assembly.GetExecutingAssembly());
            string userName = "jacksmith_729";
            var url = "https://i.instagram.com/api/v1/users/check_username/";
            JsonElements jsonElements = new JsonElements()
            {
                Username = "",
                Csrftoken = "iHfB9vVxod2GtXbWVo6XtQsKHLGl5nxd",
                Uid = "2071648333",
                Uuid = "44819636-82b5-4191-b7d6-bdb958149571",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(
                    "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22iHfB9vVxod2GtXbWVo6XtQsKHLGl5nxd%22%2C%22_uid%22%3A%222071648333%22%2C%22username%22%3A%22%22%2C%22_uuid%22%3A%2244819636-82b5-4191-b7d6-bdb958149571%22%7D");
            }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "iHfB9vVxod2GtXbWVo6XtQsKHLGl5nxd";
            accountModel.Uuid = "44819636-82b5-4191-b7d6-bdb958149571";
            var result = _sut.CheckUsernameAsync(account, accountModel, userName);
            result.Exception.Should().BeNull();
            // result.Should().BeNull();
        }

        [TestMethod]
        public void GetHashTagFeed_should_return_if_GetHashTagFeed_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetHashTagFeed.json", Assembly.GetExecutingAssembly());
            string hashTag = "nehakakkar";
            var url = "https://i.instagram.com/api/v1/feed/tag/nehakakkar/?maxid=&rank_token=&ranked_content=true";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetHashtagFeed(account, hashTag);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetHashTagFeed_should_return_Null_if_GetHashTagFeed_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetHashTagFeed.json", Assembly.GetExecutingAssembly());
            string hashTag = "nehakakkar";
            var url = "https://i.instagram.com/api/v1/feed/tag//?maxid=&rank_token=&ranked_content=true";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetHashtagFeed(account, hashTag);
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetLocationFeed_should_return_if_GetLocationFeed_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetLocationFeedResponse.json", Assembly.GetExecutingAssembly());
            string LocationId = "566966959";
            var url = "https://i.instagram.com/api/v1/feed/location/566966959/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetLocationFeed(account, LocationId, CancellationToken);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetLocationFeed_should_return_False_if_GetLocationFeed_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetLocationFeedResponse.json", Assembly.GetExecutingAssembly());
            string LocationId = "566966959";
            var url = "https://i.instagram.com/api/v1/feed/location//";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetLocationFeed(account, LocationId, CancellationToken);
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetLocationFeedAlternate_Should_if_GetLoactionFeedAlternate_Success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);

            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetLocationFeedAlternateResponse.json", Assembly.GetExecutingAssembly());
            string LocationId = "566966959";
            var url = "https://i.instagram.com/api/v1/locations/566966959/sections/";
            JsonElements jsonElements = new JsonElements()
            {
                Uuid = "c1d797b7-33aa-444a-94a7-e73702867281",
                Tab = "ranked",
                Csrftoken = "",
                SessionId = "6a888592-ae6c-42a1-8657-c0c3d0278f52"
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(
                    "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22%22%2C%22_uuid%22%3A%22c1d797b7-33aa-444a-94a7-e73702867281%22%2C%22tab%22%3A%22ranked%22%2C%22session_id%22%3A%226a888592-ae6c-42a1-8657-c0c3d0278f52%22%7D");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.Uuid = "c1d797b7-33aa-444a-94a7-e73702867281";
            account.DeviceDetails.AdId = "6a888592-ae6c-42a1-8657-c0c3d0278f52";
            accountModel.AdId = "6a888592-ae6c-42a1-8657-c0c3d0278f52";
            var result = _sut.GetLocationFeedAlternate(account, accountModel, LocationId, CancellationToken, null, null);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetLocationFeedAlternate_Should_Be_False_if_GetLoactionFeedAlternate_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);

            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetLocationFeedAlternateResponse.json", Assembly.GetExecutingAssembly());
            string LocationId = "566966959";
            var url = "https://i.instagram.com/api/v1/locations//sections/";
            JsonElements jsonElements = new JsonElements()
            {
                Uuid = "c1d797b7-33aa-444a-94a7-e73702867281",
                Tab = "ranked",
                Csrftoken = "",
                SessionId = "6a888592-ae6c-42a1-8657-c0c3d0278f52"
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(
                    "signed_body=30029d6ecce04155c4f984aac96b4bbc246e71d5b706eac93748d82868349d49.%7B%22_csrftoken%22%3A%22%22%2C%22_uuid%22%3A%22c1d797b7-33aa-444a-94a7-e73702867281%22%2C%22tab%22%3A%22ranked%22%2C%22session_id%22%3A%226a888592-ae6c-42a1-8657-c0c3d0278f52%22%7D&ig_sig_key_version=4");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.Uuid = "c1d797b7-33aa-444a-94a7-e73702867281";
            account.DeviceDetails.AdId = "6a888592-ae6c-42a1-8657-c0c3d0278f52";
            var result = _sut.GetLocationFeedAlternate(account, accountModel, LocationId, CancellationToken, null, null);
            result.Should().BeNull();
        }

        [TestMethod]
        public void FeedLocationCheck_should_return_if_FeedLocationCheck_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.LocationCheckResponse.json", Assembly.GetExecutingAssembly());
            string LocationId = "458924037641581";
            var url =
                $"https://i.instagram.com/api/v1/locations/458924037641581/related/?visited=[\"id\":\"458924037641581\",\"type\":\"location\"]&related_types=[\"location\",\"hashtag\",\"user\"]";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.FeedLocationCheck(account, LocationId);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void FeedLocationCheck_should_return_False_if_FeedLocationCheck_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.LocationCheckResponse.json", Assembly.GetExecutingAssembly());
            string LocationId = "458924037641581";
            var url =
                $"https://i.instagram.com/api/v1/locations//related/?visited=[\"id\":\"458924037641581\",\"type\":\"location\"]&related_types=[\"location\",\"hashtag\",\"user\"]";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.FeedLocationCheck(account, LocationId);
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetMediaComments_should_return_if_GetMediaComments_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetMediaCommentsResponse.json", Assembly.GetExecutingAssembly());
            string mediaId = "2000872912799570454";
            var url = $"https://i.instagram.com/api/v1/media/2000872912799570454/comments/?can_support_threading=true";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetMediaComments(account, mediaId, CancellationToken);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetMediaComments_should_return_False_if_GetMediaComments_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetMediaCommentsResponse.json", Assembly.GetExecutingAssembly());
            string mediaId = "1989867090619469047";
            var url = $"https://i.instagram.com/api/v1/media//comments/?ig_sig_key_version=4";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetMediaComments(account, mediaId, CancellationToken);
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetMediaLikers_should_return_if_GetMediaLikers_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetMediaLikersResponse.json", Assembly.GetExecutingAssembly());
            string mediaId = "2000872912799570454";
            var url = "https://i.instagram.com/api/v1/media/2000872912799570454/likers/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetMediaLikers(account, mediaId, CancellationToken);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetMediaLikers_should_return_Failed_if_GetMediaLikers_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetMediaLikersResponse.json", Assembly.GetExecutingAssembly());
            string mediaId = "1989867090619469047";
            var url = "https://i.instagram.com/api/v1/media//likers/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetMediaLikers(account, mediaId, CancellationToken);
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetSuggestedUsers_should_return_if_GetSuggestedUsers_success()
        {
            var requestParameters = new IgRequestParameters();
            string userId = "5340";
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetSuggestedUsersResponse.json", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/discover/chaining/?target_id=5340";

            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel = new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var accountModel = new AccountModel(account);
            var result = _sut.GetSuggestedUsers(account, accountModel, userId, CancellationToken, "");
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetSuggestedUsers_should_return_False_if_GetSuggestedUsers_Failed()
        {
            var requestParameters = new IgRequestParameters();
            string userId = "5340";
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetSuggestedUsersResponse.json", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/discover/chaining/?target_id=";

            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },

                };
            var accountModel = new AccountModel(account);
            var result = _sut.GetSuggestedUsers(account, accountModel, userId, CancellationToken, "");
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetUserFeed_should_return_if_GetUserFeed_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetUserFeedResponse.json", Assembly.GetExecutingAssembly());
            string userId = "243103112";
            var url = "https://i.instagram.com/api/v1/feed/user/243103112/?exclude_comment=false&only_fetch_first_carousel_media=false";
            httpHelper.GetRequestAsync(url, CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            var result = _sut.GetUserFeed(account, accountModel, userId, CancellationToken);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetUserFeed_should_return_False_if_GetUserFeed_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetUserFeedResponse.json", Assembly.GetExecutingAssembly());
            string userId = "243103112";
            var url = "https://i.instagram.com/api/v1/feed/user//?rank_token=&ranked_content=true";
            httpHelper.GetRequestAsync(url, CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            var result = _sut.GetUserFeed(account, accountModel, userId, CancellationToken);
            result.Should().BeNull();
        }

        [TestMethod]
        public FollowerAndFollowingIgResponseHandler GetUserFollowers_should_return_if_GetUserFollowers_success()
        {
            var requestParameters = new IgRequestParameters();
            FollowerAndFollowingIgResponseHandler response;
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetUserFollowersResponse.json", Assembly.GetExecutingAssembly());
            string userId = "5340";
            var url = "https://i.instagram.com/api/v1/friendships/5340/followers/";
            httpHelper.GetRequestAsync(url, CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetUserFollowers(account, userId, CancellationToken);
            response = result;
            result.Success.Should().Be(true);
            return response;


        }

        [TestMethod]
        public FollowerAndFollowingIgResponseHandler GetUserFollowers_should_return_False_if_GetUserFollowers_Failed()
        {
            var requestParameters = new IgRequestParameters();
            FollowerAndFollowingIgResponseHandler response;
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetUserFollowersResponse.json", Assembly.GetExecutingAssembly());
            string userId = "5340";
            var url = "https://i.instagram.com/api/v1/friendships//followers/";
            httpHelper.GetRequestAsync(url, CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.GetUserFollowers(account, userId, CancellationToken);
            response = result;
            result.Should().BeNull();
            return response;
        }

        [TestMethod]
        public void GetUserFollowings_should_return_if_GetUserFollowings_success()
        {
            var requestParameters = new IgRequestParameters();

            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetUserFollowingResponse.json", Assembly.GetExecutingAssembly());
            string userId = "5340";
            var url = "https://i.instagram.com/api/v1/friendships/5340/following/";
            httpHelper.GetRequestAsync(url, CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.GetUserFollowings(account, accountModel, userId, CancellationToken);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetUserFollowings_should_return_Null_if_GetUserFollowings_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetUserFollowingResponse.json", Assembly.GetExecutingAssembly());
            string userId = "5340";
            var url = "https://i.instagram.com/api/v1/friendships//following";
            httpHelper.GetRequestAsync(url, CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.GetUserFollowings(account, accountModel, userId, CancellationToken);
            result.Should().BeNull();
        }

        [TestMethod]
        public void SearchForTag_should_return_if_SearchForTag_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchForTagResponse.json", Assembly.GetExecutingAssembly());
            string tag = "car";
            var url = "https://i.instagram.com/api/v1/tags/search/?timezone_offset=19800&q=car&count=50&rank_token=";
            _dateProvider.GetTimezoneOffset().Returns(19800);
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchForTag(account, accountModel, tag);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void SearchForTag_should_return_Null_if_SearchForTag_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchForTagResponse.json", Assembly.GetExecutingAssembly());
            string tag = "car";
            var url = "https://i.instagram.com/api/v1/tags/search/?timezone_offset=19800&q=&count=50&rank_token=";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchForTag(account, accountModel, tag);
            result.Should().BeNull();
        }

        [TestMethod]
        public void SearchForKeyword_should_return_if_SearchForKeyword_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchKeywordResponse.json", Assembly.GetExecutingAssembly());
            string keyword = "sachin";
            var url = "https://i.instagram.com/api/v1/users/search/?timezone_offset=19800&q=sachin&count=50";
            _dateProvider.GetTimezoneOffset().Returns(19800);
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchForkeyword(account, keyword, CancellationToken);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void SearchForKeyword_should_return_Null_if_SearchForKeyword_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchKeywordResponse.json", Assembly.GetExecutingAssembly());
            string keyword = "sachin";
            var url = "https://i.instagram.com/api/v1/users/search/?timezone_offset=19800&q=&count=50";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchForkeyword(account, keyword, CancellationToken);
            result.Should().BeNull();
        }

        [TestMethod]
        public void SearchUsername_should_return_if_SearchUsername_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchUserNameResponse.json", Assembly.GetExecutingAssembly());
            string keyword = "sachin";
            var url = "https://i.instagram.com/api/v1/users/sachin/usernameinfo/";
            httpHelper.GetRequestAsync(url, CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchUsername(account, keyword, CancellationToken);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void SearchUsername_should_return_Null_if_SearchUsername_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchUserNameResponse.json", Assembly.GetExecutingAssembly());
            string keyword = "sachin";
            var url = "https://i.instagram.com/api/v1/users//usernameinfo/";
            httpHelper.GetRequestAsync(url, CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchUsername(account, keyword, CancellationToken);
            result.Should().BeNull();
        }

        [TestMethod]
        public void SearchUsername_should_return_if_SearchAccountUsername_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.AccountUserInfo.json", Assembly.GetExecutingAssembly());
            string keyword = "jacksmith729";
            var url = "https://i.instagram.com/api/v1/users/jacksmith729/usernameinfo/";
            httpHelper.GetRequestAsync(url, CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchUsername(account, keyword, CancellationToken);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void SearchUserInfoById_should_return_if_SearchUserInfoById_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchUserInfoByIdResponse.json",
                Assembly.GetExecutingAssembly());
            string UserId = "2071648333";
            var url = "https://i.instagram.com/api/v1/users/2071648333/info/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchUserInfoById(account,accountModel, UserId, CancellationToken);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void SearchUserInfoById_should_return_Null_if_SearchUserInfoById_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchUserInfoByIdResponse.json",
                Assembly.GetExecutingAssembly());
            string UserId = "2071648333";
            var url = "https://i.instagram.com/api/v1/users//info/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchUserInfoById(account,null, UserId, CancellationToken);
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetRecentFollowers_should_return_if_GetRecentFollowers_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetRecentFollowersResponse.json",
                Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/friendships/recent_followers/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.GetRecentFollowers(account);
            result.UsersList.Count.Should().BeGreaterThan(0);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetRecentFollowers_should_return_Null_if_GetRecentFollowers_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetRecentFollowersResponse.json",
                Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/friendships//";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.GetRecentFollowers(account);
            result.Should().BeNull();
        }


        [TestMethod]
        public void SearchForLocation_should_return_if_SearchForLocation_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            string location = "india";
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchForLocationResponse.json", Assembly.GetExecutingAssembly());
            var url =
                "https://i.instagram.com/api/v1/location_search/?latitude=33.985805&longitude=-118.25411166666666&search_query=india";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchForLocation(account, location);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void SearchForLocation_should_return_Null_if_SearchForLocation_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            string location = "india";
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchForLocationResponse.json", Assembly.GetExecutingAssembly());
            var url =
                "https://i.instagram.com/api/v1/location_search/?latitude=33.985805&longitude=-118.25411166666666&search_query=";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchForLocation(account, location);
            result.Should().BeNull();
        }

        [TestMethod]
        public void SearchLocationId_should_return_if_SearchLocationId_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            string LocationId = "566966959";
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchLocationIdResponse.json", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/feed/location/566966959";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchLocationId(account, LocationId);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void SearchLocationId_should_return_Null_if_SearchLocationId_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            string LocationId = "566966959";
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchLocationIdResponse.json", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/feed/location/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.SearchLocationId(account, LocationId);
            result.Should().BeNull();
        }

        [TestMethod]
        public void TurnOffCommenting_should_return_if_TurnOffCommenting_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.TurnOffCommentingResponse.json", Assembly.GetExecutingAssembly());
            string MediaId = "1952051622561555880_2071648333";
            var url = "https://i.instagram.com/api/v1/media/1952051622561555880_2071648333/disable_comments/";
            JsonElements jsonElements = new JsonElements()
            {
                Csrftoken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO",
                Uuid = "676480d2-b925-4bf7-a94d-5077b5681388",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var finalpostdata = Encoding.UTF8.GetString(postData);
                    finalpostdata.Should().Be(
                        "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO%22%2C%22_uuid%22%3A%22676480d2-b925-4bf7-a94d-5077b5681388%22%7D");
                }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "6975636384" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO";
            accountModel.Uuid = "676480d2-b925-4bf7-a94d-5077b5681388";
            var result = _sut.TurnOffCommenting(account, accountModel, MediaId);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void TurnOffCommenting_should_return_Null_if_TurnOffCommenting_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.TurnOffCommentingResponse.json", Assembly.GetExecutingAssembly());
            string MediaId = "1952051622561555880_2071648333";
            var url = "https://i.instagram.com/api/v1/media//disable_comments/";
            JsonElements jsonElements = new JsonElements()
            {
                Csrftoken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO",
                Uuid = "676480d2-b925-4bf7-a94d-5077b5681388",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(
                    "signed_body=c0041e39fbd57916976c945f05c5da690418c9c642389c520c0a28028a3202d3.%7B%22_csrftoken%22%3A%22xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO%22%2C%22_uuid%22%3A%22676480d2-b925-4bf7-a94d-5077b5681388%22%7D&ig_sig_key_version=4");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "6975636384" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO";
            accountModel.Uuid = "676480d2-b925-4bf7-a94d-5077b5681388";
            var result = _sut.TurnOffCommenting(account, accountModel, MediaId);
            result.Should().BeNull();
        }

        [TestMethod]
        public void LikeOnComment_should_return_if_LikeOnComment_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.LikeOnCommentResponse.json", Assembly.GetExecutingAssembly());
            string MediaId = "17917605616263640";
            var url = "https://i.instagram.com/api/v1/media/17917605616263640/comment_like/";
            JsonElements jsonElements = new JsonElements()
            {
                Csrftoken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO",
                Uid = "2071648333",
                Uuid = "676480d2-b925-4bf7-a94d-5077b5681388",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var finalpostdata = Encoding.UTF8.GetString(postData);
                    finalpostdata.Should().Be(
                        "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO%22%2C%22_uid%22%3A%222071648333%22%2C%22_uuid%22%3A%22676480d2-b925-4bf7-a94d-5077b5681388%22%7D");
                }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO";
            accountModel.Uuid = "676480d2-b925-4bf7-a94d-5077b5681388";
            var result = _sut.LikeOnComment(account, accountModel, CancellationToken, MediaId);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void LikeOnComment_should_return_Null_if_LikeOnComment_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.LikeOnCommentResponse.json", Assembly.GetExecutingAssembly());
            string MediaId = "17917605616263640";
            var url = "https://i.instagram.com/api/v1/media//comment_like/";
            JsonElements jsonElements = new JsonElements()
            {
                Csrftoken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO",
                Uid = "2071648333",
                Uuid = "676480d2-b925-4bf7-a94d-5077b5681388",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(
                    "signed_body=6fd7220f280f315d1a58300e767ee3aec0092e7a05f26c235ad33410ac9924d5.%7B%22_csrftoken%22%3A%22xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO%22%2C%22_uid%22%3A%222071648333%22%2C%22_uuid%22%3A%22676480d2-b925-4bf7-a94d-5077b5681388%22%7D&ig_sig_key_version=4");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO";
            accountModel.Uuid = "676480d2-b925-4bf7-a94d-5077b5681388";
            var result = _sut.LikeOnComment(account, accountModel, CancellationToken, MediaId);
            result.Should().BeNull();
        }

        [TestMethod]
        public void SendPhotoAsDirectMessage_return_Null_if_SendPhotoAsDirectMessage_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SendPhotoAsDirectMessageResponse.json", Assembly.GetExecutingAssembly());
            string MediaId = "17917605616263640";
            var url = "https://i.instagram.com/api/v1/media//comment_like/";
            JsonElements jsonElements = new JsonElements()
            {
                Csrftoken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO",
                Uid = "2071648333",
                Uuid = "676480d2-b925-4bf7-a94d-5077b5681388",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);

                finalpostdata.Should().Be(
                    "signed_body=6fd7220f280f315d1a58300e767ee3aec0092e7a05f26c235ad33410ac9924d5.%7B%22_csrftoken%22%3A%22xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO%22%2C%22_uid%22%3A%222071648333%22%2C%22_uuid%22%3A%22676480d2-b925-4bf7-a94d-5077b5681388%22%7D&ig_sig_key_version=4");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO";
            accountModel.Uuid = "676480d2-b925-4bf7-a94d-5077b5681388";
            var result = _sut.SendPhotoAsDirectMessage(account, accountModel, CancellationToken, MediaId, "");
            result.Should().BeNull();
        }

        public void SomeoneTaggedPost(ref DominatorAccountModel account, string userId)
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SomeoneTaggedPostResponse.json", Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/usertags/{userId}/feed/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
        }

        [TestMethod]
        [DataRow("243103112")]
        public void SomeoneTaggedPost_should_return_true_if_somenoneTaggedPost_success(string userId)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            SomeoneTaggedPost(ref account, userId);
            var result = _sut.SomeoneTaggedPost(account, userId, CancellationToken, null);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        [DataRow("")]
        public void SomeoneTaggedPost_should_return_false_if_userId_will_be_wrong_or_empty_somenoneTaggedPost_failed(string userId)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            SomeoneTaggedPost(ref account, userId);
            var result = _sut.SomeoneTaggedPost(account, userId, CancellationToken, null);
            result.Success.Should().Be(true);
        }

        public void ApproveFollowRequest(string userId, string expectedFinalPost, ref DominatorAccountModel account)
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.ApprovedRequestResponse.json", Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/friendships/approve/{userId}/";
            JsonElements jsonElements = new JsonElements()
            {
                Csrftoken = "VraqIQZeXlxu18q4xsXUkLq8HZ3WlEZd",
                Uid = "2071648333",
                RadioType = "mobile-hspa+",
                UserId = userId,
                Uuid = "2bded4db-1663-4386-8752-8f6d68d4cab3",
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(expectedFinalPost);
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333" },
               };

        }
        [TestMethod]
        [DataRow("4458140854")]
        public void ApproveFollowRequest_should_return_true_if_ApproveFollowRequest_success(string userId)
        {
            DominatorAccountModel account = new DominatorAccountModel();
            ApproveFollowRequest(userId,
                "signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22VraqIQZeXlxu18q4xsXUkLq8HZ3WlEZd%22%2C%22radio_type%22%3A%22mobile-hspa%2B%22%2C%22_uid%22%3A%222071648333%22%2C%22user_id%22%3A%224458140854%22%2C%22_uuid%22%3A%222bded4db-1663-4386-8752-8f6d68d4cab3%22%7D",
                ref account);
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "VraqIQZeXlxu18q4xsXUkLq8HZ3WlEZd";
            accountModel.Uuid = "2bded4db-1663-4386-8752-8f6d68d4cab3";

            var result = _sut.AcceptRequest(account, accountModel, CancellationToken, userId);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void FriendShipPendingRequest_should_return_true_if_FriendshipPendingRequest_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.FriendshipPendingRequest.json", Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/friendships/pending/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            var result = _sut.PendingRequest(account);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void Getv2Inbox_should_return_if_Getv2Inbox_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.Getv2InboxResponse.json", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/direct_v2/inbox/?persistentBadging=true";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.Getv2Inbox(account);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetMediaLikedForUnlike_Will_return_true()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetLikedMediaForUnlikeResponse.json", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/feed/liked/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.GetLikedMedia(account);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void GetMediaLikedForUnlike_Will_return_Null()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetLikedMediaForUnlikeResponse.json", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/feed/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.GetLikedMedia(account);
            result.Should().BeNull();
        }

        [TestMethod]
        public void GetUserFollowings_should_return_if_GetAccountUserFollowings_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.GetAccountUserFollowingResponse.json", Assembly.GetExecutingAssembly());
            string userId = "2071648333";
            var url = "https://i.instagram.com/api/v1/friendships/2071648333/following/";
            httpHelper.GetRequestAsync(url, CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };
            AccountModel accountModel = new AccountModel(account);
            accountModel.RankToken = "";
            var result = _sut.GetUserFollowings(account, accountModel, userId, CancellationToken);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void HashTagUserFeedResponse_should_return_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.HashTagUserFeedResponse.json", Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/tags/hudba/sections/";
            JsonElements jsonElements = new JsonElements()
            {
                SupportedTabs = "[\"top\", \"recent\", \"places\"]",
                Csrftoken = "Ahm7ZtXFXipbFeZIOeTnzkKG3QGmEohZ",
                Uuid = "aaf9e39b-de8d-4e48-baa5-c019193ee3b4",

            };
            requestParameters.Body = jsonElements;
            requestParameters.DontSign();
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(
                    "_csrftoken=Ahm7ZtXFXipbFeZIOeTnzkKG3QGmEohZ&_uuid=aaf9e39b-de8d-4e48-baa5-c019193ee3b4&supported_tabs=[\"top\", \"recent\", \"places\"]");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            DominatorAccountModel account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       { AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333" },
               };
            AccountModel accountModel = new AccountModel(account);
            accountModel.CsrfToken = "Ahm7ZtXFXipbFeZIOeTnzkKG3QGmEohZ";
            accountModel.Uuid = "aaf9e39b-de8d-4e48-baa5-c019193ee3b4";
            var result = _sut.GetHashtagFeedForUserScraper(account, accountModel, "hudba", CancellationToken, 0, false, null, null);
            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void UploadPhoto_should_return_true_if_UploadPhoto_Success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            DominatorAccountModel account =
                new DominatorAccountModel
                {
                    AccountBaseModel = new DominatorAccountBaseModel
                        {AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333"},
                };
            AccountModel accountModel = new AccountModel(account);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.ImageUploadingPostResponse.json",
                Assembly.GetExecutingAssembly());

            string uploadId = "1581407091873";
            string imagePath = @".\RequiredData\B8YZfilhV10.jpg";
            var imageStream = Convert.ToBase64String(TestUtils.ReadFileFromResourcesAsBytes(
                "GramDominatorCore.UnitTests.RequiredData.B8YZfilhV10.jpg", Assembly.GetExecutingAssembly()));

            var url = $"https://i.instagram.com/rupload_igphoto/{uploadId}";
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Convert.ToBase64String(a);
                    postData.Should().Be(imageStream);
                }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });

            httpHelper.PostRequest("https://i.instagram.com/api/v1/media/configure/?", Arg.Do((byte[] a) =>
                {
                    var postdata = Encoding.UTF8.GetString(a);
                    postdata.Should().StartWith("signed_body=SIGNATURE.%7B%22caption%22%3A%22%22%2C%22_csrftoken%22%3A%22%22%2C%22device%22%3A%7B%22android_release%22%3A%");
                }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var result = _sut.UploadPhoto(account, accountModel, CancellationToken, imagePath, null, uploadId, false,
                "", null);

            result.Success.Should().Be(true);
        }

        [TestMethod]
        public void ConfigurePhoto()
        {                
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            DominatorAccountModel account =new DominatorAccountModel
            {
                AccountBaseModel =new DominatorAccountBaseModel{ AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983",},
                DeviceDetails=new DeviceGenerator
                { Model= "ironcmcc",Manufacturer= " Samsung", AndroidRelease= "8.1.0",AndroidVersion="27" },
                
            };
            AccountModel accountModel = new AccountModel(account);
            accountModel.CsrfToken = "AfMztIT3RB0auLXEOZEdcthUIJLorXzB";
            accountModel.Uuid = "6cc45b1f-5f55-4fb4-88a2-bc5f04156546";
            string uploadId = "1581407091873";
            string imagePath = @".\RequiredData\B8YZfilhV10.jpg";
            var pageresponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.ConfigurePhotoResponse.json",
              Assembly.GetExecutingAssembly());
            var jsonElements = new JsonElements()
            {
                Csrftoken = accountModel.CsrfToken,
                MediaFolder = "Instagram",
                SourceType = "4",
                Uid = account.AccountBaseModel.UserId,
                Uuid = accountModel.Uuid,
                Caption = "",
                UploadId = uploadId,
                DateTimeDigitalized = "2:11:2020 1:18:57 PM",
                DateTimeOriginal = "2:11:2020 1:18:57 PM",
                SceneCaptureType = "standard",
                CameraModel =account.DeviceDetails.Model,
                CameraMake = account.DeviceDetails.Manufacturer,
                CreationLoggerSessionId = "d09c60ce-22c4-43a4-99a5-acab5ab41934",
                Device = new JsonElements.DeviceJson()
                {
                    AndroidRelease = account.DeviceDetails.AndroidRelease,
                    AndroidVersion = account.DeviceDetails.AndroidVersion,
                    Manufacturer = account.DeviceDetails.Manufacturer,
                    Model = account.DeviceDetails.Model,
                },
                Edits = new JsonElements.EditsJson()
                {
                    CropOriginalSize = new[] { 1080, 1080 },
                    CropCenter = new[] { 0.0, 0.0 },
                    CropZoom = 1
                },
                Extra = new JsonElements.ExtraJson()
                {
                    SourceHeight = 1080,
                    SourceWidth = 1080
                }
            };
            var configureUrl = $"https://i.instagram.com/api/v1/media/configure/?";
            requestParameters.Body = jsonElements;
            byte[] ConfigurePostData = requestParameters.GenerateBody();
            httpHelper.PostRequest(configureUrl, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(ConfigurePostData);
                finalpostdata.Should().Be("signed_body=SIGNATURE.%7B%22caption%22%3A%22%22%2C%22_csrftoken%22%3A%22AfMztIT3RB0auLXEOZEdcthUIJLorXzB%22%2C%22device%22%3A%7B%22android_release%22%3A%228.1.0%22%2C%22android_version%22%3A%2227%22%2C%22manufacturer%22%3A%22+Samsung%22%2C%22model%22%3A%22ironcmcc%22%7D%2C%22edits%22%3A%7B%22crop_center%22%3A%5B0.0%2C0.0%5D%2C%22crop_original_size%22%3A%5B1080%2C1080%5D%2C%22crop_zoom%22%3A1%7D%2C%22extra%22%3A%7B%22source_height%22%3A1080%2C%22source_width%22%3A1080%7D%2C%22media_folder%22%3A%22Instagram%22%2C%22source_type%22%3A%224%22%2C%22_uid%22%3A%227150086983%22%2C%22upload_id%22%3A%221581407091873%22%2C%22_uuid%22%3A%226cc45b1f-5f55-4fb4-88a2-bc5f04156546%22%2C%22date_time_original%22%3A%222%3A11%3A2020+1%3A18%3A57+PM%22%2C%22scene_capture_type%22%3A%22standard%22%2C%22date_time_digitalized%22%3A%222%3A11%3A2020+1%3A18%3A57+PM%22%2C%22camera_model%22%3A%22ironcmcc%22%2C%22camera_make%22%3A%22+Samsung%22%2C%22creation_logger_session_id%22%3A%22d09c60ce-22c4-43a4-99a5-acab5ab41934%22%7D");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            _sut.ConfigurePhoto(account, accountModel, CancellationToken, imagePath, uploadId, null, "", null);
        }

        [TestMethod]
        public void UploadTimeLinePhoto_return_true()
        {
            var requestParameters = new IgRequestParameters();
            string QueryTypeDisplayName = "Custom Photos";
            httpHelper.GetRequestParameter().Returns(requestParameters);
            string imagePath = @".\RequiredData\B8YZfilhV10.jpg";
            var imageStream = Convert.ToBase64String(TestUtils.ReadFileFromResourcesAsBytes(
                "GramDominatorCore.UnitTests.RequiredData.B8YZfilhV10.jpg", Assembly.GetExecutingAssembly()));

            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.ImageUploadingGetResponse.json", Assembly.GetExecutingAssembly());
            _dateProvider.UtcNowUnix().Returns(1577836800000);
            var url = "https://i.instagram.com/rupload_igphoto/1577836800000";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                };

            var imageUploadResp = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.ImageUploadingPostResponse.json",
                Assembly.GetExecutingAssembly());
            httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Convert.ToBase64String(a);
                    postData.Should().Be(imageStream);
                }))
                .Returns(new ResponseParameter
                {
                    Response = imageUploadResp,
                    Exception = null,
                    HasError = false
                });
            httpHelper.PostRequest("https://i.instagram.com/api/v1/media/configure/?", Arg.Do((byte[] a) =>
                {
                    var postdata = Encoding.UTF8.GetString(a);
                    postdata.Should().StartWith("signed_body=SIGNATURE.%7B%22caption%22%3A%22Custom+Photos%22%2C%22_csrftoken%22%3A%22%22%2C%22device%22%3A%7B%22");
                }))
                .Returns(new ResponseParameter
                {
                    Response = imageUploadResp,
                    Exception = null,
                    HasError = false
                });

            _dateProvider.UtcNow().Returns(new DateTime(2020, 01, 01));
            AccountModel accountModel = new AccountModel(account);
            var result = _sut.UploadTimeLinePhoto(account, accountModel,CancellationToken, imagePath, null, null, false, QueryTypeDisplayName);
            result.Success.Should().Be(true);
        }

         [TestMethod]
        public void ChangeProfilePictureAsync_should_return_true_if_ChangeProfilePictureAsync_success()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            string uploadId = "1581330295960";
            JsonElements jsonElements = new JsonElements()
            {
                Csrftoken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz",
                Uid = "7150086983",
                Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07",
                UploadId = uploadId
            };
            var pageresponse = TestUtils.ReadFileFromResources(
            "GramDominatorCore.UnitTests.TestData.ChangeProfilePictureResponse.json",
            Assembly.GetExecutingAssembly());
            string imagePath = @".\RequiredData\B8YZfilhV10.jpg";
            var imageStream = TestUtils.ReadFileFromResources(
        "GramDominatorCore.UnitTests.RequiredData.B8YZfilhV10.jpg", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/accounts/change_profile_picture/";
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
                {
                    var finalpostdata = Encoding.UTF8.GetString(postData);
                    finalpostdata.Should().Be("signed_body=SIGNATURE.%7B%22_csrftoken%22%3A%22t9Wwm8geusRml2Wl7fOP4objbZyR1dpz%22%2C%22_uid%22%3A%227150086983%22%2C%22upload_id%22%3A%221581330295960%22%2C%22_uuid%22%3A%221ecd8d17-dbf0-47b6-88c3-e6177c7e0b07%22%7D");
                }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            var result = _sut.ChangeProfilePictureAsync(account, accountModel, imagePath,uploadId);
            result.Result.Success.Should().Be(true);
        }

         [TestMethod]  
        public void ChangeProfilePictureAsync_should_return_Null_if_ChangeProfilePictureAsync_Failed()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            string uploadId = "1581330295960";
            JsonElements jsonElements = new JsonElements()
            {
                Csrftoken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz",
                Uid = "",
                Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07",
                UploadId = uploadId
            };
            var pageresponse = TestUtils.ReadFileFromResources(
            "GramDominatorCore.UnitTests.TestData.ChangeProfilePictureResponse.json",
            Assembly.GetExecutingAssembly());
            string imagePath = @"D:\GitBucketRepositories\gramdominator-library\GramDominatorCore.UnitTests\RequiredData\B8YZfilhV10.jpg";
            var imageStream = TestUtils.ReadFileFromResources(
        "GramDominatorCore.UnitTests.RequiredData.B8YZfilhV10.jpg", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/accounts/change_profile_picture/";
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be("signed_body=457589dd94c8816a50deba9a65769d564ca85f6739877a318ef14bf8ed8a61e2.%7B%22_csrftoken%22%3A%22t9Wwm8geusRml2Wl7fOP4objbZyR1dpz%22%2C%22_uid%22%3A%227150086983%22%2C%22upload_id%22%3A%221581330295960%22%2C%22_uuid%22%3A%221ecd8d17-dbf0-47b6-88c3-e6177c7e0b07%22%7D&ig_sig_key_version=4");
            }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        { AccountNetwork = SocialNetworks.Instagram, UserId = "7150086983" },
                };
            var accountModel = new AccountModel(account);
            accountModel.CsrfToken = "t9Wwm8geusRml2Wl7fOP4objbZyR1dpz";
            accountModel.Uuid = "1ecd8d17-dbf0-47b6-88c3-e6177c7e0b07";
            var result = _sut.ChangeProfilePictureAsync(account, accountModel, imagePath,uploadId);
            result.Result.Should().BeNull();
        }

        // [TestMethod]
        //pending test for image
        public void SendPhotoAsDirectMessage_return_if_SendPhotoAsDirectMessage_success()
        {
            //var requestParameters = new IgRequestParameters();

            //httpHelper.GetRequestParameter().Returns(requestParameters);
            //var pageresponse = TestUtils.ReadFileFromResources(
            //    "GramDominatorCore.UnitTests.TestData.SendPhotoAsDirectMessageResponse.json", Assembly.GetExecutingAssembly());
            //string MediaId = "17917605616263640";
            //var url = "https://i.instagram.com/api/v1/media/17917605616263640/comment_like/";
            //JsonElements jsonElements = new JsonElements()
            //{
            //    Csrftoken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO",
            //    Uid = "2071648333",
            //    Uuid = "676480d2-b925-4bf7-a94d-5077b5681388",
            //};
            //requestParameters.Body = jsonElements;
            //byte[] postData = requestParameters.GenerateBody();
            //httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            //{
            //    var finalpostdata = Encoding.UTF8.GetString(postData);

            //    finalpostdata.Should().Be(
            //        "signed_body=93a03e62cfa6da4886270eb710e9a7c8d74d0a9037fda60495fa30ae16a29c8d.%7B%22_csrftoken%22%3A%22xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO%22%2C%22_uid%22%3A%222071648333%22%2C%22_uuid%22%3A%22676480d2-b925-4bf7-a94d-5077b5681388%22%7D&ig_sig_key_version=4");
            //}))
            //    .Returns(new ResponseParameter
            //    {
            //        Response = pageresponse,
            //        Exception = null,
            //        HasError = false
            //    });
            //var account =
            //    new DominatorAccountModel
            //    {
            //        AccountBaseModel =
            //            new DominatorAccountBaseModel
            //            { AccountNetwork = SocialNetworks.Instagram, UserId = "2071648333" },
            //    };
            //var accountModel = new AccountModel(account);
            //accountModel.CsrfToken = "xI8zLeCWY1zKiQVixkuRi4G6uGsV2oDO";
            //accountModel.Uuid = "676480d2-b925-4bf7-a94d-5077b5681388";
            //var result = _sut.SendPhotoAsDirectMessage(account, accountModel, CancellationToken, MediaId, "");
            //result.Success.Should().Be(true);
        }
    }

}
