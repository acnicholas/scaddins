namespace SCaddins.ModelSetupWizard
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    class WorksetParameter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private bool visibleInAllViews;
        private string name;
        private bool visibleInAllViewsInitialValue;

        public WorksetParameter(string name, bool visibleInAllViews, bool existing)
        {
            Name = name;
            VisibleInAllViews = visibleInAllViews;
            visibleInAllViewsInitialValue = visibleInAllViews;
            IsExisting = existing;
            ExistingName = existing ? name : string.Empty;
        }

        public WorksetParameter() : this(string.Empty, false, false) { }

        public WorksetParameter(string name, bool visibleInAllViews, string existingName) : this(name, visibleInAllViews, true)
        {
            ExistingName = existingName;
        }

        public override string ToString()
        {
            return string.Format("{0};{1};{2}", Name, VisibleInAllViews, ExistingName);
        }

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value != name)
                {
                    name = value;
                    NotifyPropertyChanged(nameof(IsModified));
                }
            }
        }

        public bool VisibleInAllViews
        {
            get
            {
                return visibleInAllViews;
            }
            set
            {
                if(value != visibleInAllViews)
                {
                    visibleInAllViews = value;
                    NotifyPropertyChanged(nameof(IsModified));
                }
            }
        }

        public bool IsExisting
        {
            get; private set;
        }

        public string ExistingName
        {
            get; private set;
        }

        public bool IsModified
        {
            get
            {
                return VisibleInAllViews != visibleInAllViewsInitialValue || (IsExisting && Name != ExistingName) || !IsExisting;
            }
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
