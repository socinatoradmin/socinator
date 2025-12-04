using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using GramDominatorCore.Request;
using Newtonsoft.Json.Linq;

namespace GramDominatorCore.GDLibrary.LoginAndUpdate
{
    public interface IEncryptPassword
    {
        string EncryptPwd(string publicKey, string keyId, string unixTimestampSeconds, string plainPwd);
    }
    public class EncryptPassword : IEncryptPassword
    {
        private static readonly object DownloadMainExeObj = new object();
        public string checkingMainSetup(string encryptedPwd, string publicKey, string keyId, string unixTimestampSeconds, string plainPwd)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainPwd);
            var pwd = System.Convert.ToBase64String(plainTextBytes);
            var process = new System.Diagnostics.Process
            {

                StartInfo = new ProcessStartInfo
                {
                    FileName = $"{ConstantVariable.GetOtherDir()}/main1.2.exe", //$"{ConstantVariable.MyAppFolderPath}\\Resources\\main.exe", //".\\Resources\\main.exe",
                    Arguments =
                               $"--key {publicKey} --key-id {keyId} --timestamp-unix-seconds {unixTimestampSeconds} --pwd {pwd}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    encryptedPwd = line.Replace("b'", string.Empty).Replace("'", string.Empty);
                }
            }

            process.WaitForExit();
            return encryptedPwd;
        }
        public string EncryptPwd(string publicKey, string keyId, string unixTimestampSeconds, string plainPwd)
        {
            var encryptedPwd = string.Empty;
            try
            {
                DownloadMainExe();

                encryptedPwd = checkingMainSetup(encryptedPwd, publicKey, keyId, unixTimestampSeconds, plainPwd);

                if (string.IsNullOrEmpty(encryptedPwd))
                {
                    File.Delete($"{ConstantVariable.GetOtherDir()}/main.exe");
                    DownloadMainExe();
                    encryptedPwd = checkingMainSetup(encryptedPwd, publicKey, keyId, unixTimestampSeconds, plainPwd);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();

            }

            return encryptedPwd;
        }




        /// <summary>
        /// Download main.exe python file (if it doesn't exist in the specific folder) to folder "Socinator" from our server to run ...
        /// </summary>
        void DownloadMainExe()
        {
            try
            {
                lock (DownloadMainExeObj)
                {
                    if (File.Exists($"{ConstantVariable.GetOtherDir()}/main1.2.exe"))
                        return;

                    var imgBytes = new WebClient().DownloadData("http://18.133.153.168/DownloadForSocinator/FilesForGD/main1.2.exe");
                    File.WriteAllBytes($"{ConstantVariable.GetOtherDir()}/main1.2.exe", imgBytes);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog("Error on downloading main1.2.exe");
            }
        }

    }
}
