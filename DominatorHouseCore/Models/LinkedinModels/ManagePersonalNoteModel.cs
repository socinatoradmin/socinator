using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Models.LinkedinModel
{

    public class ManagePersonalNoteModel : BindableBase
    {
        public ManagePersonalNoteModel()
        {
            _PersonalNoteId = Utilities.GetGuid();
        }

        private string _PersonalNoteId;

        public string PersonalNoteId
        {
            get => _PersonalNoteId;
            set
            {
                if (value == _PersonalNoteId)
                    return;
                SetProperty(ref _PersonalNoteId, value);
            }
        }

        private string _personalNoteText;

        public string PersonalNoteText
        {
            get => _personalNoteText;
            set
            {
                if (value == _personalNoteText)
                    return;
                SetProperty(ref _personalNoteText, value);
            }
        }

    }
}
