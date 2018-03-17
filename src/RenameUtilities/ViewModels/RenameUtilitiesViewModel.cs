using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;


namespace SCaddins.RenameUtilities.ViewModels
{
    class RenameUtilitiesViewModel : Screen
    {
        private RenameManager manager;
        private string selectedParameterCategory;
        private RenameParameter selectedRenameParameter;
        private string selectedRenameMode;

        public RenameUtilitiesViewModel(RenameManager manager)
        {
            this.manager = manager;
            selectedParameterCategory = string.Empty;
            selectedParameterCategory = null;
            selectedRenameMode = string.Empty;
        }

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
                return manager.RenameParametersByCategory(selectedParameterCategory);
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

        public BindableCollection<string> RenameModes
        {
            get
            {
                return manager.RenameModes;
            }
        }

        public string SelectedRenameMode
        {
            get { return selectedRenameMode; }
            set
            {
                selectedRenameMode = value;
                NotifyOfPropertyChange(() => SelectedRenameMode);
            }
        }

        public string Pattern
        {
            get
            {
                return "Pattern";
            }
        }

        public string Replacement
        {
            get
            {
                return "Replacement";
            }
        }

        public BindableCollection<SCaddins.RenameUtilities.RenameCandidate> RenameCandidates
        {
            get
            {
                return manager.RenameCandidates;
            }
        }

    }
}
