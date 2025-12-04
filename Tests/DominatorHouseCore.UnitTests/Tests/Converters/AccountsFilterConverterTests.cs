using DominatorHouseCore.Converters;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Globalization;

namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class AccountsFilterConverterTests
    {
        private AccountsFilterConverter _sut;
        private object[] values;
        List<DominatorAccountModel> LstDominatorAccountModel;
        [TestInitialize]
        public void SetUp()
        {
            _sut = new AccountsFilterConverter();
            LstDominatorAccountModel = new List<DominatorAccountModel>
            {
                     new DominatorAccountModel
                     {
                              AccountBaseModel = new DominatorAccountBaseModel
                              {
                                  UserName = "kumar",
                                  AccountNetwork = SocialNetworks.Quora
                              }
                     },
                    new DominatorAccountModel
                   {
                        AccountBaseModel=new DominatorAccountBaseModel
                        {
                            UserName="kumar facebook",
                            AccountNetwork=SocialNetworks.Facebook
                        }
                    },
                    new DominatorAccountModel
                   {
                        AccountBaseModel=new DominatorAccountBaseModel
                        {
                            UserName="kumar youtube",
                            AccountNetwork=SocialNetworks.Youtube
                        }
                    },
                     new DominatorAccountModel
                     {
                        AccountBaseModel = new DominatorAccountBaseModel
                        {
                            UserName = "kumar harsh",
                            AccountNetwork = SocialNetworks.Quora
                        }
                 }
            };
        }

        [TestMethod]
        public void should_return_Null_if_input_length_less_than_two()
        {
            values = new object[] { "account" };
            var result = _sut.Convert(values, values.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().Be(null);
        }

        [TestMethod]
        public void should_return_collection_of_quora_network_account_if_input_contains_list_and_network_is_quora()
        {
            values = new object[] { LstDominatorAccountModel, SocialNetworks.Quora };
            var result = (IEnumerable<DominatorAccountModel>)_sut.Convert(values, values.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().NotBeEmpty().And.HaveCount(2);
        }

        [TestMethod]
        public void should_return_collection_of_all_network_account_if_input_contains_list_and_network_is_social()
        {
            values = new object[] { LstDominatorAccountModel, SocialNetworks.Social };
            var result = (IEnumerable<DominatorAccountModel>)_sut.Convert(values, values.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().NotBeEmpty().And.HaveCount(LstDominatorAccountModel.Count);
        }

        [TestMethod]
        public void should_return_empty_collection_if_input_contains_list_and_network_is_Instagram()
        {
            values = new object[] { LstDominatorAccountModel, SocialNetworks.Instagram };
            var result = (IEnumerable<DominatorAccountModel>)_sut.Convert(values, values.GetType(), null, CultureInfo.CurrentUICulture);
            result.Should().BeEmpty();
        }
    }
}
