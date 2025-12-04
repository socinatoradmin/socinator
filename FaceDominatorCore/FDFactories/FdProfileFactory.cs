using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using FaceDominatorCore.FDEnums;
using FaceDominatorCore.FDLibrary.FdClassLibrary;
using FaceDominatorCore.FDLibrary.FdFunctions;
using FaceDominatorCore.FDLibrary.FdFunctions.FdBrowserManager;
using FaceDominatorCore.FDLibrary.FdProcesses;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;

namespace FaceDominatorCore.FDFactories
{
    public class FdProfileFactory : ProfileFactory
    {
        //private readonly IAccountScopeFactory _accountScopeFactory;
        //private readonly IAccountsFileManager _accountsFileManager;

        public FacebookUser fbUser { get; set; }

        public override void EditProfile(DominatorAccountModel accountModel)
        {
            Task.Factory.StartNew(() => { EditFacebookProfile(accountModel); });
        }

        public void EditFacebookProfile(DominatorAccountModel accountModel)
        {
            fbUser = new FacebookUser();
            var _accountsFileManager = InstanceProvider.GetInstance<IAccountsFileManager>();
            var accountsData = _accountsFileManager.GetAccountById(accountModel.AccountId);
            accountModel.ExtraParameters = accountsData.ExtraParameters;

            if (accountModel.ExtraParameters.ContainsKey(
                ModuleExtraDetails.UserDetails.ToString()))
            {
                var tempData = accountModel
                    .ExtraParameters[ModuleExtraDetails.UserDetails.ToString()].Replace("\"\"{", "{")
                    .Replace("}\"\"", "}");
                fbUser =
                    JsonConvert.DeserializeObject<FacebookUser>(Uri.UnescapeDataString(tempData), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook,
                    accountModel.AccountBaseModel.UserName,
                    "LangKeyEditFacebookProfile".FromResourceDictionary(),
                    "LangKeyPleaseUpdateCheckAccountStatus".FromResourceDictionary());
                return;
            }

            EditProfileModel editProfileModelCurrent;

            if (string.IsNullOrEmpty(fbUser?.UserId))
                try
                {
                    ServiceLocator
                        .Current
                        .GetInstance<IUnityContainer>()
                        .RegisterType<IFdLoginProcess, FdLoginProcess>();
                    var AccountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                    var _logInProcess = AccountScopeFactory[$"{accountModel.AccountId}"].Resolve<IFdLoginProcess>();
                    _logInProcess.CheckLogin(accountModel, accountModel.Token);

                    //fbUser = InstanceProvider.GetInstance<IAccountContactConfig>()
                    //    .GetUserContactDetails(accountModel, true).Result;
                    //TdUtility.SaveUserProfileDetails(accountModel);
                }
                catch (Exception exception)
                {
                    exception.DebugLog();
                }

            if (fbUser != null)
                try
                {
                    editProfileModelCurrent = new EditProfileModel
                    {
                        ProfilePicPath = fbUser.ProfilePicUrl,
                        Username = string.IsNullOrEmpty(fbUser.Username) ? fbUser.Familyname : fbUser.Username,
                        Fullname = string.IsNullOrEmpty(fbUser.FullName) ? fbUser.Familyname : fbUser.FullName,
                        ExternalUrl = fbUser.WebsiteUrl ?? string.Empty,
                        Bio = fbUser.Bio ?? string.Empty,
                        Email = fbUser.Email ?? string.Empty,
                        PhoneNumber = fbUser.PhoneNumber ?? string.Empty,
                        IsMaleChecked = fbUser.Gender.ToLower().Equals("male"),
                        IsFemaleChecked = fbUser.Gender.ToLower().Equals("female"),
                        IsNonSpecifiedChecked = fbUser.Gender.ToLower().Equals("unknown") ||
                                                        fbUser.Gender.ToLower().Equals("not specified")
                    };
                }
                catch (Exception)
                {

                    throw;
                }
            else
                editProfileModelCurrent = new EditProfileModel
                {
                    Username = accountModel.UserName
                };

            Application.Current.Dispatcher.Invoke(() =>
            {
                var objEditProfile = new EditProfile(editProfileModelCurrent);
                var dialog = new Dialog();
                var win = dialog.GetMetroWindow(objEditProfile, "LangKeyEditProfile".FromResourceDictionary());

                objEditProfile.btnSubmit.Click += (sender, e) =>
                {
                    try
                    {
                        Task.Factory.StartNew(() => InitializeUpdateProfile(accountModel, objEditProfile));
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    editProfileModelCurrent = null;
                    Application.Current.Dispatcher.Invoke(() => { win.Close(); });
                };


                if (editProfileModelCurrent != null)
                    win.ShowDialog();
            });
        }

        private void InitializeUpdateProfile(DominatorAccountModel accountModel, EditProfile objEditProfile)
        {
            if (!accountModel.IsUserLoggedIn)
            {
                try
                {
                    var accountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                    var _logInProcess = accountScopeFactory[$"{accountModel.AccountId}"].Resolve<IFdLoginProcess>();
                    _logInProcess.CheckLogin(accountModel, accountModel.Token);
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }

                if (!accountModel.IsUserLoggedIn)
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Facebook, accountModel.UserName,
                        "LangKeyEditFacebookProfile".FromResourceDictionary(),
                        "LangKeyUserNotAbleToLogin".FromResourceDictionary());
                    return;
                }
            }
            var AccountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
            var BrowserManager = AccountScopeFactory[$"{accountModel.AccountId}"].Resolve<IFdBrowserManager>();
            try
            {
                var editProfileModel = objEditProfile.EditProfileViewModel.EditProfileModel;
                var ProfileId = !string.IsNullOrEmpty(fbUser.UserId) ? fbUser.UserId : fbUser.ProfileUrl.Split('/').LastOrDefault(x => x != string.Empty);
                EditProfilePicture(accountModel, editProfileModel, ref BrowserManager);
                EditNameAndUserName(accountModel, editProfileModel, ref BrowserManager, ProfileId);
                UpdateAdvancedDetails(accountModel, editProfileModel, ref BrowserManager, ProfileId);
                FdFunctions objFdFunctions = new FdFunctions();
                objFdFunctions.UpdateAccountInfoToModel(accountModel, fbUser);
            }
            catch (Exception) { }
        }

        private void UpdateAdvancedDetails(DominatorAccountModel accountModel, EditProfileModel editProfileModel, ref IFdBrowserManager browserManager, string ProfileId = "")
        {
            try
            {
                var UpdateWebsite = !(fbUser.WebsiteUrl == editProfileModel.ExternalUrl || string.IsNullOrEmpty(editProfileModel.ExternalUrl?.Trim()));
                var UpdateBio = !(fbUser.Bio == editProfileModel.Bio || string.IsNullOrEmpty(editProfileModel.Bio?.Trim()));
                var UpdateEmail = false;
                var UpdatePhone = false;
                var UpdateGender = false;
                if (editProfileModel.IsCheckedPrivateInfo)
                    UpdateGender = editProfileModel.IsFemaleChecked || editProfileModel.IsMaleChecked || editProfileModel.IsNonSpecifiedChecked;
                if (UpdateWebsite || UpdateBio || UpdateEmail || UpdatePhone || UpdateGender)
                {
                    var url = editProfileModel.Username.Contains("about_contact_and_basic_info") ? $"https://www.facebook.com/{editProfileModel.Username?.Trim()}"
                        : $"https://www.facebook.com/{editProfileModel.Username?.Trim()?.Replace(" ", "")}/about_contact_and_basic_info";
                    browserManager.OpenNewWindowAndGoToUrl(accountModel, url);
                    var UpdateResponse = browserManager.UpdateAdvancedProfileDetails(accountModel, editProfileModel, UpdateWebsite, UpdateBio, UpdateEmail, UpdatePhone, UpdateGender, ProfileId);
                    if (UpdateResponse != null && UpdateResponse.Status)
                    {
                        if (UpdateResponse.UpdatedWebsite)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                                accountModel.UserName, "Update User Website", $" As ==> {editProfileModel.ExternalUrl}");
                            fbUser.WebsiteUrl = editProfileModel.ExternalUrl;
                        }
                        if (UpdateResponse.UpdatedBio)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                                accountModel.UserName, "Update User Bio", $" As ==> {editProfileModel.Bio}");
                            fbUser.Bio = editProfileModel.Bio;
                        }
                        if (UpdateResponse.UpdatedEmail)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                                accountModel.UserName, "Update User Email", $" As ==> {editProfileModel.Email}");
                            fbUser.Email = editProfileModel.Email;
                        }
                        if (UpdateResponse.UpdatedPhone)
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                                accountModel.UserName, "Update User Contact", $" As ==> {editProfileModel.PhoneNumber}");
                            fbUser.PhoneNumber = editProfileModel.PhoneNumber;
                        }
                        if (UpdateResponse.UpdatedGender)
                        {
                            var gender = editProfileModel.IsFemaleChecked ? "Female" : editProfileModel.IsMaleChecked ? "Male" : "No Specified";
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                                accountModel.UserName, "Update User Gender", $" As ==> {gender}");
                            fbUser.Gender = gender;
                        }
                    }
                }
            }
            finally
            {
                if (browserManager != null && browserManager.BrowserWindow != null)
                    browserManager.CloseBrowser(accountModel);
            }
        }

        private void EditNameAndUserName(DominatorAccountModel accountModel, EditProfileModel editProfileModel, ref IFdBrowserManager browserManager, string ProfileId = "")
        {
            try
            {
                var UpdateUserName = !(string.IsNullOrEmpty(editProfileModel.Username?.Trim()) || fbUser.Username == editProfileModel.Username);
                var UpdateFullName = !(string.IsNullOrEmpty(editProfileModel.Fullname?.Trim()) || fbUser.FullName == editProfileModel.Fullname);
                if (UpdateUserName || UpdateFullName)
                {
                    var EditNameAndUserNameUrl = $"https://accountscenter.facebook.com/profiles/{ProfileId}";
                    browserManager.OpenNewWindowAndGoToUrl(accountModel, EditNameAndUserNameUrl);
                    var UpdatedResponse = browserManager.UpdateUserNameAndName(accountModel, editProfileModel, UpdateUserName, UpdateFullName, ProfileId);
                    if (UpdatedResponse != null && UpdatedResponse.Status)
                    {
                        if (UpdatedResponse.UpdatedFullName && string.IsNullOrEmpty(UpdatedResponse.fullNameUpdateError))
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                                accountModel.UserName, "Updated Full Name", $" As ==> {editProfileModel.Fullname}");
                            fbUser.FullName = editProfileModel.Fullname;
                        }

                        if (UpdatedResponse.UpdatedUserName && string.IsNullOrEmpty(UpdatedResponse.userUpdateError))
                        {
                            GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                                accountModel.UserName, "Updated User Name", $" As ==> {editProfileModel.Username?.Replace(" ", "")}");
                            fbUser.Username = editProfileModel.Username?.Replace(" ", "");
                        }
                    }
                    if (!string.IsNullOrEmpty(UpdatedResponse?.fullNameUpdateError))
                        GlobusLogHelper.log.Info(Log.ActivityFailed, accountModel.AccountBaseModel.AccountNetwork,
                            accountModel.UserName, "Failed to Update Full Name", $" As ==> {editProfileModel.Fullname}");
                    if (!string.IsNullOrEmpty(UpdatedResponse?.userUpdateError))
                        GlobusLogHelper.log.Info(Log.ActivityFailed, accountModel.AccountBaseModel.AccountNetwork,
                            accountModel.UserName, "Failed to Update User Name", $" As ==> {editProfileModel.Fullname}");
                    Task.Delay(10000).Wait();
                }
            }
            finally
            {
                if (browserManager != null && browserManager.BrowserWindow != null)
                    browserManager.CloseBrowser(accountModel);
            }
        }
        private void EditProfilePicture(DominatorAccountModel accountModel, EditProfileModel editProfileModel, ref IFdBrowserManager BrowserManager)
        {
            try
            {
                if (fbUser.ProfilePicUrl == editProfileModel.ProfilePicPath
                    || string.IsNullOrEmpty(editProfileModel.ProfilePicPath?.Trim()))
                    return;

                BrowserManager.OpenNewWindowAndGoToUrl(accountModel, fbUser.ProfileUrl);

                var updateProfilePicResponseHandler = BrowserManager.UpdateProfilePic(accountModel, editProfileModel).Wait(TimeSpan.FromMinutes(1));
                Task.Delay(10000).Wait();
                if (updateProfilePicResponseHandler)
                {
                    fbUser.ProfilePicUrl = editProfileModel.ProfilePicPath;
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                            accountModel.UserName, "LangKeyEditFacebookProfile".FromResourceDictionary(),
                            "LangKeyUpdatedProfilePic".FromResourceDictionary());

                }
            }
            finally
            {
                if (BrowserManager != null && BrowserManager.BrowserWindow != null)
                    BrowserManager.CloseBrowser(accountModel);
            }
        }
    }
}
