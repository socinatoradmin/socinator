using System.ComponentModel;

namespace FaceDominatorCore.FDEnums
{
    public enum GroupType
    {
        [Description("Any Group")]
        Any = 1,

        [Description("Public Group")]
        Public = 2,

        [Description("Closed Group")]
        Closed = 3
    }
}
