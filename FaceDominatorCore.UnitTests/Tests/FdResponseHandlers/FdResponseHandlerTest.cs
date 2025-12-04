using System;
using System.Reflection;
using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FaceDominatorCore.FDRequest;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDResponse.CommonResponse;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDResponse.FriendsResponse;
using FaceDominatorCore.FDResponse.MessagesResponse;
using FaceDominatorCore.FDResponse.ScrapersResponse;
using FaceDominatorCore.FDResponse.LikeCommentsResponse;
using FaceDominatorCore.FDResponse.EventsResponse;
using FaceDominatorCore.FDResponse.AccountsResponse;
using System.Collections.Generic;
using FaceDominatorCore.FDResponse.GroupsResponse;
using FaceDominatorCore.FDResponse.InviterResponse;
using FaceDominatorCore.FDResponse;
using DominatorHouseCore.Utility;
using DominatorHouse.ThreadUtils;
using NSubstitute;
using Unity;

namespace FaceDominatorCore.UnitTests.Tests.FdResponseHandlers
{
    [TestClass]
    public class FdResponseHandlerTest : UnityInitializationTests
    {
        protected IDelayService DelayService;
        public FdResponseHandlerTest()
        {
            DelayService = Substitute.For<IDelayService>();
           // Container.RegisterInstance(DelayService);
        }

        [TestMethod]
        public void FdFriendsInfoNewResponseHandler_Should_Return_True()
        {
            // arrange
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

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };
            // act
            var result = new FdFriendsInfoNewResponseHandler(responseParameters, null, account.AccountBaseModel.UserId, false);

            // assert
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(18);
            result.Status.Should().Be(true);
            result.HasMoreResults.Should().Be(true);
            result.ObjFdScraperResponseParameters.FriendsPager.AjaxPipeToken.Should().Be("AXjR9StsVbhBmjFQ");


            var resultWithFriendsPager = new FdFriendsInfoNewResponseHandler(responseParameters, result.ObjFdScraperResponseParameters.FriendsPager, account.AccountBaseModel.UserId, false);

            resultWithFriendsPager.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(18);
            resultWithFriendsPager.Status.Should().Be(true);
            resultWithFriendsPager.HasMoreResults.Should().Be(true);
            resultWithFriendsPager.ObjFdScraperResponseParameters.FriendsPager.AjaxPipeToken.Should().Be("AXjR9StsVbhBmjFQ");
        }

        [TestMethod]
        public void FdFriendsInfoNewResponseHandler_Should_Return_False()
        {
            // arrange
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

            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };
            // act
            var result = new FdFriendsInfoNewResponseHandler
                (responseParameters, null, account.AccountBaseModel.UserId, false);

            // assert
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNull();
            result.Status.Should().BeFalse();
            result.HasMoreResults.Should().BeTrue();
            result.ObjFdScraperResponseParameters.FriendsPager.Should().BeNull();

            var resultWithFriendsPager = new FdFriendsInfoNewResponseHandler
                (responseParameters, result.ObjFdScraperResponseParameters.FriendsPager, account.AccountBaseModel.UserId, false);

            resultWithFriendsPager.ObjFdScraperResponseParameters.ListUser.Should().BeNull();
            resultWithFriendsPager.Status.Should().BeFalse();
            resultWithFriendsPager.HasMoreResults.Should().BeTrue();
            resultWithFriendsPager.ObjFdScraperResponseParameters.FriendsPager.Should().BeNull();

        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void FdFriendsInfoNewResponseHandler_should_throw_null_ref_exception_if_response_is_null()
        {
            // arrange
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

            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };
            // act
            var result = new FdFriendsInfoNewResponseHandler
                (responseParameters, null, account.AccountBaseModel.UserId, false);

            // assert
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            result.Status.Should().BeFalse();
            result.HasMoreResults.Should().BeTrue();
            result.ObjFdScraperResponseParameters.FriendsPager.Should().BeNull();

            var resultWithFriendsPager = new FdFriendsInfoNewResponseHandler
                (responseParameters, result.ObjFdScraperResponseParameters.FriendsPager, account.AccountBaseModel.UserId, false);

            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            result.Status.Should().BeFalse();
            result.HasMoreResults.Should().BeTrue();
            result.ObjFdScraperResponseParameters.FriendsPager.Should().BeNull();

            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            result = new FdFriendsInfoNewResponseHandler
                (responseParameters, null, account.AccountBaseModel.UserId, false);

            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            result.Status.Should().BeFalse();
            result.HasMoreResults.Should().BeTrue();
            result.ObjFdScraperResponseParameters.FriendsPager.Data.Should().Be("{\"year\":,\"month\":,\"log_filter\":\"friends\",\"profile_id\":100015937932195}");

        }

        [TestMethod]
        public void FdLoginResponseHandler_Should_Return_True()
        {
            // arrange
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };
            // act
            var result = new FdLoginResponseHandler(responseParameters, false,new DominatorAccountModel());

            // assert
            result.LoginStatus.Should().Be(true);
            result.FbDtsg.Should().Be("AQHXqUOhRq75%3AAQE6ecOPiL7M");
            result.UserId.Should().Be("100015937932195");

        }

        [TestMethod]
        public void FdLoginResponseHandler_Should_Return_False()
        {
            //arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };
            // act
            var result = new FdLoginResponseHandler(responseParameters, false,new DominatorAccountModel());

            // assert
            result.LoginStatus.Should().BeFalse();
            result.FbDtsg.Should().BeEmpty();
            result.UserId.Should().BeNull();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());

            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };
            //act
            result = new FdLoginResponseHandler(responseParameters, true,new DominatorAccountModel());

            // assert
            result.LoginStatus.Should().BeFalse();
            result.FbDtsg.Should().BeEmpty();
            result.UserId.Should().BeNull();
            result.FbErrorDetails.Status.Should().Be("LangKeyLoginFailed".FromResourceDictionary());
        }

        [TestMethod]
        public void FriendsUpdateResponseHandler_Should_Return_True()
        {
            // arrange
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateFriendsFromPageResponse.html", Assembly.GetExecutingAssembly());


            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };
            // act
            var result = new FriendsUpdateResponseHandler(responseParameters, false);

            // assert
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(442);
            result.HasMoreResults.Should().Be(true);
            result.FbErrorDetails.Should().BeNull();
            result.Status.Should().BeFalse();
        }

        [TestMethod]
        public void FriendsUpdateResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new FriendsUpdateResponseHandler(responseParameters, false);

            // assert
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNull();
            result.HasMoreResults.Should().Be(true);
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new FriendsUpdateResponseHandler(responseParameters, false);

            // assert
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            result.HasMoreResults.Should().Be(true);
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();

        }
        
        [TestMethod]
        public void LocationResponseHandler_Should_Return_True()
        {
            // arrange
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.LocationResponse.html", Assembly.GetExecutingAssembly());


            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };
            // act
            var result = new LocationResponseHandler(responseParameters);

            // assert
            result.LocationId.Should().Be("106377336067638");
        }

        [TestMethod]
        public void LocationResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new LocationResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.LocationId.Should().BeEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new LocationResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.LocationId.Should().BeEmpty();
        }

        [TestMethod]
        public void AdReactionListResponseHandler_Should_Return_True()
        {
            // arrange
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedAdsResponse.html", Assembly.GetExecutingAssembly());


            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };
            // act
            var result = new AdReactionListResponseHandler(responseParameters);

            // assert
            result.ListPostReaction.Count.Should().Be(1);
            result.ListReactionPermission.Count.Should().Be(1);
        }

        [TestMethod]
        public void AdReactionListResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new AdReactionListResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ListPostReaction.Count.Should().Be(0);
            result.ListReactionPermission.Count.Should().Be(0);

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new AdReactionListResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.ListPostReaction.Count.Should().Be(0);
            result.ListReactionPermission.Count.Should().Be(0);
        }

        [TestMethod]
        public void CheckJoiningStatusResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupJoiningStatusResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };
            var result = new CheckJoiningStatusResponseHandler(responseParameters);

            result.JoiningStatus.Should().Be("Join Group");
            result.IsMember.Should().Be(false);
            result.GroupId.Should().Be("1413952862244117");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CheckJoiningStatusResponseHandler_should_throw_null_ref_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new CheckJoiningStatusResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.GroupId.Should().BeNullOrEmpty();
            result.IsIncompleteSource.Should().BeFalse();
            result.IsMember.Should().BeFalse();
            result.JoiningStatus.Should().BeNullOrEmpty();
            result.IsQuestionsAnswered.Should().BeFalse();
            result.IsQuestionsAsked.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response =null,
                Exception = null
            };

            // act
            result = new CheckJoiningStatusResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.GroupId.Should().BeNullOrEmpty();
            result.IsIncompleteSource.Should().BeTrue();
            result.IsMember.Should().BeFalse();
            result.JoiningStatus.Should().BeNullOrEmpty();
            result.IsQuestionsAnswered.Should().BeFalse();
            result.IsQuestionsAsked.Should().BeFalse();
        }

        [TestMethod]
        public void FanpageLikersResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPageLikersResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new FanpageLikersResponseHandler(responseParameters, null);

            result.HasMoreResults.Should().Be(false);
            result.ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser.Count.Should().Be(12);

        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void FanpageLikersResponseHandler_should_throw_null_ref_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new FanpageLikersResponseHandler(responseParameters, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.FdPageLikersParameters.Should().BeNull();
            result.HasMoreResults.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new FanpageLikersResponseHandler(responseParameters, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.FdPageLikersParameters.LstFacebookUser.Count.Should().Be(0);
        }

        [TestMethod]
        public void FbUserIdResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FriendProfileResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new FbUserIdResponseHandler(responseParameters);

            result.FbDtsg.Should().Be("AQF_Ze0l6PEW%3AAQF6suEako8f");
            result.UserId.Should().Be("100016035780835");
            result.FbErrorDetails.Should().Be(null);
        }

        [TestMethod]
        public void FbUserIdResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new FbUserIdResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.UserId.Should().BeNullOrEmpty();
            result.FbDtsg.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new FbUserIdResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.UserId.Should().BeNullOrEmpty();
            result.FbDtsg.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void FdFriendOfFriendResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FriendOfFriendResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new FdFriendOfFriendResponseHandler
                (responseParameters, false, string.Empty);

            result.FbErrorDetails.Should().Be(null);
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(20);
            result.HasMoreResults.Should().Be(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void FdFriendOfFriendResponseHandler_should_throw_null_ref_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new FdFriendOfFriendResponseHandler(responseParameters, false, string.Empty);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().BeFalse();
            result.PageletData.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNull();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response =null,
                Exception = null
            };

            // act
            result = new FdFriendOfFriendResponseHandler(responseParameters, false, string.Empty);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().BeFalse();
            result.PageletData.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
        }

        [TestMethod]
        public void FdSearchPeopleResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.searchPeopleGraphResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new FdSearchPeopleResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(16);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void FdSearchPeopleResponseHandler_should_throw_null_ref_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new FdSearchPeopleResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.FinalEncodedQuery.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNull();
            result.PageletData.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new FdSearchPeopleResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.FinalEncodedQuery.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            result.PageletData.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void FdUserInfoResponseHandlerMobile_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetDetailedInfoUserMobileScraperAsyncResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new FdUserInfoResponseHandlerMobile(responseParameters, new FacebookUser());

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(false);
            result.ObjFdScraperResponseParameters.FacebookUser.UserId.Should().Be("100022417937758");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void FdUserInfoResponseHandlerMobile_should_throw_null_ref_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new FdUserInfoResponseHandlerMobile(responseParameters, new FacebookUser());

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNull();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new FdUserInfoResponseHandlerMobile(responseParameters, new FacebookUser());

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.FacebookUser.UserId.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void GetFanpageFullDetailsResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPageDetailsResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new GetFanpageFullDetailsResponseHandler(responseParameters);

            result.FbErrorDetails.Should().BeNull();
            result.ObjFacebookAdsDetails.OwnerId.Should().Be("106270215428");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GetFanpageFullDetailsResponseHandler_should_throw_null_ref_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new FdUserInfoResponseHandlerMobile(responseParameters, new FacebookUser());

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.FacebookUser.Should().BeNull();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new FdUserInfoResponseHandlerMobile(responseParameters, new FacebookUser());

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.FacebookUser.UserId.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void GetGroupTokenResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupTokenResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new GetGroupTokenResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Token.Should().Be("1547218804-7:1547184198");
        }

        [TestMethod]
        public void GetGroupTokenResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new GetGroupTokenResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.Token.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new GetGroupTokenResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.Token.Should().BeEmpty();
        }

        [TestMethod]
        public void GroupMembersResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupMemberCount.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };
            GroupMemberCategory groupMemberCategory = new GroupMemberCategory();

            var result = new GroupMembersResponseHandler
                (responseParameters, false, string.Empty, groupMemberCategory);

            result.FbErrorDetails.Should().Be(null);
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(19);
            result.Status.Should().Be(true);
            result.PageletData.Should().Be("AQHRkrFDz717ckYU3qBfYsubymfBj_Q3FkadzHZL6crwk2Z1fr343-pI9-o342rDnO5F6yh8CoBP4GKLPcjk9WPyxg");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void GroupMembersResponseHandler_should_throw_null_ref_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new GroupMembersResponseHandler
                (responseParameters, false, string.Empty, new GroupMemberCategory());

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.Should().BeNull();
            result.PageletData.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new GroupMembersResponseHandler
                (responseParameters, false, string.Empty, new GroupMemberCategory());

            //// assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.PageletData.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();
        }

        [TestMethod]
        public void MessageSenderResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
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

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new MessageSenderResponseHandler
                (responseParameters, account.AccountBaseModel.UserId, account.AccountBaseModel.AccountId);

            result.FbErrorDetails.Should().Be(null);
            result.HasMoreResults.Should().Be(true);
            result.ObjFdScraperResponseParameters.MessageSenderDetailsList.Count.Should().Be(20);
        }

        [TestMethod]
        public void MessageSenderResponseHandler_Should_Return_False()
        {
            //assert
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

            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new MessageSenderResponseHandler
                (responseParameters, account.AccountBaseModel.UserId, account.AccountBaseModel.AccountId);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().Be(true);

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new MessageSenderResponseHandler
                (responseParameters, account.AccountBaseModel.UserId, account.AccountBaseModel.AccountId);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().Be(true);

        }

        [TestMethod]
        public void NewsFeedPaginationResonseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.NewsFeedPagination.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new NewsFeedPaginationResonseHandler(responseParameters, false, "");

            result.FbErrorDetails.Should().Be(null);
            result.ObjFdScraperResponseParameters.FinalEncodedQuery.Should().Be
                ("{\"view\":\"list\",\"encoded_query\":\"Abr4FpLQm66-C9bk8isrq7ATzrongUAgbY5Jztep6B1sTZ-_n2G15eVqt9aq0hxc6Jut80Qkxx4zyeFTDWrZ6_LBfbBcsBaXvWseoL31DtmQL0sDnMmh4ZVElQzdkq5PwTjUwMqIMRDld_8RgVGTEJ5e9e5aPvUWkreCIy05N_x82iyXiTPGu8xfwNDN51R5oUNGvCAOh8Gophf01FQDlbdRxYVfdPEOx-cgjScZb_OutEsGpOy5H7AtDlwe8xYMYToNyOoSOH4BsttLWPSrL188jelb3I3HVEfuRljDsPIVMCtaI9d1FlfakwLdjkg41K5tdP9qUwamfoNSv1BtgEese5RL0j5GTPnzMddLDyU5zhZO925bU9nXJUUxtjbAB77h4suAKfSH2guwE8c_D5lGG7CzNZceT37X1KKiKpD1IhvCgviKvAT6oMWZxu2Qn78Hy8orP_a2fGvc76fTSNdH581yt-Bno4_f8DrEweFZStFj-bVBEhhrJQCKfv_rfPY6clcwXLxte8vW9KPh5qN4rNva7TQH6k3yTO24mDDRt3a1W5IPBZnMzN6l-mD1a55ckSm-fBGRfSHGo5Q7TOfQeMV2yBFAdKxStUJV2Mtz8ceRztaZOVO1odlzNTkKt9Etf-9H92zaoYy5wZnD1t04o41EDkQaamPSql_xzMyKABxnjtrgAsbAarNLmctHlNDVPDkDDv-PcvBHPLRV63IHNZWhSk5kbWnjKptwlwiGoMj1oprMCnY4LPJa-50qDP9yBoZ9xZz4c_U2iJwZR3Uln1ZdRBMNTIUbtp68rS7PRpiX0eMP558DR0sg94iRspfi1ueNjliumi4sS5TVbVHnhmoq5ZOVI-sPGZJnliiE7t4S_gvzckmeXNbKNQDELfI15Q3oVP2tuEubIvIN4puo76o1nsSDJopMqje8G1-0LpbUQhZNQR8mOuP_ERxvuBbqvDo5sMshyUN7nX6BNahVDCIWFe5NzSfe7uYrrDHotx67I39oE6s2gUDfLmyHxcmbkCdpaESYpXA6hWl9A5O48l3xJbZsXy61eb9vEs52DDLkvTdIYNtXOKvj3eZ4u2Yo5oHkos-gOAxGca8rKVrg_nJIy5LM9OAQqYVJUuPVCcZlvknj1Ds1s9tz66Fnp78Akv2dvTv9GjKoGavjtIWw\",\"encoded_title\":\"WyJwZW9wbGUraW4rcGVvcGxlK2luK0luZGlhIl0\",ref:\"unknown\",logger_source:\"www_main\",typeahead_sid:\"\",tl_log:false,impression_id:\"0DLF9Mim21uHb0W1j\",filter_ids:{\"100007674608397\":100007674608397,\"100013330032295\":100013330032295,\"100004163414716\":100004163414716,\"100007896494159\":100007896494159,\"100002380902439\":100002380902439,\"100011436197188\":100011436197188,\"100001684686844\":100001684686844},experience_type:\"grammar\",exclude_ids:null,browse_location:\"browse_location:browse\",trending_source:null,reaction_surface:null,reaction_session_id:null,ref_path:\"/search/people/\",is_trending:false,topic_id:null,place_id:null,story_id:null,callsite:\"browse_ui:init_result_set\",has_top_pagelet:true,display_params:{crct:\"none\"},\"cursor\":");
            result.PageletData.Should().Be
                ("{\"view\":\"list\",\"encoded_query\":\"Abr4FpLQm66-C9bk8isrq7ATzrongUAgbY5Jztep6B1sTZ-_n2G15eVqt9aq0hxc6Jut80Qkxx4zyeFTDWrZ6_LBfbBcsBaXvWseoL31DtmQL0sDnMmh4ZVElQzdkq5PwTjUwMqIMRDld_8RgVGTEJ5e9e5aPvUWkreCIy05N_x82iyXiTPGu8xfwNDN51R5oUNGvCAOh8Gophf01FQDlbdRxYVfdPEOx-cgjScZb_OutEsGpOy5H7AtDlwe8xYMYToNyOoSOH4BsttLWPSrL188jelb3I3HVEfuRljDsPIVMCtaI9d1FlfakwLdjkg41K5tdP9qUwamfoNSv1BtgEese5RL0j5GTPnzMddLDyU5zhZO925bU9nXJUUxtjbAB77h4suAKfSH2guwE8c_D5lGG7CzNZceT37X1KKiKpD1IhvCgviKvAT6oMWZxu2Qn78Hy8orP_a2fGvc76fTSNdH581yt-Bno4_f8DrEweFZStFj-bVBEhhrJQCKfv_rfPY6clcwXLxte8vW9KPh5qN4rNva7TQH6k3yTO24mDDRt3a1W5IPBZnMzN6l-mD1a55ckSm-fBGRfSHGo5Q7TOfQeMV2yBFAdKxStUJV2Mtz8ceRztaZOVO1odlzNTkKt9Etf-9H92zaoYy5wZnD1t04o41EDkQaamPSql_xzMyKABxnjtrgAsbAarNLmctHlNDVPDkDDv-PcvBHPLRV63IHNZWhSk5kbWnjKptwlwiGoMj1oprMCnY4LPJa-50qDP9yBoZ9xZz4c_U2iJwZR3Uln1ZdRBMNTIUbtp68rS7PRpiX0eMP558DR0sg94iRspfi1ueNjliumi4sS5TVbVHnhmoq5ZOVI-sPGZJnliiE7t4S_gvzckmeXNbKNQDELfI15Q3oVP2tuEubIvIN4puo76o1nsSDJopMqje8G1-0LpbUQhZNQR8mOuP_ERxvuBbqvDo5sMshyUN7nX6BNahVDCIWFe5NzSfe7uYrrDHotx67I39oE6s2gUDfLmyHxcmbkCdpaESYpXA6hWl9A5O48l3xJbZsXy61eb9vEs52DDLkvTdIYNtXOKvj3eZ4u2Yo5oHkos-gOAxGca8rKVrg_nJIy5LM9OAQqYVJUuPVCcZlvknj1Ds1s9tz66Fnp78Akv2dvTv9GjKoGavjtIWw\",\"encoded_title\":\"WyJwZW9wbGUraW4rcGVvcGxlK2luK0luZGlhIl0\",ref:\"unknown\",logger_source:\"www_main\",typeahead_sid:\"\",tl_log:false,impression_id:\"0DLF9Mim21uHb0W1j\",filter_ids:{\"100007674608397\":100007674608397,\"100013330032295\":100013330032295,\"100004163414716\":100004163414716,\"100007896494159\":100007896494159,\"100002380902439\":100002380902439,\"100011436197188\":100011436197188,\"100001684686844\":100001684686844},experience_type:\"grammar\",exclude_ids:null,browse_location:\"browse_location:browse\",trending_source:null,reaction_surface:null,reaction_session_id:null,ref_path:\"/search/people/\",is_trending:false,topic_id:null,place_id:null,story_id:null,callsite:\"browse_ui:init_result_set\",has_top_pagelet:true,display_params:{crct:\"none\"},\"cursor\":\"AbqIYx8Hy1jI5f53cSOhJkA_DJ5m7o3erdh_mUbJW6_msDkG9JVtJ1gn1t0JxYwJpDngBtjabm-YB3fT159_UN78xiaxa5vv2_fCAmhX-KQoDPCKFFVokG6YpXIKONClealMiO74jgyrWFmaZ53PIZISC8XOmDJQhBeygOkKF1cXleTQrSvja3AwwEC9cdeSKMTUvPcrDTwIdlUUhgLtM7H2Qr5LO79pN4T5TyaZ8OWgvYct7l3xVj8Yk_mQHccS_v_WHwYSU3vOn4kxs6slFUI8c-520aOOumYuf0Ldqr0YQs9faFGywT06lDCYF65JSkYVyNkNn8ZzHqXSom0qq3oMmNdcC0vFbHynvG4d2OMtSpLTQK--DfnhxeI0btgeB4M_me8nDCcqxcAG9q05K8WixJTOKmcJgr1wd8W7ykk6OIi7eO0Cf0lEhYRxNYBfU9nZ37fbwEZgI1R21A1vRHFOWQz3lEevWgBvLaDTAQ5l8YMh4i5PStdvn4p_jWIskJMOBSp8SawJKt4O5yV2TCNLLQ73ds2jxWgdxBCn_Kmlqj6Gkb1mSGzy6d61vothMHBTXLOt762MCZ_orqDwzama47ygxK8zi8C52tpduGn_T2QhsVsL8nxRUK5ZcQUJAszANWP5SnPLdi1lldTV469OjpGix296Mys9ST3vd2OaT6kG9ZG_FhPL1hI5iXpDUrpYBhwz-czAXGhq-Ebq9ZzJoIZK-hTxX0z6RgBkdYEaBKsZjXcfHFRHtoL16mikWBST8EsZ31k2rL4mt2rX-v5Jcqwb9Ly-JhePm1vplV7l4uw0fTDYjIsKoonLFIy40u6mVqW0dFkavfYlvUUheLKB63V-Nz5A-w9hG_gGxi5splkjF-oYmZzC-cpB8wzEq7qFgTCqBkDAGFNaSLfiKhJbRFtcBOReDAho0II5vlu3F9xYIgfo7qIW2ElmmIs_pxZp4H9pRvlGwGvWJMY5jM6y-HYGf2bHubvUQNvMRnTQTLZJjLHysqpLNA65ygiI5-ZdIEwsVj57lKhEVqOpUJ6k5rQ1BjiDEaBspPqsfLB0b4AtlIy7iP3xRJkkvhzFTIxU1XaBK7Gq5PwXsQM31mjjmno3Z-ykPjktPTRy1HU4xKYspc0vAA206wsk4GZJR8XD5BlVWMelxMgI66Jxdt7buCSCnV4qDtjmwGmT2rP0ps-T-lpEKwGNkUzYBxeOtwoz0A4mahET8ehlKivTN4-aYimZ9RRsBuQplP_TnqUtiYSF77TvEoWm7GYPx-iwoT1aA_1qpKZjjl3ia_0ZE8GVPh78ZQ0f66mZzx-Fcoy22RmdxOnB_svoaGf6EHUoHnHez4SNy8tt6XozMDQoGgkHK2PnJAeQAg88oOvtWc1O7DbbU3HJ4mKZDGxsEzfh41I499R9n9wx14RwMXk9srenwuObFuKBruO9Db9Eu5TZ3HI-yiRXDBtklL8o1zCascL-blJuj87EewtZlM7ErAhZc9Rz3vljGno-F3O6LEjwCrL8wHLfTfkO8RBxygIJNQg4QOKFKNr4b_qG5hiMWaV8EFE6wLOJwmzjujk8xnUcnPM-7KH014L6FYQv3HdZnXmrbJNzRScPW7jPKu2M17BUFhyKgwyABh7yfONH6T_RC2_CndPs9axRJ5n6YOYPDd27c9uGNROYebQ76H0V51XSZ7iIZqUpdBn30-oWXvt_V7iRg-0DqJmzrHljwTW2lwGi0yQB0S4LVBE5E9Cbv3gmU0ZXsRvRFvZeEqJGRuIUUK5RN5Ob441bbMADrCwqAHDy0VuMzdCyVesXG5iD12zotQD9Kn1_wPdswLuy0rmiJVmr7kvjVBeuRgxBQy-QIyvVJ4G1k9is_XBM_yEMTN-KeZDq5Ri78cQP8rqNjmm_Alew_qvWxtM9a-df5RqQviBbChHoLcwqS7OMQVU3x_ndNRjzWDd9uvtBcHlmrKGWvuRh8L35xmmmkla5CE-k80BFD9sBSuHka2mwBskUasjIewWO08KKtuegySN2wjWWp2a6FjBqlhdUN7iGe8h2-wlQE_ls8f0Yn-BQHRCK-PA70tyukn8IeGsZvsYd4H96NJYp4EjuY1v67UtM72DfGeTZoPvbnjRgrR71NYl4uMWK\",\"page_number\":2,\"em\":false,\"tr\":null}");
        }

        [TestMethod]
        public void NewsFeedPaginationResonseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new NewsFeedPaginationResonseHandler(responseParameters, false, "");

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.FinalEncodedQuery.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.IsPagination.Should().Be(false);
            result.PageletData.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new NewsFeedPaginationResonseHandler(responseParameters, false, "");

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.FinalEncodedQuery.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.IsPagination.Should().Be(false);
            result.PageletData.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void PostCommentorResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new PostCommentorResponseHandler
                (responseParameters, true, "2056455264441101", "", true);

            result.FbErrorDetails.Should().Be(null);
            result.ObjFdScraperResponseParameters.CommentList.Count.Should().Be(50);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.CommentCount.Should().Be(0);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PostCommentorResponseHandler_should_throw_null_Arg_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new PostCommentorResponseHandler
                (responseParameters, true, "2056455264441101", "", true);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.CommentList.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.CommentCount.Should().Be(0);

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new PostCommentorResponseHandler(responseParameters, true, "2056455264441101", "", true);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.CommentList.Count.Should().Be(0);
            result.ObjFdScraperResponseParameters.CommentCount.Should().Be(0);
        }

        [TestMethod]
        public void PostIdResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new PostIdResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.PostId.Should().Be("2056455264441101");
        }

        [TestMethod]
        public void PostIdResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new PostIdResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.PostId.Should().BeEmpty();
            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new PostIdResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.PostId.Should().Be("0");
        }

        [TestMethod]
        public void PostLikersResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostLikersRespose.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new PostLikersResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(50);
            result.Status.Should().Be(true);
            result.HasMoreResults.Should().Be(true);
            result.ObjFdScraperResponseParameters.ShownIds.Should().Be("100002299426074,100033494253335,100033383948654,100033232071570,100033217633804,100033108513660,100033044979106,100032874007793,100032428980412,100032403291552,100031988799868,100031863578682,100031299109081,100031244192186,100030950714560,100030530745689,100030300170713,100029687777356,100029586680373,100029542561313,100029477924038,100029329419902,100029270683133,100029242932288,100029239816250,100029197699822,100028907320525,100028703232833,100028660808346,100028449952337,100028347761555,100028202206616,100027805566196,100027407618887,100027293827571,100027263929824,100026773351208,100026510534533,100025536410185,100025532091162,100025466769228,100025449274234,100025402428022,100025300008145,100025137036717,100024870350972,100024821716111,100024628327768,100024596273895,100024442119113");
            result.ObjFdScraperResponseParameters.TotalCount.Should().Be("169");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PostLikersResponseHandler_should_throw_null_ref_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new PostLikersResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNull();
            result.ObjFdScraperResponseParameters.ShownIds.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.TotalCount.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new PostLikersResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNull();
            result.ObjFdScraperResponseParameters.ShownIds.Should().BeEmpty();
            result.ObjFdScraperResponseParameters.TotalCount.Length.Should().Be(1);

        }

        [TestMethod]
        public void PostReactionListResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new PostReactionListResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.ListPostReaction.Count.Should().Be(2);
            result.ListReactionPermission.Count.Should().Be(2);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PostReactionListResponseHandler_should_throw_null_ref_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new PostReactionListResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ListPostReaction.Count.Should().Be(0);
            result.ListReactionPermission.Count.Should().Be(0);

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new PostReactionListResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.ListPostReaction.Count.Should().Be(0);
            result.ListReactionPermission.Count.Should().Be(0);
        }

        [TestMethod]
        public void PostScraperForKeywordResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromKeyWordsResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var postReactionListResult = new PostReactionListResponseHandler(responseParameters);

            var result = new PostScraperForKeywordResponseHandler
                (responseParameters, new FdScraperResponseParameters(), postReactionListResult.ListPostReaction);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.AjaxToken.Should().Be("");
            result.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(5);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PostScraperForKeywordResponseHandler_should_throw_null_ref_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var postReactionListResult = new PostReactionListResponseHandler(responseParameters);
            var result = new PostScraperForKeywordResponseHandler
                (responseParameters, new FdScraperResponseParameters(), postReactionListResult.ListPostReaction);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.EntityId.Should().BeNullOrEmpty();
            result.PageletData.Should().BeNull();
            result.Status.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new PostScraperForKeywordResponseHandler
                (responseParameters, new FdScraperResponseParameters(), postReactionListResult.ListPostReaction);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.EntityId.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.Should().BeNull();
            result.PageletData.Should().BeNull();
            result.Status.Should().BeFalse();
        }

        [TestMethod]
        public void PostScraperResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostDetailsResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            FacebookPostDetails facebookPostDetails = new FacebookPostDetails()
            {
                Id = "10205581200565265",
            };

            var result = new PostScraperResponseHandler(responseParameters, facebookPostDetails);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.PostDetails.Id.Should().Be("10205581200565265");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PostScraperResponseHandler_should_throw_null_arg_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new PostScraperResponseHandler(responseParameters, new FacebookPostDetails
            { Id = "10205581200565265", });

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.Status.Should().BeFalse();
            result.ObjFdScraperResponseParameters.PostDetails.Should().BeNull();
            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new PostScraperResponseHandler(responseParameters, new FacebookPostDetails
            { Id = "10205581200565265", });

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.Status.Should().BeFalse();
            result.ObjFdScraperResponseParameters.PostDetails.Should().BeNull();
        }

        [TestMethod]
        public void PostSharerResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostSharerResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new PostSharerResponseHandler(responseParameters, false);

            result.FbErrorDetails.Should().Be(null);
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(1);
            result.PageletData.Should().Be
                ("{\"target_fbid\":2056455264441101,\"cursor\":\"AQHRNjNszDBezLme0XY0osoPFzukmXmKcwmnKjIJqJC624-gNtiuEHo2t-7sjfhefxIAhv4bnJOibYdZlELLfPH_fw\"}");
            result.Status.Should().Be(true);
            result.HasMoreResults.Should().Be(true);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void PostSharerResponseHandler_should_throw_null_arg_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new PostSharerResponseHandler(responseParameters, false);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNull();
            result.PageletData.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new PostSharerResponseHandler(responseParameters, false);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            result.PageletData.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();

        }

        [TestMethod]
        public void ReactionCountResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetCommentResponseforVideosResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };
            string postId = "";
            var result = new ReactionCountResponseHandler(responseParameters, ref postId);

            result.FbErrorDetails.Should().Be(null);
            result.CommentCount.Should().Be(40);
            result.LikeCount.Should().Be(384);
            result.ShareCount.Should().Be(161);
            result.Offset.Should().Be(0);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ReactionCountResponseHandler_should_throw_null_arg_exception_if_response_is_null()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            string postId = "";
            var result = new ReactionCountResponseHandler(responseParameters, ref postId);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.CommentCount.Should().Be(0);
            result.LikeCount.Should().Be(0);
            result.Offset.Should().Be(0);
            result.ShareCount.Should().Be(0);

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = null,
                Exception = null
            };

            // act
            result = new ReactionCountResponseHandler(responseParameters, ref postId);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.CommentCount.Should().Be(0);
            result.LikeCount.Should().Be(0);
            result.Offset.Should().Be(0);
            result.ShareCount.Should().Be(0);
        }

        [TestMethod]
        public void ScrapGroupPostListResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromGroupsNewResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };
            var listOfKeyValuePair =
                new PostReactionListResponseHandler(responseParameters).ListPostReaction;

            var result = new ScrapGroupPostListResponseHandler(responseParameters, listOfKeyValuePair, null);

            result.FbErrorDetails.Should().Be(null);
            result.ObjFdScraperResponseParameters.AjaxToken.Should().Be("AXgcWo3hWJZrLSf5");
            result.HasMoreResults.Should().Be(true);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(11);
            result.PageletData.Should().Be
                ("{\"last_view_time\":1550502926,\"is_first_story_seen\":true,\"postIdentifiersToFilter\":[\"MjUxNTA0ODYxODUyMzAxOTo2OjA=\",\"MjUxNDcyOTAyODU1NDk3ODo2OjA=\",\"MjUxNDE4MjM4NTI3NjMwOTo2OjA=\",\"MjUxNDk3OTgzODUyOTg5Nzo2OjA=\",\"MjUxMTk1MTE5ODgzMjc2MTo2OjA=\",\"MjUxMTg3NjY2ODg0MDIxNDo2OjA=\",\"MjUxNDM1NTE2MTkyNTY5ODo2OjA=\",\"MjUxNDk5NTc1NTE5NDk3Mjo2OjA=\",\"MjUxMTMwNDc2NTU2NDA3MTo2OjA=\",\"MjUxNTAzNTU2MTg1NzY1ODo2OjA=\",\"MjUxNDg5NzI3MTg3MTQ4Nzo2OjA=\"],\"story_index\":11,\"end_cursor\":\"MDoyNTE0ODk3MjcxODcxNDg3OjI1MTQ4OTcyNzE4NzE0ODcsMDpVM1Z3WlhKSGNtOTFjRTFoYkd4VGRISmxZVzFGYm5SUmRXVnllVnh1TVROY2JrMVVWVEZOUkZWM1RYcEJNVTlVYjNoT1ZGVjNUbFJCZWsxRVZUVlBha1Y0VDJrd2VFNVVSVE5OYW1zMVQxUlpNazFVVFhwTmVtTTBUMFJCTVU5cVFUWk9hbGt4VDFSTk1VOVVhekJOVkUwMFQxUlpORTlVVFRKTlp6MDk6S3c9PQ==\",\"group_id\":164122973615607,\"has_cards\":true,\"multi_permalinks\":[],\"posts_visible\":9,\"sorting_setting\":null}");
        }

        [TestMethod]
        public void ScrapGroupPostListResponseHandler_Should_Return_False()
        {

            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            List<KeyValuePair<string, string>> listOfKeyValuePair =
                new PostReactionListResponseHandler(responseParameters).ListPostReaction;
            var result = new ScrapGroupPostListResponseHandler(responseParameters, listOfKeyValuePair, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.AjaxToken.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ListPostDetails.Should().BeNull();
            result.PageletData.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new ScrapGroupPostListResponseHandler(responseParameters, listOfKeyValuePair, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.AjaxToken.Should().BeEmpty();
            result.HasMoreResults.Should().BeTrue();
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNullOrEmpty();
            result.PageletData.Should().Be("{\"}");
        }

        [TestMethod]
        public void ScrapNewPostListFromNewsFeedResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedAdsResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            List<KeyValuePair<string, string>> listPostReaction
                = new AdReactionListResponseHandler(responseParameters).ListPostReaction;

            var result = new ScrapNewPostListFromNewsFeedResponseHandler(responseParameters, null, listPostReaction);

            result.FbErrorDetails.Should().Be(null);
            result.HasMoreResults.Should().Be(true);
            result.Status.Should().Be(true);
            result.ObjPajination.AjaxToken.Should().Be("AXgu_IkiklOI7DAx");
            result.ObjPajination.SectionId.Should().Be(408239535924329);
            result.ObjPajination.Pagelet.Should().Be
                ("{\"client_stories_count\":2,\"cursor\":\"MTU1MDU4NTYzMzoxNTUwNTg1NjMzOjE6NDA2MDgzOTE3MDMzMDMzMzg4OTowOjY2NTk3MTQ1ODkwMzQ2NTU2Mzc=\",\"feed_stream_id\":80846080,\"pager_config\":\"{\\\"edge\\\":null,\\\"source_id\\\":null,\\\"section_id\\\":\\\"408239535924329\\\",\\\"pause_at\\\":null,\\\"stream_id\\\":null,\\\"section_type\\\":1,\\\"sizes\\\":null,\\\"most_recent\\\":false,\\\"ranking_model\\\":null,\\\"query_context\\\":[]}\",\"scroll_count\":1,\"story_id\":null}");
        }

        [TestMethod]
        public void ScrapNewPostListFromNewsFeedResponseHandler_Should_Return_False()
        {

            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            List<KeyValuePair<string, string>> listPostReaction
                = new AdReactionListResponseHandler(responseParameters).ListPostReaction;
            var result = new ScrapNewPostListFromNewsFeedResponseHandler(responseParameters, null, listPostReaction);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.FetchStream.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();
            result.ListFacebookAdsDetails.Should().BeNull();
            result.ObjPajination.Should().BeNull();
            result.ObjPajination.Should().BeNull();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            listPostReaction
                = new AdReactionListResponseHandler(responseParameters).ListPostReaction;
            result = new ScrapNewPostListFromNewsFeedResponseHandler(responseParameters, null, listPostReaction);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.FetchStream.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();
            result.ListFacebookAdsDetails.Count.Should().Be(0);
            result.ObjPajination.AjaxToken.Should().BeNull();

        }

        [TestMethod]
        public void ScrapPostFromAlbumsResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromAlbumsResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new ScrapPostFromAlbumsResponseHandler(responseParameters, string.Empty);

            result.FbErrorDetails.Should().Be(null);
            result.HasMoreResults.Should().Be(false);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(8);
            result.ObjFdScraperResponseParameters.AjaxToken.Should().Be("AQFxCLnribtG:AQEPGBbw6NOp");

        }

        [TestMethod]
        public void ScrapPostFromAlbumsResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new ScrapPostFromAlbumsResponseHandler(responseParameters, string.Empty);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.EntityId.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.AjaxToken.Should().BeNullOrEmpty();
            result.Status.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new ScrapPostFromAlbumsResponseHandler(responseParameters, string.Empty);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.EntityId.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.AjaxToken.Should().BeNullOrEmpty();
            result.Status.Should().BeFalse();

        }

        [TestMethod]
        public void ScrapPostListFromFanpageResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromFanpagesResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            List<KeyValuePair<string, string>> listPostReaction
                = new PostReactionListResponseHandler(responseParameters).ListPostReaction;

            var result = new ScrapPostListFromFanpageResponseHandler(responseParameters, listPostReaction);

            result.FbErrorDetails.Should().Be(null);
            result.HasMoreResults.Should().Be(true);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(1);
            result.PageletData.Should().Be
                ("{\"timeline_cursor\":\"timeline_unit:1:00000000001546688195:04611686018427387904:09223372036854775807:04611686018427387904\",\"timeline_section_cursor\":{\"profile_id\":339142240259836,\"start\":0,\"end\":1551427199,\"query_type\":36,\"filter\":1},\"has_next_page\":true}");
        }

        [TestMethod]
        public void ScrapPostListFromFanpageResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            List<KeyValuePair<string, string>> listPostReaction
                = new PostReactionListResponseHandler(responseParameters).ListPostReaction;
            var result = new ScrapPostListFromFanpageResponseHandler(responseParameters, listPostReaction);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ListPostDetails.Should().BeNullOrEmpty();
            result.PageletData.Should().BeNull();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new ScrapPostListFromFanpageResponseHandler(responseParameters, listPostReaction);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(0);
            result.PageletData.Should().BeNull();
        }

        [TestMethod]
        public void ScrapPostListFromTimelineResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromFanpagesResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            List<KeyValuePair<string, string>> listPostReaction
                = new PostReactionListResponseHandler(responseParameters).ListPostReaction;

            var result = new ScrapPostListFromTimelineResponseHandler(responseParameters, string.Empty, listPostReaction);

            result.FbErrorDetails.Should().Be(null);
            result.HasMoreResults.Should().Be(false);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(1);
            result.ObjFdScraperResponseParameters.ListPostReaction.Count.Should().Be(2);
        }

        [TestMethod]
        public void ScrapPostListFromTimelineResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            List<KeyValuePair<string, string>> listPostReaction
                = new PostReactionListResponseHandler(responseParameters).ListPostReaction;
            var result = new ScrapPostListFromTimelineResponseHandler(responseParameters, string.Empty, listPostReaction);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.EntityId.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.AjaxToken.Should().BeEmpty();
            result.Status.Should().BeFalse();
            result.PageletData.Should().BeNull();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new ScrapPostListFromTimelineResponseHandler(responseParameters, string.Empty, listPostReaction);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.EntityId.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.AjaxToken.Should().BeEmpty();
            result.Status.Should().BeFalse();
            result.PageletData.Should().BeNull();
        }

        [TestMethod]
        public void SearchOwnPageResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateOwnPagesAsyncResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new SearchOwnPageResponseHandler(responseParameters, null);

            result.FbErrorDetails.Should().Be(null);
            result.HasMoreResults.Should().Be(false);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListPage.Count.Should().Be(2);
        }

        [TestMethod]
        public void SearchOwnPageResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new SearchOwnPageResponseHandler(responseParameters, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.ListPage.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.FinalEncodedQuery.Should().BeNullOrEmpty();
            result.PageletData.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new SearchOwnPageResponseHandler(responseParameters, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.ListPage.Count.Should().Be(0);
            result.ObjFdScraperResponseParameters.FinalEncodedQuery.Should().BeNullOrEmpty();
            result.PageletData.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();

        }

        [TestMethod]
        public void UserMessageResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetRecentMessageDetailsResponse.html", Assembly.GetExecutingAssembly());

            var account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       {
                           AccountNetwork = SocialNetworks.Facebook,
                           UserId = "100015937932195",
                           AccountId = "b40296f6-1c62-40a8-b62e-27f691a18517",
                       },
               };

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new UserMessageResponseHandler
                (responseParameters, account.AccountBaseModel.UserId, account.AccountBaseModel.AccountId, "Balaram Balaram");

            result.FbErrorDetails.Should().Be(null);
            result.HasMoreResults.Should().Be(true);
            result.UserId.Should().Be("100015937932195");
        }

        [TestMethod]
        public void UserMessageResponseHandler_Should_Return_False()
        {
            // arrange
            var account =
        new DominatorAccountModel
        {
            AccountBaseModel =
                new DominatorAccountBaseModel
                {
                    AccountNetwork = SocialNetworks.Facebook,
                    UserId = "100015937932195",
                    AccountId = "b40296f6-1c62-40a8-b62e-27f691a18517",
                }
        };

            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new UserMessageResponseHandler
                (responseParameters, account.AccountBaseModel.UserId, account.AccountBaseModel.AccountId, "Balaram Balaram");

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new UserMessageResponseHandler
                (responseParameters, account.AccountBaseModel.UserId, account.AccountBaseModel.AccountId, "Balaram Balaram");

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();

        }

        [TestMethod]
        public void EventDetailsResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.eventResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new EventDetailsResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.FdEvents.EventId.Should().Be("572476599891934");
            result.FdEvents.EventName.Should().Be("TechDX");
        }

        [TestMethod]
        public void EventDetailsResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new EventDetailsResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.FdEvents.EventId.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new EventDetailsResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.FdEvents.EventId.Should().Be("0");
        }

        [TestMethod]
        public void AcceptFriendRequestResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.acceptFriendRequestResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new AcceptFriendRequestResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.IsAcceptedRequest.Should().Be(true);
        }

        [TestMethod]
        public void AcceptFriendRequestResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new AcceptFriendRequestResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.IsAcceptedRequest.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new AcceptFriendRequestResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.IsAcceptedRequest.Should().BeFalse();
        }

        [TestMethod]
        public void CancelIncomingRequestResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.CancelIncomingRequestRespose.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new CancelIncomingRequestResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.IsCancelledRequest.Should().Be(true);
        }

        [TestMethod]
        public void CancelIncomingRequestResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new CancelIncomingRequestResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.IsCancelledRequest.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new CancelIncomingRequestResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.IsCancelledRequest.Should().BeFalse();
        }

        [TestMethod]
        public void CancelSentRequestResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.cancelRequestResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new CancelSentRequestResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.IsCancelledRequest.Should().Be(true);
        }

        [TestMethod]
        public void CancelSentRequestResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new CancelSentRequestResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.IsCancelledRequest.Should().BeFalse();
            result.Error.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new CancelSentRequestResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.IsCancelledRequest.Should().BeTrue();
            result.Error.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void FdUserBirthdayResponseHandlerMobile_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetUsersBirtdayResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new FdUserBirthdayResponseHandlerMobile(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.HasMoreResults.Should().Be(false);
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(10);
            result.PageletData.Should().Be("2019-03-01");
        }

        [TestMethod]
        public void FdUserBirthdayResponseHandlerMobile_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new FdUserBirthdayResponseHandlerMobile(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            result.PageletData.Should().BeNull();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new FdUserBirthdayResponseHandlerMobile(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            result.PageletData.Should().BeEmpty();
        }

        [TestMethod]
        public void FriendSuggestedByAFriendResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetFriendSuggestedByFacebookRespose.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new FriendSuggestedByAFriendResponseHandler(responseParameters, null);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.HasMoreResults.Should().Be(false);
            result.ObjFdScraperResponseParameters.ExtraData.Should().Be("AQKY-zSnG9PHUxdzLT8inEiQKWRp54SG8ADVYt4_HkE0kzhJBGrE3U1RBWvllh-6PNU");
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(10);
            result.ObjFdScraperResponseParameters.PaginationUserIds.Count.Should().Be(10);
        }

        [TestMethod]
        public void FriendSuggestedByAFriendResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new FriendSuggestedByAFriendResponseHandler(responseParameters, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ExtraData.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.PaginationUserIds.Should().BeNull();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new FriendSuggestedByAFriendResponseHandler(responseParameters, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ExtraData.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            result.ObjFdScraperResponseParameters.PaginationUserIds.Count.Should().Be(0);
        }

        [TestMethod]
        public void IncomingFriendListResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.IncomingFriendRequestResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new IncomingFriendListResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.EndingId.Should().Be("100007873207174");
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(50);
            result.ObjFdScraperResponseParameters.SeenTimeStamp.Should().Be("1491499387");
        }

        [TestMethod]
        public void IncomingFriendListResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new IncomingFriendListResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.EndingId.Should().BeNull();
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.SeenTimeStamp.Should().BeNull();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new IncomingFriendListResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.EndingId.Should().BeNull();
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            result.ObjFdScraperResponseParameters.SeenTimeStamp.Should().BeNull();
        }

        [TestMethod]
        public void MutualFriendsResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.IncomingMessageResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new MutualFriendsResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(false);

        }

        [TestMethod]
        public void MutualFriendsResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new MutualFriendsResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNullOrEmpty();
            result.HasMoreResults.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new MutualFriendsResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
            result.HasMoreResults.Should().BeFalse();

        }

        [TestMethod]
        public void SendRequestResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendFriendRequestRespose.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new SendRequestResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.RequestStatus.Should().Be("success");
        }

        [TestMethod]
        public void SendRequestResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new SendRequestResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.RequestStatus.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new SendRequestResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.RequestStatus.Should().BeNullOrEmpty();

        }

        [TestMethod]
        public void SentFriendRequestListNewResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetSentFriendRequestIdsNewRespose.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new SentFriendRequestListNewResponseHandler(responseParameters, new FriendsPager(), "100022404889387", false);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(false);
            result.ObjFdScraperResponseParameters.FriendsPager.AjaxPipeToken.Should().Be("AXjWRkk5zPUOU-QW");
        }

        [TestMethod]
        public void SentFriendRequestListNewResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new SentFriendRequestListNewResponseHandler
                (responseParameters, new FriendsPager(), "100022404889387", false);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().BeTrue();
            result.ObjFdScraperResponseParameters.FriendsPager.Should().BeNull();
            result.ObjFdScraperResponseParameters.ListUser.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new SentFriendRequestListNewResponseHandler
                (responseParameters, new FriendsPager(), "100022404889387", false);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().BeTrue();
            result.ObjFdScraperResponseParameters.FriendsPager.AjaxPipeToken.Should().BeEmpty();
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(0);
        }

        [TestMethod]
        public void GetFriendsJoiningStatusResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupFriendMemberResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new GetFriendsJoiningStatusResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.memberList.Count.Should().Be(11);
        }

        [TestMethod]
        public void GetFriendsJoiningStatusResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new GetFriendsJoiningStatusResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.memberList.Count.Should().Be(0);

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new GetFriendsJoiningStatusResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.memberList.Count.Should().Be(0);
        }

        [TestMethod]
        public void GroupJoinerResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GroupJoiningStatusPostResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new GroupJoinerResponseHandler(responseParameters, false);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(false);
            result.IsRequestSent.Should().Be(true);
        }

        [TestMethod]
        public void GroupJoinerResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new GroupJoinerResponseHandler(responseParameters, false);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.IsRequestSent.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new GroupJoinerResponseHandler(responseParameters, false);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.IsRequestSent.Should().BeFalse();
        }

        [TestMethod]
        public void UnjoinGroupResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                 ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UnjoinGroupResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new UnjoinGroupResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.IsUnjoinedGroup.Should().Be(true);
        }

        [TestMethod]
        public void UnjoinGroupResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new UnjoinGroupResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.IsUnjoinedGroup.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new UnjoinGroupResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.IsUnjoinedGroup.Should().BeFalse();
        }

        [TestMethod]
        public void GroupInviterResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendGroupInvittationTofriendsResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new GroupInviterResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.FailedReason.Should().Be(null);
        }

        [TestMethod]
        public void GroupInviterResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new GroupInviterResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.FailedReason.Should().BeNullOrEmpty();
            result.Status.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new GroupInviterResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.FailedReason.Should().BeNullOrEmpty();
            result.Status.Should().BeFalse();
        }

        [TestMethod]
        public void PageInviterResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendPageInvittationTofriendsResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new PageInviterResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
        }

        [TestMethod]
        public void PageInviterResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new PageInviterResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.Status.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new PageInviterResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.Status.Should().BeFalse();
        }

        [TestMethod]
        public void CommentOnPostResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ReplyOnPostResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new CommentOnPostResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.IsCommentedOnPost.Should().Be(true);
            result.ObjFdScraperResponseParameters.CommentId.Should().Be("1992458774214531");
            result.ObjFdScraperResponseParameters.IsBlocked.Should().Be(false);
        }

        [TestMethod]
        public void CommentOnPostResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new CommentOnPostResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.CommentId.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.IsBlocked.Should().BeFalse();
            result.ObjFdScraperResponseParameters.IsCommentedOnPost.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new CommentOnPostResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.CommentId.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.IsBlocked.Should().BeFalse();
            result.ObjFdScraperResponseParameters.IsCommentedOnPost.Should().BeFalse();

        }

        [TestMethod]
        public void LikeFanpageResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ReplyOnPostResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new LikeFanpageResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.Error.Should().Be("");
        }

        [TestMethod]
        public void LikeFanpageResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new LikeFanpageResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.Error.Should().BeNullOrEmpty();
            result.Status.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new LikeFanpageResponseHandler(responseParameters);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.Error.Should().BeNullOrEmpty();
            result.Status.Should().BeFalse();
        }

        [TestMethod]
        public void ScrapGroupPostListResponseHandlerNew_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromGroupsNewResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            List<KeyValuePair<string, string>> listOfKeyValuePair = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> objKeyvaluePair = new KeyValuePair<string, string>("1651012428259980", "752<:>2723<:>95");
            listOfKeyValuePair.Add(objKeyvaluePair);

            objKeyvaluePair = new KeyValuePair<string, string>("2515048618523019", "0<:>7<:>1");
            listOfKeyValuePair.Add(objKeyvaluePair);

            objKeyvaluePair = new KeyValuePair<string, string>("2514729028554978", "7<:>46<:>12");
            listOfKeyValuePair.Add(objKeyvaluePair);

            objKeyvaluePair = new KeyValuePair<string, string>("2514182385276309", "13<:>134<:>31");
            listOfKeyValuePair.Add(objKeyvaluePair);

            var result = new ScrapGroupPostListResponseHandlerNew(responseParameters, listOfKeyValuePair, null);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.HasMoreResults.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListPostDetails.Count.Should().Be(11);
            result.ObjFdScraperResponseParameters.ListPostReaction.Count.Should().Be(4);
            result.ObjFdScraperResponseParameters.AjaxToken.Should().Be("AXgcWo3hWJZrLSf5");
        }

        [TestMethod]
        public void ScrapGroupPostListResponseHandlerNew_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new ScrapGroupPostListResponseHandlerNew
                (responseParameters, new List<KeyValuePair<string, string>>(), null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.AjaxToken.Should().BeNullOrEmpty();
            result.Status.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new ScrapGroupPostListResponseHandlerNew(responseParameters, null, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().BeTrue();
            result.ObjFdScraperResponseParameters.AjaxToken.Should().BeNullOrEmpty();
            result.Status.Should().BeFalse();
        }

        [TestMethod]
        public void ScrapPostListFromFanpageResponseHandlerNew_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                 ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetPostListFromFanpagesResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            List<KeyValuePair<string, string>> listOfKeyValuePair = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> objKeyvaluePair = new KeyValuePair<string, string>("1651012428259980", "752<:>2723<:>95");
            listOfKeyValuePair.Add(objKeyvaluePair);

            objKeyvaluePair = new KeyValuePair<string, string>("2515048618523019", "0<:>7<:>1");
            listOfKeyValuePair.Add(objKeyvaluePair);

            objKeyvaluePair = new KeyValuePair<string, string>("2514729028554978", "7<:>46<:>12");
            listOfKeyValuePair.Add(objKeyvaluePair);

            objKeyvaluePair = new KeyValuePair<string, string>("2514182385276309", "13<:>134<:>31");
            listOfKeyValuePair.Add(objKeyvaluePair);

            var result = new ScrapPostListFromFanpageResponseHandlerNew(responseParameters, listOfKeyValuePair);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.HasMoreResults.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListPostReaction.Count.Should().Be(4);
            result.ObjFdScraperResponseParameters.AjaxToken.Should().Be("");
        }

        [TestMethod]
        public void ScrapPostListFromFanpageResponseHandlerNew_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new ScrapPostListFromFanpageResponseHandlerNew
                (responseParameters, new List<KeyValuePair<string, string>>());

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.AjaxToken.Should().BeEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new ScrapPostListFromFanpageResponseHandlerNew(responseParameters, null);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.AjaxToken.Should().BeNullOrEmpty();
            result.Status.Should().Be(false);
        }

        [TestMethod]
        public void ScrapPostListFromNewsFeedResponseHandlerNew_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.newsFeedResponse.html", Assembly.GetExecutingAssembly());

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

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            List<KeyValuePair<string, string>> listOfKeyValuePair = new List<KeyValuePair<string, string>>();
            KeyValuePair<string, string> objKeyvaluePair = new KeyValuePair<string, string>("1651012428259980", "752<:>2723<:>95");
            listOfKeyValuePair.Add(objKeyvaluePair);

            objKeyvaluePair = new KeyValuePair<string, string>("2515048618523019", "0<:>7<:>1");
            listOfKeyValuePair.Add(objKeyvaluePair);

            objKeyvaluePair = new KeyValuePair<string, string>("2514729028554978", "7<:>46<:>12");
            listOfKeyValuePair.Add(objKeyvaluePair);

            objKeyvaluePair = new KeyValuePair<string, string>("2514182385276309", "13<:>134<:>31");
            listOfKeyValuePair.Add(objKeyvaluePair);

            IHttpHelper httpHelper = new FdHttpHelper();

            var result = new ScrapPostListFromNewsFeedResponseHandlerNew
                (responseParameters, null, listOfKeyValuePair, httpHelper);

            result.FbErrorDetails.Should().Be(null);
            result.HasMoreResults.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListPostReaction.Count.Should().Be(4);
            result.Status.Should().Be(false);
            result.ObjFdScraperResponseParameters.AjaxToken.Should().Be("AXiuZKjaXswoe-Fo");
            result.ObjFdScraperResponseParameters.SectionId.Should().Be(408239535924329);
        }

        [TestMethod]
        public void ScrapPostListFromNewsFeedResponseHandlerNew_Should_Return_False()
        {
            // arrange
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


            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new ScrapPostListFromNewsFeedResponseHandlerNew
                (responseParameters, null, null, null);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.AlbumName.Should().BeNullOrEmpty();
            result.Status.Should().Be(false);

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new ScrapPostListFromNewsFeedResponseHandlerNew(responseParameters, null, null, null);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.AjaxToken.Should().BeNullOrEmpty();
            result.Status.Should().Be(false);
        }

        [TestMethod]
        public void FdSendTextMessageResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.SendTextMessageResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new FdSendTextMessageResponseHandler(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.IsMessageSent.Should().Be(true);
        }

        [TestMethod]
        public void FdSendTextMessageResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new FdSendTextMessageResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.IsMessageSent.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new FdSendTextMessageResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.IsMessageSent.Should().BeFalse();
        }

        [TestMethod]
        public void GetLastMessageResponseHandler_Should_Return_True()
        {
            var response
                = TestUtils.ReadFileFromResources("FaceDominatorCore.UnitTests.TestData.FdLibraryData.messageResponse.html", Assembly.GetExecutingAssembly());

            var account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       {
                           AccountNetwork = SocialNetworks.Facebook,
                           UserId = "100007189272538",
                       },
               };

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new GetLastMessageResponseHandler(responseParameters, account.AccountBaseModel.UserId);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(false);
        }

        [TestMethod]
        public void GetLastMessageResponseHandler_Should_Return_False()
        {
            // arrange
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100007189272538",
                        },
                };

            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new GetLastMessageResponseHandler(responseParameters, account.AccountBaseModel.UserId);

            // assert
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.Status.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new GetLastMessageResponseHandler(responseParameters, account.AccountBaseModel.UserId);

            // assert
            result.FbErrorDetails.Should().BeNull();
            result.Status.Should().Be(false);
        }

        [TestMethod]
        public void IncommingMessageResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetMessageRequestDetailsResponse.html", Assembly.GetExecutingAssembly());

            var account =
               new DominatorAccountModel
               {
                   AccountBaseModel =
                       new DominatorAccountBaseModel
                       {
                           AccountNetwork = SocialNetworks.Facebook,
                           UserId = "100007189272538",
                       },
               };

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new IncommingMessageResponseHandler
                (responseParameters, account.AccountBaseModel.UserId, new List<FdMessageDetails>());

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(false);
            result.ObjFdScraperResponseParameters.MessageDetailsList.Count.Should().Be(49);
            result.PageletData.Should().BeNullOrEmpty();

        }

        [TestMethod]
        public void IncommingMessageResponseHandler_Should_Return_False()
        {
            // arrange
            var account =
                new DominatorAccountModel
                {
                    AccountBaseModel =
                        new DominatorAccountBaseModel
                        {
                            AccountNetwork = SocialNetworks.Facebook,
                            UserId = "100007189272538",
                        },
                };

            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new IncommingMessageResponseHandler
                (responseParameters, account.AccountBaseModel.UserId, new List<FdMessageDetails>());

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.MessageDetailsList.Should().BeNullOrEmpty();
            result.PageletData.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new IncommingMessageResponseHandler
                (responseParameters, account.AccountBaseModel.UserId, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.MessageDetailsList.Count.Should().Be(0);
            result.PageletData.Should().BeNullOrEmpty();

        }

        [TestMethod]
        public void EventGuestsResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.GetInterestedGuestsForEventsUUserResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new EventGuestsResponseHandler
               (responseParameters, EventGuestType.Interested);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListUser.Count.Should().Be(4);
        }

        [TestMethod]
        public void EventGuestsResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new EventGuestsResponseHandler(responseParameters, EventGuestType.Interested);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.EntityId.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new EventGuestsResponseHandler(responseParameters, EventGuestType.Interested);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.EntityId.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void FanpageScraperResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.FanPageResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };
            FanpageDetails fanpageDetails = new FanpageDetails()
            {
                FanPageID = "519504621470372",
                FanPageUrl = "https://www.facebook.com/mumbaiakkians/",
                FanPageName = "Akshay Kumar Mumbai Fan Club - MA",
            };
            var result = new FanpageScraperResponseHandler(responseParameters, fanpageDetails);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.FanpageDetails.FanPageLikerCount.Should().Be("37543");
            result.ObjFdScraperResponseParameters.FanpageDetails.FanpageFollowerCount.Should().Be("37538");
        }

        [TestMethod]
        public void FanpageScraperResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new FanpageScraperResponseHandler(responseParameters, new FanpageDetails());

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.FanpageDetails.Should().BeNull();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new FanpageScraperResponseHandler(responseParameters, null);

            // assert
            result.Status.Should().BeTrue();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.FanpageDetails.FanPageID.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void GroupScraperResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                 ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ScrapGroupsResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new GroupScraperResponseHandler(responseParameters, null);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListGroup.Count.Should().Be(16);
            result.PageletData.Should().Be(null);
        }

        [TestMethod]
        public void GroupScraperResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new GroupScraperResponseHandler(responseParameters, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.FinalEncodedQuery.Should().BeNullOrEmpty();
            result.ObjFdScraperResponseParameters.ListGroup.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new GroupScraperResponseHandler(responseParameters, null);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.ListGroup.Count.Should().Be(0);
            result.PageletData.Should().Be(null);
        }

        [TestMethod]
        public void GroupScraperResponseHandlerNew_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.ScrapGroupsNewAsyncResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new GroupScraperResponseHandlerNew(responseParameters, null, string.Empty);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListGroup.Count.Should().Be(30);
            result.PageletData.Should().Be("163240953694419,1606119359634346,589307534524989,164122973615607,409201619562266,217357758330234,203483426777974,157321985018885,112332265596216,174323925928427,615787168444166,146720375538124,207112006063705,1550876568266674,106420499450157,316643071803104,296779867067558,2138298736390953,1617655414924132,449610642065033,195182630513779,1413952862244117,542181525933913,1241830245840812,798217670358536,201535340381410,158284765009976,1061672593907564,404674879597312,744456549063931");
        }

        [TestMethod]
        public void GroupScraperResponseHandlerNew_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new GroupScraperResponseHandlerNew
                (responseParameters, null, string.Empty);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.HasMoreResults.Should().BeFalse();
            result.ObjFdScraperResponseParameters.ListGroup.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new GroupScraperResponseHandlerNew(responseParameters, null, string.Empty);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.ListGroup.Count.Should().Be(0);
            result.PageletData.Should().Be(null);
        }

        [TestMethod]
        public void SearchFanpageDetailsResponseHandler_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
               ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.UpdateLikedPagesAsyncResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new SearchFanpageDetailsResponseHandler(responseParameters,true);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.ObjFdScraperResponseParameters.ListPage.Count.Should().Be(76);
        }

        [TestMethod]
        public void SearchFanpageDetailsResponseHandler_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new SearchFanpageDetailsResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.ObjFdScraperResponseParameters.ListPage.Should().BeNullOrEmpty();
            result.PageletData.Should().BeNullOrEmpty();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new SearchFanpageDetailsResponseHandler(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.ObjFdScraperResponseParameters.ListPage.Should().BeNullOrEmpty();
            result.PageletData.Should().BeNullOrEmpty();
        }

        [TestMethod]
        public void EventAndMessangerResponse_Should_Return_True()
        {
            var response = TestUtils.ReadFileFromResources
                ("FaceDominatorCore.UnitTests.TestData.FdLibraryData.InviteAsPersonalMessageResponse.html", Assembly.GetExecutingAssembly());

            var responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            var result = new EventAndMessangerResponse(responseParameters);

            result.FbErrorDetails.Should().Be(null);
            result.Status.Should().Be(true);
            result.IsEventInviteSent.Should().Be(true);
        }

        [TestMethod]
        public void EventAndMessangerResponse_Should_Return_False()
        {
            // arrange
            var responseParameters = new ResponseParameter
            {
                HasError = true,
                Response = null,
                Exception = new Exception()
            };

            // act
            var result = new EventAndMessangerResponse(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Status.Should().Be("LangKeyWebException".FromResourceDictionary());
            result.IsEventInviteSent.Should().BeFalse();

            // arrange
            responseParameters = new ResponseParameter
            {
                HasError = false,
                Response = string.Empty,
                Exception = null
            };

            // act
            result = new EventAndMessangerResponse(responseParameters);

            // assert
            result.Status.Should().BeFalse();
            result.FbErrorDetails.Should().BeNull();
            result.IsEventInviteSent.Should().BeFalse();
        }

    }
}
