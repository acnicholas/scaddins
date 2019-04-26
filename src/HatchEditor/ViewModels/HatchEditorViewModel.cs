namespace SCaddins.HatchEditor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Autodesk.Revit.DB;
    using Caliburn.Micro;

    internal class HatchEditorViewModel : Screen
    {
        private Document doc;
        private ObservableCollection<FillPattern> fillPattensInModel;
        private FillPattern selectedFillPattern;
        private string currentPatternDefinition;

        public HatchEditorViewModel(Autodesk.Revit.DB.Document doc)
        {
            this.doc = doc;
            FillPatterns = new ObservableCollection<FillPattern>(HatchEditor.Command.FillPatterns(doc));
            CurrentPatternDefinition = "test";
        }

        public ObservableCollection<FillPattern> FillPatterns
        {
            get
            {
                return fillPattensInModel;
            }

            set
            {
                fillPattensInModel = value;
                NotifyOfPropertyChange(() => FillPatterns);
            }
        }

        public FillPattern SelectedFillPattern
        {
            get
            {
                return selectedFillPattern;
            }

            set
            {
                selectedFillPattern = value;
                CurrentPatternDefinition = HatchEditor.Command.GetPatternDefinition(SelectedFillPattern);
                NotifyOfPropertyChange(() => SelectedFillPattern);
                NotifyOfPropertyChange(() => CurrentPatternType);
            }
        }

        public string CurrentPatternDefinition
        {
            get
            {
                return currentPatternDefinition;
            }

            set
            {
                currentPatternDefinition = value;
                NotifyOfPropertyChange(() => CurrentPatternDefinition);
            }
        }

        public FillPatternTarget CurrentPatternType
        {
            get
            {
                return selectedFillPattern.Target;
            } 
        }

    }
}
