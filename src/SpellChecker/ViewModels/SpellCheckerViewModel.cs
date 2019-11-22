using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace SCaddins.SpellChecker.ViewModels
{
    public class SpellCheckerViewModel : Screen
    {
        private SpellChecker manager;

        public SpellCheckerViewModel(SpellChecker manager)
        {
            this.manager = manager;
        }

        public BindableCollection<CorrectionCandiate> SpellingErrors => manager.SpellingErrors;

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 480;
                settings.Width = 768;
                //// settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                ////     new System.Uri("pack://application:,,,/SCaddins;component/Assets/rename.png"));
                settings.Title = "Spelling (Australian)";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        public void AddToDictionary()
        {

        }

        public void  Change()
        {

        }

        public void ChangeAll()
        {

        }

        public void IgnoreAll()
        {

        }

        public void IgnoreOnce()
        {

        }

        public void Options()
        {

        }


    }
}
