namespace SCaddins.RoomConverter.ViewModels
{
    using Autodesk.Revit.Creation;
    using Caliburn.Micro;

    public class SettingsViewModel : PropertyChangedBase, Common.ViewModels.ISettingPanel
    {

        private Autodesk.Revit.DB.Document doc;
        public SettingsViewModel(Autodesk.Revit.DB.Document doc)
        {
            this.doc = doc;
        }

        public bool RoomIdParameterExistsAndAssigned
        {
            get { return RoomConversionManager.RoomIdParameterExists(doc); }
        }

        public void CreateRoomIdParameter()
        {
            RoomConversionManager.CreateRoomIdParameter(doc);
        }

        public void Apply()
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public void ResetToDefault()
        {
            throw new System.NotImplementedException();
        }
    }
}
