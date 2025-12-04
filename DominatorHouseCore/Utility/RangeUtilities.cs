#region

using ProtoBuf;
using System;

#endregion

namespace DominatorHouseCore.Utility
{
    [Serializable]
    [ProtoContract]
    public class RangeUtilities : BindableBase
    {
        public RangeUtilities()
        {
        }

        public RangeUtilities(int start, int end) : this()
        {
            StartValue = start;
            EndValue = end;
        }

        private int _startValue;

        [ProtoMember(1)]
        public int StartValue
        {
            get
            {
                if (_endValue < _startValue)
                    EndValue = _startValue;
                return _startValue;
            }
            set
            {
                if (value == _startValue || value < 0)
                    return;
                SetProperty(ref _startValue, value);
            }
        }

        private int _endValue;

        [ProtoMember(2)]
        public int EndValue
        {
            get => _endValue;
            set
            {
                if (value < _startValue)
                    value = _startValue;
                if (value == _endValue)
                    return;
                SetProperty(ref _endValue, value);
            }
        }

        /// <summary>
        ///     GetRandom is used to get the random numbers between the Begin and EndValue
        /// </summary>
        /// <returns>Returns a integer value which lies between those two ranges</returns>
        public int GetRandom()
        {
            return RandomUtilties.GetRandomNumber(EndValue, StartValue);
        }

        /// <summary>
        ///     InRange is used to check whether the given numbers in between the StartValue and EndValue
        /// </summary>
        /// <param name="number">number which is used to check in between the ranges</param>
        /// <returns></returns>
        public bool InRange(int number)
        {
            return number >= StartValue && number <= EndValue;
        }
    }
}