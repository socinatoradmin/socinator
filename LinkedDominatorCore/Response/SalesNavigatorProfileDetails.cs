using System;
using DominatorHouseCore;
using LinkedDominatorCore.LDModel;
using LinkedDominatorCore.LDUtility;
using Newtonsoft.Json.Linq;

namespace LinkedDominatorCore.Response
{
    public class SalesNavigatorProfileDetails
    {
        public SalesNavigatorProfileDetails(string response)
        {
            try
            {
                if (!string.IsNullOrEmpty(response)) Success = true;

                var objJObject = JObject.Parse(response);

                var MemberId = objJObject["memberId"].ToString();
                var AuthToken = objJObject["authToken"].ToString();
                objLinkedinUser = new LinkedinUser(MemberId);

                objLinkedinUser.ProfileUrl = "https://www.linkedin.com/sales/profile/" + MemberId + "," + AuthToken +
                                             ",NAME_SEARCH?";
                objLinkedinUser.ProfileUrl = Utils.InsertSpecialCharactersInCsv(objLinkedinUser.ProfileUrl);

                #region ProfilePicUrl

                try
                {
                    objLinkedinUser.ProfilePicUrl = objJObject["pictureId"].ToString();
                    objLinkedinUser.ProfilePicUrl = Utils.InsertSpecialCharactersInCsv(objLinkedinUser.ProfilePicUrl);
                    objLinkedinUser.HasAnonymousProfilePicture = true;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    objLinkedinUser.ProfilePicUrl = "N/A";
                }

                #endregion

                #region FullName

                try
                {
                    objLinkedinUser.FullName = objJObject["fullName"].ToString();
                    objLinkedinUser.FullName = Utils.InsertSpecialCharactersInCsv(objLinkedinUser.FullName);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    objLinkedinUser.FullName = "N/A";
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                Success = false;
            }
        }

        public bool Success { get; }
        public LinkedinUser objLinkedinUser { get; }
    }
}