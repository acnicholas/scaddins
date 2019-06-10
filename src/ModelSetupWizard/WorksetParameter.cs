namespace SCaddins.ModelSetupWizard
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class WorksetParameter : INotifyPropertyChanged
    {
        private string name;

        private bool visibleInAllViews;

        private bool visibleInAllViewsInitialValue;

        public WorksetParameter(string name, bool visibleInAllViews, int id)
        {
            Name = name;
            VisibleInAllViews = visibleInAllViews;
            visibleInAllViewsInitialValue = visibleInAllViews;
            Id = id;
            ExistingName = IsExisting ? Name : string.Empty;
        }

        public WorksetParameter() : this(string.Empty, false, -1)
        {
        }

        public WorksetParameter(string name, bool visibleInAllViews, string existingName)
        {
            Name = name;
            VisibleInAllViews = visibleInAllViews;
            visibleInAllViewsInitialValue = visibleInAllViews;
            Id = -1;
            ExistingName = existingName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string ExistingName
        {
            get; private set;
        }

        public int Id
        {
            get; private set;
        }

        public bool IsExisting
        {
            get { return Id > -1; }
        }

        public bool IsModified
        {
            get
            {
                return VisibleInAllViews != visibleInAllViewsInitialValue || (IsExisting && Name != ExistingName) || !IsExisting;
            }
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
                if (value != visibleInAllViews)
                {
                    visibleInAllViews = value;
                    NotifyPropertyChanged(nameof(IsModified));
                }
            }
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0};{1};{2}", Name, VisibleInAllViews, ExistingName);
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
