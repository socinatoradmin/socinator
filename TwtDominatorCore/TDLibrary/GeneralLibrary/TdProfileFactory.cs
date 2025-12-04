using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using TwtDominatorCore.Requests;
using TwtDominatorCore.TDUtility;
using Unity;
using static TwtDominatorCore.TDEnums.Enums;

namespace TwtDominatorCore.TDLibrary.GeneralLibrary
{
    public class TdProfileFactory : ProfileFactory
    {
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IAccountsFileManager _accountsFileManager;
        private JsonJArrayHandler handler => JsonJArrayHandler.GetInstance;
        public TdProfileFactory(IAccountsFileManager accountsFileManager, IAccountScopeFactory accountScopeFactory)
        {
            _accountsFileManager = accountsFileManager;
            _accountScopeFactory = accountScopeFactory;
        }

        private UserProfileDetails UserProfileDetails { get; set; }
        private static string Domain =>TdConstants.Domain;
        public override void EditProfile(DominatorAccountModel accountModel)
        {
            Task.Factory.StartNew(() => { EditTwitterProfile(accountModel); });
        }

        private void EditTwitterProfile(DominatorAccountModel accountModel)
        {
            try
            {
                #region Get Current Profile Details

                try
                {
                    UserProfileDetails = new UserProfileDetails();
                    var wholeData = _accountsFileManager.GetAccountById(accountModel.AccountId);
                    accountModel.ExtraParameters = wholeData.ExtraParameters;

                    if (accountModel.ExtraParameters.ContainsKey(
                        ModuleExtraDetails.UserProfileDetails.ToString()))
                    {
                        var tempData = accountModel
                            .ExtraParameters[ModuleExtraDetails.UserProfileDetails.ToString()].Replace("\"\"{", "{")
                            .Replace("}\"\"", "}");
                        UserProfileDetails =
                            JsonConvert.DeserializeObject<UserProfileDetails>(Uri.UnescapeDataString(tempData));
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Twitter,
                            accountModel.AccountBaseModel.UserName,
                            "LangKeyEditTwitterProfile".FromResourceDictionary(),
                            "LangKeyPleaseUpdateCheckAccountStatus".FromResourceDictionary());
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion

                #region Set Current Profile Details

                EditProfileModel editProfileModelCurrent;

                if (string.IsNullOrEmpty(UserProfileDetails?.UserName))
                    try
                    {
                        var AccountScopeFactory = InstanceProvider.GetInstance<IAccountScopeFactory>();
                        var _logInProcess = AccountScopeFactory[accountModel.AccountId].Resolve<ITwtLogInProcess>();
                        _logInProcess.CheckLogin(accountModel, accountModel.Token);

                        UserProfileDetails = InstanceProvider.GetInstance<IAccountContactConfig>()
                            .GetUserContactDetails(accountModel, true).Result;
                        TdUtility.SaveUserProfileDetails(accountModel);
                    }
                    catch (Exception exception)
                    {
                        exception.DebugLog();
                    }

                var defaultProfilePicUrl =
                    "https://abs.twimg.com/sticky/default_profile_images/default_profile_400x400.png";

                if (UserProfileDetails != null)
                    editProfileModelCurrent = new EditProfileModel
                    {
                        ProfilePicPath = UserProfileDetails.ProfilePicUrl == ""
                            ? defaultProfilePicUrl
                            : UserProfileDetails.ProfilePicUrl,
                        Username = UserProfileDetails.UserName,
                        Fullname = UserProfileDetails.FullName,
                        ExternalUrl = UserProfileDetails.WebsiteUrl,
                        Bio = UserProfileDetails.Bio,
                        Email = UserProfileDetails.Email,
                        PhoneNumber = UserProfileDetails.PhoneNumber,
                        IsMaleChecked = UserProfileDetails.Gender.ToLower().Equals("male"),
                        IsFemaleChecked = UserProfileDetails.Gender.ToLower().Equals("female"),
                        IsNonSpecifiedChecked = UserProfileDetails.Gender.ToLower().Equals("unknown") ||
                                                UserProfileDetails.Gender.ToLower().Equals("not specified")
                    };
                else
                    editProfileModelCurrent = new EditProfileModel
                    {
                        Username = accountModel.UserName
                    };

                #endregion

                #region Edit Profile Picture and Profile Details Process

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

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void InitializeUpdateProfile(DominatorAccountModel accountModel, EditProfile objEditProfile)
        {
            try
            {
                #region Log in process

                if (!accountModel.IsUserLoggedIn)
                {
                    try
                    {
                        var logInProcess = _accountScopeFactory[accountModel.AccountId].Resolve<ITwtLogInProcess>();
                        logInProcess.CheckLogin(accountModel, accountModel.Token);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    if (!accountModel.IsUserLoggedIn)
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Twitter, accountModel.UserName,
                            "LangKeyEditTwitterProfile".FromResourceDictionary(),
                            "LangKeyUserNotAbleToLogin".FromResourceDictionary());
                        return;
                    }
                }

                #endregion


                var editProfileModel = objEditProfile.EditProfileViewModel.EditProfileModel;

                #region Calling update methods with their respective changes

                EditProfilePicture(accountModel, editProfileModel);
                UpdateScreenName(accountModel, editProfileModel);
                EditBiography(accountModel, editProfileModel);
                EditEmail(accountModel, editProfileModel);
                EditWebsite(accountModel, editProfileModel);
                EditContact(accountModel, editProfileModel);
                EditFullName(accountModel, editProfileModel);
                EditGender(accountModel, editProfileModel);
                UpdateAccountFileManager(accountModel, UserProfileDetails);

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Edit Biography if changed
        /// </summary>
        /// <param name="accountModel"></param>
        /// < param name="userProfileResponse"></param>
        /// <param name="editProfileModel"></param>
        /// < param name="twtFunct"></param>
        /// <returns></returns>
        private void EditBiography(DominatorAccountModel accountModel, EditProfileModel editProfileModel)
        {
            try
            {
                var twitterFunct = _accountScopeFactory[accountModel.AccountId].Resolve<ITwitterFunctions>();
                if (UserProfileDetails.Bio == editProfileModel.Bio ||
                    string.IsNullOrEmpty(editProfileModel.Bio?.Trim()))
                    return;

                var UpdateProfileBioResponseHandler =
                    twitterFunct.UpdateProfileBiography(accountModel, editProfileModel.Bio.Trim());

                if (UpdateProfileBioResponseHandler.Success)
                {
                    UserProfileDetails.Bio = editProfileModel.Bio;
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.UserName, "LangKeyEditTwitterProfile".FromResourceDictionary(),
                        "LangKeyUpdatedBio".FromResourceDictionary());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        ///     Check Username availability if changed
        /// </summary>
        /// <param name="accountModel"></param>
        /// < param name="userProfileResponse"></param>
        /// <param name="editProfileModel"></param>
        /// < param name="twtFunct"></param>
        /// <returns></returns>
        private void UpdateScreenName(DominatorAccountModel accountModel, EditProfileModel editProfileModel)
        {
            var httpHelper = _accountScopeFactory[accountModel.AccountId].Resolve<ITdHttpHelper>();
            var twitterFunct = _accountScopeFactory[accountModel.AccountId].Resolve<ITwitterFunctions>();
            if (UserProfileDetails.UserName == editProfileModel.Username ||
                string.IsNullOrEmpty(editProfileModel.Username?.Trim()))
                return;

            try
            {
                //$https://{Domain}/i/api/i/users/username_available.json?full_name=Leonard%20Y.%20Johnson&suggest=true&username=LeonardYJohnso1
                var tdRequestParameter = (TdRequestParameters)httpHelper.GetRequestParameter();
                tdRequestParameter.Headers.Clear();

                var csrftoken = accountModel.Cookies.OfType<Cookie>().FirstOrDefault(x => x.Name == "ct0" && x.Domain == ".x.com").Value 
                    ?? accountModel.Cookies.OfType<Cookie>().FirstOrDefault(x => x.Name == "ct0").Value;
                twitterFunct.SetHeaderForEditProfile($"https://{Domain}/settings/screen_name", csrftoken);
                httpHelper.SetRequestParameter(tdRequestParameter);
                var checkUsernameAvailableUrl =
                    $"https://{Domain}/i/api/i/users/username_available.json?full_name={Uri.EscapeUriString(UserProfileDetails.FullName)}&suggest=true&username={editProfileModel.Username}";
                var ResponseParameter = httpHelper.GetRequest(checkUsernameAvailableUrl);
                var jsonObject = JObject.Parse(ResponseParameter.Response);
                bool.TryParse(jsonObject["valid"].ToString(), out var IsAvailable);

                if (IsAvailable)
                {
                    var UpdateProfileScreenName =
                        twitterFunct.UpdateProfileScreenName(accountModel, editProfileModel.Username.Trim());

                    if (UpdateProfileScreenName.Success)
                    {
                        if (!accountModel.UserName.Contains("@"))
                            accountModel.AccountBaseModel.UserName = editProfileModel.Username;

                        accountModel.AccountBaseModel.ProfileId = editProfileModel.Username;
                        UserProfileDetails.UserName = editProfileModel.Username;
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                            accountModel.UserName, "LangKeyEditTwitterProfile".FromResourceDictionary(),
                            "LangKeyUpdatedUsername".FromResourceDictionary());
                    }
                }
                else
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Twitter, accountModel.UserName,
                        "LangKeyEditTwitterProfile".FromResourceDictionary(),
                        string.Format("LangKeyAlreadyTakenSomething".FromResourceDictionary(),
                            "LangKeyUsername".FromResourceDictionary()));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditProfilePicture(DominatorAccountModel accountModel,
            EditProfileModel editProfileModel)
        {
            var twitterFunct = _accountScopeFactory[accountModel.AccountId].Resolve<ITwitterFunctions>();

            try
            {
                if (UserProfileDetails.ProfilePicUrl == editProfileModel.ProfilePicPath ||
                    string.IsNullOrEmpty(editProfileModel.ProfilePicPath?.Trim()))
                    return;

                var UpdateProfilePicResponseHandler =
                    twitterFunct.UpdateProfilePic(accountModel, editProfileModel.ProfilePicPath.Trim());
                if (UpdateProfilePicResponseHandler.Success)
                {
                    UserProfileDetails.ProfilePicUrl = UpdateProfilePicResponseHandler.UpdatedProfilePicUrl;
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.UserName, "LangKeyEditTwitterProfile".FromResourceDictionary(),
                        "LangKeyUpdatedProfilePic".FromResourceDictionary());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditEmail(DominatorAccountModel accountModel, EditProfileModel editProfileModel)
        {
            var httpHelper = _accountScopeFactory[accountModel.AccountId].Resolve<ITdHttpHelper>();
            var twitterFunct = _accountScopeFactory[accountModel.AccountId].Resolve<ITwitterFunctions>();
            try
            {
                if (UserProfileDetails.Email.Equals(editProfileModel.Email) ||
                    string.IsNullOrEmpty(editProfileModel.Email?.Trim()))
                    return;

                var refererUrl = $"https://{Domain}/settings/account";
                var checkEmailUrl =
                    $"https://{Domain}/users/email_available?email={WebUtility.UrlEncode(editProfileModel.Email)}&scribeContext%5Bcomponent%5D=form&scribeContext%5Belement%5D=email&value={WebUtility.UrlEncode(editProfileModel.Email)}";
                var ReqParam = httpHelper.GetRequestParameter();
                ReqParam.Referer = refererUrl;
                var ResponseParameter = httpHelper.GetRequest(checkEmailUrl.Trim());

                var jsonObject = handler.ParseJsonToJObject(ResponseParameter.Response);
                bool.TryParse(handler.GetJTokenValue(jsonObject,"valid"), out bool IsAvailable);

                if (IsAvailable)
                {
                    var UpdateProfileEmail =
                        twitterFunct.UpdateProfileEmail(accountModel, editProfileModel.Email);

                    if (UpdateProfileEmail.Success)
                    {
                        if (accountModel.UserName.Contains("@"))
                            accountModel.AccountBaseModel.UserName = editProfileModel.Email;


                        UserProfileDetails.Email = editProfileModel.Email;
                        GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                            accountModel.UserName, "LangKeyEditTwitterProfile".FromResourceDictionary(),
                            "LangKeyUpdatedEmail".FromResourceDictionary());
                    }
                }

                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Twitter, accountModel.UserName,
                    "LangKeyEditTwitterProfile".FromResourceDictionary(),
                    string.Format("LangKeyAlreadyTakenSomething".FromResourceDictionary(),
                        "LangKeyEmail".FromResourceDictionary()));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditWebsite(DominatorAccountModel accountModel, EditProfileModel editProfileModel)
        {
            var twitterFunct = _accountScopeFactory[accountModel.AccountId].Resolve<ITwitterFunctions>();
            try
            {
                if (UserProfileDetails.WebsiteUrl.Equals(editProfileModel.ExternalUrl) ||
                    string.IsNullOrEmpty(editProfileModel.ExternalUrl?.Trim()))
                    return;

                var UpdateWebsiteUrl =
                    twitterFunct.UpdateProfileWebsite(accountModel, editProfileModel.ExternalUrl.Trim());
                if (UpdateWebsiteUrl.Success)
                {
                    UserProfileDetails.WebsiteUrl = editProfileModel.ExternalUrl;
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.UserName, "LangKeyEditTwitterProfile".FromResourceDictionary(),
                        "LangKeyUpdatedWebsite".FromResourceDictionary());
                }
                else
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Twitter, accountModel.UserName,
                        "LangKeyEditTwitterProfile".FromResourceDictionary(),
                        "LangKeyUnableToUpdatedWebsite".FromResourceDictionary());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditContact(DominatorAccountModel accountModel, EditProfileModel editProfileModel)
        {
            var twitterFunct = _accountScopeFactory[accountModel.AccountId].Resolve<ITwitterFunctions>();
            try
            {
                if (UserProfileDetails.PhoneNumber.Equals(editProfileModel.PhoneNumber) ||
                    string.IsNullOrEmpty(editProfileModel.PhoneNumber?.Trim()))
                    return;

                var UpdatePhoneNumber =
                    twitterFunct.UpdateProfileContact(accountModel, editProfileModel.PhoneNumber.Trim());

                if (UpdatePhoneNumber.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.UserName, "LangKeyEditTwitterProfile".FromResourceDictionary(),
                        "LangKeyUpdatedPhoneNumber".FromResourceDictionary());
                    UserProfileDetails.Contact = editProfileModel.PhoneNumber;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditFullName(DominatorAccountModel accountModel, EditProfileModel editProfileModel)
        {
            var twitterFunct = _accountScopeFactory[accountModel.AccountId].Resolve<ITwitterFunctions>();
            try
            {
                if (UserProfileDetails.FullName.Equals(editProfileModel.Fullname) ||
                    string.IsNullOrEmpty(editProfileModel.Fullname?.Trim()))
                    return;

                var UpdateProfileFullName =
                    twitterFunct.UpdateProfileFullName(accountModel, editProfileModel.Fullname.Trim());

                if (UpdateProfileFullName.Success)
                {
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.UserName, "LangKeyEditTwitterProfile".FromResourceDictionary(),
                        "LangKeyUpdatedFullName".FromResourceDictionary());
                    UserProfileDetails.FullName = editProfileModel.Fullname;
                }else if(!UpdateProfileFullName.Success && UpdateProfileFullName?.Issue != null && !string.IsNullOrEmpty(UpdateProfileFullName?.Issue?.Message))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.UserName, "Update FullName",UpdateProfileFullName?.Issue?.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditGender(DominatorAccountModel accountModel, EditProfileModel editProfileModel)
        {
            var twitterFunct = _accountScopeFactory[accountModel.AccountId].Resolve<ITwitterFunctions>();

            try
            {
                string gender = Console.ReadLine();
                if (editProfileModel.IsFemaleChecked)
                    gender = "female";
                else if (editProfileModel.IsMaleChecked)
                    gender = "male";
                else if (editProfileModel.IsNonSpecifiedChecked)
                    gender = "not specified";


                if (UserProfileDetails.Gender.Equals(gender) || string.IsNullOrEmpty(gender))
                    return;

                var UpdateProfileScreenName = twitterFunct.UpdateProfileGender(accountModel, gender);

                if (UpdateProfileScreenName.Success)
                {
                    UserProfileDetails.Gender = gender;
                    GlobusLogHelper.log.Info(Log.ActivitySuccessful, accountModel.AccountBaseModel.AccountNetwork,
                        accountModel.UserName, "LangKeyEditTwitterProfile".FromResourceDictionary(),
                        "LangKeyUpdatedGender".FromResourceDictionary());
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void UpdateAccountFileManager(DominatorAccountModel accountModel, UserProfileDetails userProfileDetails)
        {
            try
            {
                var serializeUserProfileData = JsonConvert.SerializeObject(userProfileDetails);

                var socinatorAccountBuilders =
                    SocinatorAccountBuilder.Instance(accountModel.AccountBaseModel.AccountId);
                socinatorAccountBuilders.AddOrUpdateDominatorAccountBase(accountModel.AccountBaseModel)
                    .AddOrUpdateCookies(accountModel.Cookies)
                    .AddOrUpdateDisplayColumn1(accountModel.DisplayColumnValue1)
                    .AddOrUpdateDisplayColumn2(accountModel.DisplayColumnValue2)
                    .AddOrUpdateDisplayColumn3(accountModel.DisplayColumnValue3)
                    .AddOrUpdateExtraParameter(ModuleExtraDetails.ModulePrivateDetails.ToString(),
                        accountModel.ModulePrivateDetails)
                    .AddOrUpdateExtraParameter(ModuleExtraDetails.UserProfileDetails.ToString(),
                        serializeUserProfileData)
                    .SaveToBinFile();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}