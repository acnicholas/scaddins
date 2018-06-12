using Caliburn.Micro;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Controls;

namespace SCaddins.RenameUtilities.ViewModels
{
    class RenameUtilitiesViewModel : Screen
    {
        private RenameManager manager;
        private string selectedParameterCategory;
        private RenameParameter selectedRenameParameter;
        List<RenameCandidate> selectedCandiates = new List<RenameCandidate>();

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 480;
                settings.Width = 768;
                settings.Title = "Rename Selected Sheet Parameters";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                settings.ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                return settings;
            }
        }

        //Constructors
        #region
        public RenameUtilitiesViewModel(RenameManager manager)
        {
            this.manager = manager;
            selectedParameterCategory = string.Empty;
            selectedParameterCategory = null;
        }
        #endregion

        public BindableCollection<string> ParameterCategories
        {
            get
            {
                return manager.AvailableParameterTypes;
            }
        }

        public string SelectedParameterCategory
        {
            get { return selectedParameterCategory; }
            set
            {
                selectedParameterCategory = value;
                NotifyOfPropertyChange(() => SelectedParameterCategory);
                NotifyOfPropertyChange(() => RenameParameters);
            }
        }

        public BindableCollection<RenameParameter> RenameParameters
        {
            get
            {
                return manager.GetParametersByCategoryName(selectedParameterCategory);
            }
        }

        public RenameParameter SelectedRenameParameter
        {
            get { return selectedRenameParameter; }
            set
            {
                selectedRenameParameter = value;
                manager.SetCandidatesByParameter(selectedRenameParameter.Parameter, selectedRenameParameter.Category);
                NotifyOfPropertyChange(() => SelectedRenameParameter);
                NotifyOfPropertyChange(() => RenameCandidates);
            }
        }

        public BindableCollection<RenameCommand> RenameModes
        {
            get
            {
                return manager.RenameModes;
            }
        }

        public RenameCommand SelectedRenameMode
        {
            get { return manager.SelectedRenameMode; }
            set
            {
                manager.SelectedRenameMode = value;
                NotifyOfPropertyChange(() => SelectedRenameMode);
                NotifyOfPropertyChange(() => Pattern);
                NotifyOfPropertyChange(() => Replacement);
                NotifyOfPropertyChange(() => ShowRenameParameters);
                NotifyOfPropertyChange(() => ReplacementLabel);
                NotifyOfPropertyChange(() => PatternLabel);
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

        public string PatternLabel
        {
            get { return manager.SelectedRenameMode.SearchPatternHint; }
        }

        public string Pattern
        {
            get { return manager.renameCommand.SearchPattern; }
            set
            {
                manager.renameCommand.SearchPattern = value;
                manager.Rename();
                NotifyOfPropertyChange(() => Pattern);
            }
        }

        public string ReplacementLabel
        {
            get { return manager.SelectedRenameMode.ReplacementPatternHint; }
        }

        public string Replacement
        {
            get { return manager.renameCommand.ReplacementPattern; }
            set
            {
                manager.renameCommand.ReplacementPattern = value;
                manager.Rename();
                NotifyOfPropertyChange(() => Replacement);
            }
        }

        public BindableCollection<RenameCandidate> RenameCandidates
        {
            get { return manager.RenameCandidates; }
        }

        public RenameCandidate SelectedRenameCandidate
        {
            get; set;
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
            manager.SetCandidatesByParameter(selectedRenameParameter.Parameter, selectedRenameParameter.Category);
            NotifyOfPropertyChange(() => RenameCandidates);
        }

        public string RenameSelectedMatchesLabel
        {
            get
            {
                var selectionCount = selectedCandiates.Where(m => m.ValueChanged).Count();
                return "Rename " + selectionCount + " matches";
            }
        }

        public void RenameAllMatches()
        {
            manager.CommitRename();
            manager.SetCandidatesByParameter(selectedRenameParameter.Parameter, selectedRenameParameter.Category);
            NotifyOfPropertyChange(() => RenameCandidates);
        }

        public string RenameAllMatchesLabel
        {
            get
            {
                var count = RenameCandidates.Where(m => m.ValueChanged).Count();
                return "Rename all " + count + " parameters";
            }
        }
    }
}
