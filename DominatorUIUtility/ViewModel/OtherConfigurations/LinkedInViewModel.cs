using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Diagnostics;
using System.Windows;

namespace DominatorUIUtility.ViewModel.OtherConfigurations
{
    public class LinkedInViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IGenericFileManager _genericFileManager;

        public LinkedInViewModel(IGenericFileManager genericFileManager) : base("LangKeyLinkedIn",
            "LinkedInControlTemplate")
        {
            _genericFileManager = genericFileManager;
            SaveCmd = new DelegateCommand(Save);
            LinkedInModel =
                _genericFileManager.GetModel<LinkedInModel>(ConstantVariable.GetOtherLinkedInSettingsFile()) ??
                new LinkedInModel();
        }

        public LinkedInModel LinkedInModel { get; }
        public DelegateCommand SaveCmd { get; }

        private void Save()
        {
            if (_genericFileManager.Overrride(LinkedInModel, ConstantVariable.GetOtherLinkedInSettingsFile()))
                Dialog.ShowDialog("LangKeySuccess".FromResourceDictionary(),
                    "LangKeyLinkedInConfigurationSaved".FromResourceDictionary());
        }
    }
    public class RedditOtherConfigViewModel : BaseTabViewModel, IOtherConfigurationViewModel
    {
        private readonly IGenericFileManager _genericFileManager;
        public RedditOtherConfigModel RedditOtherConfigModel { get; }
        public DelegateCommand SaveCmd { get; }
        public RedditOtherConfigViewModel(IGenericFileManager genericFileManager) 
            : base("LangKeyReddit", "RedditControlTemplate")
        {
            _genericFileManager = genericFileManager;
            SaveCmd = new DelegateCommand(Save);
            RedditOtherConfigModel =
                _genericFileManager.GetModel<RedditOtherConfigModel>(ConstantVariable.GetOtherRedditSettingsFile()) ??
                new RedditOtherConfigModel();
        }
        private void Save()
        {
            
            try
            {
                if (_genericFileManager.Overrride(RedditOtherConfigModel, ConstantVariable.GetOtherRedditSettingsFile()))
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var result = Dialog.ShowCustomDialog("LangKeySuccess".FromResourceDictionary(),
                            "LangKeyConfirmRestartAfterSaveSetting".FromResourceDictionary(),
                            "LangKeyRestartNow".FromResourceDictionary(),
                            "LangKeyRestartLater".FromResourceDictionary());
                        if (result == MessageDialogResult.Affirmative)
                        {
                            Application.Current.Shutdown();
                            Process.Start(Application.ResourceAssembly.Location);
                            Process.GetCurrentProcess().Kill();
                            Environment.Exit(0);
                        }
                    });
                }
            }
            catch { }
        }
    }
}