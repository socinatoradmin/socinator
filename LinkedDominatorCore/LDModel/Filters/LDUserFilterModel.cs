using System.Collections.Generic;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace LinkedDominatorCore.LDModel.Filters
{
    [ProtoContract]
    public class LDUserFilterModel : BindableBase
    {
        private string _invalidWords;

        private bool _IsCheckedEducationCheckbox;

        private bool _IsCheckedExperienceCheckbox;


        private bool _IscheckedFilterMinimumCharacterInBio;
        private bool _IsCheckedFilterProfileImageCheckbox;

        private bool _IsCheckedHasInvalidWordsCheckBox;


        private bool _isCheckedHasValidWordsCheckBox;

        private bool _IsCheckedMinimumConnectionsCheckbox;

        private bool _IscheckedMinimumSkillsCount;


        private bool _IsCheckedRangeofConnectionsCheckbox;

        /// <summary>
        ///     here we also scrap data of skills and industry knowledge with bio details
        /// </summary>
        private bool _isEnableAdvanceBioAndSkills;

        private List<string> _LstInvalidWords;

        private List<string> _lstValidWords;

        private int _MaximumConnections = 500;

        private int _MinimumCharacterInBio = 5;


        private int _MinimumConnections = 1;

        private int _MinimumSkillsCount = 5;

        private BlacklistSettings _restrictedGrouplist = new BlacklistSettings();

        private BlacklistSettings _restrictedProfilelist = new BlacklistSettings();

        [ProtoMember(17)] private string _validWords;

        [ProtoMember(1)]
        public bool IsCheckedFilterProfileImageCheckbox
        {
            get => _IsCheckedFilterProfileImageCheckbox;
            set => SetProperty(ref _IsCheckedFilterProfileImageCheckbox, value);
        }

        [ProtoMember(2)]
        public bool IsCheckedMinimumConnectionsCheckbox
        {
            get => _IsCheckedMinimumConnectionsCheckbox;
            set
            {
                if (value)
                    IsCheckedRangeofConnectionsCheckbox = false;
                SetProperty(ref _IsCheckedMinimumConnectionsCheckbox, value);
            }
        }

        [ProtoMember(3)]
        public bool IsCheckedRangeofConnectionsCheckbox
        {
            get => _IsCheckedRangeofConnectionsCheckbox;
            set
            {
                if (value)
                    IsCheckedMinimumConnectionsCheckbox = false;
                SetProperty(ref _IsCheckedRangeofConnectionsCheckbox, value);
            }
        }

        [ProtoMember(4)]
        public int MinimumConnections
        {
            get => _MinimumConnections;
            set
            {
                if (value == _MinimumConnections)
                    return;
                SetProperty(ref _MinimumConnections, value);
            }
        }

        [ProtoMember(5)]
        public int MaximumConnections
        {
            get => _MaximumConnections;
            set
            {
                if (value == _MaximumConnections)
                    return;
                SetProperty(ref _MaximumConnections, value);
            }
        }

        [ProtoMember(6)]
        public bool IscheckedFilterMinimumCharacterInBio
        {
            get => _IscheckedFilterMinimumCharacterInBio;
            set
            {
                if (value == _IscheckedFilterMinimumCharacterInBio) return;
                SetProperty(ref _IscheckedFilterMinimumCharacterInBio, value);
            }
        }

        [ProtoMember(7)]
        public int MinimumCharacterInBio
        {
            get => _MinimumCharacterInBio;
            set
            {
                if (value == _MinimumCharacterInBio)
                    return;
                SetProperty(ref _MinimumCharacterInBio, value);
            }
        }

        [ProtoMember(8)]
        public bool IsCheckedHasInvalidWordsCheckBox
        {
            get => _IsCheckedHasInvalidWordsCheckBox;
            set
            {
                if (value == _IsCheckedHasInvalidWordsCheckBox) return;
                SetProperty(ref _IsCheckedHasInvalidWordsCheckBox, value);
            }
        }

        [ProtoMember(9)]
        public string InvalidWords
        {
            get => _invalidWords;
            set
            {
                if (value == _invalidWords)
                    return;
                SetProperty(ref _invalidWords, value);
            }
        }

        public List<string> LstInvalidWords
        {
            get => _LstInvalidWords;
            set
            {
                if (value == _LstInvalidWords)
                    return;
                SetProperty(ref _LstInvalidWords, value);
            }
        }

        [ProtoMember(10)]
        public bool IsCheckedExperienceCheckbox
        {
            get => _IsCheckedExperienceCheckbox;
            set
            {
                if (value == _IsCheckedExperienceCheckbox) return;
                SetProperty(ref _IsCheckedExperienceCheckbox, value);
            }
        }

        [ProtoMember(11)]
        public bool IsCheckedEducationCheckbox
        {
            get => _IsCheckedEducationCheckbox;
            set
            {
                if (value == _IsCheckedEducationCheckbox) return;
                SetProperty(ref _IsCheckedEducationCheckbox, value);
            }
        }

        [ProtoMember(12)]
        public bool IscheckedMinimumSkillsCount
        {
            get => _IscheckedMinimumSkillsCount;
            set
            {
                if (value == _IscheckedMinimumSkillsCount) return;
                SetProperty(ref _IscheckedMinimumSkillsCount, value);
            }
        }

        [ProtoMember(13)]
        public int MinimumSkillsCount
        {
            get => _MinimumSkillsCount;
            set
            {
                if (value == _MinimumSkillsCount)
                    return;
                SetProperty(ref _MinimumSkillsCount, value);
            }
        }

        [ProtoMember(14)]
        public BlacklistSettings RestrictedGrouplist
        {
            get => _restrictedGrouplist;
            set
            {
                if (value == _restrictedGrouplist) return;
                SetProperty(ref _restrictedGrouplist, value);
            }
        }

        [ProtoMember(15)]
        public BlacklistSettings RestrictedProfilelist
        {
            get => _restrictedProfilelist;
            set
            {
                if (value == _restrictedProfilelist) return;
                SetProperty(ref _restrictedProfilelist, value);
            }
        }

        [ProtoMember(16)]
        public bool IsCheckedHasValidWordsCheckBox
        {
            get => _isCheckedHasValidWordsCheckBox;
            set
            {
                if (value == _isCheckedHasValidWordsCheckBox) return;
                SetProperty(ref _isCheckedHasValidWordsCheckBox, value);
            }
        }

        public string ValidWords
        {
            get => _validWords;
            set => SetProperty(ref _validWords, value);
        }

        [ProtoMember(18)]
        public List<string> LstValidWords
        {
            get => _lstValidWords;
            set => SetProperty(ref _lstValidWords, value);
        }

        [ProtoMember(19)]
        public bool IsEnableAdvanceBioAndSkills
        {
            get => _isEnableAdvanceBioAndSkills;
            set => SetProperty(ref _isEnableAdvanceBioAndSkills, value);
        }
    }
}