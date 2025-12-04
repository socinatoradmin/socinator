using System;
using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;

namespace PinDominatorCore.PDModel
{
    public class BoardInfo : BindableBase, ICloneable, IPost
    {
        private string _boardDescription;
        private string _boardName;
        private string _section;

        private string _category;

        private int _selectedIndex;
        public List<string> SectionList { get; set; } = new List<string>();
        public string BoardName
        {
            get => _boardName;
            set
            {
                if (_boardName != null && _boardName == value)
                    return;
                SetProperty(ref _boardName, value);
            }
        }
        public string Section
        {
            get => _section;
            set
            {
                if (_section != null && _section == value)
                    return;
                SetProperty(ref _section, value);
            }
        }
        public string BoardDescription
        {
            get => _boardDescription;
            set
            {
                if (_boardDescription != null && _boardDescription == value)
                    return;
                SetProperty(ref _boardDescription, value);
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                if (_category != null && _category == value)
                    return;
                SetProperty(ref _category, value);
            }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (_selectedIndex != 0 && _selectedIndex == value)
                    return;
                SetProperty(ref _selectedIndex, value);
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }


        public string Id { get; set; }
        public string Caption { get; set; }
        public string Code { get; set; }

        private bool _chkSpintax;

        public bool ChkSpintax
        {
            get { return _chkSpintax; }
            set
            {
                if (_chkSpintax == value)
                    return;
                SetProperty(ref _chkSpintax, value);

                if (!value)
                    SkipCommonCategory = false;
            }
        }

        private bool _skipCommonCategory;

        public bool SkipCommonCategory
        {
            get { return _skipCommonCategory; }
            set
            {
                if (_skipCommonCategory == value)
                    return;
                SetProperty(ref _skipCommonCategory, value);

                if (value)
                    UseRandomCategory = !value;
            }
        }

        private bool _useRandomCategory;

        public bool UseRandomCategory
        {
            get { return _useRandomCategory; }
            set
            {
                if (_useRandomCategory == value)
                    return;
                SetProperty(ref _useRandomCategory, value);

                if (value)
                    SkipCommonCategory = !value;
            }
        }
        private bool _keepBoardSecret;
        public bool KeepBoardSecret
        {
            get { return _keepBoardSecret; }
            set
            {
                if (_keepBoardSecret == value) return;
                SetProperty(ref _keepBoardSecret, value);
            }
        }
        public string LearnMoreBoardPolicy => "https://help.pinterest.com/en-gb/article/change-board-privacy?source=secret_create";
    }
}