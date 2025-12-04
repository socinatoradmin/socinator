using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDModel;
using FluentAssertions;
using MS.Internal.Xml;
using System.IO;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Settings;

namespace GdCoreUnitTest
{
    [TestClass]
    public class ScrapeFilterTest
    {
        public ScrapeFilterTest()
        {
            SoftwareSettings.Settings = SoftwareSettingsFileManager.GetSoftwareSettings();
            modulesetting = Newtonsoft.Json.JsonConvert.DeserializeObject<ModuleSetting>(activitysetting);
            image = new ScrapeFilter.Image(modulesetting);
        }
        string activitysetting = File.ReadAllText(@"C:\Users\GLB-123\Desktop\activity setting.txt");
        private ScrapeFilter.Image image;
        private ModuleSetting modulesetting;
        InstagramPost instagramPost = new InstagramPost();
        [TestMethod] 
        public void should_return_True_FilterPostAge()
        {
            //arrang
            bool isCheckComment = true;
            bool isActual = false;
            //act
            if (image.FilterPostAge(instagramPost))
                isActual = true;
            //assert
            Assert.AreEqual(isCheckComment, isActual);
        }
        public void should_return_True_FilterComments()
        {
            //arrang
            bool isCheckComment = true;
            bool isActual = false;
            //act
            isActual = image.FilterComments(instagramPost);

            //assert
            isActual.Should().BeTrue();
        }
    }
}
