using SCaddins.RunScript.ViewModels;
using System;
using System.Diagnostics;

namespace SCaddins.RunScript.Views
{
    public partial class RunScriptView
    {
        public RunScriptView()
        {
            InitializeComponent();         
            RunScriptViewModel.SetBrowser(this.Editor);
        }
    }
}
    