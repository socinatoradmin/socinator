namespace Dominator.WebDriver.Models
{
    public class SettingsModel
    {
        /// <summary>
        /// URL for download chromedriver.
        /// </summary>
        public string ChromeDriverDownloadUrl { get; set; }

        /// <summary>
        /// Chromedriver version.
        /// </summary>
        public string ChromeDriverVersion { get; set; }

        /// <summary>
        /// Chromedriver download archive.
        /// </summary>
        public string ChromeDriverArchiveName { get; set; }

        /// <summary>
        /// Chromedriver name.
        /// </summary>
        public string ChromeDriverName { get; set; }

        /// <summary>
        /// Chromedriver name windows version.
        /// </summary>
        public string ChromeDriverWinName { get; set; }

        /// <summary>
        /// URL for download standalone chrome version.
        /// </summary>
        public string ChromeBinaryDownloadUrl { get; set; }

        /// <summary>
        /// Chrome binary name.
        /// It can run standalone.
        /// </summary>
        public string ChromeBinaryName { get; set; }
    }
}
