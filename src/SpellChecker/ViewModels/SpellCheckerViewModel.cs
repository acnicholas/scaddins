// (C) Copyright 2019-2020 by Andrew Nicholas (andrewnicholas@iinet.net.au)
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.SpellChecker.ViewModels
{
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
            }
            if (manager.GetCurrentSuggestions().Count > 0)
            {
                ReplacementText = manager.GetCurrentSuggestions().First();
            }
        }

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

        public bool AddToDictionaryEnabled
        {
            get; set;
        }

        public bool CanChange => !string.IsNullOrEmpty(ReplacementText);

        public bool CanChangeAll => !string.IsNullOrEmpty(ReplacementText);

        public string NotInDictionary => @"Not In Dictionary [" + manager.CurrentElementType + @"]";

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

        public string SelectedSuggestion
        {
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

        public List<string> Suggestions
        {
            get
            {
                return manager.GetCurrentSuggestions();
            }
        }

        public string UnknownWord => manager.CurrentUnknownWord;

        public string UnknownWordContext
        {
            get; set;
        }

        public void AddToDictionary()
        {
            // manager.AddWordToDictionary(UnknownWord);
        }

        public void Apply()
        {
            // FIXME put this in command.
            manager.ProcessAllAutoReplacements();
            manager.CommitSpellingChangesToModel();
            TryCloseAsync(true);
        }

        public void Cancel()
        {
            TryCloseAsync(false);
        }

        public void Change()
        {
            manager.CurrentCandidate.ReplaceCurrent(ReplacementText);

            // Find the next spelling error
            Next();
        }

        public void ChangeAll()
        {
            manager.CurrentCandidate.ReplaceCurrent(ReplacementText);
            manager.AddToAutoReplacementList(UnknownWord, ReplacementText);

            // Find the next spelling error
            Next();
        }

        public void IgnoreAll()
        {
            manager.IgnoreAll();

            // Find the next spelling error
            Next();
        }

        public void IgnoreOnce()
        {
            // Find the next spelling error
            Next();
        }

        public void Show()
        {
            if (manager.CurrentCandidate != null)
            {
                var uiapp = new Autodesk.Revit.UI.UIApplication(manager.Document.Application);
                Autodesk.Revit.DB.Element e = manager.Document.GetElement(manager.CurrentCandidate.ParentElementId);
                if (e is Autodesk.Revit.DB.View)
                {
                    uiapp.ActiveUIDocument.ActiveView = (Autodesk.Revit.DB.View)e;
                }
                else
                {
                    uiapp.ActiveUIDocument.ShowElements(e.Id);
                }
            }
        }

        // Find the next spelling error
        private void Next()
        {
            if (!manager.MoveNext())
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
