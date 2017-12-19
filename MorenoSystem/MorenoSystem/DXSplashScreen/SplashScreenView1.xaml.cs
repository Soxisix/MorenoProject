using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DevExpress.Xpf.Core;

namespace MorenoSystem
{
    /// <summary>
    /// Interaction logic for SplashScreenView1.xaml
    /// </summary>
    public partial class SplashScreenView1 : Window, ISplashScreen {
        public SplashScreenView1() {
            InitializeComponent();
            this.board.Completed += OnAnimationCompleted;
            Footer_Text.Text = "Copyright © 2017-" + DateTime.Now.Year;
        }
        public void Progress(double value)
        {
            progressBar.Value = value;
        }
        public void CloseSplashScreen()
        {
            this.board.Begin(this);
        }
        public void SetProgressState(bool isIndeterminate)
        {
            progressBar.IsIndeterminate = isIndeterminate;
        }
        void OnAnimationCompleted(object sender, EventArgs e)
        {
            this.board.Completed -= OnAnimationCompleted;
            this.Close();
        }
    }
}
