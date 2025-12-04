#region

using DominatorHouseCore.Diagnostics.LogHelper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel.Common;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public interface ILogViewModel
    {
        void Add(string message, LogLevel logLevel);
        string LogType { get; set; }
    }

    public class LogViewModel : BindableBase, ILogViewModel
    {
        private const int MaxLogSize = 1000;
        private LoggerModel _selected;
        private SocialNetworks? _selectedNetwork;
        private DominatorAccountModel _selectedAccount;
        private ObservableCollection<LoggerModel> _logs;

        public object SyncObject { get; }

        public ObservableCollection<LoggerModel> Logs
        {
            get => _logs;
            set
            {
                SetProperty(ref _logs, value, nameof(Logs));
                LogCollection?.View.Refresh();
            }
        }

        public LoggerModel Selected
        {
            get => _selected;
            set
            {
                SetProperty(ref _selected, value, nameof(Selected));
                CopyCmd.RaiseCanExecuteChanged();
            }
        }

        public SocialNetworks? SelectedNetwork
        {
            get => _selectedNetwork;
            set
            {
                if (_selectedNetwork == value)
                    return;
                SetProperty(ref _selectedNetwork, value, nameof(SelectedNetwork));
                OnPropertyChanged(nameof(NetworkIsSelected));
                LogCollection.View.Refresh();
                ActivityTypes.Selected = null;
            }
        }

        public DominatorAccountModel SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                if (_selectedAccount == value)
                    return;
                SetProperty(ref _selectedAccount, value, nameof(SelectedNetwork));
                LogCollection.View.Refresh();
            }
        }

        public bool NetworkIsSelected => SelectedNetwork.HasValue && SelectedNetwork != SocialNetworks.Social;

        public DelegateCommand CopyCmd { get; set; }
        public SelectableViewModel<ActivityType?> ActivityTypes { get; }

        public LogViewModel()
        {
            SyncObject = new object();
            Logs = new ObservableCollection<LoggerModel>();
            CopyCmd = new DelegateCommand(Copy, CanCopy);
            BindingOperations.EnableCollectionSynchronization(Logs, SyncObject);
            ActivityTypes =
                new SelectableViewModel<ActivityType?>(Enum.GetValues(typeof(ActivityType)).Cast<ActivityType?>());
            LogCollection = new CollectionViewSource();
            ActivityTypes.ItemSelected += OnActivityTypeSlectionChange;
            LogCollection.Source = Logs;
            LogCollection.Filter += FilterLog;
        }

        private void OnActivityTypeSlectionChange(object sender, ActivityType? e)
        {
            LogCollection.View.Refresh();
        }

        private string _logType = "Info";

        public string LogType
        {
            get => _logType;
            set
            {
                SetProperty(ref _logType, value);
                LogCollection?.View.Refresh();
            }
        }

        private readonly CollectionViewSource LogCollection;
        private ICollectionView _sourceCollection;

        public ICollectionView SourceCollection
        {
            get => LogCollection.View;
            set => SetProperty(ref _sourceCollection, value);
        }

        private void FilterLog(object sender, FilterEventArgs e)
        {
            var logs = e.Item as LoggerModel;

            if (logs?.AccountCampaign != null && logs.AccountCampaign.Replace("\"","").Equals(SelectedAccount?.AccountBaseModel.UserName,
                    StringComparison.InvariantCultureIgnoreCase))
            {
                e.Accepted = true;
            }
            else if (SelectedAccount != null)
            {
                e.Accepted = false;
                return;
            }

            if (string.IsNullOrEmpty(SelectedNetwork?.ToString())
                && LogType.Equals(logs.LogType, StringComparison.InvariantCultureIgnoreCase))
            {
                e.Accepted = true;
                return;
            }

            if (logs.Network.Equals(SelectedNetwork?.ToString(), StringComparison.InvariantCultureIgnoreCase)
                && LogType.Equals(logs.LogType, StringComparison.InvariantCultureIgnoreCase))
                e.Accepted = true;
            else e.Accepted = false;
            if (!string.IsNullOrEmpty(ActivityTypes.Selected?.ToString()))
                if (logs.Network.Equals(SelectedNetwork?.ToString(), StringComparison.InvariantCultureIgnoreCase) &&
                    logs.ActivityType != null && logs.ActivityType.Equals(ActivityTypes.Selected?.ToString(),
                        StringComparison.InvariantCultureIgnoreCase))
                    e.Accepted = true;
                else
                    e.Accepted = false;
        }

        private bool CanCopy()
        {
            return Selected != null;
        }


        public void Add(string message, LogLevel logLevel)
        {
            lock (SyncObject)
            {
                var messages = message.Split('\t');

                var log = messages.Length == 5
                    ? new LoggerModel
                    {
                        DateTime = $"{DateTime.Now.ToString("dd-MMM-yyyy hh:mm:yyyy tt")}",
                        Network = messages[0].Trim(),
                        AccountCampaign = messages[1].Trim(),
                        ActivityType = messages[2].Trim(),
                        Message = messages[3].Trim(),
                        MessageCode = messages[4].Trim(),
                        LogType = logLevel.ToString()
                    }
                    : new LoggerModel
                    {
                        Network = SocialNetworks.Social.ToString(),
                        DateTime = $"{DateTime.Now.ToString("dd-MMM-yyyy hh:mm:yyyy tt")}",
                        Message = message,
                        LogType = logLevel.ToString()
                    };

                Logs.Insert(0, log);

                OnPropertyChanged(nameof(Logs));

                if (Logs.Count > MaxLogSize)
                    Logs.RemoveAt(Logs.Count - 1);
            }
        }

        private void Copy()
        {
            if (Selected != null)
            {
                try
                {
                    Clipboard.SetText(Selected.Message);
                    ToasterNotification.ShowSuccess("LangKeyMessageCopied".FromResourceDictionary());
                }
                catch(Exception ex) {
                    ex.DebugLog();
                }
            }
        }
    }
}