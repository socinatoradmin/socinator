#region

using System;
using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    /// <summary>
    ///     ContentSelectGroup is used in binding where content along with select options such as inside combobox content with
    ///     checkbox
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class ContentSelectGroup : BindableBase
    {
        private string _content;

        /// <summary>
        ///     Provide the content
        /// </summary>
        [ProtoMember(1)]
        public string Content
        {
            get => _content;
            set
            {
                if (_content != null && value == _content)
                    return;
                SetProperty(ref _content, value);
            }
        }


        private bool _isContentSelected;

        /// <summary>
        ///     IsContentSelected is used to give the status whether the content is selected or not
        /// </summary>
        [ProtoMember(2)]
        public bool IsContentSelected
        {
            get => _isContentSelected;
            set
            {
                if (value == _isContentSelected)
                    return;
                SetProperty(ref _isContentSelected, value);
            }
        }


        public override bool Equals(object obj)
        {
            var other = obj as ContentSelectGroup;
            if (other == null) return false;

            if (other.Content == null && Content == null) return true;

            return Content?.Equals(other.Content, StringComparison.CurrentCultureIgnoreCase) ?? false;
        }

        public override int GetHashCode()
        {
            return Content.GetHashCode();
        }
    }
}