using FluentAssertions;
using DominatorHouseCore.Models;
using System.Linq;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DominatorUIUtility.ViewModel;

namespace DominatorHouseCore.UnitTests.Tests.ViewModels
{
    [TestClass]
    public class SelectAccountViewModelTest
    {
        SelectAccountViewModel _selectAccountViewModel;
        [TestInitialize]
        public void Initilize()
        {
            _selectAccountViewModel = new SelectAccountViewModel()
            {
                SelectAccountModel = new SelectAccountModel { GroupText = "first" }
            };
        }

        [TestMethod]
        public void should_SelectDeselectAccountByGroup_method_update_IsAllAccountSelected_to_true_if_needToselect_is_true_and_all_group_is_checked()
        {
            var needToselect = true;
            _selectAccountViewModel.SelectAccountModel.Groups = new ObservableCollection<ContentSelectGroup>
            {
                new ContentSelectGroup
                {
                    Content="first",
                    IsContentSelected=true
                },
                 new ContentSelectGroup
                {
                    Content="second",
                    IsContentSelected=true
                }
            };
            _selectAccountViewModel.LstSelectAccount = new ObservableCollection<SelectAccountModel>
            {
                new SelectAccountModel
                {
                   GroupName="first"
                },
                 new SelectAccountModel
                {
                   GroupName="second"
                }
            };
            _selectAccountViewModel.SelectDeselectAccountByGroup(needToselect);
            _selectAccountViewModel.IsAllAccountSelected.Should().BeTrue();
            _selectAccountViewModel.LstSelectAccount.Select(x => x.IsAccountSelected).Count().Should().Be(2);
        }
        [TestMethod]
        public void should_SelectDeselectAccountByGroup_method_update_IsAllAccountSelected_to_false_if_needToselect_is_true_and_all_groups_are_not_checked()
        {
            var needToselect = true;
            _selectAccountViewModel.SelectAccountModel.Groups = new ObservableCollection<ContentSelectGroup>
            {
                new ContentSelectGroup
                {
                    Content="first"
                },
                 new ContentSelectGroup
                {
                    Content="second"
                }
            };
            _selectAccountViewModel.LstSelectAccount = new ObservableCollection<SelectAccountModel>
            {
                new SelectAccountModel
                {
                   GroupName="abc"
                },
                 new SelectAccountModel
                {
                   GroupName="def"
                }
            };
            _selectAccountViewModel.SelectDeselectAccountByGroup(needToselect);
            _selectAccountViewModel.IsAllAccountSelected.Should().BeFalse();
        }
        [TestMethod]
        public void should_AccountGroupSelected_method_update_GroupText_to_1_Group_S_Selected_if_all_groups_are_checked()
        {
            _selectAccountViewModel.SelectAccountModel.Groups = new ObservableCollection<ContentSelectGroup>
            {
                new ContentSelectGroup
                {
                    Content="first",
                    IsContentSelected=true
                }
            };
            _selectAccountViewModel.AccountGroupSelected();
            _selectAccountViewModel.SelectAccountModel.GroupText.Should().NotBeNullOrWhiteSpace();
        }
        [TestMethod]
        public void should_AccountGroupSelected_method_update_GroupText_to_0_Group_S_Selected_if_groups_are_not_checked()
        {
            _selectAccountViewModel.SelectAccountModel.Groups = new ObservableCollection<ContentSelectGroup>
            {
                new ContentSelectGroup
                {
                    Content="first",
                    IsContentSelected=false
                }
            };
            _selectAccountViewModel.AccountGroupSelected();
            _selectAccountViewModel.SelectAccountModel.GroupText.Should().NotBeNullOrWhiteSpace();
        }
      
    }
}
