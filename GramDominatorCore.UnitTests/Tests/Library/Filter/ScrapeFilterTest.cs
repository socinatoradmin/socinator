using System;
using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using FluentAssertions;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using GramDominatorCore.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;

namespace GramDominatorCore.UnitTests.Tests.Library.Filter
{
    [TestClass]
    public class ScrapeFilterTest : UnityInitializationTests
    {
        private ISoftwareSettings _softwareSetting;
        private IDateProvider _dateProvider;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _softwareSetting = Substitute.For<ISoftwareSettings>();
            Container.RegisterInstance(_softwareSetting);
            _dateProvider = Substitute.For<IDateProvider>();
            Container.RegisterInstance(_dateProvider);
            _softwareSetting.Settings.Returns(new SoftwareSettingsModel());          
        }

        #region PostAge Filter
        [TestMethod]
        public void Is_FilterBeforePostAge_Will_Return_True()
        {
            //arrange
            var instagramPost = new InstagramPost
            {
                TakenAt = 1556895681
            };
            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    FilterPostAge = true,
                    FilterBeforePostAge = true,
                    MaxPostAge =300
                },
            };
            var image = new ScrapeFilter.Image(moduleSetting);
            _dateProvider.UtcNow().Returns(new DateTime(2019, 05, 03));
            //act
            bool check = image.FilterPostAge(instagramPost);
            //assert
            check.Should().BeTrue();
        }

        [TestMethod]
        public void Is_FilterBeforePostAge_Will_Return_False()
        {
            //arrange
            var instagramPost = new InstagramPost
            {
                TakenAt = 1556895681
            };
            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    FilterPostAge = true,
                    FilterBeforePostAge = true,
                    MaxPostAge = 4
                },
            };
            var image = new ScrapeFilter.Image(moduleSetting);
            _dateProvider.UtcNow().Returns(new DateTime(2019, 05, 08));
            //act
            bool check = image.FilterPostAge(instagramPost);
            //assert
            check.Should().BeFalse();
        }

        [TestMethod]
        public void Is_FilterLastPostAg_Will_Return_True()
        {
            var instagramPost = new InstagramPost
            {
                TakenAt = 1556895681
            };
            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    FilterPostAge = true,
                    FilterBeforePostAge = false,
                    MaxLastPostAge = 2
                },
            };
            var image = new ScrapeFilter.Image(moduleSetting);
            _dateProvider.UtcNow().Returns(new DateTime(2019, 05, 07));
            //act
            bool check = image.FilterPostAge(instagramPost);
            //assert
            check.Should().BeTrue();
        }

        [TestMethod]
        public void Is_FilterPostAge_FilterLastPostAg_Will_Return_False()
        {
            var instagramPost = new InstagramPost
            {
                TakenAt = 1556895681
            };
            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    FilterPostAge = true,
                    FilterBeforePostAge = false,
                    MaxLastPostAge = 1000
                },
            };
            var image = new ScrapeFilter.Image(moduleSetting);
            //act
            bool check = image.FilterPostAge(instagramPost);
            //assert
            check.Should().BeFalse();
        }
        #endregion

        #region CommentRange Filter
        [TestMethod]
        public void Is_FilterComments_Will_Return_False()
        {
            var instagramPost = new InstagramPost
            {
                CommentCount = 11
            };
            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    FilterComments = true,
                    CommentsCountRange = new RangeUtilities(9, 15),
                },
            };
            var image = new ScrapeFilter.Image(moduleSetting);
            //act
            bool check = image.FilterComments(instagramPost);
            //assert
            check.Should().BeFalse();
        }

        [TestMethod]
        public void Is_FilterComments_Will_Return_True()
        {
            var instagramPost = new InstagramPost
            {
                CommentCount = 7
            };
            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    FilterComments = true,
                    CommentsCountRange = new RangeUtilities(10, 15),
                }
            };
            var image = new ScrapeFilter.Image(moduleSetting);
            //act
            bool check = image.FilterComments(instagramPost);
            //assert
            check.Should().BeTrue();
        }
        #endregion

        #region LikeRange Filter
        [TestMethod]
        public void Is_FilterLike_Will_Return_False()
        {
            var instagramPost = new InstagramPost
            {
                LikeCount = 11
            };
            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    FilterLikes = true,
                    LikesCountRange = new RangeUtilities(9, 15),
                },
            };
            var image = new ScrapeFilter.Image(moduleSetting);
            //act
            bool check = image.FilterLikes(instagramPost);
            //assert
            check.Should().BeFalse();
        }

        [TestMethod]
        public void Is_FilterLike_Will_Return_True()
        {
            var instagramPost = new InstagramPost
            {
                LikeCount = 7
            };
            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    FilterLikes = true,
                    LikesCountRange = new RangeUtilities(10, 15),
                }
            };
            var image = new ScrapeFilter.Image(moduleSetting);
            //act
            bool check = image.FilterLikes(instagramPost);
            //assert
            check.Should().BeTrue();
        }
        #endregion


        #region Filter PostType

        [TestMethod]
        public void Is_FilterPostType_VideoType_Will_Return_True()
        {
            var instagramPost = new InstagramPost
            {
                MediaType = MediaType.Video
            };
            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    PostCategory = new PostCategory
                    {
                        FilterPostCategory = true,
                        IgnorePostVideos = true
                    }
                },
            };
            var image = new ScrapeFilter.Image(moduleSetting);
            //act
            bool check = image.FilterPostType(instagramPost);
            //assert
            check.Should().BeTrue();
        }

        [TestMethod]
        public void Is_FilterPostType_Image_Will_Return_True()
        {
            var instagramPost = new InstagramPost
            {
                MediaType = MediaType.Image
            };

            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    PostCategory = new PostCategory
                    {
                        FilterPostCategory = true,
                        IgnorePostImages = true
                    }
                },

            };
            var image = new ScrapeFilter.Image(moduleSetting);
            //act
            bool check = image.FilterPostType(instagramPost);
            //assert
            check.Should().BeTrue();
        }

        [TestMethod]
        public void Is_FilterPostType_AlbumType_Will_Return_True()
        {
            var instagramPost = new InstagramPost
            {
                MediaType = MediaType.Album
            };

            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    PostCategory = new PostCategory
                    {
                        FilterPostCategory = true,
                        IgnorePostAlbums = true
                    }
                },

            };
            var image = new ScrapeFilter.Image(moduleSetting);
            //act
            bool check = image.FilterPostType(instagramPost);
            //assert
            check.Should().BeTrue();
        }

        [TestMethod]
        public void Is_FilterPostType_NoMediaType_Will_Return_False()
        {
            var instagramPost = new InstagramPost
            {
                MediaType = MediaType.Album
            };

            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    PostCategory = new PostCategory
                    {
                        FilterPostCategory = true,
                    }
                },

            };
            var image = new ScrapeFilter.Image(moduleSetting);
            //act
            bool check = image.FilterPostType(instagramPost);
            //assert
            check.Should().BeFalse();
        }
        #endregion

        #region FilterCaptionLength
        [TestMethod]
        public void Is_FilterCaptionLength_Will_Return_True()
        {
            var instagramPost = new InstagramPost
            {
                Caption = "wow what a lovely"
            };

            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    FilterCharsLenghInCaption = true,
                    MinimumPostCaptionChars = 20
                },

            };
            var image = new ScrapeFilter.Image(moduleSetting);
            //act
            bool check = image.FilterCaptionLength(instagramPost);
            //assert
            check.Should().BeTrue();
        }

        [TestMethod]
        public void Is_FilterCaptionLength_Will_Return_False()
        {
            var instagramPost = new InstagramPost
            {
                Caption = "wow what a lovely"
            };

            var moduleSetting = new ModuleSetting
            {
                PostFilterModel = new PostFilterModel
                {
                    FilterCharsLenghInCaption = true,
                    MinimumPostCaptionChars = 10
                },

            };
            var image = new ScrapeFilter.Image(moduleSetting);
            //act
            bool check = image.FilterCaptionLength(instagramPost);
            //assert
            check.Should().BeFalse();
        } 
        #endregion
    }
}
