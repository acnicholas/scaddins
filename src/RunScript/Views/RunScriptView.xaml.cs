using CefSharp;
using SCaddins.RunScript.ViewModels;
using System;
using System.IO;
using System.Reflection;
using System.Web;

namespace SCaddins.RunScript.Views
{
    public partial class RunScriptView
    {
        public RunScriptView()
        {

            InitializeComponent();
            RunScriptViewModel.SetBrowser(this.Editor);
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // this.Editor.Address = @"C:\Home\Andrew\Code\cs\scaddins\src\Monaco\index.html";
            this.Editor.Address = Constants.InstallDirectory + @"monaco\index.html";
        }

        private void UserControl_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Editor.UpdateLayout();
        }

        private void Grid_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Editor.Width = this.Width;
            Editor.Height = this.Height;
            Editor.UpdateLayout();
        }
    }
}
    