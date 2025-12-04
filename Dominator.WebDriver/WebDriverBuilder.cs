using Dominator.WebDriver.Interfaces;
using Dominator.WebDriver.Models;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Dominator.WebDriver
{
    public class WebDriverBuilder : IWebDriverBuilder
    {
        public WebDriverBuilder()
        {

        }
        private readonly string _chromeDir;
        private SettingsModel _settings { get; }
        private readonly IChromeDriverInstallerService _chromeDriverInstallerService;

        public WebDriverBuilder(
            string chromeDir, 
            IFileProvider fileProvider, 
            IChromeDriverInstallerService chromeDriverInstallerService)
        {
            _settings = ReadSettings(fileProvider);
            _chromeDir = chromeDir;
            _chromeDriverInstallerService = chromeDriverInstallerService;

            Task.Run(async () => await InstallChrome()).Wait();
        }

        public IWebDriverContext CreateContext()
        {
            return new WebDriverContext(_chromeDir, _settings);
        }

        private async Task InstallChrome()
        {
            await _chromeDriverInstallerService.Install(_settings);
        }

        private SettingsModel ReadSettings(IFileProvider fileProvider)
        {
            try
            {
                var execPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var fileName = Path.Combine(execPath, "webdriversettings.json");
                if (!fileProvider.Exists(fileName))
                {
                    return null;
                }
                var json = fileProvider.ReadAllText(fileName);
                return JsonConvert.DeserializeObject<SettingsModel>(json);
            }
            catch (System.Exception ex)
            {
                return new SettingsModel();
            }
        }
    }
}