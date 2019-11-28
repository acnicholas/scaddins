namespace SCaddins.SpellChecker.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Caliburn.Micro;

    public class SpellCheckerViewModel : Screen
    {
        private SpellChecker manager;
        private string replacementText;
        private string selectedSuggestion;
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

        public bool AddToDictionaryEnabled {
            get; set;
        }

        public bool CanChange => !string.IsNullOrEmpty(ReplacementText);

        public bool CanChangeAll => !string.IsNullOrEmpty(ReplacementText);

        public string NotInDictionary => @"Not In Dictionary [" + manager.CurrentElementType + @"]";

        public string ReplacementText {
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

        public List<string> Suggestions {
            get
            {
                return manager.GetCurrentSuggestions();
            }
        }

        public string UnknownWord => manager.CurrentUnknownWord;

        public string UnknownWordContext {
            get; set;
        }

        public void AddToDictionary()
        {
            // manager.AddWordToDictionary(UnknownWord);
        }

        public void Apply()
        {
            // FIXME put this in command.
            manager.CommitSpellingChangesToModel();
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }

        public void Change()
        {
            manager.CurrentCandidate.ReplaceCurrent(ReplacementText);
            Next();
        }

        public void ChangeAll()
        {
            manager.CurrentCandidate.ReplaceCurrent(ReplacementText);
            manager.AddToAutoReplacementList(UnknownWord, ReplacementText);
            Next();
        }

        public void IgnoreAll()
        {
            manager.IgnoreAll();
            Next();
        }

        public void IgnoreOnce()
        {
            Next();
        }

        public void Options()
        {
            var viewModel = new SpellCheckerOptionsViewModel();
            var result = SCaddinsApp.WindowManager.ShowDialog(viewModel, null, SpellCheckerOptionsViewModel.DefaultWindowSettings);
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
                SCaddinsApp.WindowManager.ShowMessageBox("Spelling Check Complete");
            }
        }
    }
}
