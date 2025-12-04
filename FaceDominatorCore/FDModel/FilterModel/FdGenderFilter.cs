/*
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace FaceDominatorCore.FDModel.FilterModel
{

    public interface IFdGenderFilter
    {
        
//        bool IsFilterByGender { get; set; }
//
//        bool SelectMaleUser { get; set; }
//
//        bool SelectFemaleUser { get; set; }

       
    }


    [ProtoContract]
    public class FdGenderFilterModel: BindableBase, IFdGenderFilter
    {

        private bool _isFilterByGender;

        [ProtoMember(1)]      
        public bool IsFilterByGender
        {
            get
            {
                return _isFilterByGender;
            }
            set
            {
                if (value == _isFilterByGender)
                    return;
                SetProperty(ref _isFilterByGender, value);
            }
        }


        private bool _selectMaleUser;

        [ProtoMember(2)]
        public bool SelectMaleUser
        {
            get
            {
                return _selectMaleUser;
            }
            set
            {
                if (value == _selectMaleUser)
                    return;
                SetProperty(ref _selectMaleUser, value);
            }
        }

        private bool _selectFemaleUser;

        [ProtoMember(3)]
        public bool SelectFemaleUser
        {
            get
            {
                return _selectFemaleUser;
            }
            set
            {
                if (value == _selectFemaleUser)
                    return;
                SetProperty(ref _selectFemaleUser, value);
            }
        }

#pragma warning disable 414
        private bool _isSaveCloseButtonVisisble = true;
#pragma warning restore 414

      
    }
}
*/
