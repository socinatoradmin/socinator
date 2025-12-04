using System;
using System.ComponentModel;

namespace GramDominatorCore.Response
{
    [Serializable]
    internal class InstagramException : Exception
    {
        [Localizable(false)]
        public InstagramException(string message)
            : base(message)
        {
        }
    }
}
