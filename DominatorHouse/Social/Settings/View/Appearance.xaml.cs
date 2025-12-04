using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using MahApps.Metro;
using Socinator.Social.Settings.ViewModel;

namespace DominatorHouse.Social.Settings.View
{
    /// <summary>
    /// Interaction logic for Appearance.xaml
    /// </summary>
    public partial class Appearance
    {
        private readonly AppearanceViewModel _objAppearanceViewModel = new AppearanceViewModel();

        public Appearance()
        {
            InitializeComponent();
            MainGrid.DataContext = _objAppearanceViewModel;
        }
       
        private void lstcolor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (ColorsCollection)lstcolor.SelectedItem;
            _objAppearanceViewModel.SelectedRecentColor = null;
            var accentColor = _objAppearanceViewModel.lstColorsCollection.SingleOrDefault(x => x.Value == selectedItem.Value);
            _objAppearanceViewModel.lstRecentColorsCollection.Add(accentColor);
            _objAppearanceViewModel.lstRecentColorsCollection = new ObservableCollection<ColorsCollection>(_objAppearanceViewModel.lstRecentColorsCollection.Distinct());

            ChangeAppearance(sender);
        }

        private void lsttheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeAppearance(sender);           
        }

        private void lstRecentcolor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeAppearance(sender);
        }
        public void ChangeAppearance(object sender)
        {
            try
            {
                Accent newAccent;
                AppTheme newAppTheme;
                string colorName;
                ColorsCollection selectedItem;
                string accentColor;

                var themeName = "Base" + ((lsttheme?.SelectedItem as ThemeCollection)?.Name ?? "Light");

                if (Equals(sender, lstcolor) || Equals(sender, lsttheme))
                {
                    colorName = ((ColorsCollection)lstcolor.SelectedItem).Name;
                    selectedItem = (ColorsCollection)lstcolor.SelectedItem;
                    accentColor = _objAppearanceViewModel.lstColorsCollection.SingleOrDefault(x => x.Value == selectedItem.Value)?.Name;
                }
                else
                {
                    colorName = ((ColorsCollection)lstRecentcolor.SelectedItem).Name;
                    selectedItem = (ColorsCollection)lstRecentcolor.SelectedItem;
                    accentColor = _objAppearanceViewModel.lstRecentColorsCollection.FirstOrDefault(x => x.Value == selectedItem.Value)?.Name;
                }
                if (colorName == "Default")
                {
                    ThemeManager.AddAccent("PrussianBlue", new Uri("pack://application:,,,/DominatorUIUtility;component/Themes/PrussianBlue.xaml"));
                    newAccent = ThemeManager.GetAccent("PrussianBlue");
                    newAppTheme = ThemeManager.GetAppTheme(themeName);
                }
                else
                {
                    newAccent = ThemeManager.GetAccent(accentColor);
                    newAppTheme = ThemeManager.GetAppTheme(themeName);
                }
                ThemeManager.ChangeAppStyle(Application.Current, newAccent, newAppTheme);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            SaveCurrentTheme();
        }

        private void SaveCurrentTheme()
        {
            Configuration configuration = new Configuration
            {
                ConfigurationDate = DateTime.Now, ConfigurationType = "Theme"
            };
            var theme = new Themes
            {
                SelectedAccentColor = new AccentColors(_objAppearanceViewModel.SelectedAccentColor.Name, _objAppearanceViewModel.SelectedAccentColor.Value),
                SelectedTheme = new Theme(_objAppearanceViewModel.SelectedTheme.Name, _objAppearanceViewModel.SelectedTheme.Value)
            };
            configuration.ConfigurationSetting = Newtonsoft.Json.JsonConvert.SerializeObject(theme);
            ConfigFileManager.SaveConfig(configuration);
        }
    }
}
