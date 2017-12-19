using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Printing;
using MahApps.Metro.Controls;
using MorenoSystem.Common.Messages;
using MorenoSystem.MyEFContext;
using MorenoSystem.Views.Student.Report;

namespace MorenoSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MorenoContext _context = new MorenoContext();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel(_context);
            
            //Thread.Sleep(2000);
            Loaded += OnLoaded;


        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            DXSplashScreen.Close();
            Thread.Sleep(1000);
            Activate();
        }
        

        private void UIElement_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void MainWindow_OnContentRendered(object sender, EventArgs e)
        {
            

            Messenger.Default.Send(new StartLoginDialog());
        }
    }
}
