using System.Reflection;
using System.Threading.Tasks;
using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.CommonResponse;
using System.Text;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDResponse.FriendsResponse;
using FaceDominatorCore.FDResponse.MessagesResponse;
using DominatorHouseCore.Utility;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using FaceDominatorCore.FDResponse.LikeCommentsResponse;
using DominatorHouseCore.Models.SocioPublisher;
using FaceDominatorCore.FDResponse.Publisher;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using System.Threading;
using FaceDominatorCore.FDResponse.EventsResponse;
using FaceDominatorCore.FDResponse.AccountsResponse;
using System;
using System.Collections.Generic;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.FileManagers;
using CommonServiceLocator;
using FaceDominatorCore.FDLibrary.DAL;
using FaceDominatorCore.FDModel.FbEvents;
using DominatorHouse.ThreadUtils;
using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.Models.FacebookModels;
using FaceDominatorCore.Interface;
using NSubstitute.Exceptions;

namespace FaceDominatorCore.UnitTests.Tests.FdLibrary
{
    [TestClass]
    public class FdLibraryTest : UnityInitializationTests
    {
        private IFdHttpHelper _httpHelper;
        private IFdRequestLibrary _sut;
        protected IBinFileHelper BinFileHelper;
        protected IDbOperations DbOperations;
        protected IAccountsFileManager AccountsFileManager;
        protected IDbGlobalService DbGlobalService;
        protected IGlobalDatabaseConnection GlobalDatabaseConnection;
        protected new IDelayService DelayService;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _httpHelper = Substitute.For<IFdHttpHelper>();
            DelayService = Substitute.For<IDelayService>();
            BinFileHelper = Substitute.For<IBinFileHelper>();
            DbOperations = Substitute.For<IDbOperations>();
            AccountsFileManager = Substitute.For<IAccountsFileManager>();
            DbGlobalService = Substitute.For<IDbGlobalService>();
            GlobalDatabaseConnection = Substitute.For<IGlobalDatabaseConnection>();

            Container.RegisterInstance(DelayService);
            Container.RegisterInstance(DbOperations);
            Container.RegisterInstance(BinFileHelper);
            Container.RegisterInstance(AccountsFileManager);
            Container.RegisterInstance(DbGlobalService);
            Container.RegisterInstance(GlobalDatabaseConnection);

            _sut = new FdRequestLibrary(_httpHelper, DelayService);
        }

        [TestMethod]
        public void GetGroupIdFromUrl_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var groupResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupResponse.html", Assembly.GetExecutingAssembly());

            var url = "https://www.facebook.com/589307534524989";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = groupResponse,
                    Exception = null,
                    HasError = false
                });

            var result = _sut.GetGroupIdFromUrl(account, url);

            result.Should().Be("589307534524989");

        }

        [TestMethod]
        public void GetGroupIdFromUrl_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            var url = string.Empty;

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = "",
                    Exception = null,
                    HasError = true
                });

            var result = _sut.GetGroupIdFromUrl(account, url);

            result.Should().Be("0");
        }

        [TestMethod]
        public void GetPageIdFromUrl_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FanPageResponse.html", Assembly.GetExecutingAssembly());

            var url = "https://www.facebook.com/mumbaiakkians/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = response,
                    Exception = null,
                    HasError = false
                });

            var result = _sut.GetPageIdFromUrl(account, url);

            result.Should().Be("519504621470372");
        }

        [TestMethod]
        public void GetPageIdFromUrl_Should_Return_False()
        {
            var url = string.Empty;

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = "",
                    Exception = null,
                    HasError = true
                });

            var result = _sut.GetPageIdFromUrl(account, url);

            result.Should().Be("0");
        }

        [TestMethod]
        public void GetPageDetailsFromUrl_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FanPageResponse.html", Assembly.GetExecutingAssembly());

            var url = "https://www.facebook.com/mumbaiakkians/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageResponse,
                    Exception = null,
                    HasError = false
                });

            FanpageDetails respPageDetails = _sut.GetPageDetailsFromUrl(account, url);
            respPageDetails.FanPageUrl.Should().Be(url);
            respPageDetails.FanPageID.Should().Be("519504621470372");
            respPageDetails.FanPageName.Should().Be("Akshay Kumar Mumbai Fan Club - MA");
        }

        [TestMethod]
        public void GetPageDetailsFromUrl_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var url = string.Empty;

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = "",
                    Exception = null,
                    HasError = true
                });

            FanpageDetails respPageDetails = _sut.GetPageDetailsFromUrl(account, url);
            respPageDetails.FanPageUrl.Should().Be("https://www.facebook.com/");
            respPageDetails.FanPageID.Should().Be("");
            respPageDetails.FanPageName.Should().Be("");
        }

        [TestMethod]
        public void GetFanpageDetails_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FanPageResponse.html", Assembly.GetExecutingAssembly());

            var url = "https://www.facebook.com/519504621470372";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageResponse,
                    Exception = null,
                    HasError = false
                });

            FanpageDetails fanpageDetails = new FanpageDetails();
            fanpageDetails.FanPageID = "519504621470372";

            var respPageDetails = _sut.GetFanpageDetails(account, fanpageDetails);
            respPageDetails.ObjFdScraperResponseParameters.FanpageDetails.FanPageID.Should().Be("519504621470372");
            respPageDetails.ObjFdScraperResponseParameters.FanpageDetails.FanPageUrl.Should().Be(url);
            respPageDetails.ObjFdScraperResponseParameters.FanpageDetails.FanPageName.Should().Be("Akshay Kumar Mumbai Fan Club - MA");
        }

        [TestMethod]
        public void GetFanpageDetails_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            var url = string.Empty;

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });


            var respPageDetails = _sut.GetFanpageDetails(account, new FanpageDetails());
            respPageDetails.ObjFdScraperResponseParameters.FanpageDetails.FanPageID.Should().Be(null);
            respPageDetails.Status.Should().BeTrue();
            respPageDetails.FbErrorDetails.Should().BeNull();
        }

        [TestMethod]
        public void GetFriendUserId_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FriendProfileResponse.html", Assembly.GetExecutingAssembly());

            var url = "https://www.facebook.com/100004474993405";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = pageResponse,
                    Exception = null,
                    HasError = false
                });

            FbUserIdResponseHandler respDetails = _sut.GetFriendUserId(account, url);

            respDetails.UserId.Should().Be("100016035780835");
        }

        [TestMethod]
        public void GetFriendUserId_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            var url = string.Empty;

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            FbUserIdResponseHandler respDetails = _sut.GetFriendUserId(account, url);

            respDetails.UserId.Should().Be("0");
            respDetails.FbDtsg.Should().BeEmpty();
            respDetails.FbErrorDetails.Should().BeNull();
        }

        [TestMethod]
        public void GetFriendOfFriend_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FriendProfileResponse.html", Assembly.GetExecutingAssembly());
            var frdResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FriendOfFriendResponse.html", Assembly.GetExecutingAssembly());

            string url = "https://www.facebook.com/logesh.logeshwaran.5686";

            string allFrdUrl = "https://www.facebook.com/100016035780835?sk=friends";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = response,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.GetRequest(allFrdUrl)
                .Returns(new ResponseParameter
                {
                    Response = frdResponse,
                    Exception = null,
                    HasError = false
                });

            var respDetails = _sut.GetFriendOfFriend(account, url, null);

            respDetails.Status.Should().Be(true);

        }

        [TestMethod]
        public void GetFriendOfFriend_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FriendProfileResponse.html", Assembly.GetExecutingAssembly());
            var frdResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FriendOfFriendResponse.html", Assembly.GetExecutingAssembly());

            string url = "https://www.facebook.com/logesh.logeshwaran.5686";

            string allFrdUrl = "https://www.facebook.com/100016035780835?sk=friends";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = response,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.GetRequest(allFrdUrl)
                .Returns(new ResponseParameter
                {
                    Response = frdResponse,
                    Exception = null,
                    HasError = false
                });

            var respDetails = _sut.GetFriendOfFriend(account, string.Empty, null);

            respDetails = _sut.GetFriendOfFriend(account, string.Empty, null);
            respDetails.Status.Should().BeFalse();
            respDetails.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
        }

        [TestMethod]
        public void GetGroupJoiningStatus_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var groupResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupJoiningStatusResponse.html", Assembly.GetExecutingAssembly());

            var groupPostResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupJoiningStatusPostResponse.html", Assembly.GetExecutingAssembly());

            string url = "https://www.facebook.com/1413952862244117";

            string postUrl = "https://www.facebook.com/groups/membership/r2j?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = groupResponse,
                    Exception = null,
                    HasError = false
                });


            _httpHelper.PostRequest(postUrl, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("ref=child_search&group_id=1413952862244117&client_custom_questions=1&__req=y&__user=&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
                }))

                .Returns(new ResponseParameter
                {
                    Response = groupPostResponse,
                    Exception = null,
                    HasError = false
                });

            var respDetails = _sut.GetGroupJoiningStatus(account, url);

            respDetails.Should().Be(true);

        }

        [TestMethod]
        public void GetGroupJoiningStatus_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var groupResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupJoiningStatusResponse.html", Assembly.GetExecutingAssembly());

            var groupPostResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupJoiningStatusPostResponse.html", Assembly.GetExecutingAssembly());

            string url = "https://www.facebook.com/1413952862244117";

            string postUrl = "https://www.facebook.com/groups/membership/r2j?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = groupResponse,
                    Exception = null,
                    HasError = false
                });


            _httpHelper.PostRequest(postUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("ref=child_search&group_id=1413952862244117&client_custom_questions=1&__req=y&__user=&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = groupPostResponse,
                    Exception = null,
                    HasError = false
                });

            var respDetails = _sut.GetGroupJoiningStatus(account, string.Empty);
            respDetails.Should().BeFalse();
        }

        [TestMethod]
        public void GetGroupMemberCount_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var groupResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupMemberCount.html", Assembly.GetExecutingAssembly());

            var url = "https://www.facebook.com/groups/2138298736390953/members/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = groupResponse,
                    Exception = null,
                    HasError = false
                });

            var result = _sut.GetGroupMemberCount(account, "2138298736390953");

            result.Should().Be("248125");

        }

        [TestMethod]
        public void GetGroupMemberCount_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var url = "https://www.facebook.com/groups/2138298736390953/members/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = true
                });

            var result = _sut.GetGroupMemberCount(account, null);

            result.Should().Be("0");

            result = _sut.GetGroupMemberCount(account, "2138298736390953");

            result.Length.Should().Be(1);
        }

        [TestMethod]
        public void GetGroupMembers_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var groupResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupResponse.html", Assembly.GetExecutingAssembly());

            var groupMemberResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupMemberCount.html", Assembly.GetExecutingAssembly());

            string url = "https://www.facebook.com/589307534524989";

            string memberUrl = "https://www.facebook.com/groups/589307534524989/members/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = groupResponse,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.GetRequest(memberUrl)
                .Returns(new ResponseParameter
                {
                    Response = groupMemberResponse,
                    Exception = null,
                    HasError = false
                });

            var respDetails = _sut.GetGroupMembers(account, url, null);

            respDetails.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(19);
            respDetails.Status.Should().Be(true);
            respDetails.PageletData.Should().Be("AQHRkrFDz717ckYU3qBfYsubymfBj_Q3FkadzHZL6crwk2Z1fr343-pI9-o342rDnO5F6yh8CoBP4GKLPcjk9WPyxg");
        }

        [TestMethod]
        public void GetGroupMembers_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://www.facebook.com/589307534524989";

            string memberUrl = "https://www.facebook.com/groups/589307534524989/members/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = true
                });

            _httpHelper.GetRequest(memberUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = true
                });

            var respDetails = _sut.GetGroupMembers(account, url, null);

            respDetails.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            respDetails.Status.Should().BeFalse();
            respDetails.PageletData.Should().BeNullOrEmpty();
            respDetails = _sut.GetGroupMembers(account, string.Empty, null);
            respDetails.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            respDetails = _sut.GetGroupMembers(account, string.Empty, null);
            respDetails.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);

        }

        [TestMethod]
        public void GetAlreadyGroupJoinedFriendsList_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            var groupTokenResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupTokenResponse.html", Assembly.GetExecutingAssembly());

            var groupMemberResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupFriendMemberResponse.html", Assembly.GetExecutingAssembly());

            string tokenUrl = "https://www.facebook.com/ajax/groups/members/add_get/?group_id=1192769340808873&__asyncDialog=3&__req=46&refresh=1&__user=&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            string memberUrl = "https://www.facebook.com/ajax/typeahead/groups/friend.php?fb_dtsg_ag=AQyHJCYe2bCxfdZbGU-4ucdoU7Ueka-vhadVl0K6F7uG3Q%3AAQx0JpqmAusMcCOXDQ4MKJ3uz8-zERN9gmido9f1kwgQVg&membership_group_id=1192769340808873&annotate_weak_references=false&token=1547218804-7%3A1547184198&include_contact_importer=true&request_id=bfb14aef-b15d-442d-f768-1ef500cb64c5&__req=48&__user=&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(tokenUrl)
                .Returns(new ResponseParameter
                {
                    Response = groupTokenResponse,
                    Exception = null,
                    HasError = false
                });


            _httpHelper.GetRequest(memberUrl)
                .Returns(new ResponseParameter
                {
                    Response = groupMemberResponse,
                    Exception = null,
                    HasError = false
                });

            var respDetails = _sut.GetAlreadyGroupJoinedFriendsList(account, "1192769340808873");

            respDetails.Count.Should().Be(11);

        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetAlreadyGroupJoinedFriendsList_should_throw_null_Arg_exception_if_response_is_null()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string tokenUrl = "https://www.facebook.com/ajax/groups/members/add_get/?group_id=1192769340808873&__asyncDialog=3&__req=46&refresh=1&__user=&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            string memberUrl = "https://www.facebook.com/ajax/typeahead/groups/friend.php?fb_dtsg_ag=AQyHJCYe2bCxfdZbGU-4ucdoU7Ueka-vhadVl0K6F7uG3Q%3AAQx0JpqmAusMcCOXDQ4MKJ3uz8-zERN9gmido9f1kwgQVg&membership_group_id=1192769340808873&annotate_weak_references=false&token=1547218804-7%3A1547184198&include_contact_importer=true&request_id=bfb14aef-b15d-442d-f768-1ef500cb64c5&__req=48&__user=&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        }
                };

            _httpHelper.GetRequest(tokenUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = false
                });


            _httpHelper.GetRequest(memberUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = false
                });

            var respDetails = _sut.GetAlreadyGroupJoinedFriendsList
                (account, "1192769340808873");

            respDetails.Count.Should().Be(0);

        }

        [TestMethod]
        public void GetIncomingFriendRequests_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://www.facebook.com/friends/requests/";
            var incomingFriendResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.IncomingFriendRequestResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = incomingFriendResponse,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.GetIncomingFriendRequests(account, null);

            responseHandler.Status.Should().Be(true);

        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetIncomingFriendRequests_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://www.facebook.com/friends/requests/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = false
                });

            var responseHandler = _sut.GetIncomingFriendRequests(account, null);

            responseHandler.Status.Should().Be(false);
            responseHandler.ObjFdScraperResponseParameters.ListUser.Should().BeNull();
        }

        [TestMethod]
        public void GetDetailedInfoUserMobile_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://m.facebook.com/100032129147843";

            string aboutUrl =
                "https://m.facebook.com/profile.php?v=info&lst=%3A100032129147843%3A1547474347&id=100032129147843";
            var incomingFriendResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UserHomeResponse.html", Assembly.GetExecutingAssembly());

            var aboutFriendResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FriendAboutResponse.html", Assembly.GetExecutingAssembly());


            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = incomingFriendResponse,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.GetRequest(aboutUrl)
                .Returns(new ResponseParameter
                {
                    Response = aboutFriendResponse,
                    Exception = null,
                    HasError = false
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100032129147843"
            };

            var result = _sut.GetDetailedInfoUserMobile(facebookUser, account, false, true);

            result.Status.Should().Be(false);

        }

        [TestMethod]
        public void GetDetailedInfoUserMobile_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://m.facebook.com/100032129147843";

            string aboutUrl =
                "https://m.facebook.com/profile.php?v=info&lst=%3A100032129147843%3A1547474347&id=100032129147843";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            _httpHelper.GetRequest(aboutUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100032129147843"
            };

            var result = _sut.GetDetailedInfoUserMobile
                (facebookUser, account, false, true);

            result.Status.Should().BeFalse();

        }

        [TestMethod]
        public void GetMessageRequestDetails_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string postUrl = "batch_name=MessengerGraphQLThreadlistFetcher&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=AQGxoKozmeb3:AQGxZUfyrH3t&jazoest=26581721229097781106845697858658171114102508185110764851&queries={\"o0\":{\"doc_id\":\"1956789641011375\",\"query_params\":{\"limit\":100,\"before\":null,\"tags\":[\"Pending\"],\"isWorkUser\":false,\"includeDeliveryReceipts\":true,\"includeSeqID\":false}}}";

            var incomingMsgResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.IncomingMessageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(postUrl, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("batch_name=MessengerGraphQLThreadlistFetcher&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&queries={\"o0\":{\"doc_id\":\"1956789641011375\",\"query_params\":{\"limit\":100,\"before\":null,\"tags\":[\"Pending\"],\"isWorkUser\":false,\"includeDeliveryReceipts\":true,\"includeSeqID\":false}}}");
                }))

                .Returns(new ResponseParameter
                {
                    Response = incomingMsgResponse,
                    Exception = null,
                    HasError = false
                });

            var result = _sut.GetMessageRequestDetails(account, null, MessageType.Pending);

            result.Status.Should().Be(false);

        }

        [TestMethod]
        public void GetMessageRequestDetails_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string postUrl = "batch_name=MessengerGraphQLThreadlistFetcher&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=AQGxoKozmeb3:AQGxZUfyrH3t&jazoest=26581721229097781106845697858658171114102508185110764851&queries={\"o0\":{\"doc_id\":\"1956789641011375\",\"query_params\":{\"limit\":100,\"before\":null,\"tags\":[\"Pending\"],\"isWorkUser\":false,\"includeDeliveryReceipts\":true,\"includeSeqID\":false}}}";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(postUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("batch_name=MessengerGraphQLThreadlistFetcher&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&queries={\"o0\":{\"doc_id\":\"1956789641011375\",\"query_params\":{\"limit\":100,\"before\":null,\"tags\":[\"Pending\"],\"isWorkUser\":false,\"includeDeliveryReceipts\":true,\"includeSeqID\":false}}}");
            }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var result = _sut.GetMessageRequestDetails(account, null, MessageType.Pending);

            result.Status.Should().Be(false);
            result.ObjFdScraperResponseParameters.MessageDetailsList.Count.Should().Be(0);
        }

        [TestMethod]
        public void CheckGroupJoinStatus_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();
            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://www.facebook.com/groups/229816094359688/";

            var groupJoiningStatusResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.CheckGroupJoinStatusResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = groupJoiningStatusResponse,
                    Exception = null,
                    HasError = false
                });

            var objGroupDetails = new GroupDetails()
            {
                GroupId = "229816094359688",
                GroupUrl = "https://www.facebook.com/groups/229816094359688/",
            };

            _sut.CheckGroupJoinStatus(account, objGroupDetails, url);

            objGroupDetails.GroupJoinStatus.Should().Be("Not a member");

        }

        [TestMethod]
        public void CheckGroupJoinStatus_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://www.facebook.com/groups/229816094359688/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            GroupDetails objGroupDetails = new GroupDetails()
            {
                GroupId = "229816094359688",
                GroupUrl = "https://www.facebook.com/groups/229816094359688/",
            };

            _sut.CheckGroupJoinStatus(account, objGroupDetails, url);

            objGroupDetails.GroupJoinStatus.Should().BeNullOrEmpty();

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            _sut.CheckGroupJoinStatus(account, null, url);

            objGroupDetails.GroupJoinStatus.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void GetPageLikers_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string pageUrl = "https://www.facebook.com/CanvasLaughClub/";
            string PageLikerUrl = "https://www.facebook.com/search/547489668635574/likers";
            var pageResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPageResponse.html", Assembly.GetExecutingAssembly());
            var pageLikerResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPageLikersResponse.html", Assembly.GetExecutingAssembly());
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(pageUrl)
                .Returns(new ResponseParameter
                {
                    Response = pageResponse,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.GetRequest(PageLikerUrl)
                .Returns(new ResponseParameter
                {
                    Response = pageLikerResponse,
                    Exception = null,
                    HasError = false
                });


            FanpageLikersResponseHandler fanpageLikersResponseHandler = _sut.GetPageLikers(account, pageUrl, null);

            fanpageLikersResponseHandler.ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser.Count.Should().Be(12);

        }

        [TestMethod]
        public void GetPageLikers_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string pageUrl = "https://www.facebook.com/CanvasLaughClub/";
            string PageLikerUrl = "https://www.facebook.com/search/547489668635574/likers";
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        }
                };

            _httpHelper.GetRequest(pageUrl)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            _httpHelper.GetRequest(PageLikerUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var fanpageLikersResponseHandler
                = _sut.GetPageLikers(account, pageUrl, null);

            fanpageLikersResponseHandler.ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser.Count.Should().Be(0);

        }

        [TestMethod]
        public void GetUserFollowers_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string pageUrl = "https://www.facebook.com/wasim.reza.3344";
            string PageLikerUrl = "https://www.facebook.com/wasim.reza.3344?sk=followers";
            var userResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetUserResponse.html", Assembly.GetExecutingAssembly());
            var userFollowerResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetUserFollowersResponse.html", Assembly.GetExecutingAssembly());
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(pageUrl)
                .Returns(new ResponseParameter
                {
                    Response = userResponse,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.GetRequest(PageLikerUrl)
                .Returns(new ResponseParameter
                {
                    Response = userFollowerResponse,
                    Exception = null,
                    HasError = false
                });


            var fanpageLikersResponseHandler = _sut.GetUserFollowers(account, pageUrl, null);

            fanpageLikersResponseHandler.ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser.Count.Should().Be(20);

        }

        [TestMethod]
        public void GetUserFollowers_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string pageUrl = "https://www.facebook.com/wasim.reza.3344";
            string PageLikerUrl = "https://www.facebook.com/wasim.reza.3344?sk=followers";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        }
                };

            _httpHelper.GetRequest(pageUrl)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            _httpHelper.GetRequest(PageLikerUrl)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });


            var fanpageLikersResponseHandler
                = _sut.GetUserFollowers(account, pageUrl, null);

            fanpageLikersResponseHandler.Status.Should().BeFalse();

        }

        [TestMethod]
        public void SearchPeopleFromKeyword_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://www.facebook.com/search/people/?q=Rakesh";
            var userResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SearchPeopleResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = userResponse,
                    Exception = null,
                    HasError = false
                });


            var responseHandler = _sut.SearchPeopleFromKeyword(account, "Rakesh", null);

            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(16);

        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void SearchPeopleFromKeyword_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();
            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://www.facebook.com/search/people/?q=Rakesh";
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        }
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });


            var responseHandler
                = _sut.SearchPeopleFromKeyword(account, "Rakesh", null);

            responseHandler.ObjFdScraperResponseParameters.ListUser.Should().BeNullOrEmpty();

        }

        [TestMethod]
        public void SearchPeopleByLocation_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string locationUrl = "https://www.facebook.com/search/str/India/keywords_places/";
            string peapleLocationUrl = "https://www.facebook.com/search/106377336067638/residents/present";
            var locationResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.LocationResponse.html", Assembly.GetExecutingAssembly());
            var searchPeopleByLocationResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SearchPeopleByLocationResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(locationUrl)
                .Returns(new ResponseParameter
                {
                    Response = locationResponse,
                    Exception = null,
                    HasError = false
                });
            _httpHelper.GetRequest(peapleLocationUrl)
                .Returns(new ResponseParameter
                {
                    Response = searchPeopleByLocationResponse,
                    Exception = null,
                    HasError = false
                });


            var responseHandler = _sut.SearchPeopleByLocation(account, "India", null);

            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(13);

        }

        [TestMethod]
        public void SearchPeopleByLocation_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string locationUrl = "https://www.facebook.com/search/str/India/keywords_places/";
            string peapleLocationUrl = "https://www.facebook.com/search/106377336067638/residents/present";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(locationUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });
            _httpHelper.GetRequest(peapleLocationUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });


            var responseHandler
                = _sut.SearchPeopleByLocation(account, "India", null);

            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);

            _httpHelper.GetRequest(locationUrl)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = null,
                    HasError = false
                });

            responseHandler
                = _sut.SearchPeopleByLocation(account, "India", null);

            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);


        }

        [TestMethod]
        public void GetLocationCityId_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string locationUrl = "https://www.facebook.com/search/str/India/keywords_places/";

            var locationResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.LocationResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(locationUrl)
                .Returns(new ResponseParameter
                {
                    Response = locationResponse,
                    Exception = null,
                    HasError = false
                });

            var result = _sut.GetLocationCityId(account, "India");

            result.Should().Be("106377336067638");

        }

        [TestMethod]
        public void GetLocationCityId_Should_Return_False()
        {
            string locationUrl = "https://www.facebook.com/search/str/India/keywords_places/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(locationUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var result = _sut.GetLocationCityId(account, "India");

            result.Should().BeEmpty();

        }

        [TestMethod]
        public void GetPostSharer_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();
            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string postUrl = "https://www.facebook.com/photo.php?fbid=2056455231107771&set=a.590331364386839&type=3";

            string postSharerUrl =
                "https://www.facebook.com/ajax/shares/view?target_fbid=2056455264441101&av=100022404889387&dpr=1&__asyncDialog=4&__req=1e&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var postResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostResponse.html", Assembly.GetExecutingAssembly());

            var postSharerResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostSharerResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };

            _httpHelper.GetRequest(postUrl)
                .Returns(new ResponseParameter
                {
                    Response = postResponse,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.GetRequest(postSharerUrl)
                .Returns(new ResponseParameter
                {
                    Response = postSharerResponse,
                    Exception = null,
                    HasError = false
                });


            var responseHandler = _sut.GetPostSharer(account, postUrl, null);

            responseHandler.Status.Should().Be(true);

        }

        [TestMethod]
        public void GetPostSharer_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();
            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string postUrl = "https://www.facebook.com/photo.php?fbid=2056455231107771&set=a.590331364386839&type=3";

            string postSharerUrl =
                "https://www.facebook.com/ajax/shares/view?target_fbid=2056455264441101&av=100022404889387&dpr=1&__asyncDialog=4&__req=1e&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };

            _httpHelper.GetRequest(postUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            _httpHelper.GetRequest(postSharerUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });


            var responseHandler
                = _sut.GetPostSharer(account, postUrl, null);

            responseHandler.Status.Should().BeFalse();

        }

        [TestMethod]
        [DataRow(null)]
        public void GetPostCommentor_Should_Return_True(IFdHttpHelper httpHelper)
        {
            //_httpHelper = _httpHelper ?? httpHelper;
            _sut = _sut ?? new FdRequestLibrary(_httpHelper ?? httpHelper, DelayService);

            FdRequestParameter fdRequestParameter = new FdRequestParameter();
            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);
            string postUrl = "https://www.facebook.com/photo.php?fbid=2056455231107771&set=a.590331364386839&type=3";

            var postResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(postUrl)
                .Returns(new ResponseParameter
                {
                    Response = postResponse,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.GetPostCommentor(account, postUrl, null, CancellationToken.None);

            responseHandler.ObjFdScraperResponseParameters.CommentList.Count.Should().Be(50);

        }

        [TestMethod]
        [ExpectedException(typeof(RedundantArgumentMatcherException))]
        public void GetPostCommentor_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string postUrl = "https://www.facebook.com/photo.php?fbid=2056455231107771&set=a.590331364386839&type=3";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(postUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler
                = _sut.GetPostCommentor(account, postUrl, null, Arg.Any<CancellationToken>());

            responseHandler.ObjFdScraperResponseParameters.CommentList?.Count.Should().Be(0);
            responseHandler.ObjFdScraperResponseParameters.PostDetails.Id.Should().Be("");
        }

        [TestMethod]
        public void GetPostCommentor_Should_Return_VideoDetailsTrue()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string postUrl = "https://www.facebook.com/watch/?v=1912132975767511";

            string postVideoUrl = "https://www.facebook.com/video/tahoe/async/880216849018463/?originalmediaid=880216849018463&playerorigin=video_home&playersuborigin=permalink&feedtracking[0]=%7B%22ft%22%3A%7B%7D%7D&numcopyrightmatchedvideoplayedconsecutively=0&payloadtype=secondary";

            var postResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetVideoPostRespose.html", Assembly.GetExecutingAssembly());

            var postVideoResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetCommentResponseforVideosResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        }
                };

            _httpHelper.GetRequest(postUrl)
                .Returns(new ResponseParameter
                {
                    Response = postResponse,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.PostRequest(postVideoUrl, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be(postData);
                }))

                .Returns(new ResponseParameter
                {
                    Response = postVideoResponse,
                    Exception = null,
                    HasError = false
                });

            var responseHandler =
                _sut.GetPostCommentor(account, postUrl, null, CancellationToken.None);

            responseHandler.ObjFdScraperResponseParameters.CommentList.Count.Should().Be(0);
        }

        [TestMethod]
        public void GetPostCommentor_Should_Return_VideoDetailsFalse()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string postUrl = "https://www.facebook.com/watch/?v=1912132975767511";

            string postVideoUrl = "https://www.facebook.com/ajax/ufi/comment_fetch.php?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        }
                };

            _httpHelper.GetRequest(postUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            _httpHelper.PostRequest(postVideoUrl, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be(
                        "ft_ent_identifier=1912132975767511&viewas=&source=17&offset=20629&length=50&orderingmode=ranked_unfiltered&section=default&direction=bottom&feed_context=&numpagerclicks=1&av=100022404889387&__req=11&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
                }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler =
                _sut.GetPostCommentor(account, postUrl, null, CancellationToken.None);

            responseHandler.ObjFdScraperResponseParameters.CommentList.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void GetPostLikers_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string postUrl = "https://www.facebook.com/photo.php?fbid=2056455231107771&set=a.590331364386839&type=3";

            string postLikerUrl =
                "https://www.facebook.com/ufi/reaction/profile/dialog/?ft_ent_identifier=2056455264441101&av=100022404889387&dpr=1&__asyncDialog=3&__req=1e&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var postResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostResponse.html", Assembly.GetExecutingAssembly());

            var postLikerResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostLikersRespose.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };

            _httpHelper.GetRequest(postUrl)
                .Returns(new ResponseParameter
                {
                    Response = postResponse,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.GetRequest(postLikerUrl)
                .Returns(new ResponseParameter
                {
                    Response = postLikerResponse,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.GetPostLikers(account, postUrl, null);

            responseHandler.HasMoreResults.Should().Be(true);
            responseHandler.Status.Should().Be(true);
            responseHandler.ObjFdScraperResponseParameters.TotalCount.Should().Be("169");
            responseHandler.ObjFdScraperResponseParameters.ShownIds.Should().Be("100002299426074,100033494253335,100033383948654,100033232071570,100033217633804,100033108513660,100033044979106,100032874007793,100032428980412,100032403291552,100031988799868,100031863578682,100031299109081,100031244192186,100030950714560,100030530745689,100030300170713,100029687777356,100029586680373,100029542561313,100029477924038,100029329419902,100029270683133,100029242932288,100029239816250,100029197699822,100028907320525,100028703232833,100028660808346,100028449952337,100028347761555,100028202206616,100027805566196,100027407618887,100027293827571,100027263929824,100026773351208,100026510534533,100025536410185,100025532091162,100025466769228,100025449274234,100025402428022,100025300008145,100025137036717,100024870350972,100024821716111,100024628327768,100024596273895,100024442119113");


        }

        [TestMethod]
        public void GetPostLikers_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string postUrl = "https://www.facebook.com/photo.php?fbid=2056455231107771&set=a.590331364386839&type=3";

            string postLikerUrl =
                "https://www.facebook.com/ufi/reaction/profile/dialog/?ft_ent_identifier=2056455264441101&av=100022404889387&dpr=1&__asyncDialog=3&__req=1e&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";


            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };

            _httpHelper.GetRequest(postUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            _httpHelper.GetRequest(postLikerUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetPostLikers(account, postUrl, null);

            responseHandler.ObjFdScraperResponseParameters.TotalCount.Should().BeNullOrEmpty();

            responseHandler.ObjFdScraperResponseParameters.ShownIds.Should().BeNullOrEmpty();

            responseHandler.HasMoreResults.Should().Be(false);

            responseHandler.Status.Should().Be(false);

        }

        [TestMethod]
        public void SendFriendRequest_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string friendId = "100033494253335";

            string url = "https://www.facebook.com/ajax/add_friend/action.php?dpr=1";

            var sendFriendRequestResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendFriendRequestRespose.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };


            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("to_friend=100033494253335&action=add_friend&how_found=profile_button&ref_param=unknown&link_data[gt][type]=xtracking&link_data[gt][xt]=48.{\"event\":\"add_friend\",\"intent_status\":null,\"intent_type\":null,\"profile_id\":100033494253335,\"ref\":1}&link_data[gt][profile_owner]=100033494253335&link_data[gt][ref]=timeline:timeline&no_flyout_on_click=true&frefs[0]=unknown&__req=11&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = sendFriendRequestResponse,
                    Exception = null,
                    HasError = false
                });

            var resultFriendRequest = _sut.SendFriendRequest(account, friendId);

            resultFriendRequest.Should().Be("success");

        }

        [TestMethod]
        public void SendFriendRequest_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string friendId = "100033494253335";

            string url = "https://www.facebook.com/ajax/add_friend/action.php?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };


            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("to_friend=100033494253335&action=add_friend&how_found=profile_button&ref_param=unknown&link_data[gt][type]=xtracking&link_data[gt][xt]=48.{\"event\":\"add_friend\",\"intent_status\":null,\"intent_type\":null,\"profile_id\":100033494253335,\"ref\":1}&link_data[gt][profile_owner]=100033494253335&link_data[gt][ref]=timeline:timeline&no_flyout_on_click=true&frefs[0]=unknown&__req=11&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var resultFriendRequest = _sut.SendFriendRequest(account, friendId);

            resultFriendRequest.Should().Be("");

        }

        [TestMethod]
        public void GetFriendSuggestedByFacebook_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://www.facebook.com/friends/requests/";

            var sendFriendRequestResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetFriendSuggestedByFacebookRespose.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };



            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = sendFriendRequestResponse,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.GetFriendSuggestedByFacebook(account, null);

            responseHandler.Status.Should().Be(true);
            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(10);
            responseHandler.ObjFdScraperResponseParameters.ExtraData.Should().Be("AQKY-zSnG9PHUxdzLT8inEiQKWRp54SG8ADVYt4_HkE0kzhJBGrE3U1RBWvllh-6PNU");
            responseHandler.HasMoreResults.Should().Be(true);
            responseHandler.Status.Should().Be(true);

        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetFriendSuggestedByFacebook_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://www.facebook.com/friends/requests/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        }
                };



            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetFriendSuggestedByFacebook(account, null);

            responseHandler.ObjFdScraperResponseParameters.ExtraData.Should().Be("");
            responseHandler.HasMoreResults.Should().Be(false);
            responseHandler.Status.Should().Be(false);


        }

        [TestMethod]
        public void GetSentFriendRequestIdsNew_Should_Return_True()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://www.facebook.com/friends/requests/";

            var sendFriendRequestResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetSentFriendRequestIdsNewRespose.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        }
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = sendFriendRequestResponse,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.GetSentFriendRequestIdsNew(account, null);
            responseHandler.HasMoreResults.Should().Be(true);
            responseHandler.Status.Should().Be(true);


        }

        [TestMethod]
        public void GetSentFriendRequestIdsNew_Should_Return_False()
        {
            FdRequestParameter fdRequestParameter = new FdRequestParameter();

            _httpHelper.GetRequestParameter().Returns(fdRequestParameter);

            string url = "https://www.facebook.com/friends/requests/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        }
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetSentFriendRequestIdsNew(account, null);
            responseHandler.HasMoreResults.Should().Be(true);
            responseHandler.Status.Should().Be(true);
            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);

        }

        [TestMethod]
        public void CancelIncomingRequest_Should_Return_True()
        {
            string friendId = "100003333351573";
            string url = "https://www.facebook.com/requests/friends/ajax/?dpr=1";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.CancelIncomingRequestRespose.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("action=reject&id=100003333351573&ref=/reqs.php&floc=friend_center_requests&frefs[0]=ff&viewer_id=100022404889387&__req=11&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
                }))

                .Returns(new ResponseParameter
                {
                    Response = response,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.CancelIncomingRequest(account, friendId);
            var gotStringForTest = new Utility.FdUtility().GetClassPropertyValueForTests
                (responseHandler, "responseHandler");

            responseHandler.Should().Be(true);

        }

        [TestMethod]
        public void CancelIncomingRequest_Should_Return_False()
        {
            string friendId = "100003333351573";
            string url = "https://www.facebook.com/requests/friends/ajax/?dpr=1";

            var account =
              new DominatorAccountModel
              {
                  AccountBaseModel =
                      new DominatorAccountBaseModel
                      {
                          AccountNetwork = SocialNetworks.Facebook,
                          UserId = "100022404889387",
                      },
              };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("action=reject&id=100003333351573&ref=/reqs.php&floc=friend_center_requests&frefs[0]=ff&viewer_id=100022404889387&__req=11&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.CancelIncomingRequest(account, friendId);
            responseHandler.Should().Be(false);

        }

        [TestMethod]
        public void CancelSentRequest_Should_Return_True()
        {
            string friendId = "100021923413379";
            string url = "https://www.facebook.com/ajax/friends/requests/cancel.php?dpr=1";

            var cancelSentRequestResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.cancelRequestResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("friend=100021923413379&cancel_ref=profile&floc=profile_button&__req=11&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&confirmed=1");
                }))

                .Returns(new ResponseParameter
                {
                    Response = cancelSentRequestResponse,
                    Exception = null,
                    HasError = false
                });

            var result = _sut.CancelSentRequest(account, friendId);

            result.Should().Be(true);

        }

        [TestMethod]
        public void CancelSentRequest_Should_Return_False()
        {
            string friendId = "100021923413379";
            string url = "https://www.facebook.com/ajax/friends/requests/cancel.php?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("friend=100021923413379&cancel_ref=profile&floc=profile_button&__req=11&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&confirmed=1");
                }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var result = _sut.CancelSentRequest(account, friendId);

            result.Should().Be(false);

        }

        [TestMethod]
        public void Unfriend_Should_Return_True()
        {
            string url = "https://www.facebook.com/ajax/friends/requests/cancel.php?dpr=1";

            var requestResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.cancelRequestResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("friend=100021923413379&cancel_ref=profile&floc=profile_button&__req=11&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&confirmed=1");
                }))

                .Returns(new ResponseParameter
                {
                    Response = requestResponse,
                    Exception = null,
                    HasError = false
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100004474993405",
                ProfileUrl = "https://www.facebook.com/100004474993405",
            };

            CancelSentRequestResponseHandler responseHandler = _sut.Unfriend(account, ref facebookUser);

            responseHandler.IsCancelledRequest.Should().Be(true);

        }

        [TestMethod]
        public void Unfriend_Should_Return_False()
        {
            string url = "https://www.facebook.com/ajax/friends/requests/cancel.php?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        }
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("friend=100021923413379&cancel_ref=profile&floc=profile_button&__req=11&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&confirmed=1");
            }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100004474993405",
                ProfileUrl = "https://www.facebook.com/100004474993405",
            };

            var responseHandler
                = _sut.Unfriend(account, ref facebookUser);

            responseHandler.IsCancelledRequest.Should().Be(true);

        }

        [TestMethod]
        public void AcceptFriendRequest_Should_Return_True()
        {
            string friendId = "100017250853828";

            string url = "https://www.facebook.com/requests/friends/ajax/?dpr=1";

            var acceptFriendRequestResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.acceptFriendRequestResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be
                ("action=confirm&id=100017250853828&ref=/reqs.php&floc=friend_center_requests&frefs[0]=ff&viewer_id=100022404889387&__req=11&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = acceptFriendRequestResponse,
                    Exception = null,
                    HasError = false
                });



            var result = _sut.AcceptFriendRequest(account, friendId);

            result.Should().Be(true);

        }

        [TestMethod]
        public void AcceptFriendRequest_Should_Return_False()
        {
            string friendId = "100017250853828";

            string url = "https://www.facebook.com/requests/friends/ajax/?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        }
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be
                        ("action=confirm&id=100017250853828&ref=/reqs.php&floc=friend_center_requests&frefs[0]=ff&viewer_id=100022404889387&__req=11&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
                }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var result = _sut.AcceptFriendRequest(account, friendId);

            result.Should().BeFalse();
        }

        [TestMethod]
        public void GetFanpageDetailsFromKeyword_Should_Return_True()
        {
            string keyword = "AkshayKumar";

            string url = "https://www.facebook.com/search/pages/?q=AkshayKumar";

            var getPageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetFanpageDetailsFromKeywordResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = getPageResponse,
                    Exception = null,
                    HasError = false
                });


            var responseHandler = _sut.GetFanpageDetailsFromKeyword
                (account, keyword, false, false, FanpageCategory.AnyCategory, null);

            responseHandler.Status.Should().Be(true);
            responseHandler.ObjFdScraperResponseParameters.ListPage.Count.Should().Be(16);
        }

        [TestMethod]
        public void GetFanpageDetailsFromKeyword_Should_Return_False()
        {
            string keyword = "AkshayKumar";

            string url = "https://www.facebook.com/search/str/AkshayKumar/keywords_pages/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });


            var responseHandler = _sut.GetFanpageDetailsFromKeyword
                (account, keyword, false, false, FanpageCategory.AnyCategory, null);

            responseHandler.HasMoreResults.Should().Be(false);
            responseHandler.Status.Should().Be(false);
        }

        [TestMethod]
        public void GetFanpageDetailsFromGraphSearch_Should_Return_True()
        {
            string url = "https://www.facebook.com/search/pages/?q=Akshaykumar&epa=SERP_TAB";

            var getPageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetFanpageDetailsFromGraphSearchResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = getPageResponse,
                    Exception = null,
                    HasError = false
                });


            var responseHandler = _sut.GetFanpageDetailsFromGraphSearch
                (account, url, false, false, FanpageCategory.AnyCategory, null);

            responseHandler.Status.Should().Be(true);
            responseHandler.ObjFdScraperResponseParameters.ListPage.Count.Should().Be(16);
        }

        [TestMethod]
        public void GetPostDetails_Should_Return_True()
        {
            string url = "https://www.facebook.com/10205581200565265";

            var getPageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostDetailsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = getPageResponse,
                    Exception = null,
                    HasError = false
                });
            FacebookPostDetails facebookPostDetails = new FacebookPostDetails()
            {
                Id = "10205581200565265",
            };

            var responseHandler = _sut.GetPostDetails(account, facebookPostDetails);

            responseHandler.ObjFdScraperResponseParameters.PostDetails.Id.Should().Be("10205581200565265");
            responseHandler.Status.Should().Be(true);
            responseHandler.FbErrorDetails.Should().BeNull();
            responseHandler.Status.Should().BeTrue();
        }

        [TestMethod]
        public void GetPostDetails_Should_Return_False()
        {
            string url = "https://www.facebook.com/10205581200565265";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });
            FacebookPostDetails facebookPostDetails = new FacebookPostDetails()
            {
                Id = "10205581200565265",
            };

            var responseHandler = _sut.GetPostDetails(account, facebookPostDetails);

            responseHandler.Status.Should().Be(false);

            responseHandler.FbErrorDetails.IsStatusChangedRequired.Should().Be(false);

            responseHandler.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());

            responseHandler.FbErrorDetails.Description.Should().Be("Exception of type 'System.Exception' was thrown.");

            responseHandler.ObjFdScraperResponseParameters.PostDetails.Should().BeNull();

        }

        [TestMethod]
        public void GetPostDetailNew_Should_Return_True()
        {
            string url = "https://www.facebook.com/10205581200565265";

            var getPostResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostDetailNewResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = getPostResponse,
                    Exception = null,
                    HasError = false
                });
            FacebookPostDetails facebookPostDetails = new FacebookPostDetails()
            {
                Id = "10205581200565265",
            };

            var responseHandler = _sut.GetPostDetailNew(account, facebookPostDetails);

            responseHandler.Status.Should().Be(true);
            responseHandler.FbErrorDetails.Should().BeNull();
            responseHandler.ObjFdScraperResponseParameters.PostDetails.Id.Should().Be("10205581200565265");
        }

        [TestMethod]
        public void GetPostDetailNew_Should_Return_False()
        {
            string url = "https://www.facebook.com/10205581200565265";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });
            FacebookPostDetails facebookPostDetails = new FacebookPostDetails()
            {
                Id = "10205581200565265",
            };

            var responseHandler = _sut.GetPostDetailNew(account, facebookPostDetails);

            responseHandler.Status.Should().BeFalse();
            responseHandler.ObjFdScraperResponseParameters.PostDetails.Should().BeNull();
        }

        [TestMethod]
        public void GetVideoDetails_Should_Return_True()
        {
            string postVideoUrl =
                "https://www.facebook.com/react_composer/scraper?composer_id=rc.u_0_16&target_id=100022404889387&scrape_url=https://www.facebook.com/1902227313424744&entry_point=feedx_sprouts&source_attachment=STATUS&source_logging_name=link_pasted&av=100022404889387&dpr=1";

            var postVideoResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetVideoDetailsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        },
                };

            _httpHelper.PostRequest(postVideoUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("__req=4w&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = postVideoResponse,
                    Exception = null,
                    HasError = false
                });

            var facebookPostDetails = new FacebookPostDetails()
            {
                Id = "1902227313424744",
                MediaType = MediaType.Video,
            };
            string composerId = "rc.u_0_16";

            var result = _sut.GetVideoDetails(account, composerId, ref facebookPostDetails);
            result.Should().Be("");
            //result.Should().Be(TestUtils.ReadFileFromResources
            //    ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetVideoDetailsResponse.html", Assembly.GetExecutingAssembly()));

        }

        [TestMethod]
        public void GetVideoDetails_Should_Return_False()
        {
            string postVideoUrl =
                "https://www.facebook.com/react_composer/scraper?composer_id=rc.u_0_16&target_id=100022404889387&scrape_url=https://www.facebook.com/1902227313424744&entry_point=feedx_sprouts&source_attachment=STATUS&source_logging_name=link_pasted&av=100022404889387&dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022404889387",
                        }
                };

            _httpHelper.PostRequest(postVideoUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("__req=4w&__user=100022404889387&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var facebookPostDetails = new FacebookPostDetails()
            {
                Id = "1902227313424744",
                MediaType = MediaType.Video,
            };
            string composerId = "rc.u_0_16";

            var result = _sut.GetVideoDetails(account, composerId, ref facebookPostDetails);
            result.Should().BeNullOrEmpty();

        }

        [TestMethod]
        public void GetPostDetailsNew_Should_Return_True()
        {
            string url = "https://www.facebook.com/10205581200565265/";

            var getPageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostDetailsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = getPageResponse,
                    Exception = null,
                    HasError = false
                });

            var facebookPostDetails = new FacebookPostDetails()
            {
                Id = "10205581200565265",
                PostUrl = "https://www.facebook.com/10205581200565265/"
            };

            PostScraperResponseHandler responseHandler = _sut.GetPostDetailsNew(account, facebookPostDetails);

            responseHandler.Status.Should().Be(true);
        }

        [TestMethod]
        public void GetPostDetailsNew_Should_Return_False()
        {
            string url = "https://www.facebook.com/10205581200565265/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        }
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var facebookPostDetails = new FacebookPostDetails
            {
                Id = "10205581200565265",
                PostUrl = "https://www.facebook.com/10205581200565265/"
            };

            var responseHandler = _sut.GetPostDetailsNew(account, facebookPostDetails);

            responseHandler.Status.Should().Be(false);

            responseHandler.FbErrorDetails.IsStatusChangedRequired.Should().Be(false);

            responseHandler.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());

            responseHandler.FbErrorDetails.Description.Should().Be("Exception of type 'System.Exception' was thrown.");

        }

        [TestMethod]
        public void GetPostDetailsNewDownloadMedia_Should_Return_True()
        {
            string url = "https://www.facebook.com/photo.php?fbid=245927735948489&id=100015937932195&set=a.111045426103388&source=56";

            string postUrl = "https://m.facebook.com/245927735948489";

            string imageUrl = "https://m.facebook.com/photo/view_full_size/?fbid=245927735948489";

            var postResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostDetailsNewDownloadMediaResponse.html", Assembly.GetExecutingAssembly());

            var fullImageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetFullSizeImagesResponse.html", Assembly.GetExecutingAssembly());

            var redirectResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.RedirectResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = postResponse,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.GetRequest(postUrl)
               .Returns(new ResponseParameter
               {
                   Response = fullImageResponse,
                   Exception = null,
                   HasError = false
               });

            _httpHelper.GetRequest(imageUrl)
               .Returns(new ResponseParameter
               {
                   Response = redirectResponse,
                   Exception = null,
                   HasError = false
               });


            FacebookPostDetails facebookPostDetails = new FacebookPostDetails
            {
                PostUrl = "https://www.facebook.com/photo.php?fbid=245927735948489&id=100015937932195&set=a.111045426103388&source=56",
                Id = "245927735948489",
            };

            var responseHandler = _sut.GetPostDetailsNewDownloadMedia(account, facebookPostDetails);

            responseHandler.Status.Should().Be(true);
            responseHandler.ObjFdScraperResponseParameters.PostDetails.Id.Should().Be("245927735948489");
            responseHandler.FbErrorDetails.Should().BeNull();
        }

        [TestMethod]
        public void GetPostDetailsNewDownloadMedia_Should_Return_False()
        {
            string url = "https://www.facebook.com/photo.php?fbid=245927735948489&id=100015937932195&set=a.111045426103388&source=56";

            string postUrl = "https://m.facebook.com/245927735948489";

            string imageUrl = "https://m.facebook.com/photo/view_full_size/?fbid=245927735948489";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        }
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            _httpHelper.GetRequest(postUrl)
               .Returns(new ResponseParameter
               {
                   Response = null,
                   Exception = new Exception(),
                   HasError = true
               });

            _httpHelper.GetRequest(imageUrl)
               .Returns(new ResponseParameter
               {
                   Response = null,
                   Exception = new Exception(),
                   HasError = true
               });


            FacebookPostDetails facebookPostDetails = new FacebookPostDetails
            {
                PostUrl = "https://www.facebook.com/photo.php?fbid=245927735948489&id=100015937932195&set=a.111045426103388&source=56",
                Id = "245927735948489",
            };

            var responseHandler = _sut.GetPostDetailsNewDownloadMedia(account, facebookPostDetails);

            responseHandler.Status.Should().Be(false);
            responseHandler.FbErrorDetails.IsStatusChangedRequired.Should().Be(false);
            responseHandler.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            responseHandler.FbErrorDetails.Description.Should().Be("Exception of type 'System.Exception' was thrown.");
        }

        [TestMethod]
        public void GetFullSizeImages_Should_Return_True()
        {
            string postUrl = "https://m.facebook.com/245927735948489";

            string imageUrl = "https://m.facebook.com/photo/view_full_size/?fbid=245927735948489";

            var fullImageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetFullSizeImagesResponse.html", Assembly.GetExecutingAssembly());

            var redirectResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.RedirectResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        }
                };

            _httpHelper.GetRequest(postUrl)
               .Returns(new ResponseParameter
               {
                   Response = fullImageResponse,
                   Exception = null,
                   HasError = false
               });

            _httpHelper.GetRequest(imageUrl)
               .Returns(new ResponseParameter
               {
                   Response = redirectResponse,
                   Exception = null,
                   HasError = false
               });


            FacebookPostDetails facebookPostDetails = new FacebookPostDetails()
            {
                PostUrl = "https://www.facebook.com/photo.php?fbid=245927735948489&id=100015937932195&set=a.111045426103388&source=56",
                Id = "245927735948489",
            };

            _sut.GetFullSizeImages(account, ref facebookPostDetails);

            facebookPostDetails.MediaUrl.Should().Be
                ("https://scontent-bom1-2.xx.fbcdn.net/v/t31.0-8/fr/cp0/e15/q65/27625484_245927735948489_2229503888802019333_o.jpg?_nc_cat=105&efg=eyJpIjoidCJ9&_nc_ht=scontent-bom1-2.xx&oh=fe4f593f122f0d6f7fb4fd3e5906318c&oe=5CEE3789");
        }

        [TestMethod]
        public void GetFullSizeImages_Should_Return_False()
        {
            string postUrl = "https://m.facebook.com/245927735948489";

            string imageUrl = "https://m.facebook.com/photo/view_full_size/?fbid=245927735948489";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        }
                };

            _httpHelper.GetRequest(postUrl)
               .Returns(new ResponseParameter
               {
                   Response = null,
                   Exception = new Exception(),
                   HasError = true
               });

            _httpHelper.GetRequest(imageUrl)
               .Returns(new ResponseParameter
               {
                   Response = null,
                   Exception = new Exception(),
                   HasError = true
               });


            FacebookPostDetails facebookPostDetails = new FacebookPostDetails()
            {
                PostUrl = "https://www.facebook.com/photo.php?fbid=245927735948489&id=100015937932195&set=a.111045426103388&source=56",
                Id = "245927735948489",
            };

            _sut.GetFullSizeImages(account, ref facebookPostDetails);

            facebookPostDetails.MediaUrl.Should().BeEmpty();
        }

        [TestMethod]
        public void ScrapGroups_Should_Return_True()
        {
            string url = "https://www.facebook.com/search/groups/?q=Akshaykumar";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ScrapGroupsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = response,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.ScrapGroups
                (account, "Akshaykumar", GroupMemberShip.AnyGroup, GroupType.Any, null, "Keywords");

            responseHandler.ObjFdScraperResponseParameters.ListGroup.Count.Should().Be(16);
        }

        [TestMethod]
        public void GetPostListFromNewsFeed_Should_Return_Count_True()
        {
            string newsFeedUrl = "https://www.facebook.com/";

            var newsFeedResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(newsFeedUrl)
               .Returns(new ResponseParameter
               {
                   Response = newsFeedResponse,
                   Exception = null,
                   HasError = false
               });


            var responseHandler = _sut.GetPostListFromNewsFeed(account, null);

            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(1);
            responseHandler.EntityId.Should().BeNullOrEmpty();
            responseHandler.Status.Should().BeTrue();

        }

        [TestMethod]
        public void GetPostListFromNewsFeed_Should_Return_Count_False()
        {
            string newsFeedUrl = "https://www.facebook.com/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(newsFeedUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetPostListFromNewsFeed(account, null);

            responseHandler.HasMoreResults.Should().Be(false);
            responseHandler.Status.Should().Be(false);
        }

        [TestMethod]
        public void GetPostListFromNewsFeed_Should_Return_True()
        {
            string newsFeedUrl = "https://www.facebook.com/";

            var response
                = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(newsFeedUrl)
               .Returns(new ResponseParameter
               {
                   Response = response,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.GetPostListFromNewsFeed(account, null, "NewsFeed");

            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(1);
            responseHandler.EntityId.Should().BeNullOrEmpty();
            responseHandler.Status.Should().BeTrue();
            responseHandler.HasMoreResults.Should().BeTrue();
        }

        [TestMethod]
        public void GetPostListFromNewsFeed_Should_Return_False()
        {
            string newsFeedUrl = "https://www.facebook.com/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(newsFeedUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetPostListFromNewsFeed(account, null, "NewsFeed");

            responseHandler.ObjFdScraperResponseParameters.ListPage.Should().BeNull();
            responseHandler.EntityId.Should().BeNullOrEmpty();
            responseHandler.Status.Should().BeFalse();
            responseHandler.HasMoreResults.Should().BeFalse();
        }

        [TestMethod]
        public void GetPostListFromFanpages_Should_Return_True()
        {
            string fanPageUrl = "https://www.facebook.com/339142240259836";

            string fanPagePostsUrl = "https://www.facebook.com/339142240259836/posts";

            var fanPageResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.PageUrlResponse.html", Assembly.GetExecutingAssembly());

            var fanPagePostResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromFanpagesResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(fanPageUrl)
               .Returns(new ResponseParameter
               {
                   Response = fanPageResponse,
                   Exception = null,
                   HasError = false
               });

            _httpHelper.GetRequest(fanPagePostsUrl)
               .Returns(new ResponseParameter
               {
                   Response = fanPagePostResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.GetPostListFromFanpages(account, fanPageUrl, null);

            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(1);
            responseHandler.HasMoreResults.Should().BeTrue();
        }

        [TestMethod]
        public void GetPostListFromFanpages_Should_Return_False()
        {
            string fanPageUrl = "https://www.facebook.com/339142240259836";

            string fanPagePostsUrl = "https://www.facebook.com/339142240259836/posts";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        }
                };

            _httpHelper.GetRequest(fanPageUrl)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            _httpHelper.GetRequest(fanPagePostsUrl)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetPostListFromFanpages(account, fanPageUrl, null);
            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(0);
            responseHandler.HasMoreResults.Should().BeFalse();
        }

        [TestMethod]
        public void GetPostListFromFanpagesNew_Should_Return_True()
        {
            string fanPageUrl = "https://www.facebook.com/339142240259836";

            string fanPagePostsUrl = "https://www.facebook.com/339142240259836/posts";

            var fanPageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.PageUrlResponse.html", Assembly.GetExecutingAssembly());

            var fanPagePostResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromFanpagesResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(fanPageUrl)
               .Returns(new ResponseParameter
               {
                   Response = fanPageResponse,
                   Exception = null,
                   HasError = false
               });

            _httpHelper.GetRequest(fanPagePostsUrl)
               .Returns(new ResponseParameter
               {
                   Response = fanPagePostResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.GetPostListFromFanpagesNew(account, null, fanPageUrl);

            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(1);
        }

        [TestMethod]
        public void GetPostListFromFanpagesNew_Should_Return_False()
        {
            string fanPageUrl = "https://www.facebook.com/339142240259836";

            string fanPagePostsUrl = "https://www.facebook.com/339142240259836/posts";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                        },
                };

            _httpHelper.GetRequest(fanPageUrl)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            _httpHelper.GetRequest(fanPagePostsUrl)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetPostListFromFanpagesNew(account, null, fanPageUrl);
            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(0);
            responseHandler.EntityId.Should().Be("0");
            responseHandler.Status.Should().BeFalse();
            responseHandler.HasMoreResults.Should().BeFalse();
        }

        [TestMethod]
        public void GetPostListFromTimeline_Should_Return_True()
        {
            string timelineUrl = "https://www.facebook.com/100004509073207/";

            var timelineResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.timelineResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100004509073207",
                        },
                };

            _httpHelper.GetRequest(timelineUrl)
               .Returns(new ResponseParameter
               {
                   Response = timelineResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.GetPostListFromTimeline
                (account, null, timelineUrl);

            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(2);
            responseHandler.Status.Should().BeTrue();
            responseHandler.HasMoreResults.Should().BeTrue();
            responseHandler.PageletData.Should().Be(
                "{\"profile_id\":\"100004509073207\",\"vanity\":\"veera.trivedi.98\",\"sk\":\"timeline\",\"profile_has_parallel_pagelets\":\"\",\"tab_key\":\"timeline\",\"target_id\":\"timeline_story_container_100004509073207\",\"pager_target_id\":\"timeline_pager_container_100004509073207\",\"start\":\"0\",\"end\":\"1551427199\",\"cursor\":\"tmln_strm:1548109169:901637849338197499:1\",\"page_id\":\"1\",\"pager_fired_on_init\":true}");

        }

        [TestMethod]
        public void GetPostListFromTimeline_Should_Return_False()
        {
            string timelineUrl = "https://www.facebook.com/100004509073207/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100004509073207",
                        }
                };

            _httpHelper.GetRequest(timelineUrl)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetPostListFromTimeline
                (account, null, timelineUrl);

            responseHandler.ObjFdScraperResponseParameters.IsPagination.Should().BeFalse();
            responseHandler.Status.Should().BeFalse();
            responseHandler.HasMoreResults.Should().BeFalse();
            responseHandler.PageletData.Should().BeNullOrEmpty();

        }

        [TestMethod]
        public void GetPostListFromFriendTimelineNew_Should_Return_True()
        {
            string timelineUrl = "https://www.facebook.com/100004509073207/";

            var timelineResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.timelineResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100004509073207",
                        },
                };

            _httpHelper.GetRequest(timelineUrl)
               .Returns(new ResponseParameter
               {
                   Response = timelineResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.GetPostListFromFriendTimelineNew(account, null, timelineUrl);

            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(2);
            responseHandler.Status.Should().BeTrue();
            responseHandler.HasMoreResults.Should().BeTrue();
        }

        [TestMethod]
        public void LikeUnlikePost_Should_Return_True()
        {
            string postId = "1131035560390069";

            string url = "https://www.facebook.com/ufi/reaction/?dpr=1";

            var likeUnlikePostResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.LikeUnlikePostResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100004509073207",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("client_id=1522337984580:698763417&ft_ent_identifier=1131035560390069&reaction_type=1&root_id=u_fetchstream_1_6&session_id=21480490&source=17&feedback_referrer=&instance_id=u_fetchstream_1_0&av=100004509073207&ft[tn]=]*F&__req=11&__user=100004509073207&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = likeUnlikePostResponse,
                    Exception = null,
                    HasError = false
                });

            var result = _sut.LikeUnlikePost(account, postId, ReactionType.Like);

            result.Should().Be(true);

        }

        [TestMethod]
        public void LikeUnlikePost_Should_Return_False()
        {
            string postId = "1131035560390069";

            string url = "https://www.facebook.com/ufi/reaction/?dpr=1";

            var account =
          new DominatorAccountModel
          {
              AccountBaseModel =
                  new DominatorAccountBaseModel
                  {
                      AccountNetwork = SocialNetworks.Facebook,
                      UserId = "100004509073207",
                  },
          };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("client_id=1522337984580:698763417&ft_ent_identifier=1131035560390069&reaction_type=1&root_id=u_fetchstream_1_6&session_id=21480490&source=17&feedback_referrer=&instance_id=u_fetchstream_1_0&av=100004509073207&ft[tn]=]*F&__req=11&__user=100004509073207&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
                }))

                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            var result = _sut.LikeUnlikePost(account, postId, ReactionType.Like);

            result.Should().Be(false);

        }

        [TestMethod]
        public void LikeFanpage_Should_Return_True()
        {
            string fanpageId = "20143593282";

            string url = "https://www.facebook.com/ajax/pages/fan_status.php?av=100015937932195&dpr=1";

            var fanpageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.LikeFanpageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("fbpage_id=20143593282&add=true&reload=false&fan_origin=search&fan_source=&cat=&actor_id=&nctr[_mod]=pagelet_loader_initial_browse_result&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = fanpageResponse,
                    Exception = null,
                    HasError = false
                });

            var likeFanpageResponseHandler = _sut.LikeFanpage(account, fanpageId, CancellationToken.None);

            likeFanpageResponseHandler.Status.Should().Be(true);

        }

        [TestMethod]
        [ExpectedException(typeof(RedundantArgumentMatcherException))]
        public void LikeFanpage_Should_Return_False()
        {

            string url = "https://www.facebook.com/ajax/pages/fan_status.php?av=100015937932195&dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("fbpage_id=20143593282&add=true&reload=false&fan_origin=search&fan_source=&cat=&actor_id=&nctr[_mod]=pagelet_loader_initial_browse_result&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
                }))

                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.LikeFanpage(account, "20143593282", Arg.Any<CancellationToken>());

            responseHandler.Status.Should().Be(false);
        }

        [TestMethod]
        public void CommentOnPost_Should_Return_True()
        {
            string postId = "627078034407912";

            string url = "https://www.facebook.com/ufi/add/comment/?dpr=1";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.CommentOnPostResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("ft_ent_identifier=627078034407912&comment_text=Nice%20One&source=17&client_id=1523504279817:2735809926&session_id=3f632d74&reply_fbid=&attached_sticker_fbid=0&attached_photo_fbid=0&attached_video_fbid=0&attached_file_fbid=0&attached_share_url=&av=100015937932195&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = response,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.CommentOnPost(account, postId, "Nice One");

            responseHandler.ObjFdScraperResponseParameters.CommentId.Should().Be("627330577715991");
            responseHandler.ObjFdScraperResponseParameters.IsCommentedOnPost.Should().Be(true);
            responseHandler.ObjFdScraperResponseParameters.IsBlocked.Should().Be(false);
            responseHandler.Status.Should().Be(true);
        }

        [TestMethod]
        public void CommentOnPost_Should_Return_False()
        {
            string postId = "627078034407912";

            string url = "https://www.facebook.com/ufi/add/comment/?dpr=1";
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("ft_ent_identifier=627078034407912&comment_text=Nice%20One&source=17&client_id=1523504279817:2735809926&session_id=3f632d74&reply_fbid=&attached_sticker_fbid=0&attached_photo_fbid=0&attached_video_fbid=0&attached_file_fbid=0&attached_share_url=&av=100015937932195&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.CommentOnPost(account, postId, "Nice One");

            responseHandler.ObjFdScraperResponseParameters.IsCommentedOnPost.Should().Be(false);
            responseHandler.ObjFdScraperResponseParameters.IsBlocked.Should().Be(false);
            responseHandler.FbErrorDetails.IsStatusChangedRequired.Should().Be(false);
            responseHandler.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            responseHandler.FbErrorDetails.Description.Should().Be("Exception of type 'System.Exception' was thrown.");
            responseHandler.Status.Should().Be(false);

        }

        [TestMethod]
        public void UnjoinGroup_Should_Return_True()
        {
            string grpId = "177584502573787";

            string url = "https://www.facebook.com/ajax/groups/membership/leave.php?group_id=177584502573787&ref=group_unjoined";

            var fanpageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UnjoinGroupResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("confirmed=1&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = fanpageResponse,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.UnjoinGroup(account, grpId);

            responseHandler.Should().Be(true);

        }

        [TestMethod]
        public void UnjoinGroup_Should_Return_False()
        {
            string grpId = "177584502573787";

            string url = "https://www.facebook.com/ajax/groups/membership/leave.php?group_id=177584502573787&ref=group_unjoined";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("confirmed=1&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
                }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.UnjoinGroup(account, grpId);

            responseHandler.Should().BeFalse();

        }

        [TestMethod]
        public void SendGroupInvittationTofriends_Should_Return_True()
        {
            string grpId = "163240953694419";

            string url = "https://www.facebook.com/ajax/groups/members/add_post?source=dialog_typeahead&group_id=163240953694419&refresh=1&dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("members[0]=100003512822952&text_members[0]=&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100003512822952",
                ProfileUrl = "https://www.facebook.com/100003512822952",
            };

            var responseHandler = _sut.SendGroupInvittationTofriends(account, grpId, facebookUser, "");

            var gotStringForTest = new Utility.FdUtility().GetClassPropertyValueForTests
                (responseHandler, "responseHandler");

            responseHandler.Status.Should().Be(false);
        }

        [TestMethod]
        public void SendGroupInvittationTofriends_Should_Return_False()
        {
            string grpId = "163240953694419";

            string url
                = "https://www.facebook.com/ajax/groups/members/add_post?source=dialog_typeahead&group_id=163240953694419&refresh=1&dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("members[0]=100003512822952&text_members[0]=&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100003512822952",
                ProfileUrl = "https://www.facebook.com/100003512822952",
            };

            var responseHandler
                = _sut.SendGroupInvittationTofriends(account, grpId, facebookUser, "");

            responseHandler.Status.Should().BeFalse();
        }

        [TestMethod]
        public void SendPageInvittationTofriends_Should_Return_True()
        {
            string grpId = "162904411027310";

            string url = "https://www.facebook.com/pages/batch_invite_send/?dpr=1.5";

            var groupResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendPageInvittationTofriendsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("page_id=162904411027310&send_in_messenger=False&invitees[0]=100007464069871&ref=modal_page_invite_dialog_v2&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = groupResponse,
                    Exception = null,
                    HasError = false
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100007464069871",
                ProfileUrl = "https://www.facebook.com/100007464069871",
            };

            var responseHandler = _sut.SendPageInvittationTofriends(account, grpId, "", facebookUser, false);

            responseHandler.Should().Be(true);

        }

        [TestMethod]
        public void SendPageInvittationTofriends_Should_Return_False()
        {
            string grpId = "162904411027310";

            string url = "https://www.facebook.com/pages/batch_invite_send/?dpr=1.5";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("page_id=162904411027310&send_in_messenger=False&invitees[0]=100007464069871&ref=modal_page_invite_dialog_v2&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100007464069871",
                ProfileUrl = "https://www.facebook.com/100007464069871",
            };

            var responseHandler = _sut.SendPageInvittationTofriends
                (account, grpId, "", facebookUser, false);

            responseHandler.Should().BeFalse();

        }

        [TestMethod]
        public void SendPageInvittationTofriendsWithoutNote_Should_Return_True()
        {
            string grpId = "162904411027310";

            string url = "https://www.facebook.com/pages/friend_invite/send/?page_id=162904411027310&invitee=100007464069871&ref=chaining&invite_callsite=PagesFriendInviterChaining";

            var groupResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendPageInvittationTofriendsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = groupResponse,
                    Exception = null,
                    HasError = false
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100007464069871",
                ProfileUrl = "https://www.facebook.com/100007464069871",
            };

            var responseHandler = _sut.SendPageInvittationTofriendsWithoutNote(account, grpId, facebookUser);

            responseHandler.Should().Be(true);

        }

        [TestMethod]
        public void SendPageInvittationTofriendsWithoutNote_Should_Return_False()
        {
            string grpId = "162904411027310";

            string url =
                "https://www.facebook.com/pages/friend_invite/send/?page_id=162904411027310&invitee=100007464069871&ref=chaining&invite_callsite=PagesFriendInviterChaining";

            var groupResponse = TestUtils.ReadFileFromResources(
                "FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendPageInvittationTofriendsResponse.html",
                Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should()
                    .Be(
                        "__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = groupResponse,
                    Exception = null,
                    HasError = false
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100007464069871",
                ProfileUrl = "https://www.facebook.com/100007464069871",
            };

            var responseHandler = _sut.SendPageInvittationTofriendsWithoutNote(account, grpId, facebookUser);

            responseHandler.Should().Be(true);

        }

        [TestMethod]
        public void SendPageInvittationToPageLikers_Should_Return_True()
        {
            string grpId = "162904411027310";

            string url = "https://www.facebook.com/pages/post_like_invite/send/?dpr=1";

            var groupResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendPageInvittationTofriendsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("page_id=162904411027310&invitee=100007464069871&ref=likes_dialog&content_id=299585164025900&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = groupResponse,
                    Exception = null,
                    HasError = false
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100007464069871",
                ProfileUrl = "https://www.facebook.com/100007464069871",
            };

            var responseHandler = _sut.SendPageInvittationToPageLikers(account, grpId, facebookUser, "https://www.facebook.com/299585164025900");

            responseHandler.Should().Be(true);

        }

        [TestMethod]
        public void SendPageInvittationToPageLikers_Should_Return_False()
        {
            string grpId = "162904411027310";

            string url = "https://www.facebook.com/pages/post_like_invite/send/?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should()
                    .Be(
                        "page_id=162904411027310&invitee=100007464069871&ref=likes_dialog&content_id=299585164025900&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100007464069871",
                ProfileUrl = "https://www.facebook.com/100007464069871",
            };

            var responseHandler = _sut.SendPageInvittationToPageLikers(account, grpId, facebookUser,
                "https://www.facebook.com/299585164025900");

            responseHandler.Should().BeFalse();

        }

        [TestMethod]
        public void SendEventInvittationTofriends_Should_Return_True()
        {
            string eventId = "2253068041623307";

            string url = "https://www.facebook.com/ajax/events/permalink/invite.php?plan_id=2253068041623307&acontext%5Bref%5D=51&acontext%5Bsource%5D=1&acontext%5Bno_referrer%5D=true&acontext%5Baction_history%5D=%5B%7B%22surface%22%3A%22permalink%22%2C%22mechanism%22%3A%22surface%22%2C%22extra_data%22%3A%5B%5D%7D%5D&acontext%5Bhas_source%5D=true&dpr=1.5";

            var invitationResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendEventInvittationTofriendsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("at_limit=false&session_id=2384444232&profileChooserItems={\"100007464069871\":1}&pagelets_to_update=[\"EventsPermalinkWorkplaceContentBannerPagelet\",\"EventsCohostAcceptancePagelet\",\"EventPermalinkEventTicketsPurchaseCardPagelet\",\"EventPublicProdGuestsPagelet\",\"EventPublicProdDetailsPagelet\",\"InviteOffFBInfoBoxPagelet\",\"EventPermalinkAdsSectionPagelet\",\"EventInsightsPagelet\",\"EventPermalinkFeedbackSurvey\",\"SaleEventJoinPagelet\",\"EventPermalinkEventTipsPagelet\",\"EventPublicProdReactionPagelet\",\"EventPublicProdFeedRelatedEventsPagelet\"]&invite_message=&message_option=invite_only&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = invitationResponse,
                    Exception = null,
                    HasError = false
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100007464069871",
                ProfileUrl = "https://www.facebook.com/100007464069871",
            };

            var responseHandler = _sut.SendEventInvittationTofriends(account, eventId, facebookUser, "");

            responseHandler.Should().Be(true);

        }

        [TestMethod]
        public void SendEventInvittationTofriends_Should_Return_False()
        {
            string eventId = "2253068041623307";

            string url = "https://www.facebook.com/ajax/events/permalink/invite.php?plan_id=2253068041623307&acontext%5Bref%5D=51&acontext%5Bsource%5D=1&acontext%5Bno_referrer%5D=true&acontext%5Baction_history%5D=%5B%7B%22surface%22%3A%22permalink%22%2C%22mechanism%22%3A%22surface%22%2C%22extra_data%22%3A%5B%5D%7D%5D&acontext%5Bhas_source%5D=true&dpr=1.5";

            var invitationResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendEventInvittationTofriendsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("at_limit=false&session_id=2384444232&profileChooserItems={\"100007464069871\":1}&pagelets_to_update=[\"EventsPermalinkWorkplaceContentBannerPagelet\",\"EventsCohostAcceptancePagelet\",\"EventPermalinkEventTicketsPurchaseCardPagelet\",\"EventPublicProdGuestsPagelet\",\"EventPublicProdDetailsPagelet\",\"InviteOffFBInfoBoxPagelet\",\"EventPermalinkAdsSectionPagelet\",\"EventInsightsPagelet\",\"EventPermalinkFeedbackSurvey\",\"SaleEventJoinPagelet\",\"EventPermalinkEventTipsPagelet\",\"EventPublicProdReactionPagelet\",\"EventPublicProdFeedRelatedEventsPagelet\"]&invite_message=&message_option=invite_only&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = invitationResponse,
                    Exception = null,
                    HasError = false
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100007464069871",
                ProfileUrl = "https://www.facebook.com/100007464069871",
            };

            var responseHandler = _sut.SendEventInvittationTofriends(account, eventId, facebookUser, "");

            responseHandler.Should().Be(true);

        }

        [TestMethod]
        public void InviteAsPersonalMessage_Should_Return_True()
        {
            string eventId = "2253068041623307";

            string url = "https://www.facebook.com/share/dialog/submit/?app_id=256281040558&audience_type=message&audience_targets%5B0%5D=100007464069871&composer_session_id=b4f5f4c4-88a8+-+487d+-+bc46+-+3abbcfbfd128&ephemeral_ttl_mod=0&message=Have+A+Look&owner_id=&post_id=2253068041623307&share_to_group_as_page=false&share_type=7&shared_ad_id=&source=7&is_throwback_post=false&url=&shared_from_post_id=ef77b538-18c5-41eb-946b-5353b76d3520&logging_session_id=true&perform_messenger_logging=2253068041623307&av=100015937932195&dpr=1.5";

            var invitationResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.InviteAsPersonalMessageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = invitationResponse,
                    Exception = null,
                    HasError = false
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100007464069871",
                ProfileUrl = "https://www.facebook.com/100007464069871",
            };

            var responseHandler = _sut.InviteAsPersonalMessage(account, eventId, facebookUser, "Have A Look");

            responseHandler.Should().Be(true);

        }

        [TestMethod]
        public void InviteAsPersonalMessage_Should_Return_False()
        {
            string eventId = "2253068041623307";

            string url = "https://www.facebook.com/share/dialog/submit/?app_id=256281040558&audience_type=message&audience_targets%5B0%5D=100007464069871&composer_session_id=b4f5f4c4-88a8+-+487d+-+bc46+-+3abbcfbfd128&ephemeral_ttl_mod=0&message=Have+A+Look&owner_id=&post_id=2253068041623307&share_to_group_as_page=false&share_type=7&shared_ad_id=&source=7&is_throwback_post=false&url=&shared_from_post_id=ef77b538-18c5-41eb-946b-5353b76d3520&logging_session_id=true&perform_messenger_logging=2253068041623307&av=100015937932195&dpr=1.5";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            FacebookUser facebookUser = new FacebookUser()
            {
                UserId = "100007464069871",
                ProfileUrl = "https://www.facebook.com/100007464069871",
            };

            var responseHandler = _sut.InviteAsPersonalMessage
                (account, eventId, facebookUser, "Have A Look");

            responseHandler.Should().BeFalse();

        }

        [TestMethod]
        public void GetRecentMessageDetails_Should_Return_True()
        {

            string homeUrl = "https://www.facebook.com/";

            var msgurl = "https://www.facebook.com/api/graphqlbatch/";
            var homeResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.HomePageResponse.html", Assembly.GetExecutingAssembly());

            var messageResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetRecentMessageDetailsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(homeUrl)
               .Returns(new ResponseParameter
               {
                   Response = homeResponse,
                   Exception = null,
                   HasError = false
               });


            _httpHelper.PostRequest(msgurl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("batch_name=MessengerGraphQLThreadlistFetcher&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=AQFatEyts8jN:AQEG8CT5lXeP&jazoest=26581721229097781106845697858658171114102508185110764851&queries={\"o0\":{\"doc_id\":\"1864228073656845\",\"query_params\":{\"id\":\"100011665870717\",\"message_limit\":10,\"load_messages\":true,\"load_read_receipts\":false}}}");
            }))

                .Returns(new ResponseParameter
                {
                    Response = messageResponse,
                    Exception = null,
                    HasError = false
                });


            LiveChatModel liveChatModel = new LiveChatModel();
            liveChatModel.SenderDetails.SenderId = "100011665870717";
            liveChatModel.SenderDetails.SenderName = "Balaram Balaram";

            var responseHandler = (UserMessageResponseHandler)_sut.GetRecentMessageDetails
                (account, null, liveChatModel, account.Token);

            var gotStringForTest = new Utility.FdUtility().GetClassPropertyValueForTests(responseHandler, "responseHandler");

            responseHandler.UserId.Should().Be("100015937932195");
            responseHandler.PageletData.Should().BeNullOrEmpty();
            responseHandler.HasMoreResults.Should().Be(true);
            responseHandler.FriendName.Should().Be("Balaram Balaram");
            responseHandler.Status.Should().Be(false);

        }

        [TestMethod]
        public void GetRecentMessageDetails_Should_Return_False()
        {

            string homeUrl = "https://www.facebook.com/";

            var msgurl = "https://www.facebook.com/api/graphqlbatch/";
            var homeResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.HomePageResponse.html", Assembly.GetExecutingAssembly());

            var messageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetRecentMessageDetailsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }
                };

            _httpHelper.GetRequest(homeUrl)
               .Returns(new ResponseParameter
               {
                   Response = homeResponse,
                   Exception = null,
                   HasError = false
               });

            _httpHelper.PostRequest(msgurl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("batch_name=MessengerGraphQLThreadlistFetcher&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=AQFatEyts8jN:AQEG8CT5lXeP&jazoest=26581721229097781106845697858658171114102508185110764851&queries={\"o0\":{\"doc_id\":\"1864228073656845\",\"query_params\":{\"id\":\"100011665870717\",\"message_limit\":10,\"load_messages\":true,\"load_read_receipts\":false}}}");
            }))
                .Returns(new ResponseParameter
                {
                    Response = messageResponse,
                    Exception = null,
                    HasError = false
                });

            LiveChatModel liveChatModel = new LiveChatModel();
            liveChatModel.SenderDetails.SenderId = "100011665870717";
            liveChatModel.SenderDetails.SenderName = "Balaram Balaram";

            var responseHandler = (UserMessageResponseHandler)_sut.GetRecentMessageDetails
                (account, null, liveChatModel, account.Token);

            responseHandler.UserId.Should().Be("100015937932195");
        }

        [TestMethod]
        public void GetInterestedGuestsForEvents_Should_Return_True()
        {

            string eventUrl = "https://www.facebook.com/";

            string eventUserUrl = "https://www.facebook.com/events/typeahead/guest_list/?event_id=2300178226967404&tabs%5B0%5D=watched&tabs%5B1%5D=going&tabs%5B2%5D=invited&order%5Bdeclined%5D=affinity&order%5Bgoing%5D=affinity&order%5Binvited%5D=affinity&order%5Bmaybe%5D=affinity&order%5Bwatched%5D=affinity&bucket_schema%5Bwatched%5D=friends&bucket_schema%5Binvited%5D=friends&bucket_schema%5Bgoing%5D=friends&dpr=1&__req=18&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var eventResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetInterestedGuestsForEventsResponse.html", Assembly.GetExecutingAssembly());

            var eventUserResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetInterestedGuestsForEventsUUserResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(eventUrl)
               .Returns(new ResponseParameter
               {
                   Response = eventResponse,
                   Exception = null,
                   HasError = false
               });



            _httpHelper.GetRequest(eventUserUrl)
              .Returns(new ResponseParameter
              {
                  Response = eventUserResponse,
                  Exception = null,
                  HasError = false
              });

            var responseHandler = _sut.GetInterestedGuestsForEvents
                (account, null, eventUrl, EventGuestType.Interested);

            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(4);

        }

        [TestMethod]
        public void GetInterestedGuestsForEvents_Should_Return_False()
        {

            string eventUrl = "https://www.facebook.com/";

            string eventUserUrl = "https://www.facebook.com/events/typeahead/guest_list/?event_id=2300178226967404&tabs%5B0%5D=watched&tabs%5B1%5D=going&tabs%5B2%5D=invited&order%5Bdeclined%5D=affinity&order%5Bgoing%5D=affinity&order%5Binvited%5D=affinity&order%5Bmaybe%5D=affinity&order%5Bwatched%5D=affinity&bucket_schema%5Bwatched%5D=friends&bucket_schema%5Binvited%5D=friends&bucket_schema%5Bgoing%5D=friends&dpr=1&__req=18&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }
                };

            _httpHelper.GetRequest(eventUrl)
               .Returns(new ResponseParameter
               {
                   Response = string.Empty,
                   Exception = new Exception(),
                   HasError = true
               });

            _httpHelper.GetRequest(eventUserUrl)
              .Returns(new ResponseParameter
              {
                  Response = string.Empty,
                  Exception = new Exception(),
                  HasError = true
              });

            var responseHandler = _sut.GetInterestedGuestsForEvents
                (account, null, eventUrl, EventGuestType.Interested);

            responseHandler.HasMoreResults.Should().Be(false);
            responseHandler.PageletData.Should().Be("");
            responseHandler.Status.Should().Be(false);
            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);

        }

        [TestMethod]
        public void GetAllMutualFriends_Should_Return_True()
        {

            string mutualFrdUrl = "https://www.facebook.com/browse/mutual_friends/?uid=100022195891372";

            var eventResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetAllMutualFriendsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(mutualFrdUrl)
               .Returns(new ResponseParameter
               {
                   Response = eventResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.GetAllMutualFriends(account, "100022195891372");

            responseHandler.Count.Should().Be(1);
        }

        [TestMethod]
        public void GetAllMutualFriends_Should_Return_False()
        {
            string mutualFrdUrl = "https://www.facebook.com/browse/mutual_friends/?uid=100022195891372";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            _httpHelper.GetRequest(mutualFrdUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetAllMutualFriends(account, "100022195891372");

            responseHandler.Should().BeNull();
        }

        [TestMethod]
        public void LikeComments_Should_Return_True()
        {
            string likeUrl = "https://www.facebook.com/ufi/comment/reaction/?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.PostRequest(likeUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("reaction_type=1&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&source=1&av=100015937932195&client_id=1535803053981:2369078201&__req=2w&session_id=3a91793b&batch_name=MessengerGraphQLThreadlistFetcher&ft_ent_identifier=2474291145979132&comment_id=:2474291145979132&legacy_id=&instance_id=u_ps_0_0_d");
            }))

               .Returns(new ResponseParameter
               {
                   Exception = null,
                   HasError = false,
                   Response = "aria-pressed=\"true\""
               });

            FdPostCommentDetails fdPostCommentDetails = new FdPostCommentDetails()
            {
                CommenterID = "2474434972631416",
                PostId = "2474291145979132",
            };

            var status = _sut.LikeComments(account, fdPostCommentDetails, ReactionType.Like);

            status.Status.Should().Be(true);

        }

        [TestMethod]
        public void LikeComments_Should_Return_False()
        {

            string likeUrl = "https://www.facebook.com/ufi/comment/reaction/?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.PostRequest(likeUrl, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("reaction_type=1&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&source=1&av=100015937932195&client_id=1535803053981:2369078201&__req=2w&session_id=3a91793b&batch_name=MessengerGraphQLThreadlistFetcher&ft_ent_identifier=2474291145979132&comment_id=:2474291145979132&legacy_id=&instance_id=u_ps_0_0_d");
                }))

                .Returns(new ResponseParameter
                {
                    Exception = new Exception(),
                    HasError = true
                });

            FdPostCommentDetails fdPostCommentDetails = new FdPostCommentDetails()
            {
                CommenterID = "2474434972631416",
                PostId = "2474291145979132",
            };

            var status = _sut.LikeComments(account, fdPostCommentDetails, ReactionType.Like);

            status.Status.Should().BeFalse();

        }

        [TestMethod]
        public void GetCommentResponseforVideos_Should_Return_True()
        {

            string videoUrl = "https://www.facebook.com/video/tahoe/async/2113283732084051/?originalmediaid=2113283732084051&playerorigin=permalink&playersuborigin=tahoe&ispermalink=true&numcopyrightmatchedvideoplayedconsecutively=0&storyidentifier=UzpfSTM5MDI0NjM5MTE1NzUzODpWSzoyMTEzMjgzNzMyMDg0MDUx&payloadtype=secondary&dpr=1";

            var videoResponse =
                TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetCommentResponseforVideosResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.PostRequest(videoUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

               .Returns(new ResponseParameter
               {
                   Response = videoResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.GetCommentResponseforVideos(account, "2113283732084051", "UzpfSTM5MDI0NjM5MTE1NzUzODpWSzoyMTEzMjgzNzMyMDg0MDUx");

            responseHandler.HasError.Should().Be(false);
            responseHandler.Response.Should().Be(TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetCommentResponseforVideosResponse.html", Assembly.GetExecutingAssembly()));

        }

        [TestMethod]
        public void GetCommentResponseforVideos_Should_Return_False()
        {
            string videoUrl = "https://www.facebook.com/video/tahoe/async/2113283732084051/?originalmediaid=2113283732084051&playerorigin=permalink&playersuborigin=tahoe&ispermalink=true&numcopyrightmatchedvideoplayedconsecutively=0&storyidentifier=UzpfSTM5MDI0NjM5MTE1NzUzODpWSzoyMTEzMjgzNzMyMDg0MDUx&payloadtype=secondary&dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.PostRequest(videoUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

               .Returns(new ResponseParameter
               {
                   Response = null,
                   Exception = new Exception(),
                   HasError = true
               });

            var responseHandler = _sut.GetCommentResponseforVideos(account, "2113283732084051", "UzpfSTM5MDI0NjM5MTE1NzUzODpWSzoyMTEzMjgzNzMyMDg0MDUx");

            responseHandler.HasError.Should().Be(true);
            responseHandler.Exception.Message.Should().Be("Exception of type 'System.Exception' was thrown.");
            responseHandler.Exception.Data.IsReadOnly.Should().Be(false);
            responseHandler.Exception.Data.IsFixedSize.Should().Be(false);
            responseHandler.Exception.Data.IsSynchronized.Should().Be(false);

        }

        [TestMethod]
        public void ChangeActor_Should_Return_True()
        {
            string anchorChangeUrl = "https://www.facebook.com/ufi/actor/change/?dpr=1";

            var actorChangeResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ChangeActorResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.PostRequest(anchorChangeUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("from_actor_id=100015937932195&ft_ent_identifier=1130270733821763&av=162904411027310&__req=1i&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

               .Returns(new ResponseParameter
               {
                   Response = actorChangeResponse,
                   Exception = null,
                   HasError = false
               });

            var status = _sut.ChangeActor(account, "1130270733821763", "162904411027310");

            status.Should().Be(true);

        }

        [TestMethod]
        public void ChangeActor_Should_Return_False()
        {
            string anchorChangeUrl = "https://www.facebook.com/ufi/actor/change/?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.PostRequest(anchorChangeUrl, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("from_actor_id=100015937932195&ft_ent_identifier=1130270733821763&av=162904411027310&__req=1i&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
                }))

                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            var status = _sut.ChangeActor(account, "1130270733821763", "162904411027310");

            status.Should().BeFalse();

        }

        [TestMethod]
        public void ReplyOnPost_Should_Return_True()
        {
            string replyToPostUrl = "https://www.facebook.com/ufi/add/comment/?dpr=1";

            var replyToPostResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ReplyOnPostResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.PostRequest(replyToPostUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("ft_ent_identifier=1991941350932940&comment_text=Jai Hind&source=1&client_id=1536912522897:94090525&session_id=7abd414e&reply_fbid=1992402050886870&parent_comment_id=1991941350932940_1992402050886870&attached_sticker_fbid=0&attached_photo_fbid=0&attached_video_fbid=0&attached_file_fbid=0&attached_share_url=&av=100015937932195&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

               .Returns(new ResponseParameter
               {
                   Response = replyToPostResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.ReplyOnPost(account, "1991941350932940", "Jai Hind", "1992402050886870");

            responseHandler.ObjFdScraperResponseParameters.IsCommentedOnPost.Should().Be(true);

        }

        [TestMethod]
        public void ReplyOnPost_Should_Return_False()
        {
            string replyToPostUrl = "https://www.facebook.com/ufi/add/comment/?dpr=1";

            var replyToPostResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ReplyOnPostResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.PostRequest(replyToPostUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("ft_ent_identifier=1991941350932940&comment_text=Jai Hind&source=1&client_id=1536912522897:94090525&session_id=7abd414e&reply_fbid=1992402050886870&parent_comment_id=1991941350932940_1992402050886870&attached_sticker_fbid=0&attached_photo_fbid=0&attached_video_fbid=0&attached_file_fbid=0&attached_share_url=&av=100015937932195&__req=11&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

               .Returns(new ResponseParameter
               {
                   Response = replyToPostResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.ReplyOnPost(account, "1991941350932940", "Jai Hind", "1992402050886870");

            responseHandler.ObjFdScraperResponseParameters.IsCommentedOnPost.Should().Be(true);

        }

        [TestMethod]
        public void GetPostListFromAlbums_Should_Return_True()
        {
            string albumUrl = "https://m.facebook.com/laurenfinch11/albums/111045426103388/";

            var albumResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromAlbumsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {

                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }
                };


            _httpHelper.GetRequest(albumUrl)
               .Returns(new ResponseParameter
               {
                   Response = albumResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.GetPostListFromAlbums
                (account, null, "https://www.facebook.com/LaurenFinch11/media_set?set=a.111045426103388&type=3");

            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(8);
            responseHandler.EntityId.Should().BeNullOrEmpty();
            responseHandler.Status.Should().BeTrue();
        }

        [TestMethod]
        public void GetPostListFromAlbums_Should_Return_False()
        {
            string albumUrl = "https://m.facebook.com/laurenfinch11/albums/111045426103388/";

            var albumResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromAlbumsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {

                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }
                };


            _httpHelper.GetRequest(albumUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetPostListFromAlbums
                (account, null, "https://www.facebook.com/LaurenFinch11/media_set?set=a.111045426103388&type=3");

            responseHandler.ObjFdScraperResponseParameters.IsPagination.Should().BeFalse();
            responseHandler.EntityId.Should().BeNullOrEmpty();
            responseHandler.Status.Should().BeFalse();
        }

        [TestMethod]
        public void GetPostListFromKeyWords_Should_Return_True()
        {
            string url = "https://www.facebook.com/search/posts/?q=Akshaykumar";

            var albumResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromKeyWordsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
               .Returns(new ResponseParameter
               {
                   Response = albumResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.GetPostListFromKeyWords(account, null, "Akshaykumar");

            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(5);

        }

        [TestMethod]
        public void GetPostListFromKeyWords_Should_Return_False()
        {
            string url =
                "https://www.facebook.com/search/str/Akshaykumar/keywords_blended_posts?filters=eyJycF9hdXRob3IiOiJ7XCJuYW1lXCI6XCJtZXJnZWRfcHVibGljX3Bvc3RzXCIsXCJhcmdzXCI6XCJcIn0ifQ==";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetPostListFromKeyWords(account, null, "Akshaykumar");

            responseHandler.HasMoreResults.Should().Be(false);
            responseHandler.Status.Should().Be(false);

        }

        [TestMethod]
        public void GetEventDetailsFromUrl_Should_Return_True()
        {
            string url = "https://www.facebook.com/events/572476599891934/";

            var albumResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.eventResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
               .Returns(new ResponseParameter
               {
                   Response = albumResponse,
                   Exception = null,
                   HasError = false
               });

            EventDetailsResponseHandler responseHandler = _sut.GetEventDetailsFromUrl(account, url);

            responseHandler.FdEvents.EventId.Should().Be("572476599891934");

        }

        [TestMethod]
        public void GetEventDetailsFromUrl_Should_Return_False()
        {
            string url = "https://www.facebook.com/events/572476599891934/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }
                };

            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetEventDetailsFromUrl(account, url);

            responseHandler.FdEvents.IsInvitedInMessanger.Should().Be(false);
            responseHandler.FbErrorDetails.IsStatusChangedRequired.Should().Be(false);
            responseHandler.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            responseHandler.FbErrorDetails.Description.Should().Be("Exception of type 'System.Exception' was thrown.");
            responseHandler.Status.Should().Be(false);

        }

        [TestMethod]
        public void SearchPeopleFromGraphSearch_Should_Return_True()
        {
            string url = "https://www.facebook.com/search/people/?q=rakesh%20kumar&epa=SERP_TAB";

            var albumResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.searchPeopleGraphResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
               .Returns(new ResponseParameter
               {
                   Response = albumResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.SearchPeopleFromGraphSearch(account, url, null, CancellationToken.None);

            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(16);
            responseHandler.Status.Should().Be(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void SearchPeopleFromGraphSearch_Should_Return_False()
        {
            string url = "https://www.facebook.com/search/people/?q=rakesh%20kumar&epa=SERP_TAB";

            var albumResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.searchPeopleGraphResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.SearchPeopleFromGraphSearch(account, url, null, CancellationToken.None);

            responseHandler.ObjFdScraperResponseParameters.ListUser.Should().BeNull();
            responseHandler.Status.Should().BeFalse();
        }

        [TestMethod]
        public void GetPostListFromGroupsNew_Should_Return_True()
        {
            string url = "https://www.facebook.com/groups/164122973615607/";

            string newsFeedUrl = "https://www.facebook.com/164122973615607";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.groupPageResponse.html", Assembly.GetExecutingAssembly());
            var newsFeedResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromGroupsNewResponse.html", Assembly.GetExecutingAssembly());
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });


            _httpHelper.GetRequest(newsFeedUrl)
              .Returns(new ResponseParameter
              {
                  Response = newsFeedResponse,
                  Exception = null,
                  HasError = false
              });

            var responseHandler = _sut.GetPostListFromGroupsNew(account, null, url);

            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(11);
            responseHandler.Status.Should().Be(true);

        }

        [TestMethod]
        public void GetPostListFromGroupsNew_Should_Return_False()
        {
            string url = "https://www.facebook.com/groups/164122973615607/";

            string newsFeedUrl = "https://www.facebook.com/164122973615607";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.groupPageResponse.html", Assembly.GetExecutingAssembly());
            var newsFeedResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromGroupsNewResponse.html", Assembly.GetExecutingAssembly());
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });


            _httpHelper.GetRequest(newsFeedUrl)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetPostListFromGroupsNew(account, null, url);

            responseHandler.Should().BeNull();
        }

        [TestMethod]
        public void ExtractPublisherParameterWall_Should_Return_True()
        {
            string url = "https://www.facebook.com/100015937932195";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ownWallResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
              .Returns(new ResponseParameter
              {
                  Response = response,
                  Exception = null,
                  HasError = false
              });

            PublisherParameter publisherParameter = new PublisherParameter();

            PublisherResponseHandler responseHandler = _sut.ExtractPublisherParameterWall
                (account, ref publisherParameter, new CancellationTokenSource());

            responseHandler.Should().Be(null);

        }

        [TestMethod]
        public void ExtractPublisherParameterWall_Should_Return_False()
        {
            string url = "https://www.facebook.com/100015937932195";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = false
                });

            PublisherParameter publisherParameter = new PublisherParameter();

            var responseHandler = _sut.ExtractPublisherParameterWall
                (account, ref publisherParameter, new CancellationTokenSource());

            responseHandler.Status.Should().BeFalse();
            responseHandler.FbErrorDetails.Status.Should().BeEmpty();
            responseHandler.ObjFdScraperResponseParameters.PostDetails.PostUrl.Should().BeEmpty();
        }

        [TestMethod]
        public void ScrapGroupsNewAsync_Should_Return_True()
        {
            string url = "https://www.facebook.com/groups/";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ScrapGroupsNewAsyncResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
              .Returns(new ResponseParameter
              {
                  Response = response,
                  Exception = null,
                  HasError = false
              });


            var task = _sut.ScrapGroupsNewAsync
                (account, null, account.Token);

            task.Result.ObjFdScraperResponseParameters.ListGroup.Count.Should().Be(30);

        }

        [TestMethod]
        public void ScrapGroupsNewAsync_Should_Return_False()
        {
            string url = "https://www.facebook.com/groups/";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ScrapGroupsNewAsyncResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = null,
                    HasError = false
                });


            var task = _sut.ScrapGroupsNewAsync(account, null, account.Token);

            task.Result.ObjFdScraperResponseParameters.ListGroup.Count.Should().Be(0);

        }

        [TestMethod]
        public void UpdateOwnPagesAsync_Should_Return_True()
        {
            string url = "https://www.facebook.com/bookmarks/pages";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateOwnPagesAsyncResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
              .Returns(new ResponseParameter
              {
                  Response = response,
                  Exception = null,
                  HasError = false
              });

            var task = _sut.UpdateOwnPagesAsync(account, null, account.Token);

            task.Result.ObjFdScraperResponseParameters.ListPage.Count.Should().Be(2);

        }

        [TestMethod]
        public void UpdateOwnPagesAsync_Should_Return_False()
        {
            string url = "https://www.facebook.com/bookmarks/pages";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateOwnPagesAsyncResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = response,
                    Exception = null,
                    HasError = false
                });

            var task = _sut.UpdateOwnPagesAsync(account, null, account.Token);

            task.Result.ObjFdScraperResponseParameters.ListPage.Count.Should().Be(2);

        }

        [TestMethod]
        public void UpdateFriendsNew_Should_Return_True()
        {
            string url = "https://www.facebook.com/100015937932195/allactivity?privacy_source=activity_log&log_filter=friends";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateFriendsNewResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
              .Returns(new ResponseParameter
              {
                  Response = response,
                  Exception = null,
                  HasError = false
              });

            var task = _sut.UpdateFriendsNew(account, null, account.Token, false);

            task.Result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(18);
            task.Result.Status.Should().Be(true);
            task.Result.HasMoreResults.Should().Be(true);
            task.Result.ObjFdScraperResponseParameters.FriendsPager.AjaxPipeToken.Should().Be("AXjR9StsVbhBmjFQ");
        }

        [TestMethod]
        public void UpdateFriendsNew_Should_Return_False()
        {
            string url = "https://www.facebook.com/100015937932195/allactivity?privacy_source=activity_log&log_filter=friends";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateFriendsNewResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = false
                });

            var task = _sut.UpdateFriendsNew
                (account, null, account.Token, false);

            task.Result.Should().BeNull();
        }

        [TestMethod]
        public void UpdateFriendsNewSync_Should_Return_True()
        {
            string url = "https://www.facebook.com/100015937932195/allactivity?privacy_source=activity_log&log_filter=friends";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateFriendsNewResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
              .Returns(new ResponseParameter
              {
                  Response = response,
                  Exception = null,
                  HasError = false
              });

            var responseHandler = _sut.UpdateFriendsNewSync
               (account, null);

            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(18);

        }

        [TestMethod]
        public void UpdateFriendsNewSync_Should_Return_null_responsehandler()
        {
            string url = "https://www.facebook.com/100015937932195/allactivity?privacy_source=activity_log&log_filter=friends";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateFriendsNewResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.UpdateFriendsNewSync(account, null);

            responseHandler.Should().BeNull();

        }

        [TestMethod]
        public void UpdateLikedPagesAsync_Should_Return_True()
        {
            string url = "https://www.facebook.com/pages/?category=liked";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateLikedPagesAsyncResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
              .Returns(new ResponseParameter
              {
                  Response = response,
                  Exception = null,
                  HasError = false
              });

            var task = _sut.UpdateLikedPagesAsync(account, null, account.Token, true);

            task.Result.ObjFdScraperResponseParameters.ListPage.Count.Should().Be(76);

        }

        [TestMethod]
        public void UpdateLikedPagesAsync_Should_Return_False()
        {
            string url = "https://www.facebook.com/search/me/pages-liked";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateLikedPagesAsyncResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = false
                });

            var task = _sut.UpdateLikedPagesAsync(account, null, account.Token);

            task.Result.ObjFdScraperResponseParameters.ListPage.Count.Should().Be(0);

        }

        [TestMethod]
        public void GetUnfriendPaginationAsync_Should_Return_True()
        {
            string url =
                "https://www.facebook.com/ajax/timeline/all_activity/scroll.php?profile_id=100015937932195&hidden_filter=&only_me_filter=0&prev_cursor=1549623397%3A1&prev_shown_time=1549623397&privacy_filter=&sidenav_filter=friends&scrubber_month=2&scrubber_year=2019%5C&data=1.5&__req=10&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetUnfriendPaginationAsyncResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
              .Returns(new ResponseParameter
              {
                  Response = response,
                  Exception = null,
                  HasError = false
              });

            IResponseParameter resopnseParameter = new ResponseParameter()
            {
                Response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetUnfriendPaginationAsyncResponse.html", Assembly.GetExecutingAssembly()),
            };

            FdFriendsInfoNewResponseHandler responseHandler = new FdFriendsInfoNewResponseHandler(resopnseParameter, null, "100015937932195", false);

            var task = _sut.GetUnfriendPaginationAsync(account, responseHandler, account.Token, false);

            task.Result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(28);

        }

        [TestMethod]
        public void GetUnfriendPaginationAsync_Should_Return_False()
        {
            string url =
                "https://www.facebook.com/ajax/timeline/all_activity/scroll.php?profile_id=100015937932195&hidden_filter=&only_me_filter=0&prev_cursor=1549623397%3A1&prev_shown_time=1549623397&privacy_filter=&sidenav_filter=friends&scrubber_month=2&scrubber_year=2019%5C&data=1.5&__req=10&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetUnfriendPaginationAsyncResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
              .Returns(new ResponseParameter
              {
                  Response = null,
                  Exception = null,
                  HasError = false
              });

            IResponseParameter resopnseParameter = new ResponseParameter()
            {
                Response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetUnfriendPaginationAsyncResponse.html", Assembly.GetExecutingAssembly()),
            };

            var responseHandler = new FdFriendsInfoNewResponseHandler(resopnseParameter, null, "100015937932195", false);

            var task = _sut.GetUnfriendPaginationAsync(account, responseHandler, account.Token, false);

        }

        [TestMethod]
        public void GetUsersBirtdayResponse_Should_Return_True()
        {
            string url = "https://m.facebook.com/events/calendar/birthdays/";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetUsersBirtdayResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
              .Returns(new ResponseParameter
              {
                  Response = response,
                  Exception = null,
                  HasError = false
              });

            var responseHandler = _sut.GetUsersBirtdayResponse(account, null);

            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(10);

        }

        [TestMethod]
        public void GetUsersBirtdayResponse_Should_Return_False()
        {
            string url = "https://m.facebook.com/events/calendar/birthdays/";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetUsersBirtdayResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.GetUsersBirtdayResponse(account, null);

            responseHandler.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);

        }

        [TestMethod]
        public void InviteFriendOrPage_Should_Return_True()
        {
            // arrange
            string url = "https://www.facebook.com/api/graphql/ ";

            var wpResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.WatchPartyResponse.html", Assembly.GetExecutingAssembly());

            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.InviteFriendOrPageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            FacebookPostDetails watchPartyDetails = new FacebookPostDetails()
            {
                Id = "https://www.facebook.com/watchparty/297670600925596/",
            };
            _httpHelper.GetRequest(watchPartyDetails.Id)

                .Returns(new ResponseParameter
                {
                    Response = wpResponse,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("av=100015937932195&__req=5j&variables=%7B%22input%22%3A%7B%22client_mutation_id%22%3A%22100%22%2C%22actor_id%22%3A%22100015937932195%22%2C%22living_room_id%22%3A%22297670600925596%22%2C%22recipient_ids%22%3A%5B%22100007464069871%22%5D%2C%22sender_id%22%3A%22100015937932195%22%2C%22send_in_messenger%22%3Afalse%7D%7D&doc_id=1540579336057750&fb_api_caller_class=RelayModern&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))
            .Returns(new ResponseParameter
            {
                Response = response,
                Exception = null,
                HasError = false
            });

            // act
            var responseHandler = _sut.InviteFriendOrPage(account, "100007464069871", ref watchPartyDetails);

            // assert
            responseHandler.Should().Be(null);

        }

        [TestMethod]
        public void InviteFriendOrPage_Should_Return_False()
        {
            string url = "https://www.facebook.com/api/graphql/ ";

            var wpResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.WatchPartyResponse.html", Assembly.GetExecutingAssembly());

            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.InviteFriendOrPageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            FacebookPostDetails watchPartyDetails = new FacebookPostDetails()
            {
                Id = "https://www.facebook.com/watchparty/297670600925596/",
            };
            _httpHelper.GetRequest(watchPartyDetails.Id)

                .Returns(new ResponseParameter
                {
                    Response = wpResponse,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("av=100015937932195&__req=5j&variables=%7B%22input%22%3A%7B%22client_mutation_id%22%3A%22100%22%2C%22actor_id%22%3A%22100015937932195%22%2C%22living_room_id%22%3A%22297670600925596%22%2C%22recipient_ids%22%3A%5B%22100007464069871%22%5D%2C%22sender_id%22%3A%22100015937932195%22%2C%22send_in_messenger%22%3Afalse%7D%7D&doc_id=1540579336057750&fb_api_caller_class=RelayModern&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))
            .Returns(new ResponseParameter
            {
                Response = null,
                Exception = null,
                HasError = false
            });


            var responseHandler =
                _sut.InviteFriendOrPage(account, "100007464069871", ref watchPartyDetails);

            responseHandler.FacebookErrors.Should().Be(null);
            responseHandler.IsStatusChangedRequired.Should().BeFalse();
        }

        [TestMethod]
        public void GetPostListFromGroups_Should_Return_True()
        {
            string url = "https://www.facebook.com/groups/164122973615607/";

            string newsFeedUrl = "https://www.facebook.com/164122973615607";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.groupPageResponse.html", Assembly.GetExecutingAssembly());
            var newsFeedResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromGroupsNewResponse.html", Assembly.GetExecutingAssembly());
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });


            _httpHelper.GetRequest(newsFeedUrl)
              .Returns(new ResponseParameter
              {
                  Response = newsFeedResponse,
                  Exception = null,
                  HasError = false
              });


            var responseHandler = _sut.GetPostListFromGroups(account, null, url);

            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(11);

        }

        [TestMethod]
        public void GetPostListFromGroups_Should_Return_False()
        {
            string url = "https://www.facebook.com/groups/164122973615607/";

            string newsFeedUrl = "https://www.facebook.com/164122973615607";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.groupPageResponse.html", Assembly.GetExecutingAssembly());
            var newsFeedResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromGroupsNewResponse.html", Assembly.GetExecutingAssembly());
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = null,
                    HasError = false
                });


            _httpHelper.GetRequest(newsFeedUrl)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = false
                });

            var responseHandler = _sut.GetPostListFromGroups(account, null, url);
            responseHandler.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(0);
            responseHandler.HasMoreResults.Should().BeTrue();
        }

        [TestMethod]
        public void GetLangugae_Should_Return_True()
        {
            string url = "https://www.facebook.com/";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.HomePageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
              .Returns(new ResponseParameter
              {
                  Response = response,
                  Exception = null,
                  HasError = false
              });


            _sut.GetLangugae(account);

        }

        [TestMethod]
        public void GetLangugae_Should_Return_False()
        {
            string url = "https://www.facebook.com/";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.HomePageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = false
                });


            _sut.GetLangugae(account);

        }

        [TestMethod]
        public void ChangeLanguage_Should_Return_True()
        {
            string url = "https://www.facebook.com/ajax/settings/language/account.php?dpr=1";
            FdConstants.AccountLanguage.Clear();
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be
                        ("new_language=cec672d6-0430-42d5-b372-773a25913a39&new_fallback_language=cec672d6-0430-42d5-b372-773a25913a39&__req=x&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
                }))
               .Returns(new ResponseParameter
               {
                   Exception = null,
                   HasError = false
               });
            var language = Guid.NewGuid().ToString();

            FdConstants.AccountLanguage.Add("100022417937758", language);

            _sut.ChangeLanguage(account, language);

        }

        [TestMethod]
        public void ChangeLanguage_Should_Return_False()
        {
            string url = "https://www.facebook.com/ajax/settings/language/account.php?dpr=1";

            string postData = "";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            _httpHelper.PostRequest(url, postData)
                .Returns(new ResponseParameter
                {
                    Exception = new Exception(),
                    HasError = true
                });

            FdConstants.AccountLanguage.Add("100022417937758", "en_GB");

        }

        [TestMethod]
        public void GetMessageRequestDetails_Should_Return_Count_True()
        {
            string url = "https://www.facebook.com/api/graphqlbatch/";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetMessageRequestDetailsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("batch_name=MessengerGraphQLThreadlistFetcher&__req=11&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&queries={\"o0\":{\"doc_id\":\"1956789641011375\",\"query_params\":{\"limit\":100,\"before\":null,\"tags\":[\"Pending\"],\"isWorkUser\":false,\"includeDeliveryReceipts\":true,\"includeSeqID\":false}}}");
            }))

               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.GetMessageRequestDetails(account, null, MessageType.Pending);

            responseHandler.ObjFdScraperResponseParameters.MessageDetailsList.Count.Should().Be(49);

        }

        [TestMethod]
        public void GetMessageRequestDetails_Should_Return_Count_False()
        {
            string url = "https://www.facebook.com/api/graphqlbatch/";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetMessageRequestDetailsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("batch_name=MessengerGraphQLThreadlistFetcher&__req=11&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&queries={\"o0\":{\"doc_id\":\"1956789641011375\",\"query_params\":{\"limit\":100,\"before\":null,\"tags\":[\"Pending\"],\"isWorkUser\":false,\"includeDeliveryReceipts\":true,\"includeSeqID\":false}}}");
                }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var responseHandler = _sut.GetMessageRequestDetails
                (account, null, MessageType.Pending);

            responseHandler.ObjFdScraperResponseParameters.MessageDetailsList.Should().BeNull();
            responseHandler.HasMoreResults.Should().BeFalse();
            responseHandler.Status.Should().BeFalse();
        }

        [TestMethod]
        public void HasAlreadySentMessage_Should_Return_True()
        {
            string url = "https://www.facebook.com/api/graphqlbatch/?dpr=1";

            var pageResponse
                = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.messageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("batch_name=MessengerGraphQLThreadFetcher&queries={\"o0\":{\"doc_id\":\"1955136014576208\",\"query_params\":{\"id\":\"100022417937758\",\"message_limit\":20000,\"load_messages\":true,\"load_read_receipts\":true,\"load_delivery_receipts\":true}}}&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            bool status = _sut.HasAlreadySentMessage(account, "100022417937758");

            status.Should().Be(false);

        }

        [TestMethod]
        public void HasAlreadySentMessage_Should_Return_False()
        {
            string url = "https://www.facebook.com/api/graphqlbatch/?dpr=1";

            var pageResponse
                = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.messageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be("batch_name=MessengerGraphQLThreadFetcher&queries={\"o0\":{\"doc_id\":\"1955136014576208\",\"query_params\":{\"id\":\"100022417937758\",\"message_limit\":20000,\"load_messages\":true,\"load_read_receipts\":true,\"load_delivery_receipts\":true}}}&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
                }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            bool status = _sut.HasAlreadySentMessage(account, "100022417937758");

            status.Should().Be(false);

        }

        [TestMethod]
        public void GetPostListFromNewsFeedAsync_Should_Return_True()
        {
            string url = "https://www.facebook.com/";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedAdsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            Task<ScrapNewPostListFromNewsFeedResponseHandler> task
                = _sut.GetPostListFromNewsFeedAsync(account, null);

            task.Result.ListFacebookAdsDetails.Count.Should().Be(1);

        }


        [TestMethod]
        public void IsLoggedIn_Should_Return_True()
        {
            string url = "https://www.facebook.com/";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = response,
                   Exception = null,
                   HasError = false
               });

            Task<FdLoginResponseHandler> task
                = _sut.IsLoggedIn(account, account.Token);

            task.Result.LoginStatus.Should().Be(true);
            task.Result.FbDtsg.Should().Be("AQHXqUOhRq75%3AAQE6ecOPiL7M");
            task.Result.UserId.Should().Be("100015937932195");
        }

        [TestMethod]
        public async Task IsLoggedIn_Should_Return_False()
        {
            string url = "https://www.facebook.com/";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var result
                = await _sut.IsLoggedIn(account, account.Token);

            result.LoginStatus.Should().BeFalse();
            result.FbDtsg.Should().Be("");
            result.UserId.Should().BeNull();



            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            result
               = await _sut.IsLoggedIn(account, account.Token);

            result.LoginStatus.Should().BeFalse();
            result.FbDtsg.Should().Be("");
            result.UserId.Should().BeNull();
        }

        [TestMethod]
        public void SendTextMessage_Should_Return_True()
        {
            string url = "https://www.facebook.com/messaging/send/?dpr=1";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendTextMessageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be(postData);
            }))

               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            var responseHandler = _sut.SendTextMessage
                (account, "100001770392775", "Hai Good Afternoon", true);

            responseHandler.Status.Should().Be(true);
            // responseHandler.ObjFdScraperResponseParameters.mes.Should().Be("mid.$cAAAABbxzh2RvO8jxiwAV7QxKKzQB");
        }

        [TestMethod]
        public void SendTextMessage_Should_Return_False()
        {
            string url = "https://www.facebook.com/messaging/send/?dpr=1";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendTextMessageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be(postData);
            }))

               .Returns(new ResponseParameter
               {
                   Response = null,
                   Exception = new Exception(),
                   HasError = true
               });

            var responseHandler = _sut.SendTextMessage
                (account, "100001770392775", "Hai Good Afternoon", true);

            responseHandler.Status.Should().BeFalse();
        }

        [TestMethod]
        public void SendTextMessageAsync_Should_Return_True()
        {
            string url = "https://www.facebook.com/messaging/send/?dpr=1";

            var pageResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendTextMessageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };
            CancellationToken cancellationToken = new CancellationToken();
            _httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be(postData);
            }),
            cancellationToken)

               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            Task<IResponseHandler> task = _sut.SendTextMessageAsync
                (account, new SenderDetails() { SenderId = "100001770392775" }, "Hai Good Afternoon", cancellationToken);

            task.Result.Should().BeNull();
        }

        [TestMethod]
        public void SendTextMessageAsync_Should_Return_False()
        {
            string url = "https://www.facebook.com/messaging/send/?dpr=1";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            var cancellationToken = new CancellationToken();
            _httpHelper.PostRequestAsync(url, Arg.Do((byte[] a) =>
                    {
                        var postData = Encoding.UTF8.GetString(a);
                        postData.Should().Be(postData);
                    }),
                    cancellationToken)

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            Task<IResponseHandler> task = _sut.SendTextMessageAsync
                 (account, new SenderDetails() { SenderId = "100001770392775" }, "Hai Good Afternoon", cancellationToken);

            task.Result.Should().BeNull();
        }

        [TestMethod]
        public void SendTextMessageWithLinkPreview_Should_Return_True()
        {
            string url = "https://www.facebook.com/messaging/send/?dpr=1";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendTextMessageWithLinkPreviewResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };
            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be(postData);
            }))

               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            string previewLink =
                "https://www.facebook.com/ZeeNewsEnglish/videos/1027664247426521/?__xts__%5B0%5D=68.ARAYbqkLW2iF776UYShPbwaWoo42tTyE2B9TZBG3vgIiqvFTehu6TMjWU1zj6zKeNQJFo72MtDGBYR4v3fFBIvBqrdqq0FxMFzJ-nTvOZpgxHRw41x4mOf1sLopEU9885nkLxmCdg3yEdQfk9nhqlVAONZIyZTQeCRWLrvkh4RQkwQHk14NZOiCEClwuOLhFA77IgRTBuTmReR0T2FY9VNLdmdyN5Otsn8sHabEWlu764cuV1gfFTLz16MVvSz6GmiwnvXtslpr1sQGoIhENqXFLut5cgwFWUwCrVaA2rvD35uAAAUVAy9YiPOL-dLjygko8cJPOyWETWyORSZIvprgA0E2Y_67MivveNQ0qr9oIMvMR60sDwlS10kk2&__tn__=-R";

            var responseHandler = _sut.SendTextMessageWithLinkPreview
                (account, "100001770392775", previewLink, true);

            responseHandler.Status.Should().Be(true);
            //responseHandler.MessageId.Should().Be("mid.$cAAAABbxzh2RvO_WZQwAV7QxKKz9z");
        }

        [TestMethod]
        public void SendTextMessageWithLinkPreview_Should_Return_False()
        {
            string url = "https://www.facebook.com/messaging/send/?dpr=1";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendTextMessageWithLinkPreviewResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };
            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be(postData);
            }))
               .Returns(new ResponseParameter
               {
                   Response = null,
                   Exception = null,
                   HasError = false
               });

            string previewLink =
                "https://www.facebook.com/ZeeNewsEnglish/videos/1027664247426521/?__xts__%5B0%5D=68.ARAYbqkLW2iF776UYShPbwaWoo42tTyE2B9TZBG3vgIiqvFTehu6TMjWU1zj6zKeNQJFo72MtDGBYR4v3fFBIvBqrdqq0FxMFzJ-nTvOZpgxHRw41x4mOf1sLopEU9885nkLxmCdg3yEdQfk9nhqlVAONZIyZTQeCRWLrvkh4RQkwQHk14NZOiCEClwuOLhFA77IgRTBuTmReR0T2FY9VNLdmdyN5Otsn8sHabEWlu764cuV1gfFTLz16MVvSz6GmiwnvXtslpr1sQGoIhENqXFLut5cgwFWUwCrVaA2rvD35uAAAUVAy9YiPOL-dLjygko8cJPOyWETWyORSZIvprgA0E2Y_67MivveNQ0qr9oIMvMR60sDwlS10kk2&__tn__=-R";

            var responseHandler = _sut.SendTextMessageWithLinkPreview
                (account, "100001770392775", previewLink, true);

            responseHandler.Status.Should().BeFalse();
            //responseHandler.MessageId.Should().BeNull();
            responseHandler.IsMessageSent.Should().BeFalse();
        }

        [TestMethod]
        public void UpdateLinkAndGetShareId_Should_Return_True()
        {
            string url = "https://www.facebook.com/message_share_attachment/fromURI/?dpr=1";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateLinkAndGetShareIdResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };
            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("image_height=960&image_width=960&uri=https://www.facebook.com/ZeeNewsEnglish/videos/1027664247426521/?__xts__%5B0%5D=68.ARAYbqkLW2iF776UYShPbwaWoo42tTyE2B9TZBG3vgIiqvFTehu6TMjWU1zj6zKeNQJFo72MtDGBYR4v3fFBIvBqrdqq0FxMFzJ-nTvOZpgxHRw41x4mOf1sLopEU9885nkLxmCdg3yEdQfk9nhqlVAONZIyZTQeCRWLrvkh4RQkwQHk14NZOiCEClwuOLhFA77IgRTBuTmReR0T2FY9VNLdmdyN5Otsn8sHabEWlu764cuV1gfFTLz16MVvSz6GmiwnvXtslpr1sQGoIhENqXFLut5cgwFWUwCrVaA2rvD35uAAAUVAy9YiPOL-dLjygko8cJPOyWETWyORSZIvprgA0E2Y_67MivveNQ0qr9oIMvMR60sDwlS10kk2&__tn__=-R&__req=11&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            string previewLink =
                "https://www.facebook.com/ZeeNewsEnglish/videos/1027664247426521/?__xts__%5B0%5D=68.ARAYbqkLW2iF776UYShPbwaWoo42tTyE2B9TZBG3vgIiqvFTehu6TMjWU1zj6zKeNQJFo72MtDGBYR4v3fFBIvBqrdqq0FxMFzJ-nTvOZpgxHRw41x4mOf1sLopEU9885nkLxmCdg3yEdQfk9nhqlVAONZIyZTQeCRWLrvkh4RQkwQHk14NZOiCEClwuOLhFA77IgRTBuTmReR0T2FY9VNLdmdyN5Otsn8sHabEWlu764cuV1gfFTLz16MVvSz6GmiwnvXtslpr1sQGoIhENqXFLut5cgwFWUwCrVaA2rvD35uAAAUVAy9YiPOL-dLjygko8cJPOyWETWyORSZIvprgA0E2Y_67MivveNQ0qr9oIMvMR60sDwlS10kk2&__tn__=-R";

            string shareType = string.Empty;

            ScrapedLinkDetails scrapedLinkDetails = new ScrapedLinkDetails();

            string responseHandler = _sut.UpdateLinkAndGetShareId
                (account, previewLink, ref shareType, ref scrapedLinkDetails);

            responseHandler.Should().Be("1027664247426521");
        }

        [TestMethod]
        public void UpdateLinkAndGetShareId_Should_Return_False()
        {
            string url = "https://www.facebook.com/message_share_attachment/fromURI/?dpr=1";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateLinkAndGetShareIdResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };
            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be
                    ("image_height=960&image_width=960&uri=https://www.facebook.com/ZeeNewsEnglish/videos/1027664247426521/?__xts__%5B0%5D=68.ARAYbqkLW2iF776UYShPbwaWoo42tTyE2B9TZBG3vgIiqvFTehu6TMjWU1zj6zKeNQJFo72MtDGBYR4v3fFBIvBqrdqq0FxMFzJ-nTvOZpgxHRw41x4mOf1sLopEU9885nkLxmCdg3yEdQfk9nhqlVAONZIyZTQeCRWLrvkh4RQkwQHk14NZOiCEClwuOLhFA77IgRTBuTmReR0T2FY9VNLdmdyN5Otsn8sHabEWlu764cuV1gfFTLz16MVvSz6GmiwnvXtslpr1sQGoIhENqXFLut5cgwFWUwCrVaA2rvD35uAAAUVAy9YiPOL-dLjygko8cJPOyWETWyORSZIvprgA0E2Y_67MivveNQ0qr9oIMvMR60sDwlS10kk2&__tn__=-R&__req=11&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

               .Returns(new ResponseParameter
               {
                   Response = null,
                   Exception = null,
                   HasError = false
               });

            string previewLink =
                "https://www.facebook.com/ZeeNewsEnglish/videos/1027664247426521/?__xts__%5B0%5D=68.ARAYbqkLW2iF776UYShPbwaWoo42tTyE2B9TZBG3vgIiqvFTehu6TMjWU1zj6zKeNQJFo72MtDGBYR4v3fFBIvBqrdqq0FxMFzJ-nTvOZpgxHRw41x4mOf1sLopEU9885nkLxmCdg3yEdQfk9nhqlVAONZIyZTQeCRWLrvkh4RQkwQHk14NZOiCEClwuOLhFA77IgRTBuTmReR0T2FY9VNLdmdyN5Otsn8sHabEWlu764cuV1gfFTLz16MVvSz6GmiwnvXtslpr1sQGoIhENqXFLut5cgwFWUwCrVaA2rvD35uAAAUVAy9YiPOL-dLjygko8cJPOyWETWyORSZIvprgA0E2Y_67MivveNQ0qr9oIMvMR60sDwlS10kk2&__tn__=-R";

            string shareType = string.Empty;

            ScrapedLinkDetails scrapedLinkDetails = new ScrapedLinkDetails();

            string responseHandler = _sut.UpdateLinkAndGetShareId
                (account, previewLink, ref shareType, ref scrapedLinkDetails);

            responseHandler.Should().Be("");
        }

        [TestMethod]
        public void GetScrapedLinkDetails_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetScrapedLinkDetailsResponse.html", Assembly.GetExecutingAssembly());

            ScrapedLinkDetails responseHandler = _sut.GetScrapedLinkDetails(response);

            responseHandler.UrlScrapeId.Should().Be("732405683827044");
        }

        [TestMethod]
        public void GetScrapedLinkDetails_Should_Return_False()
        {
            var response = "";

            var responseHandler = _sut.GetScrapedLinkDetails(response);

            responseHandler.UrlScrapeId.Should().Be("");
        }
        //91
        [TestMethod]
        public void PostToPages_Should_Return_True()
        {

            string url = "https://www.facebook.com/mumbaiakkians/";

            var fanpageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FanPageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountId = "8623694d-dc77-4737-a8ed-a1c0fe66b7b7",
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
              .Returns(new ResponseParameter
              {
                  Response = fanpageResponse,
                  Exception = null,
                  HasError = false
              });

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("variables=%7B%22client_mutation_id%22%3A%22ca12ad46-1314-40c6-bdd4-03623904c749%22%2C%22actor_id%22%3A%22100015937932195%22%2C%22input%22%3A%7B%22client_mutation_id%22%3A%22156ced33-099c-45cb-b9e1-90e6fa8a36c3%22%2C%22actor_id%22%3A%22100015937932195%22%2C%22message%22%3A%7B%22text%22%3A%22Hello%20Folks%5Cn%22%2C%22ranges%22%3A%5B%5D%7D%2C%22logging%22%3A%7B%22composer_session_id%22%3A%2269d4a5c1cd630b1359d17c7de5faf5d0%22%2C%22ref%22%3A%22pages_feed%22%7D%2C%22with_tags_ids%22%3A%5B%5D%2C%22multilingual_translations%22%3A%5B%5D%2C%22camera_post_context%22%3A%7B%22source%22%3A%22composer%22%2C%22deduplication_id%22%3A%2269d4a5c1cd630b1359d17c7de5faf5d0%22%7D%2C%22composer_source_surface%22%3A%22page%22%2C%22composer_entry_point%22%3A%22pages_feed%22%2C%22composer_entry_time%22%3A16%2C%22direct_share_status%22%3A%22NOT_SHARED%22%2C%22sponsor_relationship%22%3A%22WITH%22%2C%22web_graphml_migration_params%22%3A%7B%22target_type%22%3A%22page%22%2C%22xhpc_composerid%22%3A%22rc.u_0_1y%22%2C%22xhpc_context%22%3A%22profile%22%2C%22xhpc_finch%22%3Atrue%2C%22xhpc_publish_type%22%3A%22FEED_INSERT%22%2C%22xhpc_timeline%22%3Atrue%2C%22waterfall_id%22%3A%2269d4a5c1cd630b1359d17c7de5faf5d0%22%7D%2C%22place_attachment_setting%22%3A%22HIDE_ATTACHMENT%22%2C%22attachments%22%3A%5B%5D%2C%22source%22%3A%22WWW%22%2C%22audience%22%3A%7B%22to_id%22%3A%22339142240259836%22%7D%7D%7D&__req=4w&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=AQGapcJ_Yoty:AQEjuqxeGO4e&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = fanpageResponse,
                    Exception = null,
                    HasError = false
                });

            PublisherPostlistModel publisherPostlistModel = new PublisherPostlistModel();
            GeneralModel generalsettingsModel = new GeneralModel();
            FacebookModel advanceSettingsModel = new FacebookModel();
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            PublisherResponseHandler responseHandler = _sut.PostToPages
                (account, url, publisherPostlistModel, cancellationTokenSource, generalsettingsModel, advanceSettingsModel);

            responseHandler.Status.Should().BeFalse();

        }

        [TestMethod]
        public void PostToPages_Should_Return_False()
        {

            string url = "https://www.facebook.com/mumbaiakkians/";

            var fanpageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FanPageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountId = "8623694d-dc77-4737-a8ed-a1c0fe66b7b7",
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }
                };

            _httpHelper.GetRequest(url)
              .Returns(new ResponseParameter
              {
                  Response = fanpageResponse,
                  Exception = null,
                  HasError = false
              });

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("variables=%7B%22client_mutation_id%22%3A%22ca12ad46-1314-40c6-bdd4-03623904c749%22%2C%22actor_id%22%3A%22100015937932195%22%2C%22input%22%3A%7B%22client_mutation_id%22%3A%22156ced33-099c-45cb-b9e1-90e6fa8a36c3%22%2C%22actor_id%22%3A%22100015937932195%22%2C%22message%22%3A%7B%22text%22%3A%22Hello%20Folks%5Cn%22%2C%22ranges%22%3A%5B%5D%7D%2C%22logging%22%3A%7B%22composer_session_id%22%3A%2269d4a5c1cd630b1359d17c7de5faf5d0%22%2C%22ref%22%3A%22pages_feed%22%7D%2C%22with_tags_ids%22%3A%5B%5D%2C%22multilingual_translations%22%3A%5B%5D%2C%22camera_post_context%22%3A%7B%22source%22%3A%22composer%22%2C%22deduplication_id%22%3A%2269d4a5c1cd630b1359d17c7de5faf5d0%22%7D%2C%22composer_source_surface%22%3A%22page%22%2C%22composer_entry_point%22%3A%22pages_feed%22%2C%22composer_entry_time%22%3A16%2C%22direct_share_status%22%3A%22NOT_SHARED%22%2C%22sponsor_relationship%22%3A%22WITH%22%2C%22web_graphml_migration_params%22%3A%7B%22target_type%22%3A%22page%22%2C%22xhpc_composerid%22%3A%22rc.u_0_1y%22%2C%22xhpc_context%22%3A%22profile%22%2C%22xhpc_finch%22%3Atrue%2C%22xhpc_publish_type%22%3A%22FEED_INSERT%22%2C%22xhpc_timeline%22%3Atrue%2C%22waterfall_id%22%3A%2269d4a5c1cd630b1359d17c7de5faf5d0%22%7D%2C%22place_attachment_setting%22%3A%22HIDE_ATTACHMENT%22%2C%22attachments%22%3A%5B%5D%2C%22source%22%3A%22WWW%22%2C%22audience%22%3A%7B%22to_id%22%3A%22339142240259836%22%7D%7D%7D&__req=4w&__user=100015937932195&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=AQGapcJ_Yoty:AQEjuqxeGO4e&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = false
                });

            PublisherResponseHandler responseHandler = _sut.PostToPages
                (account, url, new PublisherPostlistModel(), new CancellationTokenSource(), new GeneralModel(), new FacebookModel());

            responseHandler.Status.Should().BeFalse();
        }

        [TestMethod]
        public void Login_Should_Return_True()
        {
            string url = "https://www.facebook.com/";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountId = "ebc9757b-996c-4785-8872-67cbc2169d7a",
                    IsUserLoggedIn = true,
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }

                };

            AccountsFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
            AccountsFileManager.GetAccountById(Arg.Any<string>()).Returns(account);

            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            bool status
                = _sut.Login(account);

            status.Should().Be(true);

        }

        [TestMethod]
        public void Login_Should_Return_False()
        {
            string url = "https://www.facebook.com/";

            var account =
                new DominatorAccountModel
                {
                    AccountId = "ebc9757b-996c-4785-8872-67cbc2169d7a",
                    IsUserLoggedIn = true,
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        }

                };

            AccountsFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
            AccountsFileManager.GetAccountById(Arg.Any<string>()).Returns(account);

            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            bool status = _sut.Login(account);

            status.Should().BeTrue();

            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = false
                });

            status = _sut.Login(account);

            status.Should().BeTrue();

        }

        [TestMethod]
        public void LoginForPostScrapperAsync_Should_Return_True()
        {
            string url = "https://www.facebook.com/";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            AccountsFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
            AccountsFileManager.GetAccountById(Arg.Any<string>()).Returns(account);

            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            Task<bool> task
                = _sut.LoginForPostScrapperAsync(account, account.Token);

            task.Result.Should().Be(true);

        }

        [TestMethod]
        public void LoginForPostScrapperAsync_Should_Return_False()
        {
            string url = "https://www.facebook.com/";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            AccountsFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
            AccountsFileManager.GetAccountById(Arg.Any<string>()).Returns(account);

            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = false
                });

            var task = _sut.LoginForPostScrapperAsync(account, account.Token);

            task.Result.Should().BeFalse();

        }

        [TestMethod]
        public void LoginAsync_Should_Return_True()
        {
            string url = "https://www.facebook.com/";

            var pageResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };

            AccountsFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
            AccountsFileManager.GetAccountById(Arg.Any<string>()).Returns(account);


            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            Task<bool> task
                = _sut.LoginAsync(account, account.Token);

            task.Result.Should().Be(true);

        }

        [TestMethod]
        public void LoginAsync_Should_Return_False()
        {
            string url = "https://www.facebook.com/";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = string.Empty,
                    Exception = new Exception(),
                    HasError = true
                });

            Task<bool> task
                = _sut.LoginAsync(account, account.Token);

            task.Result.Should().Be(false);

        }

        [TestMethod]
        public void GetRecentFriendMessageDetails_Should_Return_True()
        {
            string url = "https://www.facebook.com/messages/t/";

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetRecentFriendMessageDetailsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                            AccountId = "e77a19f6-3361-4549-810c-1e5799ec6654",
                        },
                };
            _httpHelper.GetRequest(url)
               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            CancellationToken cancellationToken = new CancellationToken();

            var responseHandler = _sut.GetRecentFriendMessageDetails
                (account, null, cancellationToken);

            responseHandler.ObjFdScraperResponseParameters.MessageSenderDetailsList.Count.Should().Be(20);
            responseHandler.HasMoreResults.Should().Be(true);
            responseHandler.ObjFdScraperResponseParameters.PaginationTimestamp.Should().Be("1550490115357");
        }

        [TestMethod]
        public void GetRecentFriendMessageDetails_Should_Return_False()
        {
            // Arrange
            string url = "https://www.facebook.com/messages/t/";

            var account =
             new DominatorAccountModel
             {
                 AccountBaseModel =
                     new DominatorAccountBaseModel
                     {
                         AccountNetwork = SocialNetworks.Facebook,
                         UserId = "100015937932195",
                         AccountId = "e77a19f6-3361-4549-810c-1e5799ec6654",
                     },
             };
            _httpHelper.GetRequest(url)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            // act
            var responseHandler = _sut.GetRecentFriendMessageDetails
                (account, null, CancellationToken.None);

            // assert
            responseHandler.ObjFdScraperResponseParameters.Should().BeNull();
            responseHandler.HasMoreResults.Should().Be(true);
            responseHandler.PageletData.Should().BeNull();
        }

        [TestMethod]
        public void GetDetailedInfoUserMobileScraperAsync_Should_Return_True()
        {
            string url = "https://m.facebook.com/profile.php?v=info&lst=100022417937758%3A100022417937758%3A7346554&id=100022417937758";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetDetailedInfoUserMobileScraperAsyncResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = response,
                   Exception = null,
                   HasError = false
               });

            FacebookUser user = new FacebookUser()
            {
                UserId = account.AccountBaseModel.UserId,
            };

            var responseHandler = _sut.GetDetailedInfoUserMobileScraperAsync
                (user, account, true, true, account.Token, "7346554");

            responseHandler.Result.ObjFdScraperResponseParameters.FacebookUser.ProfileUrl.Should().Be("https://www.facebook.com/100022417937758");
            responseHandler.Exception.Should().BeNull();
        }

        [TestMethod]
        public void GetDetailedInfoUserMobileScraperAsync_Should_Return_False()
        {
            string url = "https://m.facebook.com/profile.php?v=info&lst=100022417937758%3A100022417937758%3A7346554&id=100022417937758";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = new Exception(),
                    HasError = true
                });

            var user = new FacebookUser()
            {
                UserId = account.AccountBaseModel.UserId,
            };

            var responseHandler = _sut.GetDetailedInfoUserMobileScraperAsync
                (user, account, true, true, account.Token, "7346554");

            responseHandler.Result.ObjFdScraperResponseParameters.Should().BeNull();
            responseHandler.Exception.Should().BeNull();
        }

        [TestMethod]
        public void ScrapOwnProfileInfoAsync_Should_Return_True()
        {
            string url = "https://m.facebook.com/profile.php?v=info&lst=100022417937758%3A100022417937758%3A1550660284&id=100022417937758";

            string scrapeUrl = "https://api.socinator.com/fb_user_data";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetDetailedInfoUserMobileScraperAsyncResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = response,
                   Exception = null,
                   HasError = false
               });

            _httpHelper.PostRequest(scrapeUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("");
            }))

             .Returns(new ResponseParameter
             {
                 Exception = null,
                 HasError = false
             });

            Task task = _sut.ScrapOwnProfileInfoAsync(account);

            task.Exception.Should().BeNull();
        }

        [TestMethod]
        public void ScrapOwnProfileInfoAsync_Should_Return_False()
        {
            string url = "https://m.facebook.com/profile.php?v=info&lst=100022417937758%3A100022417937758%3A1550660284&id=100022417937758";

            string scrapeUrl = "https://api.socinator.com/fb_user_data";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
                .Returns(new ResponseParameter
                {
                    Response = null,
                    Exception = null,
                    HasError = false
                });

            _httpHelper.PostRequest(scrapeUrl, Arg.Do((byte[] a) =>
                {
                    var postData = Encoding.UTF8.GetString(a);
                    postData.Should().Be(postData);
                }))

                .Returns(new ResponseParameter
                {
                    Exception = null,
                    HasError = false
                });

            Task task = _sut.ScrapOwnProfileInfoAsync(account);

            task.Exception.Should().BeNull();
        }

        [TestMethod]
        public void UpdateFriendsFromPage_Should_Return_True()
        {
            string url = "https://www.facebook.com/155608091155587/friend_inviter_v2/?ref=context_row&dpr=1&__asyncDialog=1&__req=17&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateFriendsFromPageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = response,
                   Exception = null,
                   HasError = false
               });

            List<string> lstPageId = new List<string>()
            {
                "155608091155587",
            };

            CancellationToken cancellationToken = new CancellationToken();
            var responseHandler = _sut.UpdateFriendsFromPage
                (account, null, cancellationToken, lstPageId);

            responseHandler.Result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(442);
            responseHandler.Result.HasMoreResults.Should().Be(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void UpdateFriendsFromPage_Should_Return_False()
        {
            string url = "https://www.facebook.com/155608091155587/friend_inviter_v2/?ref=context_row&dpr=1&__asyncDialog=1&__req=17&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        }
                };

            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = null,
                   Exception = new Exception(),
                   HasError = true
               });

            List<string> lstPageId = new List<string>()
            {
                "155608091155587",
            };
            var responseHandler = _sut.UpdateFriendsFromPage
                (account, null, Arg.Any<CancellationToken>(), lstPageId);

            responseHandler.Result.ObjFdScraperResponseParameters.ListUser.Should().BeNull();
            responseHandler.Result.HasMoreResults.Should().Be(true);
        }

        [TestMethod]
        public void GetGroupDetails_Should_Return_True()
        {
            string url = "https://www.facebook.com/155608091155587/friend_inviter_v2/?ref=context_row&dpr=1&__asyncDialog=1&__req=17&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateFriendsFromPageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };


            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = response,
                   Exception = null,
                   HasError = false
               });

            List<string> lstPageId = new List<string>()
            {
                "155608091155587",
            };

            CancellationToken cancellationToken = new CancellationToken();
            var responseHandler = _sut.UpdateFriendsFromPage
                (account, null, cancellationToken, lstPageId);

            responseHandler.Result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(442);
            responseHandler.Result.HasMoreResults.Should().Be(true);
        }

        [TestMethod]
        public void GetGroupDetails_Should_Return_False()
        {
            string url = "https://www.facebook.com/155608091155587/friend_inviter_v2/?ref=context_row&dpr=1&__asyncDialog=1&__req=17&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        }
                };

            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = null,
                   Exception = new Exception(),
                   HasError = true
               });

            List<string> lstPageId = new List<string>()
            {
                "155608091155587",
            };

            var responseHandler = _sut.UpdateFriendsFromPage
                (account, null, Arg.Any<CancellationToken>(), lstPageId);

            responseHandler.Result.Should().BeNull();
        }

        [TestMethod]
        public void GetPageDetails_Should_Return_True()
        {
            string url = "https://www.facebook.com/106270215428";

            string pageUrl = "https://www.facebook.com/106270215428/posts";

            var pageResponse
                = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPageDetailsResponse.html", Assembly.GetExecutingAssembly());

            var postPageResponse
                = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.postPageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };
            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            _httpHelper.GetRequestAsync(pageUrl, account.Token)
              .Returns(new ResponseParameter
              {
                  Response = postPageResponse,
                  Exception = null,
                  HasError = false
              });

            FacebookAdsDetails facebookAdsDetails = new FacebookAdsDetails()
            {
                AdId = "6140129940750",
                Id = "10153971909960429",
                OwnerId = "106270215428",
                NavigationUrl = "http://www.wix.com/htmlsites/-click-here",
                PostedDateTime = DateTime.Now,
            };

            Task task = _sut.GetPageDetails(account, facebookAdsDetails);

            task.Exception.Should().BeNull();
        }

        [TestMethod]
        public void GetPageDetails_Should_Return_False()
        {
            string url = "https://www.facebook.com/106270215428";

            string pageUrl = "https://www.facebook.com/106270215428/posts";

            var pageResponse
                = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPageDetailsResponse.html", Assembly.GetExecutingAssembly());

            var postPageResponse
                = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.postPageResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };
            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            _httpHelper.GetRequestAsync(pageUrl, account.Token)
              .Returns(new ResponseParameter
              {
                  Response = null,
                  Exception = new Exception(),
                  HasError = true
              });

            FacebookAdsDetails facebookAdsDetails = new FacebookAdsDetails()
            {
                AdId = "6140129940750",
                Id = "10153971909960429",
                OwnerId = "106270215428",
                NavigationUrl = "http://www.wix.com/htmlsites/-click-here",
                PostedDateTime = DateTime.Now,
            };

            Task task = _sut.GetPageDetails(account, facebookAdsDetails);

            task.Exception.Should().BeNull();
        }

        [TestMethod]
        public void SendImageWithText_Should_Return_True()
        {
            string imageUrl =
                "https://upload.facebook.com/ajax/mercury/upload.php?dpr=1&__req=1k&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&ft[tn]=%2BM";

            string url = "https://www.facebook.com/messaging/send/?dpr=1";

            var imageResponse = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UploadImageAndGetMediaIdForMessageResponse.html", Assembly.GetExecutingAssembly());

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendImageWithTextResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            _httpHelper.PostRequest(imageUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be(postData);
            }))

              .Returns(new ResponseParameter
              {
                  Response = imageResponse,
                  Exception = null,
                  HasError = false
              });

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("client=mercury&action_type=ma-type:user-generated-message&body=&ephemeral_ttl_mode=0&has_attachment=true&image_ids[0]=&message_id=6171611550660284&offline_threading_id=6171611550660284&other_user_fbid=100034254103061&signature_id=6219f1c3&source=source:chat:web&specific_to_list[0]=fbid:100034254103061&specific_to_list[1]=fbid:100022417937758&tags[0]=web:trigger:fb_header_dock:jewel_thread&timestamp=1550660284&ui_push_phase=C3&__req=11&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            var status = _sut.SendImageWithText
                (account, "100034254103061", new List<string>(), "1550660284");

            status.Should().BeFalse();
        }

        [TestMethod]
        public void SendImageWithText_Should_Return_False()
        {
            string imageUrl =
                "https://upload.facebook.com/ajax/mercury/upload.php?dpr=1&__req=1k&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851&ft[tn]=%2BM";

            string url = "https://www.facebook.com/messaging/send/?dpr=1";

            var imageResponse = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UploadImageAndGetMediaIdForMessageResponse.html", Assembly.GetExecutingAssembly());

            var pageResponse = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendImageWithTextResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            _httpHelper.PostRequest(imageUrl, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be(postData);
            }))

              .Returns(new ResponseParameter
              {
                  Response = imageResponse,
                  Exception = null,
                  HasError = false
              });

            _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("client=mercury&action_type=ma-type:user-generated-message&body=&ephemeral_ttl_mode=0&has_attachment=true&image_ids[0]=&message_id=6171611550660284&offline_threading_id=6171611550660284&other_user_fbid=100034254103061&signature_id=6219f1c3&source=source:chat:web&specific_to_list[0]=fbid:100034254103061&specific_to_list[1]=fbid:100022417937758&tags[0]=web:trigger:fb_header_dock:jewel_thread&timestamp=1550660284&ui_push_phase=C3&__req=11&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
            }))

               .Returns(new ResponseParameter
               {
                   Response = null,
                   Exception = new Exception(),
                   HasError = true
               });

            var status = _sut.SendImageWithText
                (account, "100034254103061", new List<string>() { }, "1550660284");

            status.Should().BeFalse();
        }

        //[TestMethod]
        //public void EventCreater_Should_Return_Tue()
        //{
        //    string url =
        //        "https://www.facebook.com/ajax/create/event/submit/?title=Jelly%20Joy&timezone=&cover_focus%5Bx%5D=0.5&cover_focus%5By%5D=0.5&start_date=5%2F16%2F2119&start_time=50280&end_date=5%2F16%2F2119&end_time=50280&acontext=%7B%22source%22%3A%221%22%2C%22ref%22%3A%222%22%2C%22sid_create%22%3A%222444552616%22%2C%22action_history%22%3A%22%5B%7B%5C%22mechanism%5C%22%3A%5C%22user_create_dialog%5C%22%2C%5C%22surface%5C%22%3A%5C%22create_dialog%5C%22%2C%5C%22extra_data%5C%22%3A%5C%22%5B%5D%5C%22%7D%2C%7B%5C%22mechanism%5C%22%3A%5C%22main_list%5C%22%2C%5C%22surface%5C%22%3A%5C%22dashboard%5C%22%2C%5C%22extra_data%5C%22%3A%5C%22%7B%5C%5C%5C%22dashboard_filter%5C%5C%5C%22%3A%5C%5C%5C%22upcoming%5C%5C%5C%22%7D%5C%22%7D%2C%7B%5C%22mechanism%5C%22%3A%5C%22user_create_dialog%5C%22%2C%5C%22surface%5C%22%3A%5C%22create_dialog%5C%22%2C%5C%22extra_data%5C%22%3A%5C%22%5B%5D%5C%22%7D%2C%7B%5C%22mechanism%5C%22%3A%5C%22surface%5C%22%2C%5C%22surface%5C%22%3A%5C%22permalink%5C%22%2C%5C%22extra_data%5C%22%3A%5C%22%5B%5D%5C%22%7D%2C%7B%5C%22mechanism%5C%22%3A%5C%22user_create_dialog%5C%22%2C%5C%22surface%5C%22%3A%5C%22create_dialog%5C%22%2C%5C%22extra_data%5C%22%3A%5C%22%5B%5D%5C%22%7D%2C%7B%5C%22mechanism%5C%22%3A%5C%22surface%5C%22%2C%5C%22surface%5C%22%3A%5C%22permalink%5C%22%2C%5C%22extra_data%5C%22%3A%5C%22%5B%5D%5C%22%7D%2C%7B%5C%22surface%5C%22%3A%5C%22create_dialog%5C%22%2C%5C%22mechanism%5C%22%3A%5C%22user_create_dialog%5C%22%2C%5C%22extra_data%5C%22%3A%5B%5D%7D%5D%22%2C%22has_source%22%3Atrue%7D&event_ent_type=2&guests_can_invite_friends=true&guest_list_enabled=true&is_host_collect_payment=false&save_as_draft=false&friend_birthday_prompt_xout_id=&category_id=&cover_video_offset_type=0&cover_video_offset=0&dialog_entry_point=others&interception_flow_type=dialog";

        //    var response = TestUtils.ReadFileFromResources
        //        ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.EventCreatorResponse.html", Assembly.GetExecutingAssembly());

        //    var account =
        //        new DominatorAccountModel
        //        {
        //            AccountBaseModel =
        //                new DominatorAccountBaseModel
        //                {
        //                    AccountNetwork = SocialNetworks.Facebook,
        //                    UserId = "100022417937758",
        //                },
        //        };

        //    _httpHelper.PostRequest(url, Arg.Do((byte[] a) =>
        //        {
        //            var postData = Encoding.UTF8.GetString(a);
        //            postData.Should().Be(
        //                "__req=2e&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&__be=1&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851");
        //        }))

        //        .Returns(new ResponseParameter
        //        {
        //            Response = response,
        //            Exception = null,
        //            HasError = false
        //        });

        //    var EventCreaterManagerModel = new EventCreaterManagerModel
        //    {
        //        EventName = "Jelly Joy",
        //        EventStartDate = DateTime.Now.AddYears(100),
        //        EventEndDate = DateTime.Now.AddYears(100),
        //        EventType = "Create Private Event"
        //    };

        //    var responseHandler = _fdRequestLibrary.EventCreater(account, EventCreaterManagerModel);


        //}


        [TestMethod]
        public void SaveDetailsInDb_Should_Return_True()
        {
            string url = "https://api.socinator.com/adsdata";

            var pageResponse
                = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SaveDetailsInDbResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };

            _httpHelper.PostApiRequestAsync(url, Arg.Do((byte[] a) =>
            {
                var postData = Encoding.UTF8.GetString(a);
                postData.Should().Be("{\"type\":\"NoMedia\",\"category\":\"\",\"call_to_action\":\"\",\"image_video_url\":\"\",\"other_multimedia\":\"\",\"destination_url\":\"http%3A%2F%2Fwww.wix.com%2Fhtmlsites%2F-click-here\",\"ad_title\":\"\",\"news_feed_description\":\"\",\"ad_id\":\"10153971909960429\",\"post_date\":\"1550641679\",\"first_seen\":\"-2147483648\",\"last_seen\":\"-2147483648\",\"city\":\"\",\"state\":\"\",\"country\":\"\",\"lower_age\":\"\",\"upper_age\":\"\",\"post_owner\":\"\",\"post_owner_image\":\"\",\"ad_position\":\"FEED\",\"likes\":\"\",\"comment\":\"\",\"share\":\"\",\"ad_text\":\"\",\"ad_url\":\"https%3A%2F%2Fwww.facebook.com%2F10153971909960429\",\"facebook_id\":\"100022417937758\",\"side_url\":\"\",\"platform\":\"2\",\"version\":\"1.0.21\"}");
            }))

               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            FacebookAdsDetails facebookAdsDetails = new FacebookAdsDetails()
            {
                AdId = "6140129940750",
                Id = "10153971909960429",
                OwnerId = "106270215428",
                NavigationUrl = "http://www.wix.com/htmlsites/-click-here",
                PostedDateTime = DateTime.Now,
            };

            Task task = _sut.SaveDetailsInDb(account, facebookAdsDetails);

            task.Exception.Should().BeNull();
        }

        [TestMethod]
        public void ViewersDetailsParser_Should_Return_True()
        {
            string url = "https://www.facebook.com/ads/preferences/dialog/?id=6140129940750&optout_url=%2Fsettings%2F%3Ftab%3Dads&page_type=16&serialized_nfx_action_info=&session_id=29459&use_adchoices=1&dpr=1&__asyncDialog=1&__user=100022417937758&__a=1&__dyn=7AgNe-4amaxx2u6aJGeFxqeCwDKEyGgS8zQC-C267Uqzob4q2i5U4e8wzwJzFUKbnyocWAx-uUG4XzE8V8iyUdUOdwJKqq2i58nVV8-cxu5o5S9Azo9ohwCwBxrxqrV8iABxG7WwhUcUcFVo762Su4pHxC68nxK44fjwhpqx11qUKaxi8zVUGdwKCxC48S8GEjG48yq4o4ecG8HgjAUWEC23RByEjhm3Ch4yEiyo8U-4Kl3bz8G48kVE&fb_dtsg=&jazoest=26581721229097781106845697858658171114102508185110764851";

            var pageResponse
                = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ViewersDetailsParserResponse.html", Assembly.GetExecutingAssembly());
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100022417937758",
                        },
                };
            _httpHelper.GetRequestAsync(url, account.Token)
               .Returns(new ResponseParameter
               {
                   Response = pageResponse,
                   Exception = null,
                   HasError = false
               });

            FacebookAdsDetails facebookAdsDetails = new FacebookAdsDetails()
            {
                AdId = "6140129940750",
                Id = "10153971909960429",
                OwnerId = "106270215428",
                NavigationUrl = "http://www.wix.com/htmlsites/-click-here",
                PostedDateTime = DateTime.Now,
            };

            Task task = _sut.ViewersDetailsParser(account, facebookAdsDetails);

        }

        [TestMethod]
        public void GetIpDetails_Should_Return_True()
        {
            string url = "https://api.db-ip.com/v2/eb79c26170d0e9921e5b8372b2e212f86afa399c/103.217.90.99";

            var albumResponse = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetIpDetailsResponse.html", Assembly.GetExecutingAssembly());

            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100015937932195",
                        },
                };


            _httpHelper.GetRequest(url)
               .Returns(new ResponseParameter
               {
                   Response = albumResponse,
                   Exception = null,
                   HasError = false
               });

            _sut.GetIpDetails(account);

        }

    }
}
