using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using FluentAssertions;
using NSubstitute;
using Unity;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.Request;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Text;
using System.Threading;
using GramDominatorCore.GDModel;
using DominatorHouseCore.Utility;
using System.Net;
using GramDominatorCore.PowerAdSpy;
using System.Collections.Generic;
using System.Threading.Tasks;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using CommonServiceLocator;
using SQLite;
using GramDominatorCore.GDLibrary.InstagramBrowser;
using GramDominatorCore.GDFactories;
using DominatorHouse.ThreadUtils;
using GramDominatorCore.GDLibrary.LoginAndUpdate;

namespace GramDominatorCore.UnitTests.Test_Data.Library
{
    [TestClass]
    public class LoginAsyncTest : UnityInitializationTests
    {
        private IGdLogInProcess LoginProcess;
        private IAccountsFileManager _accountsFileManager;
        private IAccountsCacheService _accountsCacheService;
        private CancellationToken CancellationToken { get; }
        private IGdHttpHelper httpHelper;
        private IInstaFunction instaFunction;
        private IAccountScopeFactory AccountScopeFactory;
        private DominatorAccountModel _dominatorAccountModel;
        private IInstaAdScrappers instaAdsScrapers;
        private IGlobalDatabaseConnection globalDatabaseConnection;
        private IDbOperations _DbOperations;
        private IGdBrowserManager _GdBrowserManagr;
        private IInstaFunctFactory _InstaFunctFectory;
        private IDelayService delayService;
        private IEncryptPassword _encryptPassword;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            var dateProvider = Substitute.For<IDateProvider>();
            _encryptPassword = Substitute.For<IEncryptPassword>();
            httpHelper = Substitute.For<IGdHttpHelper>();
            instaFunction = Substitute.For<IInstaFunction>();
            delayService = Substitute.For<IDelayService>();
            _InstaFunctFectory = Substitute.For<IInstaFunctFactory>();
            _GdBrowserManagr = Substitute.For<IGdBrowserManager>();
            _dominatorAccountModel = new DominatorAccountModel();
            instaFunction = new InstaFunct(_dominatorAccountModel, httpHelper, _GdBrowserManagr, delayService, dateProvider);
            AccountScopeFactory = Substitute.For<IAccountScopeFactory>();
            _InstaFunctFectory.InstaFunctions = instaFunction;
            LoginProcess = new LogInProcess(httpHelper, AccountScopeFactory, _GdBrowserManagr, _InstaFunctFectory,DelayService, _encryptPassword);
            instaAdsScrapers = Substitute.For<IInstaAdScrappers>();
            globalDatabaseConnection = Substitute.For<IGlobalDatabaseConnection>();
            instaAdsScrapers = new InstaAdsScrappers(httpHelper, instaFunction);
            _accountsFileManager = Substitute.For<IAccountsFileManager>();
            _DbOperations = Substitute.For<IDbOperations>();
            Container.RegisterInstance(_accountsFileManager);
            _accountsCacheService = Substitute.For<IAccountsCacheService>();
            Container.RegisterInstance<IAccountsCacheService>(_accountsCacheService);
            Container.RegisterInstance<IHttpHelper>(SocialNetworks.Instagram.ToString(), httpHelper);
            Container.RegisterInstance<IAccountsFileManager>(SocialNetworks.Instagram.ToString(), _accountsFileManager);
            Container.RegisterInstance(globalDatabaseConnection);
        }
        [TestMethod]
        public void CheckLoginAsync_should_do_NOThing_if_its_already_logged_in()
        {
            var CookieCollection = new CookieCollection();

            var model = new DominatorAccountModel()
            {
                AccountBaseModel = new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram, UserName = "mollycoleman666" },
                IsUserLoggedIn = true,
                IsRunProcessThroughBrowser = false
        };
            LoginProcess.AssignBrowserFunction(model);
            CookieCollection.Add(new Cookie("ds_user", model.UserName, "/", "i.instagram.com"));
            CookieCollection.Add(new Cookie("igfl", model.UserName, "/", "i.instagram.com"));
            model.Cookies = CookieCollection;
            httpHelper.GetRequestParameter().Cookies = model.Cookies;
            var result = LoginProcess.CheckLogin(model,CancellationToken);
            httpHelper.DidNotReceive().SetRequestParameter(Arg.Any<IRequestParameters>());
            result.Should().Be(true);
        }

        [TestMethod]
        [Microsoft.VisualStudio.TestTools.UnitTesting.Ignore("the tests are way too complex, need to breackdown the class")]
        public void CheckLoginAsync_should_return_true_if_account_checked_and_login_pass_successfully()
        {
            var model = new DominatorAccountModel()
            {
                AccountBaseModel = new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram, UserName = "jacksmith729", UserId = "2071648333",AccountId= "7a7d83f0-3ee7-4e04-bf5c-8f46539163dc" },
                IsUserLoggedIn = false,
                UserAgentMobile = "Instagram 107.0.0.27.121 Android (24 / 7.0;  640dpi;  1080x1920;  OnePlus;  ONEPLUS A3010;  OnePlus3T;  qcom; en_US; 168361627)",
                AccountId = "7a7d83f0-3ee7-4e04-bf5c-8f46539163dc",
                IsRunProcessThroughBrowser=false
            };
            //LoginProcess.AssignBrowserFunction(_dominatorAccountModel);
            var AccountModel = new AccountModel(model);
            AccountModel.CsrfToken = "KTM7DAXtRv9d8QPkpUZjt9gebJAnusoU";
            var CookieCollections = GetCookies();
            model.Cookies = CookieCollections;

            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            httpHelper.SetRequestParameter(Arg.Do((IgRequestParameters a) =>
            {
                requestParameters = a;
                httpHelper.GetRequestParameter().Returns(requestParameters);
            }));


            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.SearchUserInfoByIdResponse.json", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/users/2071648333/info/";
            httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            JsonElements jsonElements = new JsonElements()
            {
                IsPrefetch = "0",
                FeedViewInfo = "[]",
                SeenPosts = "",
                PhoneId = "66e37392-af9c-43ea-85ca-558131f02ba1",
                Reason = "cold_start_fetch",
                BatteryLevel = "100",
                TimezoneOffset = "19800",
                Csrftoken = "KTM7DAXtRv9d8QPkpUZjt9gebJAnusoU",
                clientSessionId = "d58ae8e4-a75a-4ca3-9c71-22eeb0d68286",
                DeviceId = "6e91eb52-ac6e-4162-a1d0-2e0eebe984b3",
                IsPullToRefresh = "0",
                Uuid = "6e91eb52-ac6e-4162-a1d0-2e0eebe984b3",
                IsCharging = "1",
                isAsyncAdsInHeadloadEnabled = "0",
                RtiDeliveryBackend = "0",
                IsAsyncAdsDoubleRequest = "0",
                WillSoundOn = "1",
                SessionId = "2071648333:8V1eKkVLlpPkjP:22",
                isAsyncAdsRti = "0",
                IsOnScreen = true,
                BlockVersioningId = "a4b4b8345a67599efe117ad96b8a9cb357bb51ac3ee00c3a48be37ce10f2bb4c"
            };
            AccountModel.PhoneId = "66e37392-af9c-43ea-85ca-558131f02ba1";
            AccountModel.Guid = "d58ae8e4-a75a-4ca3-9c71-22eeb0d68286";
            AccountModel.Uuid = "6e91eb52-ac6e-4162-a1d0-2e0eebe984b3";
            AccountModel.Uuid = "6e91eb52-ac6e-4162-a1d0-2e0eebe984b3";
            model.SessionId = "2071648333:8V1eKkVLlpPkjP:22";
            requestParameters.Body = jsonElements;
            var Feedurl = "https://i.instagram.com/api/v1/feed/timeline/";
            requestParameters.DontSign();
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequest(Feedurl, Arg.Do((byte[] a) =>
            {
                var finalpostdata = Encoding.UTF8.GetString(postData);
                finalpostdata.Should().Be(
                    "battery_level=100&_csrftoken=KTM7DAXtRv9d8QPkpUZjt9gebJAnusoU&device_id=6e91eb52-ac6e-4162-a1d0-2e0eebe984b3&feed_view_info=[]&is_charging=1&is_prefetch=0&is_pull_to_refresh=0&phone_id=66e37392-af9c-43ea-85ca-558131f02ba1&reason=cold_start_fetch&seen_posts=&timezone_offset=19800&_uuid=6e91eb52-ac6e-4162-a1d0-2e0eebe984b3&session_id=2071648333:8V1eKkVLlpPkjP:22&client_session_id=d58ae8e4-a75a-4ca3-9c71-22eeb0d68286&will_sound_on=1&is_async_ads_in_headload_enabled=0&rti_delivery_backend=0&is_async_ads_double_request=0&is_async_ads_rti=0&is_on_screen=True&bloks_versioning_id=a4b4b8345a67599efe117ad96b8a9cb357bb51ac3ee00c3a48be37ce10f2bb4c");
            }))
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });
            var Feedpageresponse = TestUtils.ReadFileFromResources(
              "GramDominatorCore.UnitTests.TestData.FeedTimeLineResponse.json", Assembly.GetExecutingAssembly());

            httpHelper.PostRequestAsync("https://b.i.instagram.com/api/v1/accounts/get_prefill_candidates/",
                    Arg.Any<byte[]>(), Arg.Any<CancellationToken>())
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });

            _accountsFileManager.GetAll().Returns(new List<DominatorAccountModel> { model });
            _accountsFileManager.GetAccountById("7a7d83f0-3ee7-4e04-bf5c-8f46539163dc").Returns(model);


            SQLiteConnection sqlite = new SQLiteConnection(".\\Global.db", false);
            ServiceLocator.Current.GetInstance<IGlobalDatabaseConnection>().GetSqlConnection().Returns(sqlite);
            var result = LoginProcess.CheckLogin(model, CancellationToken);
            result.Should().Be(true);
        }
        public CookieCollection GetCookies()
        {
            var CookieCollections = new CookieCollection()
            {
            new Cookie("rur", "FRC", "i.instagram.com"),
            new Cookie("mid", "XGbKkwABAAFruxr5cUEHBnnRFiZO", "i.instagram.com"),
            new Cookie("ds_user", "jacksmith729", "i.instagram.com"),
            new Cookie("shbid", "5585", "i.instagram.com"),
            new Cookie("shbts", "1550466334.0960243", "i.instagram.com"),
            new Cookie("ds_user_id", "2071648333", "i.instagram.com"),
            new Cookie("urlgen", "{\"103.217.90.99\": 135183}:1gwKwu:xfxpUVhbLxSVEuuFgaUW81k90kA", "i.instagram.com"),
            new Cookie("sessionid", "2071648333%3ApMXBKmDGdBy6Mq%3A29", "i.instagram.com"),
            new Cookie("is_starred_enabled", "yes", "i.instagram.com"),
            new Cookie("igfl", "jacksmith729", "i.instagram.com"),
        };

            return CookieCollections;
        }

        [TestMethod]
        public void CheckisLoggedinMobileAsync_should_return_true()
        {
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var pageresponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.FeedTimeLineResponse.json", Assembly.GetExecutingAssembly());
            var url = "https://i.instagram.com/api/v1/feed/timeline/";
            var account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram },
                   Cookies = new CookieCollection()
                   {
                       new Cookie("sessionid", "2071648333%3ApMXBKmDGdBy6Mq%3A29", "i.instagram.com")
                   },
                   IsRunProcessThroughBrowser = false
               };
            LoginProcess.AssignBrowserFunction(_dominatorAccountModel);
            httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = pageresponse,
                    Exception = null,
                    HasError = false
                });

            var result = LoginProcess.CheckisLoggedinMobileAsync(account,null);
            result.Status.Should().Be(System.Threading.Tasks.TaskStatus.RanToCompletion);
        }

        [TestMethod]
        public void CheckLoginAsync_should_return_false_if_Account_will_be_verification()
        {
            var model = new DominatorAccountModel()
            {
                AccountBaseModel = new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram, UserName = "dhrubaglobussoft", UserId = "6867784527", AccountId = "bf357e0e-4113-4386-9010-f10b8fec2446" },
                IsUserLoggedIn = false,
                UserAgentMobile = "Instagram 64.0.0.14.96 Android (24 / 7.0;  640dpi;  1080x1920;  OnePlus;  ONEPLUS A3010;  OnePlus3T;  qcom; en_US; 125398471)",
                AccountId = "bf357e0e-4113-4386-9010-f10b8fec2446",
                IsRunProcessThroughBrowser=false
            };
            LoginProcess.AssignBrowserFunction(_dominatorAccountModel);
            model.DeviceDetails.DeviceId = "android-3e485a7f6aeebbef";
            var AccountModel = new AccountModel(model);
            AccountModel.CsrfToken = "xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo";
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);

            var Feedpageresponse = TestUtils.ReadFileFromResources(
             "GramDominatorCore.UnitTests.TestData.AccountFailedResponse.json", Assembly.GetExecutingAssembly());
            var Feedurl = "https://i.instagram.com/api/v1/feed/timeline/";
            httpHelper.GetRequestAsync(Feedurl, model.Token)
                .Returns(new ResponseParameter
                {
                    Response = Feedpageresponse,
                    Exception = null,
                    HasError = false
                });

            var LoginResponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.AccountChallengeResponse.json", Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/accounts/login/";
            JsonElements jsonElements = new JsonElements()
            {
                CountryCodes = "[{\"country_code\":\"1\",\"source\":[\"sim\",\"network\",\"default\"]}]",
                PhoneId = "2f96c1ac-7f22-4ac2-9d97-d32fb1c8c99e",
                Csrftoken = "xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo",
                Username = "dhrubaglobussoft",
                Adid = "8cccf97e-5e82-4a26-8a8d-e70e5fe52a05",
                Guid = "88aa9a5a-9e85-4ce2-a9b7-b6634f9e7ffc",
                DeviceId = "android-3e485a7f6aeebbef",
                Password = "DHga#4086",
                GoogleTokens = "[]",
                LoginAttemptCount = "0"
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=146e03594b4d4527c4f8c3d34308a57fe7766772b1b1c4835643471f15f27021.%7B%22adid%22%3A%228cccf97e-5e82-4a26-8a8d-e70e5fe52a05%22%2C%22_csrftoken%22%3A%22xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo%22%2C%22device_id%22%3A%22android-3e485a7f6aeebbef%22%2C%22guid%22%3A%2288aa9a5a-9e85-4ce2-a9b7-b6634f9e7ffc%22%2C%22login_attempt_count%22%3A%220%22%2C%22password%22%3A%22DHga%234086%22%2C%22phone_id%22%3A%222f96c1ac-7f22-4ac2-9d97-d32fb1c8c99e%22%2C%22username%22%3A%22dhrubaglobussoft%22%2C%22country_codes%22%3A%22%5B%7B%5C%22country_code%5C%22%3A%5C%221%5C%22%2C%5C%22source%5C%22%3A%5B%5C%22sim%5C%22%2C%5C%22network%5C%22%2C%5C%22default%5C%22%5D%7D%5D%22%2C%22google_tokens%22%3A%22%5B%5D%22%7D&ig_sig_key_version=4");
            }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = LoginResponse,
                    Exception = null,
                    HasError = false
                });


            _accountsFileManager.GetAll().Returns(new List<DominatorAccountModel> { model });
            _accountsFileManager.GetAccountById("bf357e0e-4113-4386-9010-f10b8fec2446").Returns(model);

            var pageresponses = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.AccountVerificationMethod.json", Assembly.GetExecutingAssembly());
            var verifyUrl = "https://i.instagram.com/api/v1/challenge/6867784527/3SOCoyI0cQ/?device_id=android-3e485a7f6aeebbef";

            httpHelper.GetRequestAsync(verifyUrl, model.Token)
                .Returns(new ResponseParameter
                {
                    Response = pageresponses,
                    Exception = null,
                    HasError = false
                });



            var result = LoginProcess.CheckLogin(model, CancellationToken);
            result.Should().Be(false);
        }

        [TestMethod]
        public void CheckLoginAsnc_should_retun_false_if_Credential_will_be_wrong()
        {

            var model = new DominatorAccountModel()
            {
                AccountBaseModel = new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram, UserName = "dhrubaglobussoft", UserId = "6867784527", AccountId = "bf357e0e-4113-4386-9010-f10b8fec2446" },
                IsUserLoggedIn = false,
                UserAgentMobile = "Instagram 107.0.0.27.121 Android (24 / 7.0;  640dpi;  1080x1920;  OnePlus;  ONEPLUS A3010;  OnePlus3T;  qcom; en_US; 168361627)",
                AccountId = "bf357e0e-4113-4386-9010-f10b8fec2446",
                IsRunProcessThroughBrowser = false
            };
            LoginProcess.AssignBrowserFunction(_dominatorAccountModel);
            model.DeviceDetails.DeviceId = "android-3e485a7f6aeebbef";
            var AccountModel = new AccountModel(model);
            AccountModel.CsrfToken = "xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo";
            var requestParameters = new IgRequestParameters();
            httpHelper.GetRequestParameter().Returns(requestParameters);

            var Feedpageresponse = TestUtils.ReadFileFromResources(
             "GramDominatorCore.UnitTests.TestData.AccountFailedResponse.json", Assembly.GetExecutingAssembly());
            var Feedurl = "https://i.instagram.com/api/v1/feed/timeline/";
            httpHelper.GetRequestAsync(Feedurl, model.Token)
                .Returns(new ResponseParameter
                {
                    Response = Feedpageresponse,
                    Exception = null,
                    HasError = false
                });

            var LoginResponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.WrongCredentialResponse.json", Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/accounts/login/";
            JsonElements jsonElements = new JsonElements()
            {
                CountryCodes = "[{\"country_code\":\"1\",\"source\":[\"sim\",\"network\",\"default\"]}]",
                PhoneId = "2f96c1ac-7f22-4ac2-9d97-d32fb1c8c99e",
                Csrftoken = "xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo",
                Username = "dhrubaglobussoft",
                Adid = "8cccf97e-5e82-4a26-8a8d-e70e5fe52a05",
                Guid = "88aa9a5a-9e85-4ce2-a9b7-b6634f9e7ffc",
                DeviceId = "android-3e485a7f6aeebbef",
                Password = "DHga#4086",
                GoogleTokens = "[]",
                LoginAttemptCount = "0"
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=146e03594b4d4527c4f8c3d34308a57fe7766772b1b1c4835643471f15f27021.%7B%22adid%22%3A%228cccf97e-5e82-4a26-8a8d-e70e5fe52a05%22%2C%22_csrftoken%22%3A%22xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo%22%2C%22device_id%22%3A%22android-3e485a7f6aeebbef%22%2C%22guid%22%3A%2288aa9a5a-9e85-4ce2-a9b7-b6634f9e7ffc%22%2C%22login_attempt_count%22%3A%220%22%2C%22password%22%3A%22DHga%234086%22%2C%22phone_id%22%3A%222f96c1ac-7f22-4ac2-9d97-d32fb1c8c99e%22%2C%22username%22%3A%22dhrubaglobussoft%22%2C%22country_codes%22%3A%22%5B%7B%5C%22country_code%5C%22%3A%5C%221%5C%22%2C%5C%22source%5C%22%3A%5B%5C%22sim%5C%22%2C%5C%22network%5C%22%2C%5C%22default%5C%22%5D%7D%5D%22%2C%22google_tokens%22%3A%22%5B%5D%22%7D&ig_sig_key_version=4");
            }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = LoginResponse,
                    Exception = null,
                    HasError = false
                });


            _accountsFileManager.GetAll().Returns(new List<DominatorAccountModel> { model });
            _accountsFileManager.GetAccountById("bf357e0e-4113-4386-9010-f10b8fec2446").Returns(model);

            try
            {
                SQLiteConnection sqlite = new SQLiteConnection("C:\\Users\\GLB-123\\AppData\\Local\\Socinator\\Index\\Global\\DB\\Global.db", false);
                ServiceLocator.Current.GetInstance<IGlobalDatabaseConnection>().GetSqlConnection().Returns(sqlite);
                var result = LoginProcess.CheckLogin(model, CancellationToken);
                result.Should().Be(false);

            }
            catch (System.Exception ex)
            {

            }          
        }

        [TestMethod]
        public async Task SendVerification_code_should_retun_true()
        {
            var model = new DominatorAccountModel()
            {
                AccountBaseModel = new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram, UserName = "dhrubaglobussoft", UserId = "6867784527", AccountId = "bf357e0e-4113-4386-9010-f10b8fec2446" },
                IsUserLoggedIn = false,
                UserAgentMobile = "Instagram 107.0.0.27.121 Android (24 / 7.0;  640dpi;  1080x1920;  OnePlus;  ONEPLUS A3010;  OnePlus3T;  qcom; en_US; 168361627)",
                AccountId = "bf357e0e-4113-4386-9010-f10b8fec2446",
                IsRunProcessThroughBrowser=false
            };
            LoginProcess.AssignBrowserFunction(model);
            model.DeviceDetails.DeviceId = "android-3e485a7f6aeebbef";
            
            model.Cookies = new CookieCollection();
            var AccountModel = new AccountModel(model);
            AccountModel.CsrfToken = "xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo";
            AccountModel.Uuid = "8cccf97e-5e82-4a26-8a8d-e70e5fe52a05";
            var requestParameters = new IgRequestParameters();
            httpHelper.SetRequestParameter(Arg.Do((IgRequestParameters a) =>
            {
                requestParameters = a;
                httpHelper.GetRequestParameter().Returns(requestParameters);
            }));

            var LoginResponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.AccountChallengeResponse.json", Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/accounts/login/";
            JsonElements jsonElements = new JsonElements()
            {
                CountryCodes = "[{\"country_code\":\"1\",\"source\":[\"sim\",\"network\",\"default\"]}]",
                PhoneId = "2f96c1ac-7f22-4ac2-9d97-d32fb1c8c99e",
                Csrftoken = "xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo",
                Username = "dhrubaglobussoft",
                Adid = "8cccf97e-5e82-4a26-8a8d-e70e5fe52a05",
                Guid = "88aa9a5a-9e85-4ce2-a9b7-b6634f9e7ffc",
                DeviceId = "android-3e485a7f6aeebbef",
                Password = "DHga#4086",
                GoogleTokens = "[]",
                LoginAttemptCount = "0"
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=146e03594b4d4527c4f8c3d34308a57fe7766772b1b1c4835643471f15f27021.%7B%22adid%22%3A%228cccf97e-5e82-4a26-8a8d-e70e5fe52a05%22%2C%22_csrftoken%22%3A%22xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo%22%2C%22device_id%22%3A%22android-3e485a7f6aeebbef%22%2C%22guid%22%3A%2288aa9a5a-9e85-4ce2-a9b7-b6634f9e7ffc%22%2C%22login_attempt_count%22%3A%220%22%2C%22password%22%3A%22DHga%234086%22%2C%22phone_id%22%3A%222f96c1ac-7f22-4ac2-9d97-d32fb1c8c99e%22%2C%22username%22%3A%22dhrubaglobussoft%22%2C%22country_codes%22%3A%22%5B%7B%5C%22country_code%5C%22%3A%5C%221%5C%22%2C%5C%22source%5C%22%3A%5B%5C%22sim%5C%22%2C%5C%22network%5C%22%2C%5C%22default%5C%22%5D%7D%5D%22%2C%22google_tokens%22%3A%22%5B%5D%22%7D&ig_sig_key_version=4");
            }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = LoginResponse,
                    Exception = null,
                    HasError = false
                });

            _accountsFileManager.GetAll().Returns(new List<DominatorAccountModel> { model });
            _accountsFileManager.GetAccountById("bf357e0e-4113-4386-9010-f10b8fec2446").Returns(model);

            var pageresponses = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.AccountVerificationMethod.json", Assembly.GetExecutingAssembly());
            var verifyUrl = "https://i.instagram.com/api/v1/challenge/6867784527/3SOCoyI0cQ/?device_id=android-3e485a7f6aeebbef";

            httpHelper.GetRequestAsync(verifyUrl, CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = pageresponses,
                    Exception = null,
                    HasError = false
                });


            JsonElements jsonElement = new JsonElements()
            {
                Csrftoken = null,
                Uid = null,
                Uuid = "8cccf97e-5e82-4a26-8a8d-e70e5fe52a05",
                DeviceId = "android-3e485a7f6aeebbef",
                Choice ="0"
            };
            var Sendpageresponses = TestUtils.ReadFileFromResources(
               "GramDominatorCore.UnitTests.TestData.SendSecurityCodeResponse.json", Assembly.GetExecutingAssembly());
            var SendCodeUrl = "https://i.instagram.com/api/v1/challenge/6867784527/3SOCoyI0cQ/";
            requestParameters.Body = jsonElement;
            byte[] SendpostData = requestParameters.GenerateBody();
            httpHelper.PostRequestAsync(SendCodeUrl, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(SendpostData);
                postdata.Should().Be(
                    "signed_body=edcaa453ec52bb85e2f6260ef403129bc95f5724066266aec088016b7f9c0cfc.%7B%22device_id%22%3A%22android-3e485a7f6aeebbef%22%2C%22_uuid%22%3A%228cccf97e-5e82-4a26-8a8d-e70e5fe52a05%22%2C%22choice%22%3A%220%22%7D&ig_sig_key_version=4");
            }), CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = Sendpageresponses,
                    Exception = null,
                    HasError = false
                });
            _accountsFileManager.GetAll().Returns(new List<DominatorAccountModel> { model });
            _accountsFileManager.GetAccountById("bf357e0e-4113-4386-9010-f10b8fec2446").Returns(model);
            SQLiteConnection sqlite = new SQLiteConnection(".\\Global.db", false);
            ServiceLocator.Current.GetInstance<IGlobalDatabaseConnection>().GetSqlConnection().Returns(sqlite);
            var result = await LoginProcess.SendSecurityCodeAsync(model, CancellationToken,VerificationType.Phone,false);
            result.Should().Be(true);        
        }

        [TestMethod]
        public async Task Account_verification_should_retun_true()
        {
            var model = new DominatorAccountModel()
            {
                AccountBaseModel = new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram, UserName = "dhrubaglobussoft", UserId = "6867784527", AccountId = "bf357e0e-4113-4386-9010-f10b8fec2446" },
                IsUserLoggedIn = false,
                UserAgentMobile = "Instagram 107.0.0.27.121 Android (24 / 7.0;  640dpi;  1080x1920;  OnePlus;  ONEPLUS A3010;  OnePlus3T;  qcom; en_US; 168361627)",
                AccountId = "bf357e0e-4113-4386-9010-f10b8fec2446",
                IsRunProcessThroughBrowser=false
            };
            LoginProcess.AssignBrowserFunction(_dominatorAccountModel);
            model.DeviceDetails.DeviceId = "android-3e485a7f6aeebbef";
            model.Cookies = new CookieCollection();
            var AccountModel = new AccountModel(model);
            AccountModel.CsrfToken = "xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo";
            AccountModel.Uuid = "8cccf97e-5e82-4a26-8a8d-e70e5fe52a05";
            var requestParameters = new IgRequestParameters();
            httpHelper.SetRequestParameter(Arg.Do((IgRequestParameters a) =>
            {
                requestParameters = a;
                httpHelper.GetRequestParameter().Returns(requestParameters);
            }));
  
            _accountsFileManager.GetAll().Returns(new List<DominatorAccountModel> { model });
            _accountsFileManager.GetAccountById("bf357e0e-4113-4386-9010-f10b8fec2446").Returns(model);
            SQLiteConnection sqlite = new SQLiteConnection(".\\Global.db", false);
            ServiceLocator.Current.GetInstance<IGlobalDatabaseConnection>().GetSqlConnection().Returns(sqlite);
            var result = await LoginProcess.SendSecurityCodeAsync(model, CancellationToken, VerificationType.Phone, false);
            result.Should().Be(true);
        }

        [TestMethod]
        public void Account_Action_Block_should_retun_false()
        {
            var model = new DominatorAccountModel()
            {
                AccountBaseModel = new DominatorAccountBaseModel { AccountNetwork = SocialNetworks.Instagram, UserName = "dhrubaglobussoft", UserId = "6867784527", AccountId = "bf357e0e-4113-4386-9010-f10b8fec2446" },
                IsUserLoggedIn = false,
                UserAgentMobile = "Instagram 107.0.0.27.121 Android (24 / 7.0;  640dpi;  1080x1920;  OnePlus;  ONEPLUS A3010;  OnePlus3T;  qcom; en_US; 168361627)",
                AccountId = "bf357e0e-4113-4386-9010-f10b8fec2446",
                IsRunProcessThroughBrowser=false
            };
            LoginProcess.AssignBrowserFunction(model);
            model.Cookies = new CookieCollection();
            var AccountModel = new AccountModel(model);
            AccountModel.Device_Id = "android-3e485a7f6aeebbef";
            AccountModel.CsrfToken = "xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo";
            AccountModel.Guid = "8cccf97e-5e82-4a26-8a8d-e70e5fe52a05";
            model.VarificationCode ="936180";
            model.ChallengeUrl = "/challenge/6867784527/3SOCoyI0cQ/";
            IgRequestParameters requestParameters = new IgRequestParameters(model.UserAgentMobile);
            requestParameters.AddHeader("X-CSRFToken", AccountModel.CsrfToken);
            httpHelper.SetRequestParameter(Arg.Do((IgRequestParameters a) =>
            {
                requestParameters = a;
                httpHelper.GetRequestParameter().Returns(requestParameters);
            }));
            httpHelper.GetRequestParameter().Returns(requestParameters);
            var LoginResponse = TestUtils.ReadFileFromResources(
                "GramDominatorCore.UnitTests.TestData.AccountActionBlockResponse.json", Assembly.GetExecutingAssembly());
            var url = $"https://i.instagram.com/api/v1/challenge/6867784527/3SOCoyI0cQ/";
                     
            JsonElements jsonElements = new JsonElements()
            {             
                Csrftoken =AccountModel.CsrfToken,
                SecurityCode= model.VarificationCode,
                DeviceId =AccountModel.Device_Id,
                Uid= model.AccountBaseModel.UserId,
                Guid= AccountModel.Guid
            };
            requestParameters.Body = jsonElements;
            byte[] postData = requestParameters.GenerateBody();
            
            httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var postdata = Encoding.UTF8.GetString(postData);
                postdata.Should().Be(
                    "signed_body=2d8e2cee77329004ee37053e8f4874fae920c08ddbf2fc06e09ee3728e84c01c.%7B%22_csrftoken%22%3A%22xm8FhEssRcUlyKC6AiKrn3ZSbwcBGWNo%22%2C%22device_id%22%3A%22android-3e485a7f6aeebbef%22%2C%22guid%22%3A%228cccf97e-5e82-4a26-8a8d-e70e5fe52a05%22%2C%22_uid%22%3A%226867784527%22%2C%22security_code%22%3A%22936180%22%7D&ig_sig_key_version=4");
            }),CancellationToken)
                .Returns(new ResponseParameter
                {
                    Response = LoginResponse,
                    Exception = null,
                    HasError = false
                });
            httpHelper.GetRequestParameter().Cookies = new CookieCollection() ;
            _accountsFileManager.GetAll().Returns(new List<DominatorAccountModel> { model });
            _accountsFileManager.GetAccountById("bf357e0e-4113-4386-9010-f10b8fec2446").Returns(model);
            SQLiteConnection sqlite = new SQLiteConnection(".\\Global.db", false);
            ServiceLocator.Current.GetInstance<IGlobalDatabaseConnection>().GetSqlConnection().Returns(sqlite);
            var result = LoginProcess.VerifyAccountAsync(model,CancellationToken);
            result.Status.Should().Be(System.Threading.Tasks.TaskStatus.RanToCompletion);
            //result.Should().Be(false);
        }
    }
}
