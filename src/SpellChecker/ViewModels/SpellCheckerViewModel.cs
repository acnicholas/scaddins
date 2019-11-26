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
        private List<string> suggestions;
        private string selectedSuggestion;

        public SpellCheckerViewModel(SpellChecker manager)
        {
            this.manager = manager;
            if (manager.MoveNext())
            {
                BadSpelling = ((CorrectionCandidate)manager.Current).Current as string;
                //// SCaddinsApp.WindowManager.ShowMessageBox(BadSpelling);
            }
        }

        public static dynamic DefaultWindowSettings {
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

        public List<String> Suggestions {
            get
            {
                return manager.GetCurrentSuggestions();
            }
        }

        public string SelectedSuggestion {
            get
            {
                return selectedSuggestion;
            }
            set
            {
                selectedSuggestion = value;
                NotifyOfPropertyChange(() => SelectedSuggestion);
                ChangeTo = selectedSuggestion;
                NotifyOfPropertyChange(() => ChangeTo);
            }
        }

        public void AddToDictionary()
        {
        }

        //public void  Change()
        //{
        //    CurrentCanditate.NewValue = SelectedSuggestion;
        //}

        public void ChangeAll()
        {
            manager.AddToIgnoreList(ChangeTo);
        }

        public string BadSpelling
        {
            get; set;
        }

        public string BadSpellingContext
        {
            get; set;
        }

        public string ChangeTo
        {
            get; set;
        }

        public void IgnoreAll()
        {

        }

        public void IgnoreOnce()
        {

            if (manager.MoveNext()) {
                NotifyOfPropertyChange(() => Suggestions);
                NotifyOfPropertyChange(() => SelectedSuggestion);
                if (Suggestions.Count > 0)
                {
                    SelectedSuggestion = Suggestions.First();
                    ChangeTo = SelectedSuggestion;
                    NotifyOfPropertyChange(() => ChangeTo);
                }
                var cc = (CorrectionCandidate)manager.Current;
                BadSpelling = cc.Current as string;
                NotifyOfPropertyChange(() => BadSpelling);
                SCaddinsApp.WindowManager.ShowMessageBox(cc.TypeString);
            } else
            {
                SCaddinsApp.WindowManager.ShowMessageBox("Finished");
            }
        }

        public void Options()
        {
            var viewModel = new ViewModels.SpellCheckerOptionsViewModel();
            var result = SCaddinsApp.WindowManager.ShowDialog(viewModel,
                null,
                ViewModels.SpellCheckerOptionsViewModel.DefaultWindowSettings);
        }


    }
}
