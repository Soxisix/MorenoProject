using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;

namespace MorenoSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnAppStartup_UpdateThemeName(object sender, StartupEventArgs e)
        {
            DXSplashScreen.Show<SplashScreenView1>();

            //DevExpress.Xpf.Core.ApplicationThemeHelper.UpdateApplicationThemeName();

            RunTypeInitializers(Assembly.GetAssembly(typeof(ImageEdit)));
            RunTypeInitializers(Assembly.GetAssembly(typeof(DocumentPreviewControl)));
            ThemeManager.SetThemeName(new ImageEdit(), Theme.Office2016WhiteName);
            ThemeManager.SetThemeName(new DocumentPreviewControl(), Theme.Office2016WhiteName);
        }

        public static void RunTypeInitializers(Assembly a)
        {

            Type[] types = a.GetExportedTypes();

            for (int i = 0; i < types.Length; i++)
            {
                RuntimeHelpers.RunClassConstructor(types[i].TypeHandle);
            }
            //Assembly.Load(a.ToString());
        }
    }
}
