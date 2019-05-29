namespace SCaddins.ModelSetupWizard
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    class WorksetParameter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public WorksetParameter(string name, bool visibleInAllViews, bool existing)
        {
            Name = name;
            VisibleInAllViews = visibleInAllViews;
            Existing = existing;
            ExistingName = existing ? name : string.Empty;
        }

        public WorksetParameter() : this(string.Empty, false, false) { }

        public string Name
        {
            get; set;
        }

        public bool VisibleInAllViews
        {
            get; set;
        }

        public bool Existing
        {
            get; private set;
        }

        public string ExistingName
        {
            get; private set;
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
