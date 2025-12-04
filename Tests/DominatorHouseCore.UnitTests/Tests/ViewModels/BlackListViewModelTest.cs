using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Dominator.Tests.Utils;
using DominatorUIUtility.ViewModel;
using DominatorHouseCore.Diagnostics;
using NSubstitute;
using DominatorHouseCore.Interfaces;
using SQLite;
using DominatorHouseCore.Enums.DHEnum;
using System.Threading.Tasks;
using System.Linq;

namespace DominatorHouseCore.UnitTests.Tests.ViewModels
{
    [TestClass, Microsoft.VisualStudio.TestTools.UnitTesting.Ignore("need to move integration tests to corresponding project")]
    public class BlackListViewModelTest : UnityInitializationTests
    {
        BlackListViewModel _blacklistViewModel;
        private IGlobalDatabaseConnection _globalDb;


        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _globalDb = Substitute.For<IGlobalDatabaseConnection>();
            Container.RegisterInstance(_globalDb);
            _blacklistViewModel = new BlackListViewModel();
            var whiteListdb = new SQLiteConnection(@"Quora.db");
            var blackListdb = new SQLiteConnection(@"Quora.db");
            _globalDb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser).ReturnsForAnyArgs(whiteListdb);
            _globalDb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser).ReturnsForAnyArgs(blackListdb);
        }
        [TestMethod]
        public void InitializeData_method_should_initialize_blacklist_user_list_from_db()
        {
            Task.Factory.StartNew(() => _blacklistViewModel.InitializeData()).ContinueWith((x) =>
            {
                _blacklistViewModel.LstBlackListUsers.Count.Should().Be(2);
                _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser);
                _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser);
            });
        }
        [TestMethod]
        public void AddToBlackList_method_should_add_user_to_blacklist_user_list_and_db()
        {
            _blacklistViewModel.BlacklistUser = "Kumar";

            Task.Factory.StartNew(() => _blacklistViewModel.InitializeData()).ContinueWith((x) => _blacklistViewModel.AddToBlackList(new object())).ContinueWith((x) =>
            {
                _blacklistViewModel.LstBlackListUsers.Count.Should().BeGreaterOrEqualTo(2);
                _blacklistViewModel.BlacklistUser.Should().BeEmpty();
                _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser);
                _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser);
            }); 
        }
        [TestMethod]
        public void Refresh_method_should_clear_blacklist_user_list_and_again_add_all_update_data_from_db()
        {
            Task.Factory.StartNew(() => _blacklistViewModel.InitializeData()).
                ContinueWith((x) => _blacklistViewModel.Refresh(new object())).ContinueWith((x) =>
            {
                _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser);
                _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser);
                _blacklistViewModel.LstBlackListUsers.Count.Should().BeGreaterOrEqualTo(2);
            });
        }
        [TestMethod]
        public void ClearCommand_should_clear_blacklist_user()
        {
            _blacklistViewModel.ClearCommand.Execute(new object());
            _blacklistViewModel.BlacklistUser.Should().BeEmpty();
        }

        [TestMethod]
        public void DeleteCommand_should_delete_user_from_blacklist_user_list_and_db()
        {
            Task.Factory.StartNew(() => _blacklistViewModel.InitializeData()).ContinueWith((x) =>
            {
                _blacklistViewModel.LstBlackListUsers[0].IsBlackListUserChecked = true;
                _blacklistViewModel.DeleteCommand.Execute(new object());
            }).ContinueWith((x) =>
                {
                    _blacklistViewModel.LstBlackListUsers.Count.Should().BeGreaterOrEqualTo(1);
                    _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser);
                    _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser);
                }); ;
        }
        [TestMethod]
        public void SelectCommand_should_set_IsAllBlackListUserChecked_true_if_all_users_are_Selected()
        {
            Task.Factory.StartNew(() => _blacklistViewModel.InitializeData()).ContinueWith((x) =>
            {
                _blacklistViewModel.LstBlackListUsers.Select(user=> { user.IsBlackListUserChecked = true; return user; }).ToList();
                _blacklistViewModel.SelectCommand.Execute(new object());
            }).ContinueWith((x) =>
            {
                _blacklistViewModel.IsAllBlackListUserChecked.Should().BeTrue();
            }); 
        }
        [TestMethod]
        public void SelectCommand_should_set_IsAllBlackListUserChecked_false_if_all_users_are_not_Selected()
        {
            Task.Factory.StartNew(() => _blacklistViewModel.InitializeData()).ContinueWith((x) =>
            {
                _blacklistViewModel.LstBlackListUsers.Select(user => { user.IsBlackListUserChecked = false; return user; }).ToList();
                _blacklistViewModel.SelectCommand.Execute(new object());
            }).ContinueWith((x) =>
            {
                _blacklistViewModel.IsAllBlackListUserChecked.Should().BeFalse();
                _blacklistViewModel.LstBlackListUsers.Where(y => y.IsBlackListUserChecked).Count().Should().Be(0);
            });
        }
    }

}
