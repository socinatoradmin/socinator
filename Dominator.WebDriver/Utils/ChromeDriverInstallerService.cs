using Dominator.WebDriver.Interfaces;
using Dominator.WebDriver.Models;
using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dominator.WebDriver.Utils
{
    public class ChromeDriverInstallerService : IChromeDriverInstallerService
    {
        private readonly IDirectoryProvider _directoryProvider;
        private readonly IFileProvider _fileProvider;
        private string _chromeDir { get; }
        public ChromeDriverInstallerService()
        {

        }
        public ChromeDriverInstallerService(
            string chromeDir,
            IDirectoryProvider directoryProvider,
            IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
            _directoryProvider = directoryProvider;
            _chromeDir = chromeDir;
        }

        public async Task Install(SettingsModel settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("WebDriver settings miss");
            }
            _directoryProvider.CreateDirectory(_chromeDir);

            var chromeDriverPath = Path.Combine(_chromeDir, settings.ChromeDriverWinName);
            var chromeBinaryPath = Path.Combine(_chromeDir, settings.ChromeBinaryName);

            if (!_fileProvider.Exists(chromeDriverPath))
            {
                await DownloadChromeDriver(settings);
            }
            if (!_fileProvider.Exists(chromeBinaryPath))
            {
                await DownloadChrome(settings);
            }
        }

        private async Task DownloadChromeDriver(SettingsModel settings)
        {
            var httpClient = new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri(settings.ChromeDriverDownloadUrl),
            };
            _directoryProvider.CreateDirectory(_chromeDir);
            var httpResponse = await httpClient.GetAsync($"{settings.ChromeDriverVersion}/{settings.ChromeDriverArchiveName}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"ChromeDriver {settings.ChromeDriverVersion} not found");
                }
                else
                {
                    throw new Exception($"ChromeDriver request failed with status code: {httpResponse.StatusCode}, reason phrase: {httpResponse.ReasonPhrase}");
                }
            }

            using (var zipFileStream = await httpResponse.Content.ReadAsStreamAsync())
            using (var zipArchive = new ZipArchive(zipFileStream, ZipArchiveMode.Read))
            using (var chromeDriverWriter = new FileStream(Path.Combine(_chromeDir, settings.ChromeDriverWinName), FileMode.Create))
            {
                var zipFile = zipArchive.GetEntry(settings.ChromeDriverWinName);
                using (Stream chromeDriverStream = zipFile.Open())
                {
                    await chromeDriverStream.CopyToAsync(chromeDriverWriter);
                }
            }
        }

        private async Task DownloadChrome(SettingsModel settings)
        {
            var httpClient = new HttpClient();
            var httpResponse = await httpClient.GetAsync(settings.ChromeBinaryDownloadUrl);

            if (!httpResponse.IsSuccessStatusCode)
            {
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new Exception($"Chrome binary not found");
                }
                else
                {
                    throw new Exception($"Chrome binary request failed with status code: {httpResponse.StatusCode}, reason phrase: {httpResponse.ReasonPhrase}");
                }
            }

            using (var stream = await httpResponse.Content.ReadAsStreamAsync())
            using (var archive = SharpCompress.Archives.SevenZip.SevenZipArchive.Open(stream))
            {
                try
                {
                    archive.WriteToDirectory(_chromeDir, new ExtractionOptions() { ExtractFullPath = true });
                }
                catch(Exception)
                {
                    //Where is can be exception due read file in archive, but it shouldn't affect.
                }
            }
            
            MoveExtractedFiles(settings);
        }

        private void MoveExtractedFiles(SettingsModel settings)
        {
            const string ARCH_CHROME_DIR = "Chrome-bin";
            var chromeBin = Path.Combine(_chromeDir, ARCH_CHROME_DIR);
            var files = _directoryProvider.GetFiles(chromeBin, settings.ChromeBinaryName);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                _fileProvider.Move(file, Path.Combine(_chromeDir, fileName));
            }

            var dirs = _directoryProvider.GetDirectories(chromeBin);
            foreach (var dir in dirs)
            {
                var dirName = Path.GetFileName(dir);
                _directoryProvider.Move(dir, Path.Combine(_chromeDir, dirName));
            }
            _directoryProvider.DeleteDirectory(chromeBin);
        }
    }
}