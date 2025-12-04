using DominatorHouseCore.Converters;
using DominatorHouseCore.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;


namespace DominatorHouseCore.UnitTests.Tests.Converters
{
    [TestClass]
    public class ProxyManagerSourceConverterTest
    {
        ProxyManagerSourceConverter _sut;
        object[] value;

        [TestInitialize]
        public void SetUp()
        {
            _sut = new ProxyManagerSourceConverter();
        }
        [TestMethod]
        public void should_return_empty_colection_if_proxyname_is_sample_and_value_need_to_filter_by_showProxiesWithError_and_shouldUnssignedProxies_and_filterByGroup_and_by_proxyname_or_proxy_Ip_as_filter()
        {
             var proxyList = new List<ProxyManagerModel> {
                                              new ProxyManagerModel
                                              {
                                                  Status="Fail",
                                                  AccountsAssignedto=new ObservableCollection<AccountAssign>(),
                                                  AccountProxy=new Proxy
                                                  {
                                                      ProxyGroup="group",
                                                      ProxyIp ="1.0.0.2",
                                                      ProxyName="sample"
                                                  }
                                              }
                                             };
            value = new object[] {
                                    proxyList,
                                    true,
                                    true,
                                    true,
                                    "group",
                                    "filter",
                                    false
                                };
            
            IEnumerable<ProxyManagerModel> result =(IEnumerable<ProxyManagerModel>)_sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().BeEmpty();
        }
        [TestMethod]
        public void should_return_proxyList_of_proxy_with_name_proxy_if_proxyname_is_proxy_and_value_need_to_filter_by_showProxiesWithError_and_shouldUnssignedProxies_and_filterByGroup_and_by_proxyname_or_proxy_Ip_as_filter()
        {
            var proxyList = new List<ProxyManagerModel> {
                                              new ProxyManagerModel
                                              {
                                                  Status="Fail",
                                                  AccountsAssignedto=new ObservableCollection<AccountAssign>(),
                                                  AccountProxy=new Proxy
                                                  {
                                                      ProxyGroup="group",
                                                      ProxyIp ="1.0.0.2",
                                                      ProxyName="proxy"
                                                  }
                                              }
                                             };
            value = new object[] {
                                    proxyList,
                                    true,
                                    true,
                                    true,
                                    "group",
                                    "proxy",
                                    false
                                };

            IEnumerable<ProxyManagerModel> result = (IEnumerable<ProxyManagerModel>)_sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().NotBeEmpty().And.HaveCount(1);
        }
        [TestMethod]
        public void should_return_proxyList_of_proxy_with_name_proxy_if_proxyname_is_proxy_and_value_need_to_filter_by_shouldUnssignedProxies_and_filterByGroup_and_by_proxyname_or_proxy_Ip_as_filter()
        {
            var proxyList = new List<ProxyManagerModel> {
                                              new ProxyManagerModel
                                              {
                                                  Status="Sucess",
                                                  AccountsAssignedto=new ObservableCollection<AccountAssign>(),
                                                  AccountProxy=new Proxy
                                                  {
                                                      ProxyGroup="group",
                                                      ProxyIp ="1.0.0.2",
                                                      ProxyName="proxy"
                                                  }
                                              }
                                             };
            value = new object[] {
                                    proxyList,
                                    false,
                                    true,
                                    true,
                                    "group",
                                    "proxy",
                                    false
                                };

            IEnumerable<ProxyManagerModel> result = (IEnumerable<ProxyManagerModel>)_sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().NotBeEmpty().And.HaveCount(1);
        }
        [TestMethod]
        public void should_return_proxyList_of_failed_proxy_if_value_is_need_to_filter_by_failed_proxy()
        {
            var proxyList = new List<ProxyManagerModel> {
                                              new ProxyManagerModel
                                              {
                                                  Status="Fail",
                                                  AccountsAssignedto=new ObservableCollection<AccountAssign>(),
                                                  AccountProxy=new Proxy
                                                  {
                                                      ProxyGroup="group",
                                                      ProxyIp ="1.0.0.2",
                                                      ProxyName="sample"
                                                  }
                                              }
                                             };
            value = new object[] {
                                    proxyList,
                                    true,
                                    false,
                                    false,
                                    "group",
                                    "",
                                    false
                                };

            IEnumerable<ProxyManagerModel> result = (IEnumerable<ProxyManagerModel>)_sut.Convert(value, null, null, CultureInfo.CurrentUICulture);
            result.Should().NotBeEmpty().And.HaveCount(1);
        }
    }
}
