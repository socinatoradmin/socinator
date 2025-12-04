#region

using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Interfaces
{
    /// <summary>
    ///     General module settings used over all social networks and modules
    /// </summary>
    public interface IGeneralSettings
    {
        JobConfiguration JobConfiguration { get; set; }

        bool IsAccountGrowthActive { get; set; }
    }
}