using System;
using System.ComponentModel;

namespace PinDominatorCore.Response
{
    [Serializable]
    internal class PinterestException : Exception
    {
        [Localizable(false)]
        public PinterestException(string message)
            : base(message)
        {
        }
    }
}