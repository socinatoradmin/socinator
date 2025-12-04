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
    public class AccountsCacheServiceTests : UnityInitializationTests
    {
        private IAccountsCacheService _sut;
        private IBinFileHelper _binFileHelper;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _binFileHelper = Substitute.For<IBinFileHelper>();
            _sut = new AccountsCacheService(_binFileHelper);
            var httpHelper = Substitute.For<IHttpHelper>();
            Container.RegisterInstance<IHttpHelper>(SocialNetworks.Twitter.ToString(), httpHelper);
        }

        [TestMethod]
        public void should_retrun_account_details()
        {
            // arrange
            var accounts = new List<DominatorAccountModel>
            {
                new DominatorAccountModel {AccountId = "123"},
                new DominatorAccountModel {AccountId = "321"},
            };
            _binFileHelper.GetAccountDetails().Returns(accounts);

            // act
            var result = _sut.GetAccountDetails();


            // assert
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(accounts);
            _binFileHelper.Received(1).GetAccountDetails();
        }


        [TestMethod]
        public void should_load_accounts_only_once()
        {
            // arrange
            var accounts = new List<DominatorAccountModel>
            {
                new DominatorAccountModel {AccountId = "123"},
                new DominatorAccountModel {AccountId = "321"},
            };
            _binFileHelper.GetAccountDetails().Returns(accounts);

            // act
            var result = _sut.GetAccountDetails();
            result = _sut.GetAccountDetails();


            // assert
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(accounts);
            // only one call of GetAccountDetails because data should be cached
            _binFileHelper.Received(1).GetAccountDetails();
        }

        [TestMethod]
        public void should_update_accounts()
        {
            // arrange
            var accounts = new List<DominatorAccountModel>
            {
                new DominatorAccountModel {AccountId = "123"},
                new DominatorAccountModel {AccountId = "321"},
            };
            var accountsNew = new DominatorAccountModel[]
            {
                new DominatorAccountModel {AccountId = "123"},
                new DominatorAccountModel {AccountId = "321"},
                new DominatorAccountModel {AccountId = "345"},
            };

            _binFileHelper.GetAccountDetails().Returns(accounts);
            _binFileHelper.UpdateAllAccounts(Arg.Do((List<DominatorAccountModel> a) =>
            {
                // assert
                a.Should().BeEquivalentTo(accountsNew.ToList());
            })).Returns(true);

            // act
            _sut.UpsertAccounts(accountsNew);


            // assert
            _sut.GetAccountDetails().Count.Should().Be(3);
            _binFileHelper.Received(1).UpdateAllAccounts(Arg.Any<List<DominatorAccountModel>>());
        }

        [TestMethod]
        public void should_delete_accounts()
        {
            // arrange
            var notDeleted = new DominatorAccountModel { AccountId = "345" };
            var accounts = new List<DominatorAccountModel>
            {
                new DominatorAccountModel { AccountId = "123" },
                new DominatorAccountModel {AccountId = "321"},
                notDeleted
            };
            var accountsTodelete = new DominatorAccountModel[]
            {
                new DominatorAccountModel {AccountId = "123"},
                new DominatorAccountModel {AccountId = "321"},
            };

            _binFileHelper.GetAccountDetails().Returns(accounts);
            _binFileHelper.UpdateAllAccounts(Arg.Do((List<DominatorAccountModel> a) =>
            {
                // assert
                a.Single().Should().Be(notDeleted);
            })).Returns(true);

            // act
            _sut.Delete(accountsTodelete);


            // assert
            _sut.GetAccountDetails().Count.Should().Be(1);
            _binFileHelper.Received(1).UpdateAllAccounts(Arg.Any<List<DominatorAccountModel>>());
        }
    }
}
