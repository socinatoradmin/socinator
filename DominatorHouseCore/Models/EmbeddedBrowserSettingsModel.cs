#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class EmbeddedBrowserSettingsModel : BindableBase
    {
        private int _maxSemultaneousConnections;

        [ProtoMember(1)]
        public int MaxSemultaneousConnections
        {
            get => _maxSemultaneousConnections;
            set
            {
                if (value == _maxSemultaneousConnections)
                    return;
                SetProperty(ref _maxSemultaneousConnections, value);
            }
        }

        private bool _isMaximizeEmbaddedBrowserChecked;

        [ProtoMember(2)]
        public bool IsMaximizeEmbaddedBrowserChecked
        {
            get => _isMaximizeEmbaddedBrowserChecked;
            set
            {
                if (value == _isMaximizeEmbaddedBrowserChecked)
                    return;
                SetProperty(ref _isMaximizeEmbaddedBrowserChecked, value);
            }
        }

        private bool _isWaitMinOfChecked;

        [ProtoMember(3)]
        public bool IsWaitMinOfChecked
        {
            get => _isWaitMinOfChecked;
            set
            {
                if (value == _isWaitMinOfChecked)
                    return;
                SetProperty(ref _isWaitMinOfChecked, value);
            }
        }

        private int _waitMinOf;

        [ProtoMember(4)]
        public int WaitMinOf
        {
            get => _waitMinOf;
            set
            {
                if (value == _waitMinOf)
                    return;
                SetProperty(ref _waitMinOf, value);
            }
        }

        private bool _isForceKillEmbeddedBrowserChecked;

        [ProtoMember(5)]
        public bool IsForceKillEmbeddedBrowserChecked
        {
            get => _isForceKillEmbeddedBrowserChecked;
            set
            {
                if (value == _isForceKillEmbeddedBrowserChecked)
                    return;
                SetProperty(ref _isForceKillEmbeddedBrowserChecked, value);
            }
        }

        private bool _isAllowEmbeddedBrowserToDownloadFilesChecked;

        [ProtoMember(6)]
        public bool IsAllowEmbeddedBrowserToDownloadFilesChecked
        {
            get => _isAllowEmbeddedBrowserToDownloadFilesChecked;
            set
            {
                if (value == _isAllowEmbeddedBrowserToDownloadFilesChecked)
                    return;
                SetProperty(ref _isAllowEmbeddedBrowserToDownloadFilesChecked, value);
            }
        }

        private bool _isPreventEmbeddedBrowserFromOpeningPopupChecked;

        [ProtoMember(7)]
        public bool IsPreventEmbeddedBrowserFromOpeningPopupChecked
        {
            get => _isPreventEmbeddedBrowserFromOpeningPopupChecked;
            set
            {
                if (value == _isPreventEmbeddedBrowserFromOpeningPopupChecked)
                    return;
                SetProperty(ref _isPreventEmbeddedBrowserFromOpeningPopupChecked, value);
            }
        }

        private bool _isUseNewTextWritingSystemForFacebookChecked;

        [ProtoMember(8)]
        public bool IsUseNewTextWritingSystemForFacebookChecked
        {
            get => _isUseNewTextWritingSystemForFacebookChecked;
            set
            {
                if (value == _isUseNewTextWritingSystemForFacebookChecked)
                    return;
                SetProperty(ref _isUseNewTextWritingSystemForFacebookChecked, value);
            }
        }

        private bool _isStopSavingEmbeddedBrowserSSChecked;

        [ProtoMember(9)]
        public bool IsStopSavingEmbeddedBrowserSSChecked
        {
            get => _isStopSavingEmbeddedBrowserSSChecked;
            set
            {
                if (value == _isStopSavingEmbeddedBrowserSSChecked)
                    return;
                SetProperty(ref _isStopSavingEmbeddedBrowserSSChecked, value);
            }
        }

        private bool _isDisableGPUAccelerationChecked;

        [ProtoMember(10)]
        public bool IsDisableGPUAccelerationChecked
        {
            get => _isDisableGPUAccelerationChecked;
            set
            {
                if (value == _isDisableGPUAccelerationChecked)
                    return;
                SetProperty(ref _isDisableGPUAccelerationChecked, value);
            }
        }

        private bool _isKeepLowFramerateChecked;

        [ProtoMember(11)]
        public bool IsKeepLowFramerateChecked
        {
            get => _isKeepLowFramerateChecked;
            set
            {
                if (value == _isKeepLowFramerateChecked)
                    return;
                SetProperty(ref _isKeepLowFramerateChecked, value);
            }
        }

        private bool _isStartEmbeddedBrowserProcessesChecked;

        [ProtoMember(12)]
        public bool IsStartEmbeddedBrowserProcessesChecked
        {
            get => _isStartEmbeddedBrowserProcessesChecked;
            set
            {
                if (value == _isStartEmbeddedBrowserProcessesChecked)
                    return;
                SetProperty(ref _isStartEmbeddedBrowserProcessesChecked, value);
            }
        }

        private bool _isOpenMaxOfChecked;

        [ProtoMember(13)]
        public bool IsOpenMaxOfChecked
        {
            get => _isOpenMaxOfChecked;
            set
            {
                if (value == _isOpenMaxOfChecked)
                    return;
                SetProperty(ref _isOpenMaxOfChecked, value);
            }
        }

        private int _openMaxOf;

        [ProtoMember(14)]
        public int OpenMaxOf
        {
            get => _openMaxOf;
            set
            {
                if (value == _openMaxOf)
                    return;
                SetProperty(ref _openMaxOf, value);
            }
        }

        private int _deleteupTo;

        [ProtoMember(15)]
        public int DeleteupTo
        {
            get => _deleteupTo;
            set
            {
                if (value == _deleteupTo)
                    return;
                SetProperty(ref _deleteupTo, value);
            }
        }
    }
}