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
        private string replacementText;
        private string unknownWord;

        public SpellCheckerViewModel(SpellChecker manager)
        {
            this.manager = manager;
            if (manager.MoveNext())
            {
                unknownWord = ((CorrectionCandidate)manager.Current).Current as string;
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
                ReplacementText = selectedSuggestion;
                NotifyOfPropertyChange(() => ReplacementText);
            }
        }

        public void AddToDictionary()
        {
            //manager.AddWordToDictionary(UnknownWord);
        }

        public void Apply()
        {
            // FIXME put this in command.
            manager.CommitSpellingChangesToModel();
            TryClose(true);
        }

        public bool AddToDictionaryEnabled
        {
            get; set;
        }

        public void Change()
        {
            manager.CurrentCandidate.ReplaceCurrent(ReplacementText);
            Next();
        }

        public bool CanChange => !string.IsNullOrEmpty(ReplacementText);

        public void ChangeAll()
        {
            manager.CurrentCandidate.ReplaceCurrent(ReplacementText);
            manager.AddToAutoReplacementList(UnknownWord, ReplacementText);
            Next();
        }

        public string NotInDictionary => @"Not In Dictionary [" + manager.CurrentElementType + @"]";

        public bool CanChangeAll => !string.IsNullOrEmpty(ReplacementText);

        public string UnknownWord => manager.CurrentUnknownWord;

        public string UnknownWordContext
        {
            get; set;
        }

        public string ReplacementText
        {
            get
            {
                return replacementText;
            }
            set
            {
                replacementText = value;
                NotifyOfPropertyChange(() => ReplacementText);
                NotifyOfPropertyChange(() => CanChangeAll);
                NotifyOfPropertyChange(() => CanChange);
            }
        }

        public void Cancel()
        {
            TryClose(false);
        }

        public void IgnoreAll()
        {
            manager.IgnoreAll();
            Next();
        }

        private void Next()
        {
            if (manager.MoveNext())
            {
                NotifyOfPropertyChange(() => UnknownWord);
                NotifyOfPropertyChange(() => Suggestions);
                NotifyOfPropertyChange(() => NotInDictionary);
                if (Suggestions.Count > 0)
                {
                    SelectedSuggestion = Suggestions.First();
                    ReplacementText = SelectedSuggestion;
                }
            }
            else
            {
                SCaddinsApp.WindowManager.ShowMessageBox("Finished");
            }
        }

        public void IgnoreOnce()
        {
            Next();
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
