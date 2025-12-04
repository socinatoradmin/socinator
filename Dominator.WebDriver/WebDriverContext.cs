namespace Dominator.WebDriver
{
    using Dominator.WebDriver.Models;
    using OpenQA.Selenium.Chrome;

    public class WebDriverContext : IWebDriverContext
    {
        private readonly string _chromeDir;
        private SettingsModel _settings { get; }
        public WebDriverContext()
        {

        }
        public WebDriverContext(string chromeDir, SettingsModel settings)
        {
            _chromeDir = chromeDir;
            _settings = settings;
        }

        public IWebDriver Create(bool headless = true)
        {
            var service = ChromeDriverService.CreateDefaultService(_chromeDir, _settings.ChromeDriverWinName);
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            var options = GetChromeOptions(headless);
            options.BinaryLocation = $"{_chromeDir}\\{_settings.ChromeBinaryName}";
            var driver = new ChromeDriver(service, options);
            return new WebDriver(driver);
        }

        /// <remarks>
        /// Available options:
        /// --incognito
        /// --disable-dev-shm-usage
        /// --errors
        /// --no-sandbox
        /// --start-maximized
        /// --disable-gpu
        /// --remote-allow-origins=*
        /// </remarks>
        private ChromeOptions GetChromeOptions(bool headless = false)
        {
            var options = new ChromeOptions();
            if (headless)
            {
                options.AddArgument("--headless");
            }
            var user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36";
            options.AddArgument($"--user-agent={user_agent}");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--lang=en-us");
            return options;
        }
    }
}