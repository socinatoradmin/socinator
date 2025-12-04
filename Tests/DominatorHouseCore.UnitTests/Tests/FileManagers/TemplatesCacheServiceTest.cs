using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class TemplatesCacheServiceTest : UnityInitializationTests
    {
        private ITemplatesCacheService _sut;
        private IBinFileHelper _binFileHelper;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _binFileHelper = Substitute.For<IBinFileHelper>();
            _sut = new TemplatesCacheService(_binFileHelper);
            var httpHelper = Substitute.For<IHttpHelper>();
            Container.RegisterInstance<IHttpHelper>(SocialNetworks.Quora.ToString(), httpHelper);
        }

        [TestMethod]
        public void should_retrun_template_details()
        {
            // arrange
            var template = new List<TemplateModel>
            {
                new TemplateModel {Id = "123"},
                new TemplateModel {Id = "321"},
            };
            _binFileHelper.GetTemplateDetails().Returns(template);

            // act
            var result = _sut.GetTemplateModels();


            // assert
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(template);
            _binFileHelper.Received(1).GetTemplateDetails();
        }


        [TestMethod]
        public void should_load_accounts_only_once()
        {
            var template = new List<TemplateModel>
            {
                new TemplateModel {Id = "123"},
                new TemplateModel {Id = "321"},
            };
            _binFileHelper.GetTemplateDetails().Returns(template);

            var result = _sut.GetTemplateModels();
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(template);
            _binFileHelper.Received(1).GetTemplateDetails();
        }

        [TestMethod]
        public void should_update_template()
        {
            var template = new List<TemplateModel>
            {
                new TemplateModel {Id = "123"},
                new TemplateModel {Id = "321"},
            };
            var newtemplate = new List<TemplateModel>
            {
                new TemplateModel {Id = "123"},
                new TemplateModel {Id = "321"},
                new TemplateModel {Id = "322"},
            };

            _binFileHelper.GetTemplateDetails().Returns(template);
            _binFileHelper.UpdateAllAccounts(Arg.Do((List<TemplateModel> a) =>
            {
                a.Should().BeEquivalentTo(newtemplate.ToList());
            })).Returns(true);
            _sut.UpsertTemplates(newtemplate.ToArray());
            _sut.GetTemplateModels().Count.Should().Be(3);

        }

        [TestMethod]
        public void should_delete_template()
        {
            var notDeleted = new TemplateModel { Id = "345" };
            var template = new List<TemplateModel>
            {
                new TemplateModel { Id = "123" },
                new TemplateModel {Id = "321"},
                notDeleted
            };
            var templateTodelete = new TemplateModel[]
            {
                new TemplateModel {Id = "123"},
                new TemplateModel {Id = "321"},
            };

            _binFileHelper.GetTemplateDetails().Returns(template);

            _binFileHelper.UpdateAllAccounts(Arg.Do((List<TemplateModel> a) =>
            {
                a.FirstOrDefault().Should().Be(notDeleted);
            })).Returns(true);
            _sut.UpsertTemplates(notDeleted);
            _sut.Delete(templateTodelete);

            _sut.GetTemplateModels().Count.Should().Be(1);

        }
    }
}
