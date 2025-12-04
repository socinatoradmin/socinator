using Dominator.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Unity;
using FluentAssertions;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel.SocioPublisher.AdvancedSettings;

namespace DominatorHouseCore.UnitTests.Tests.ViewModels
{
    [TestClass]
    public class FacebookViewModelTest : UnityInitializationTests
    {
        FacebookViewModel _facebookViewModel;
        private IBinFileHelper _binFileHelper;
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _binFileHelper = Substitute.For<IBinFileHelper>();
            Container.RegisterInstance(_binFileHelper);
            _facebookViewModel = new FacebookViewModel();
        }
        [TestMethod]
        public void SaveMentionCommad_should_add_three_User_to_ListCustomMentionUser_if_CustomMentionUser_have_string_which_having_tab_and_enter_character_together_twice()
        {
            _facebookViewModel.FacebookModel.CustomMentionUser = "x\r\ny\r\nz";
            _facebookViewModel.SaveMentionCommad.Execute(new object());
            _facebookViewModel.FacebookModel.ListCustomMentionUser.Count.Should().Be(3);
            _facebookViewModel.FacebookModel.ListCustomMentionUser[0].Should().Be("x");
            _facebookViewModel.FacebookModel.ListCustomMentionUser[1].Should().Be("y");
            _facebookViewModel.FacebookModel.ListCustomMentionUser[2].Should().Be("z");
        }
        [TestMethod]
        public void SavePageCommad_should_add_three_urls_to_ListCustomPageUrl_if_CustomPageUrl_having_url_saparated_with_tab_and_enter_together_twice()
        {
            _facebookViewModel.FacebookModel.CustomPageUrl = "x\r\ny\r\nz";
            _facebookViewModel.SavePageCommad.Execute(new object());
            _facebookViewModel.FacebookModel.ListCustomPageUrl.Count.Should().Be(3);
            _facebookViewModel.FacebookModel.ListCustomPageUrl[0].Should().Be("x");
            _facebookViewModel.FacebookModel.ListCustomPageUrl[1].Should().Be("y");
            _facebookViewModel.FacebookModel.ListCustomPageUrl[2].Should().Be("z");
        }

       
        [TestMethod]
        public void SaveFriendCommad_should_add_two_custom_tagged_to_ListCustomTaggedUser_if_CustomTaggedUser_having_CustomTaggedUser_saparated_with_tab_and_enter()
        {
            _facebookViewModel.FacebookModel.CustomTaggedUser = "user1\r\nuser2";
            _facebookViewModel.SaveFriendCommad.Execute(new object());
            _facebookViewModel.FacebookModel.ListCustomTaggedUser.Count.Should().Be(2);
            _facebookViewModel.FacebookModel.ListCustomTaggedUser[0].Should().Be("user1");
            _facebookViewModel.FacebookModel.ListCustomTaggedUser[1].Should().Be("user2");
  
        }
    }
}
