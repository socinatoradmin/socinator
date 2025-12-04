using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;

namespace Uninstaller
{
    [RunInstaller(true)]
    public class CustomUninstaller: Installer
    {
        public override void Uninstall(IDictionary savedState)
        {
            string installPath = Context.Parameters["targetdir"];
            try
            {
                if (Directory.Exists(installPath))
                    Directory.Delete(installPath, true);
            }
            catch (Exception ex)
            {
                // Log if needed
            }
        }
    }
}
