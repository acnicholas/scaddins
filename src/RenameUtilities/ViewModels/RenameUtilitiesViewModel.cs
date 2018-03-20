using Caliburn.Micro;

namespace SCaddins.RenameUtilities.ViewModels
{
    class RenameUtilitiesViewModel : Screen
    {
        private RenameManager manager;
        private string selectedParameterCategory;
        private RenameParameter selectedRenameParameter;

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
                manager.DryRename();
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
                manager.DryRename();
                NotifyOfPropertyChange(() => Replacement);
            }
        }

        public BindableCollection<SCaddins.RenameUtilities.RenameCandidate> RenameCandidates
        {
            get { return manager.RenameCandidates; }
        }

    }
}
