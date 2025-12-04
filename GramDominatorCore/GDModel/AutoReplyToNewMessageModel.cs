using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using GramDominatorCore.GDLibrary;
using ProtoBuf;
using DominatorHouseCore.Utility;

namespace GramDominatorCore.GDModel
{
    [ProtoContract]
    public class AutoReplyToNewMessageModel : ModuleSetting, IGeneralSettings
    {
        #region Private Members

        private bool _isReplyToMessagesThatContainSpecificWord﻿Checked;

        private bool _isReplyToPendingMessages﻿﻿Checked = true;

        private bool _isReplyToAllMessages﻿﻿Checked;

        private string _specificWord;

        private List<string> _lstMessage = new List<string>();

        #endregion

        [ProtoMember(1)]
        JobConfiguration IGeneralSettings.JobConfiguration { get; set; }

        [ProtoMember(2)]
        public override UserFilterModel UserFilterModel { get; set; } = new UserFilterModel();

        [ProtoMember(4)]
        public bool IsReplyToMessagesThatContainSpecificWord﻿Checked
        {
            get
            {
                return _isReplyToMessagesThatContainSpecificWord﻿Checked;
            }
            set
            {
                if (_isReplyToMessagesThatContainSpecificWord﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToMessagesThatContainSpecificWord﻿Checked, value);

            }
        }

        [ProtoMember(5)]
        public bool IsReplyToPendingMessages﻿﻿Checked
        {
            get
            {
                return _isReplyToPendingMessages﻿﻿Checked;
            }
            set
            {
                if (_isReplyToPendingMessages﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToPendingMessages﻿﻿Checked, value);

            }
        }

        [ProtoMember(6)]
        public bool IsReplyToAllMessagesChecked
        {
            get
            {
                return _isReplyToAllMessages﻿﻿Checked;
            }
            set
            {
                if (_isReplyToAllMessages﻿﻿Checked == value)
                    return;
                SetProperty(ref _isReplyToAllMessages﻿﻿Checked, value);

            }
        }

        [ProtoMember(7)]
        public string SpecificWord
        {
            get
            {
                return _specificWord;
            }
            set
            {
                if (_specificWord == value)
                    return;
                SetProperty(ref _specificWord, value);

            }
        }

        [ProtoMember(9)]
        public List<string> LstMessage
        {
            get
            {
                return _lstMessage;
            }
            set
            {
                if (_lstMessage == value)
                    return;
                SetProperty(ref _lstMessage, value);
            }
        }

        [ProtoMember(10)]
        public override MangeBlacklist.SkipBlacklist SkipBlacklist { get; set; } = new MangeBlacklist.SkipBlacklist();

        [ProtoMember(11)]
        public ObservableCollection<ManageMessagesModel> LstDisplayManageMessageModel { get; set; } = new ObservableCollection<ManageMessagesModel>();

        public ManageMessagesModel ManageMessagesModel { get; set; } = new ManageMessagesModel();


        #region Set Job Configuration speed
        public JobConfiguration SlowSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(13, 20),
            ActivitiesPerHour = new RangeUtilities(1, 2),
            ActivitiesPerWeek = new RangeUtilities(80, 120),
            ActivitiesPerJob = new RangeUtilities(1, 2),
            DelayBetweenJobs = new RangeUtilities(88, 132),
            DelayBetweenActivity = new RangeUtilities(30, 60),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration MediumSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(33, 50),
            ActivitiesPerHour = new RangeUtilities(3, 5),
            ActivitiesPerWeek = new RangeUtilities(200, 300),
            ActivitiesPerJob = new RangeUtilities(4, 6),
            DelayBetweenJobs = new RangeUtilities(87, 130),
            DelayBetweenActivity = new RangeUtilities(25, 50),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };

        public JobConfiguration FastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(53, 80),
            ActivitiesPerHour = new RangeUtilities(5, 8),
            ActivitiesPerWeek = new RangeUtilities(320, 480),
            ActivitiesPerJob = new RangeUtilities(6, 10),
            DelayBetweenJobs = new RangeUtilities(86, 130),
            DelayBetweenActivity = new RangeUtilities(15, 30),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };


        public JobConfiguration SuperfastSpeed = new JobConfiguration()
        {
            ActivitiesPerDay = new RangeUtilities(66, 100),
            ActivitiesPerHour = new RangeUtilities(6, 10),
            ActivitiesPerWeek = new RangeUtilities(400, 600),
            ActivitiesPerJob = new RangeUtilities(8, 12),
            DelayBetweenJobs = new RangeUtilities(87, 131),
            DelayBetweenActivity = new RangeUtilities(8, 15),
            DelayBetweenAccounts = new RangeUtilities(0, 0)
        };
        #endregion
    }
}
