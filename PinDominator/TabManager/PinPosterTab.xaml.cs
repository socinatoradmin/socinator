using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;
using PinDominator.PDViews.PinPoster;

namespace PinDominator.TabManager
{
    /// <summary>
    ///     Interaction logic for PinPosterTab.xaml
    /// </summary>
    public partial class PinPosterTab : UserControl
    {
        public PinPosterTab()
        {
            InitializeComponent();

            var tabItems = new List<TabItemTemplates>
            {
                //new TabItemTemplates
                //{
                //    Title=FindResource("LangKeyPoster").ToString(),
                //    Content=new Lazy<UserControl>(Poster.GetSingeltonObjectPoster)
                //},
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyRepin") == null
                        ? "Repin"
                        : Application.Current.FindResource("LangKeyRepin")?.ToString(),
                    Content = new Lazy<UserControl>(RePin.GetSingletonObjectRePin﻿)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyDelete") == null
                        ? "Delete"
                        : Application.Current.FindResource("LangKeyDelete")?.ToString(),
                    Content = new Lazy<UserControl>(DeletePins.GetSingletonObjectDeletePins)
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyEdit") == null
                        ? "Edit"
                        : Application.Current.FindResource("LangKeyEdit")?.ToString(),
                    Content = new Lazy<UserControl>(EditPin.GetSingletonObjectEditPin)
                }
                //new TabItemTemplates
                //{
                //    Title=FindResource("LangKeyAutoPostsList").ToString(),
                //    Content=new Lazy<UserControl>(()=>new AutoPostsList())
                //}
            };

            PinPosterTabs.ItemsSource = tabItems;
        }

        private static PinPosterTab ObjPinPosterTab { get; set; }

        /// <summary>
        ///     GetSingeltonObjectPinPosterTab is used to get the object of the current user control,
        ///     if object is already created then its won't create a new object, simply it returns already created object,
        ///     otherwise create a new object and then its return.
        /// </summary>
        /// <returns>Current UI class object</returns>
        public static PinPosterTab GetSingeltonObjectPinPosterTab()
        {
            return ObjPinPosterTab ?? (ObjPinPosterTab = new PinPosterTab());
        }


        public void SetIndex(int index)
        {
            PinPosterTabs.SelectedIndex = index;
        }
    }
}