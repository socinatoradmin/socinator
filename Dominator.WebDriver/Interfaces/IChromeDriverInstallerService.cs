using Dominator.WebDriver.Models;
using System.Threading.Tasks;
namespace Dominator.WebDriver.Interfaces
{
    public interface IChromeDriverInstallerService
    {
        Task Install(SettingsModel settings);
    }
}