#region

using DominatorHouseCore.Models;

#endregion

namespace DominatorHouseCore.Interfaces.StartUp
{
    public interface IStartupJobConfiguration
    {
        JobConfiguration JobConfiguration { get; set; }
    }
}