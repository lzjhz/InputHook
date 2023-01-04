using InputHook.Hooks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace InputHook
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnStartup(object sender, StartupEventArgs e)
        {
            Helpers.NativeMethods.AllocConsole();
        }

        private void OnExit(object sender, ExitEventArgs e)
        {
            Helpers.NativeMethods.FreeConsole();
        }
    }
}
