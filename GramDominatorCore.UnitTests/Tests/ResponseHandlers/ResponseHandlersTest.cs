using Dominator.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Request;
using GramDominatorCore.Response;
using FluentAssertions;
using GramDominatorCore.GDModel;
using GramDominatorCore.GDLibrary.Response;
using System;

namespace GramDominatorCore.UnitTests.Tests.ResponseHandlers
{
    [TestClass]
    public class ResponseHandlersTest
    {     
        [TestMethod]
        public void CheckUsernameResponse_Should_Return_True()
        {
            // arrange
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.CheckUserNameAvailableResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new CheckUsernameResponse(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        [Timeout(10000)]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckUsernameResponse_Should_Return_False_If_Response_Is_Empty()
        {
            // arrange
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = "",
                Exception = null
            };

            // act
            var result = new CheckUsernameResponse(parameters).Success;
            // assert
            result.Should().Be(false);
        }
        [TestMethod]
        public void FriendshipsResponse_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.BlockFollower.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new FriendshipsResponse(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void UsernameInfoIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.ChangeProfilePictureResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new UsernameInfoIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void CommonIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.CommentResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new CommonIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void DeleteMediaIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.DeleteMedia.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new DeleteMediaIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void FeedIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.GetHashTagFeed.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new FeedIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void MediaCommentsIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.GetMediaCommentsResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new MediaCommentsIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void MediaLikersIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.GetMediaLikersResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new MediaLikersIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void FollowerAndFollowingIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.GetRecentFollowersResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new FollowerAndFollowingIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void SuggestedUsersIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.GetSuggestedUsersResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new SuggestedUsersIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void UserFeedIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.GetUserFeedResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new UserFeedIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void V2InboxResponse_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.Getv2InboxResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new V2InboxResponse(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void VisualThreadResponse_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.GetVisualThreadResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new VisualThreadResponse(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void LikeResponse_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.LikeResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new LikeResponse(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void ExploreResponse_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.LocationCheckResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new ExploreResponse(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void LoginIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.LoginResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new LoginIgResponseHandler(parameters);
            // assert
            result.Success.Should().Be(true);
            result.Username.Should().Be("metcalfe_penelope");
            result.FullName.Should().Be("");
            result.IsPrivate.Should().Be(false);
            result.IsVerified.Should().Be(false);
            result.Pk.Should().Be("5854658120");
            result.HasAnonymousProfilePicture.Should().Be(false);
            result.ProfilePicUrl.Should().Be("https://scontent-bom1-2.cdninstagram.com/vp/ab2e62347c4c68c5506f0314c1fe837b/5CC69797/t51.2885-19/s150x150/46256437_372268763544237_803953573560844288_n.jpg?_nc_ht=scontent-bom1-2.cdninstagram.com");
            result.TwoFactor.Should().Be(null);

        }

        [TestMethod]
        public void MediaInfoIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.MediaInfoResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new MediaInfoIgResponseHandler(parameters);
            // assert
            result.Success.Should().Be(true);
            result.InstagramPost.Caption.Should().Be("#Plane #Hills #Mountains");
            result.InstagramPost.Code.Should().Be("Brk_sDoBHx0");
            result.InstagramPost.CommentCount.Should().Be(3);
            result.InstagramPost.CommentLikesEnabled.Should().Be(true);
            result.InstagramPost.CommentsDisabled.Should().Be(false);
            result.InstagramPost.DeviceTimestamp.Should().Be("1545242061496190");
            result.InstagramPost.HasDetailedLocation.Should().Be(true);
            result.InstagramPost.HasLiked.Should().Be(false);
            result.InstagramPost.HasLocation.Should().Be(true);
            result.InstagramPost.Id.Should().Be("1937953844155939956_5340");
            result.InstagramPost.LikeCount.Should().Be(32);
            result.InstagramPost.Images.Count.Should().Be(7);
            result.InstagramPost.IsAd.Should().Be(false);
            result.InstagramPost.CommentCount.Should().Be(3);
            result.InstagramPost.MediaType.Should().Be(MediaType.Image);
            result.InstagramPost.TakenAt.Should().Be(1545242122);
            result.InstagramPost.PhotoOfYou.Should().Be(false);
            result.InstagramPost.Pk.Should().Be("1937953844155939956");
           // result.InstagramPost.Video.Should().Be("");
            //result.InstagramPost.VideoDuration.Should().Be(3434343434);
         //   result.InstagramPost.User.Should().Be("");
           // result.InstagramPost.ViewCount.count.Should().Be(454);

        }

        [TestMethod]
        public void LocationIgReponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.SearchForLocationResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new LocationIgReponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void LocationAlternateIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.GetLocationFeedAlternateResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new FeedIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void SearchTagIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.SearchForTagResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new SearchTagIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void SearchKeywordIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.SearchKeywordResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new SearchKeywordIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void LocationIdIgReponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.SearchLocationIdResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new LocationIdIgReponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void SendMessageIgResponseHandler_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.SendMessage.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new SendMessageIgResponseHandler(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void UserFriendshipResponse_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.UserFriendshipResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new UserFriendshipResponse(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void UserFeedResponse_should_return_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.GetLikedMediaResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new UserFeedResponse(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void GetMediaLikedForUnlikeResposne_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.GetLikedMediaForUnlikeResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new UserFeedResponse(parameters).Success;
            // assert
            result.Should().Be(true);
        }

        [TestMethod]
        public void HashTagUserFeedResponse_true()
        {
            var response = TestUtils.ReadFileFromResources("GramDominatorCore.UnitTests.TestData.HashTagUserFeedResponse.json", Assembly.GetExecutingAssembly());
            var parameters = new ResponseParameter()
            {
                HasError = false,
                Response = response,
                Exception = null
            };

            // act
            var result = new UserFeedResponse(parameters).Success;
            // assert
            result.Should().Be(true);
        }
    }
}
