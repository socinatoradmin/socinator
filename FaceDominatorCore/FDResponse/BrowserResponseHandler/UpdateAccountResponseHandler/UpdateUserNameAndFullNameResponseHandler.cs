using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using FaceDominatorCore.FDResponse.BaseResponse;
using FaceDominatorCore.Interface;
using System;

namespace FaceDominatorCore.FDResponse.BrowserResponseHandler.UpdateAccountResponseHandler
{
    public class UpdateUserNameAndFullNameResponseHandler : FdResponseHandler, ICommonResponseParam
    {
        public bool UpdatedFullName { get; set; }
        public bool UpdatedUserName { get; set; }
        public string userUpdateError { get; set; }
        public string fullNameUpdateError { get; set; }
        public UpdateUserNameAndFullNameResponseHandler(IResponseParameter responseParameter, EditProfileModel editProfileModel, bool updateUserName = false, bool updateFullName = false) : base(responseParameter)
        {
            try
            {
                if (updateUserName && !string.IsNullOrEmpty(responseParameter.Response) && responseParameter.Response.Contains(editProfileModel.Username.Replace(" ", "")))
                    UpdatedUserName = true;
                if (updateUserName && (responseParameter.Response.Contains("You're currently unable to choose a username") || responseParameter.Response.Contains("Username is not available")))
                {
                    UpdatedUserName = false;
                    userUpdateError = "Unable to Choose UserName..";
                }
                if (updateFullName && responseParameter.Response.Contains(editProfileModel.Fullname))
                    UpdatedUserName = true;
                if (updateFullName && !responseParameter.Response.Contains(editProfileModel.Fullname))
                {
                    UpdatedUserName = false;
                    fullNameUpdateError = "Failed to Update..";
                }
                Status = UpdatedUserName || UpdatedUserName;
            }
            catch (Exception) { }
        }

        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool HasMoreResults { get; set; }
        public bool Status { get; set; }
    }
    public class AdvancedProfileUpdateResponseHandler : FdResponseHandler, ICommonResponseParam
    {
        public bool UpdatedWebsite { get; set; }
        public bool UpdatedBio { get; set; }
        public bool UpdatedEmail { get; set; }
        public bool UpdatedPhone { get; set; }
        public bool UpdatedGender { get; set; }
        public AdvancedProfileUpdateResponseHandler(IResponseParameter responseParameter, bool WebisteUpdate = false, bool BioUpdate = false, bool ContactUpdate = false, bool EmailUpdate = false, bool GenderUpdate = false) : base(responseParameter)
        {
            try
            {
                UpdatedWebsite = WebisteUpdate;
                UpdatedBio = BioUpdate;
                UpdatedGender = GenderUpdate;
                Status = UpdatedWebsite || UpdatedBio || UpdatedGender;
            }
            catch (Exception) { }
        }

        public string EntityId { get; set; }
        public string PageletData { get; set; }
        public bool HasMoreResults { get; set; }
        public bool Status { get; set; }
    }
}
