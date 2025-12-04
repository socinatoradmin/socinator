#region

using System;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IResponseParameter
    {
        bool HasError { get; set; }

        string Response { get; set; }

        Exception Exception { get; set; }
    }
}