#region

using CommonServiceLocator;
using CsvHelper;
using CsvHelper.Configuration;
using DominatorHouseCore.FileManagers;
using ExcelDataReader;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFFolderBrowser;

#endregion

namespace DominatorHouseCore.Utility
{
    public class FileUtilities
    {
        /// <summary>
        ///     FileBrowseAndReader() is used to browse and read the file data from OpenFileDialog
        /// </summary>
        /// <returns>Returns unique list of item from all files</returns>
        public static List<string> FileBrowseAndReader()
        {
            var fileData = new List<string>();

            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Text documents (.txt)|*.txt|CSV files (*.csv)|*.csv|All files (*.*)|*.*"
            };

            var openFileDialogResult = openFileDialog.ShowDialog();

            if (openFileDialogResult != true) return new List<string>();

            foreach (var fileName in openFileDialog.FileNames)
                try
                {
                    var extension = Path.GetExtension(fileName);
                    if (string.IsNullOrEmpty(extension)) continue;
                    if (extension.Equals(".xls", StringComparison.CurrentCultureIgnoreCase) ||
                        extension.Equals(".xlsx", StringComparison.CurrentCultureIgnoreCase))
                        fileData.AddRange(GetExcelFileContent(fileName));
                    else if (extension.Equals(".csv", StringComparison.CurrentCultureIgnoreCase))
                        fileData.AddRange(GetCsvFileContent(fileName));
                    else if (extension.Equals(".txt", StringComparison.CurrentCultureIgnoreCase))
                        fileData.AddRange(GetTextFileContent(fileName));
                    // ReSharper disable once RedundantJumpStatement
                    else continue;

                    //if (!extension.Contains(".txt") && !extension.Contains(".csv"))
                    //        continue;

                    //fileData.AddRange(GetFileContent(fileName));
                }
                catch (Exception ex)
                {
                    Dialog.ShowDialog("LangKeyWarning".FromResourceDictionary(),
                        ex.Message + $" {"LangKeyCloseFileRetry".FromResourceDictionary()}");
                    Console.WriteLine(ex.StackTrace);
                }

            return fileData;
        }

        /// <summary>
        ///     Read the file data from specified files
        /// </summary>
        /// <param name="fileName">given input file</param>
        /// <returns>Unique file details</returns>
        // ReSharper disable once UnusedMember.Global
        public static List<string> GetFileContent(string fileName)
        {
            const int bufferSize = 16384;

            var listFileContent = new List<string>();

            var stringBuilder = new StringBuilder();

            using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    using (var streamReader = new StreamReader(fileStream, Encoding.Default))
                    {
                        var fileContents = new char[bufferSize];
                        var charsRead = streamReader.Read(fileContents, 0, bufferSize);

                        if (charsRead == 0)
                            throw new Exception("File is 0 bytes");

                        while (charsRead > 0)
                        {
                            stringBuilder.Append(fileContents);
                            charsRead = streamReader.Read(fileContents, 0, bufferSize);
                        }

                        var contentArray = stringBuilder.ToString().Split('\r', '\n');

                        var data = contentArray.Select(line => line.EndsWith("\0") ? line.Replace("\0", "") : line);

                        listFileContent.AddRange(data.Distinct(StringComparer.CurrentCultureIgnoreCase));

                        listFileContent.RemoveAll(string.IsNullOrEmpty);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return listFileContent.Distinct().ToList();
        }

        /// <summary>
        ///     GetExportPath is used to get the selected path
        /// </summary>
        /// <returns></returns>
        public static string GetExportPath(bool clickedFromSetting = false)
        {
            var softwareSettingsFileManager = InstanceProvider.GetInstance<ISoftwareSettingsFileManager>();
            var softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();
            if (!clickedFromSetting && softwareSettings.IsDefaultExportPathSelected &&
                !string.IsNullOrEmpty(softwareSettings.ExportPath))
                return softwareSettings.ExportPath;

            var exportPath = string.Empty;

            var openBrowserDialog = new WPFFolderBrowserDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var result = openBrowserDialog.ShowDialog(Application.Current.MainWindow);

            if (result == true)
                exportPath = openBrowserDialog.FileName;
            //  softwareSettings.ExportPath = exportPath;

            return exportPath;
        }

        public static string GetExportPath(Window OwnerWindow)
        {
            var exportPath = string.Empty;

            var openBrowserDialog = new WPFFolderBrowserDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            var result = openBrowserDialog.ShowDialog(OwnerWindow);

            if (result == true) exportPath = openBrowserDialog.FileName;

            return exportPath;
        }

        public static void AddHeaderToCsv(string filename, string header)
        {
            using (var streamWriter = new StreamWriter(filename, false))
            {
                streamWriter.WriteLine(header);
            }
        }

        public static dynamic GetImageOrVideo(bool multiselect, string filter)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = multiselect,
                Filter = filter
            };
            var openFileDialogResult = openFileDialog.ShowDialog();
            if (openFileDialogResult != true)
                return null;
            if (multiselect)
                return openFileDialog.FileNames.ToList();
            return openFileDialog.FileNames[0];
        }

        public static List<string> GetExcelFileContent(string fileName)
        {
            var content = new List<string>();
            using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    do
                    {
                        while (reader.Read())
                        {
                            var rowContent = string.Empty;

                            for (var i = 0; i < reader.FieldCount; i++)
                            {
                                var val = reader.GetValue(i);
                                rowContent += (string.IsNullOrEmpty(val?.ToString()) ? string.Empty : val) + "\t";
                            }

                            if (string.IsNullOrEmpty(rowContent.Trim()))
                                break;
                            content.Add(rowContent);
                        }
                    } while (reader.NextResult());
                }
            }

            return content;
        }

        public static List<string> GetCsvFileContent(string fileName)
        {
            using (var reader = File.OpenText(fileName))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null
            }))
            {
                var csvSplitList = new List<string>();

                try
                {
                    while (csv.Read())
                    {
                        
                        var rowContent = string.Empty;
                        var columnCount = 0;
                        var hasColumn = true;
                        string columnValue;

                        while (hasColumn)
                        {
                            hasColumn = csv.TryGetField(columnCount, out columnValue);
                            if (!hasColumn) continue;
                            columnCount += 1;
                            rowContent += columnValue + "\t";
                        }

                        var data = rowContent.Trim();
                        if (!string.IsNullOrEmpty(data))
                            csvSplitList.Add(rowContent.Substring(0, rowContent.Length - 1));
                    }

                    return csvSplitList;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                    return new List<string>();
                }
            }
        }

        public static List<string> GetTextFileContent(string fileName)
        {
            using (var file = new StreamReader(fileName))
            {
                var csvSplitList = new List<string>();
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    var data = line.Trim();
                    if (!string.IsNullOrEmpty(data))
                        //csvSplitList.Add(ImageExtracter.CheckUrlValid(data) ? data : data.Replace(":", "\t"));
                        csvSplitList.Add(ImageExtracter.CheckUrlValid(data.Split('\t').Last())
                            ? data
                            : data.Replace(":", "\t"));
                }

                return csvSplitList;
            }
        }

        public static void Copy(string fileToCopy, string destinationFileName)
        {
            try
            {
                if (File.Exists(fileToCopy) && !File.Exists(destinationFileName))
                    File.Copy(fileToCopy, destinationFileName);
            }
            catch (Exception exc)
            {
                exc.DebugLog();
            }
        }

        public static bool ReWriteDataIntoFile(string data, string filePath)
        {
            try
            {
                using (var reader = new StreamWriter(filePath))
                {
                    reader.Write(data);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<string> ReadFile(string fileName)
        {
            try
            {
                using (var file = new StreamReader(fileName))
                {
                    var data = await file.ReadToEndAsync();

                    return data;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return "";
            }
        }
        public static bool DeleteFile(string fileName)
        {
            try
            {
                File.Delete(fileName);
                return true;
            }
            catch { return false; }
        }

        public static bool FileExist(string file)
        {
            try
            {
                return File.Exists(file);
            }
            catch { return false ; }
        }
    }
}