using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class ImageExtracterTest
    {
        [TestMethod, Ignore("doesn't work anymore")]
        public void should_return_lists_of_imageurl_if_url_is_valid()
        {
            string title = "";
            var result = ImageExtracter.ExtractImageUrls("https://www.google.com/search?q=flower&tbm=isch&ved=2ahUKEwjD2_r_xv_rAhXGXZoKHZ02DhgQ2-cCegQIABAA&oq=flower&gs_lcp=CgNpbWcQA1AAWABg0_RZaABwAHgAgAEAiAEAkgEAmAEAqgELZ3dzLXdpei1pbWc&sclient=img&ei=nWRrX4OZCMa76QSd7bjAAQ&bih=576&biw=1366", ref title);
            result.Should().NotBeEmpty().And.HaveCount(1);
        }
        [TestMethod]
        [ExpectedException(typeof(UriFormatException))]
        public void should_throw_UriFormatException_if_url_is_not_valid()
        {
            string title = "";
            ImageExtracter.ExtractImageUrls("url", ref title);
        }
        [TestMethod]

        public void should_return_empty_list_if_url_not_contain_image()
        {
            string title = "";
            var result = ImageExtracter.ExtractImageUrls("http://www.yahoo.com", ref title);
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void should_remove_invalid_url()
        {
            string title = "";
            var imageurls = new List<string>();
            imageurls.Add("https://hips.hearstapps.com/hmg-prod.s3.amazonaws.com/images/close-up-of-tulips-blooming-in-field-royalty-free-image-1584131616.jpg?crop=0.630xw:1.00xh;0.186xw,0&resize=640:*");
            imageurls.Add("https://images.unsplash.com/photo-1533907650686-70576141c030?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1350&q=80");
            imageurls.Add("invalid");
            imageurls.Add("url");
            var result = ImageExtracter.RemoveInvalidUrls(imageurls);
            result.Should().NotBeEmpty().And.HaveCount(2);
        }
    }
}
