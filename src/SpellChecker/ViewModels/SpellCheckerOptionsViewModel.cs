using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace SCaddins.SpellChecker.ViewModels
{
    class SpellCheckerOptionsViewModel : Screen
    {
        public static dynamic DefaultWindowSettings {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 640;
                settings.Width = 480;
                //// settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                ////     new System.Uri("pack://application:,,,/SCaddins;component/Assets/rename.png"));
                settings.Title = "Spelling Options";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }
    }
}
