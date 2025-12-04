using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.Views.Publisher.AdvancedSettings;

namespace DominatorUIUtility.Views.Publisher
{
    /// <summary>
    ///     Interaction logic for CampaignsAdvanceSetting.xaml
    /// </summary>
    public partial class CampaignsAdvanceSetting : UserControl
    {
        private readonly IGenericFileManager _genericFileManager;

        public CampaignsAdvanceSetting()
        {
            _genericFileManager = InstanceProvider.GetInstance<IGenericFileManager>();
            InitializeComponent();
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title = FindResource("LangKeyGeneral").ToString(),
                    Content = new Lazy<UserControl>(General.GetSingeltonGeneralObject)
                }
            };

            #region Facebook

            if (SocinatorInitialize.IsNetworkAvailable(SocialNetworks.Facebook))
                tabItems.Add(new TabItemTemplates
                {
                    Title = FindResource("LangKeyFacebook").ToString(),
                    Content = new Lazy<UserControl>(Facebook.GetSingeltonFacebookObject)
                });

            #endregion

            #region Gplus

            //if (SocinatorInitialize.IsNetworkAvailable(SocialNetworks.Gplus))
            //{
            //    tabItems.Add(new TabItemTemplates
            //    {
            //        Title = FindResource("LangKeyGoogle+").ToString(),
            //        Content = new Lazy<UserControl>(GooglePlus.GetSingeltonGooglePlusObject)
            //    });
            //}

            #endregion

            #region Pinterest

            if (SocinatorInitialize.IsNetworkAvailable(SocialNetworks.Pinterest))
                tabItems.Add(new TabItemTemplates
                {
                    Title = FindResource("LangKeyPinterest").ToString(),
                    Content = new Lazy<UserControl>(Pinterest.GetSingeltonPinterestObject)
                });

            #endregion

            #region Twitter

            if (SocinatorInitialize.IsNetworkAvailable(SocialNetworks.Twitter))
                tabItems.Add(new TabItemTemplates
                {
                    Title = FindResource("LangKeyTwitter").ToString(),
                    Content = new Lazy<UserControl>(Twitter.GetSingletonTwitterObject)
                });

            #endregion

            #region Instagram

            if (SocinatorInitialize.IsNetworkAvailable(SocialNetworks.Instagram))
                tabItems.Add(new TabItemTemplates
                {
                    Title = FindResource("LangKeyInstagram").ToString(),
                    Content = new Lazy<UserControl>(Instagram.GetSingeltonInstagramObject)
                });

            #endregion

            #region Tumblr

            if (SocinatorInitialize.IsNetworkAvailable(SocialNetworks.Tumblr))
                tabItems.Add(new TabItemTemplates
                {
                    Title = FindResource("LangKeyTumblr").ToString(),
                    Content = new Lazy<UserControl>(Tumblr.GetSingeltonTumblr)
                });

            #endregion

            #region Reddit

            if (SocinatorInitialize.IsNetworkAvailable(SocialNetworks.Reddit))
                tabItems.Add(new TabItemTemplates
                {
                    Title = FindResource("LangKeyReddit").ToString(),
                    Content = new Lazy<UserControl>(Reddit.GetSingeltonRedditObject)
                });

            #endregion

            CampaignsAdvanceSettingTab.ItemsSource = tabItems;
            AdvanceSetting = new AdvanceSetting();
        }

        public AdvanceSetting AdvanceSetting { get; set; }

        public void AddUpdateDetails<T>(T moduleToUpdate, T updatedModel,
            List<T> lstModels, string file, SocialNetworks networks) where T : class
        {
            if (moduleToUpdate == null)
            {
                _genericFileManager.AddModule(updatedModel,
                    ConstantVariable.GetPublisherOtherConfigFile(networks));
            }
            else
            {
                var moduleToUpdateIndex = lstModels.IndexOf(moduleToUpdate);
                lstModels[moduleToUpdateIndex] = updatedModel;

                _genericFileManager.UpdateModuleDetails(lstModels, file);
            }
        }
    }
}