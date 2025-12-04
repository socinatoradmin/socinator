#region

using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Interfaces
{
    public interface IViewCampaignsFactory
    {
        /// <summary>
        ///     To define the views for the campaign with either edit or duplicate mode
        ///     --------------------------
        ///     Implementation details
        ///     --------------------------
        ///     Step 1: Get the full details for campaign by using <see cref="CampaignsFileManager.GetCampaignById" /> method along
        ///     with campaign Id as parameter, which returns <see cref="CampaignDetails" />
        ///     Step 2: Get the full details for templates by using <see cref="TemplatesFileManager.GetTemplateById" />methods
        ///     along with template id as parameter which got values from step 1's result
        ///     <seealso cref="CampaignDetails.TemplateId" />
        ///     Step 3: By depends on openCampaignType value, bind the neccessary details for edit campaign or duplicate campaign
        ///     like is campaign name editable, cancel buttons visibility
        ///     Step 4: Fetch the activity type from template details and call the respective view
        /// </summary>
        /// <param name="campaignId">pass the campaign Id</param>
        /// <param name="openCampaignType">type of view where edit or duplicate</param>
        void ViewCampaigns(string campaignId, string openCampaignType);
    }
}