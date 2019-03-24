using Squirrel;
using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pWindowJax
{
    internal static class Program
    {
        [STAThread]
        internal static async Task Main()
        {
            string version;

            using (var updateManager = await UpdateManager.GitHubUpdateManager("https://github.com/miterosan/pWindowJax"))
            {
                var release = await updateManager.UpdateApp();

                version = updateManager.CurrentlyInstalledVersion()?.Version.ToString() ?? "0.0.0";

                if (release != null)
                    UpdateManager.RestartApp();

                updateManager.CreateShortcutsForExecutable(Assembly.GetEntryAssembly().Location, ShortcutLocation.Startup, false);
            }

            Application.Run(new MainController { Version = version });
        }
    }
}


