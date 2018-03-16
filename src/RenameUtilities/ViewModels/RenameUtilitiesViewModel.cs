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
        private BindableCollection<SCaddins.RenameUtilities.RenameParameter> renameParameters;

        public RenameUtilitiesViewModel(RenameManager manager)
        {
            this.manager = manager;
            selectedParameterCategory = string.Empty;
            selectedParameterCategory = null;
            renameParameters = new BindableCollection<SCaddins.RenameUtilities.RenameParameter>();
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
            get {
                return manager.RenameParametersByCategory(selectedParameterCategory); }
        }

        public RenameParameter SelectedRenameParameter
        {
            get { return selectedRenameParameter; }
            set
            {
                selectedRenameParameter = value;
                NotifyOfPropertyChange(() => SelectedRenameParameter);
            }
        }

    }
}
