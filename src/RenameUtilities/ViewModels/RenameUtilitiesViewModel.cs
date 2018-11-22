// (C) Copyright 2018 by Andrew Nicholas
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

namespace SCaddins.RenameUtilities.ViewModels
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Caliburn.Micro;

    internal class RenameUtilitiesViewModel : Screen
    {
        private RenameManager manager;
        private List<RenameCandidate> selectedCandiates = new List<RenameCandidate>();
        private string selectedParameterCategory;
        private RenameParameter selectedRenameParameter;

        public RenameUtilitiesViewModel(RenameManager manager)
        {
            this.manager = manager;
            selectedParameterCategory = string.Empty;
            selectedParameterCategory = null;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 480;
                settings.Width = 768;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new System.Uri("pack://application:,,,/SCaddins;component/Assets/rename.png"));
                settings.Title = "Rename Selected Sheet Parameters";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        ////Constructors
        #region
        #endregion

        public BindableCollection<string> ParameterCategories
        {
            get
            {
                return manager.AvailableParameterTypes;
            }
        }

        public string Pattern
        {
            get
            {
                return manager.ActiveRenameCommand.SearchPattern;
            }

            set
            {
                manager.ActiveRenameCommand.SearchPattern = value;
                manager.Rename();
                NotifyOfPropertyChange(() => Pattern);
                NotifyOfPropertyChange(() => RenameAllMatchesLabel);
            }
        }

        public string PatternLabel
        {
            get { return manager.SelectedRenameMode.SearchPatternHint; }
        }

        public string RenameAllMatchesLabel
        {
            get
            {
                var count = RenameCandidates.Where(m => m.ValueChanged).Count();
                return "Rename all " + count + " parameters";
            }
        }

        public BindableCollection<RenameCandidate> RenameCandidates
        {
            get { return manager.RenameCandidates; }
        }

        public BindableCollection<RenameCommand> RenameModes
        {
            get
            {
                return manager.RenameModes;
            }
        }

        public BindableCollection<RenameParameter> RenameParameters
        {
            get
            {
                return manager.GetParametersByCategoryName(selectedParameterCategory);
            }
        }

        public string RenameSelectedMatchesLabel
        {
            get
            {
                var selectionCount = selectedCandiates.Where(m => m.ValueChanged).Count();
                return "Rename " + selectionCount + " selected matches";
            }
        }

        public string Replacement
        {
            get
            {
                return manager.ActiveRenameCommand.ReplacementPattern;
            }

            set
            {
                manager.ActiveRenameCommand.ReplacementPattern = value;
                manager.Rename();
                NotifyOfPropertyChange(() => Replacement);
                NotifyOfPropertyChange(() => RenameAllMatchesLabel);
            }
        }

        public string ReplacementLabel
        {
            get
            {
                return manager.SelectedRenameMode.ReplacementPatternHint;
            }
        }

        public string SelectedParameterCategory
        {
            get
            {
                return selectedParameterCategory;
            }

            set
            {
                selectedParameterCategory = value;
                NotifyOfPropertyChange(() => SelectedParameterCategory);
                NotifyOfPropertyChange(() => RenameParameters);
            }
        }

        public RenameCandidate SelectedRenameCandidate
        {
            get; set;
        }

        public RenameCommand SelectedRenameMode
        {
            get
            {
                return manager.SelectedRenameMode;
            }

            set
            {
                manager.SelectedRenameMode = value;
                NotifyOfPropertyChange(() => SelectedRenameMode);
                NotifyOfPropertyChange(() => Pattern);
                NotifyOfPropertyChange(() => Replacement);
                NotifyOfPropertyChange(() => ShowRenameParameters);
                NotifyOfPropertyChange(() => ReplacementLabel);
                NotifyOfPropertyChange(() => PatternLabel);
                NotifyOfPropertyChange(() => RenameAllMatchesLabel);
            }
        }

        public RenameParameter SelectedRenameParameter
        {
            get
            {
                return selectedRenameParameter;
            }

            set
            {
                selectedRenameParameter = value;
                manager.SetCandidatesByParameter(selectedRenameParameter.Parameter, selectedRenameParameter.Category, selectedRenameParameter.Type);
                NotifyOfPropertyChange(() => SelectedRenameParameter);
                NotifyOfPropertyChange(() => RenameCandidates);
            }
        }

        public System.Windows.Visibility ShowRenameParameters
        {
            get
            {
                return manager.SelectedRenameMode.HasInputParameters == true
                    ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            }
        }

        public void RenameAllMatches()
        {
            manager.CommitRename();
            manager.SetCandidatesByParameter(selectedRenameParameter.Parameter, selectedRenameParameter.Category, selectedRenameParameter.Type);
            NotifyOfPropertyChange(() => RenameCandidates);
        }

        public void RenameCandidatesSelectionChanged(System.Windows.Controls.SelectionChangedEventArgs obj)
        {
            selectedCandiates.AddRange(obj.AddedItems.Cast<RenameCandidate>());
            obj.RemovedItems.Cast<RenameCandidate>().ToList().ForEach(w => selectedCandiates.Remove(w));
            NotifyOfPropertyChange(() => RenameSelectedMatchesLabel);
            NotifyOfPropertyChange(() => RenameAllMatchesLabel);
        }

        public void RenameSelectedMatches()
        {
            manager.CommitRenameSelection(selectedCandiates);
            manager.SetCandidatesByParameter(selectedRenameParameter.Parameter, selectedRenameParameter.Category, selectedRenameParameter.Type);
            NotifyOfPropertyChange(() => RenameCandidates);
        }
    }
}