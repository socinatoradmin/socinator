using System;
using System.Threading.Tasks;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using GramDominatorCore.GDModel;
using GramDominatorCore.Request;
using CommonServiceLocator;
using Unity;
using System.Threading;
using GramDominatorCore.Response;
using System.Web.Script;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace GramDominatorCore.GDLibrary
{
    public class GdProfileFactory : ProfileFactory
    {
        private IGdLogInProcess loginProcess;
        private IInstaFunction instaFunction;
        public override void EditProfile(DominatorAccountModel accountModel)
        {
            Task.Run(async() =>
            {
                await EditInstaProfile(accountModel);
            });
        }

        private async Task EditInstaProfile(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                #region Login Process
                AccountModel accountModel = new AccountModel(dominatorAccountModel);
                bool Male = false; bool FeMale = false; bool UnKnown = false;
                var scope = InstanceProvider.GetInstance<IAccountScopeFactory>();
                loginProcess = scope[dominatorAccountModel.AccountId].Resolve<IGdLogInProcess>();
                instaFunction = scope[dominatorAccountModel.AccountId].Resolve<IInstaFunction>();
                if (!dominatorAccountModel.IsUserLoggedIn)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, dominatorAccountModel.UserName,
                        "Edit Insta Profile", "Attempt to login");

                    if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        loginProcess.LoginWithAlternativeMethod(dominatorAccountModel, dominatorAccountModel.Token); 
                    }
                    else
                    {
                        bool isLoggedIn = instaFunction.GdBrowserManager.BrowserLogin(dominatorAccountModel, dominatorAccountModel.Token);
                    }

                    if (!dominatorAccountModel.IsUserLoggedIn)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Instagram, dominatorAccountModel.UserName,
                            "Edit Insta Profile", "User not able to logged in");
                        return;
                    }
                }

                #endregion

                #region Set Current Profile Details
                UsernameInfoIgResponseHandler userProfileResponse;
                EditProfileModel editProfileModelCurrent;

                if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    userProfileResponse = instaFunction.GetProfileDetails(dominatorAccountModel); 
                }
                else
                {
                    if (instaFunction.GdBrowserManager is null)
                        instaFunction.GdBrowserManager.BrowserLogin(dominatorAccountModel, dominatorAccountModel.Token);
                    //userProfileResponse = instaFunction.GdBrowserManager.GetUserInfo(dominatorAccountModel, dominatorAccountModel.AccountBaseModel.UserName, CancellationToken.None);
                    userProfileResponse = instaFunction.SearchUsername(dominatorAccountModel, dominatorAccountModel.AccountBaseModel.UserName, CancellationToken.None);
                    editProfileModelCurrent = null;
                    var ownGender = await instaFunction.GdBrowserManager.EditProfileAsync(dominatorAccountModel,editProfileModelCurrent,CancellationToken.None);
                    userProfileResponse.GenderByCode = ownGender.GenderByCode;
                }

                if (userProfileResponse != null && userProfileResponse.Success)
                {

                    if (userProfileResponse.GenderByCode.Contains("1") || userProfileResponse.GenderByCode.Contains("Male"))
                        Male = true;
                    else if (userProfileResponse.GenderByCode.Contains("2") || userProfileResponse.GenderByCode.Contains("Female"))
                        FeMale = true;
                    else
                        UnKnown = true;

                    editProfileModelCurrent = new EditProfileModel
                    {
                        ProfilePicPath = userProfileResponse.ProfilePicUrl,
                        Username = userProfileResponse.Username,//dominatorAccountModel.UserName,
                        Fullname = userProfileResponse.FullName,
                        ExternalUrl = userProfileResponse.ExternalUrl,
                        Bio = userProfileResponse.Biography,
                        Email = userProfileResponse.Email,
                        PhoneNumber = userProfileResponse.PhoneNumber,
                        IsMaleChecked = Male,
                        IsFemaleChecked = FeMale,
                        IsNonSpecifiedChecked = UnKnown
                    };
                }
                else
                {
                    editProfileModelCurrent = new EditProfileModel
                    {
                        Username = dominatorAccountModel.UserName
                    };
                }
                #endregion

                #region Edit Profile Picture and Profile Details Process

                Application.Current.Dispatcher.Invoke(() =>
                {
                    var objEditProfile = new EditProfile(editProfileModelCurrent);
                    Dialog dialog = new Dialog();
                    var win = dialog.GetMetroWindow(objEditProfile, "LangKeyEditProfile".FromResourceDictionary());

                    objEditProfile.btnSubmit.Click += async (sender, e) =>
                    {
                        try
                        {
                            EditProfileModel editProfileModel = objEditProfile.EditProfileViewModel.EditProfileModel;
                            int gender = editProfileModel.IsMaleChecked ? 1 : editProfileModel.IsFemaleChecked ? 2 : 3;


                            if (userProfileResponse != null)
                            {
                                await EditProfilePictureAsync(dominatorAccountModel, userProfileResponse.ProfilePicUrl,
                                    editProfileModel, instaFunction);

                                await IsUsernameAvailableAsync(dominatorAccountModel, userProfileResponse, editProfileModel,
                                    instaFunction);

                                await EditGenderAsync(dominatorAccountModel, userProfileResponse, editProfileModel, gender, instaFunction);

                                if (!await EditBiographyAsync(dominatorAccountModel, userProfileResponse, editProfileModel,
                                    instaFunction))
                                    return;
                            }

                            #region Final Edit Profile request
                            UsernameInfoIgResponseHandler userEditProfileResponse = null;
                            if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                            {
                                userEditProfileResponse = await instaFunction.EditProfileAsync(dominatorAccountModel, accountModel, editProfileModel.ExternalUrl, editProfileModel.PhoneNumber, editProfileModel.Fullname, editProfileModel.Bio, editProfileModel.Email, gender,
                                                        editProfileModel.Username); 
                            }
                            else
                            {
                                userEditProfileResponse = await instaFunction.GdBrowserManager.EditProfileAsync(dominatorAccountModel, editProfileModel, CancellationToken.None);
                            }

                            if (userEditProfileResponse != null && userEditProfileResponse.Success)
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, "Edit Insta Profile", "Instagram profile details was edited successfully");
                            }
                            else if (userEditProfileResponse.ToString().Contains("You need an email or confirmed phone number"))
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, "Edit Insta Profile", "Please confirm your email or phone number");
                            }
                            else
                            {
                                GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.UserName, "Edit Insta Profile", "Failed to edit instagram profile details");
                            }
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        editProfileModelCurrent = null;
                        dominatorAccountModel.IsUserLoggedIn = false;
                        instaFunction.GdBrowserManager.CloseBrowser();
                        Application.Current.Dispatcher.Invoke(() => { win.Close();  });
                    };


                    if (editProfileModelCurrent != null)
                        win.ShowDialog();
                });
                dominatorAccountModel.IsUserLoggedIn = false;
                instaFunction.GdBrowserManager.CloseBrowser();

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task EditGenderAsync(DominatorAccountModel dominatorAccountModel, UsernameInfoIgResponseHandler userProfileResponse, EditProfileModel editProfileModel, int gender, IInstaFunction instaFunction)
        {
            string genderInfo = gender == 1 ? "Male" : gender == 2 ? "Female" : "Unknown";
            if (!string.IsNullOrEmpty(userProfileResponse.Gender) && !genderInfo.Equals(userProfileResponse.Gender))
            {
                try
                {
                    CommonIgResponseHandler genderResponse = null;
                    if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        
                    }
                    else
                    {
                        genderResponse = await instaFunction.GdBrowserManager.SetGenderAsync(dominatorAccountModel, editProfileModel, CancellationToken.None);
                    }
                    if (genderResponse == null || !genderResponse.Success)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.UserName, "Edit Insta Profile", "Could not set Gender, please try again");
                    }

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            }
        }

        /// <summary>
        /// Edit Biography if changed
        /// </summary>
        /// <param name="accountModel"></param>
        /// <param name="userProfileResponse"></param>
        /// <param name="editProfileModel"></param>
        /// <param name="instaFunct"></param>
        /// <returns></returns>
        private async Task<bool> EditBiographyAsync(DominatorAccountModel dominatorAccountModel, UsernameInfoIgResponseHandler userProfileResponse, EditProfileModel editProfileModel, IInstaFunction instaFunct)
        {
            if (userProfileResponse != null && userProfileResponse.Biography != editProfileModel.Bio)
            {
                AccountModel accountModel = new AccountModel(dominatorAccountModel);

                CommonIgResponseHandler setBioResponse = null;

                if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                {
                    setBioResponse = await instaFunct.SetBiographyAsync(dominatorAccountModel, accountModel, editProfileModel.Bio); 
                }
                else
                {
                    setBioResponse = await instaFunct.GdBrowserManager.SetBiographyAsync(dominatorAccountModel,editProfileModel.Bio);
                }

                if (setBioResponse == null || !setBioResponse.Success)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.UserName, "Edit Insta Profile", "Could not set Biography, please try again");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check Username availability if changed
        /// </summary>
        /// <param name="accountModel"></param>
        /// <param name="userProfileResponse"></param>
        /// <param name="editProfileModel"></param>
        /// <param name="instaFunct"></param>
        /// <returns></returns>
        private async Task<bool> IsUsernameAvailableAsync(DominatorAccountModel dominatorAccountModel, UsernameInfoIgResponseHandler userProfileResponse, EditProfileModel editProfileModel, IInstaFunction instaFunct)
        {
            if (userProfileResponse != null && userProfileResponse.Username != editProfileModel.Username)
            {
                AccountModel accountModel = new AccountModel(dominatorAccountModel);
                var setBioResponse = await instaFunct.CheckUsernameAsync(dominatorAccountModel, accountModel, editProfileModel.Username);

                if (setBioResponse != null && setBioResponse.Success)
                {
                    userProfileResponse.Username = editProfileModel.Username;
                    if (!setBioResponse.IsAvailable)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.UserName, "Edit Insta Profile",
                            $"Username : {editProfileModel.Username} is not available");
                        return false;
                    }
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                        dominatorAccountModel.UserName, "Edit Insta Profile", "Could not able to set new Useranme");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Edit Profile picture if changed
        /// </summary>
        /// <param name="accountModel"></param>
        /// <param name="profilePicPathCurrent"></param>
        /// <param name="editProfileModel"></param>
        /// <param name="instaFunct"></param>
        private async Task EditProfilePictureAsync(DominatorAccountModel dominatorAccountModel, string profilePicPathCurrent, EditProfileModel editProfileModel, IInstaFunction instaFunct)
        {
            if (!string.IsNullOrEmpty(editProfileModel.ProfilePicPath) && profilePicPathCurrent != editProfileModel.ProfilePicPath)
            {
                try
                {
                    AccountModel accountModel = new AccountModel(dominatorAccountModel);
                    string uploadId = string.Empty;
                    string hashCode = string.Empty;

                    UsernameInfoIgResponseHandler uploadProfileResponse = null;

                    if (!dominatorAccountModel.IsRunProcessThroughBrowser)
                    {
                        var profileResponse = instaFunct.UploadProfilePicture(dominatorAccountModel, accountModel, editProfileModel.ProfilePicPath, ref uploadId, ref hashCode, CancellationToken.None);
                        if (!profileResponse.Success)
                            return;
                        uploadProfileResponse = await instaFunct.ChangeProfilePictureAsync(dominatorAccountModel, accountModel, editProfileModel.ProfilePicPath, uploadId); 
                    }
                    else
                    {
                        uploadProfileResponse = await instaFunct.GdBrowserManager.ChangeProfilePictureAsync(dominatorAccountModel,editProfileModel,CancellationToken.None);
                    }

                    if (uploadProfileResponse != null && uploadProfileResponse.Success)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.UserName, "Edit Insta Profile", "Profile picture was edited successfully");
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccountModel.AccountBaseModel.AccountNetwork,
                            dominatorAccountModel.UserName, "Edit Insta Profile", "Failed to edit profile picture");
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }
    }
}
