using FaceDominatorCore.FDEnums;

namespace FaceDominatorCore.FDLibrary.FdClassLibrary
{
    public class FdErrorDetails
    {
        /// <summary>
        /// Is it need to change the account login status to failed, due to error
        /// </summary>
        public bool IsStatusChangedRequired { get; set; }


        /// <summary>
        /// To specify the error report in short notes
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// To give the briefy description about the errors
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// To specify the error in enum format
        /// </summary>
        public FacebookErrors? FacebookErrors { get; set; }
    }
}
