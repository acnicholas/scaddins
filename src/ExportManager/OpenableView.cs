namespace SCaddins.ExportManager
{
    using System;
    using Autodesk.Revit.UI;
        
    public class OpenableView
    {
        private Autodesk.Revit.DB.View view;

        public OpenableView(string name, string number, Autodesk.Revit.DB.View view)
        {
            this.view = view;
            Name = name;
            SheetNumber = number;
        }
        
        public string Name
        {
            get; set;
        }
        
        public string SheetNumber
        {
            get; set;
        }
        
        public string ViewType {
            get {
                return view.ViewType.ToString();
            }
        }
               
        public void Open()
        {
            if (view != null) {
                UIApplication uiapp = new UIApplication(view.Document.Application);
                uiapp.ActiveUIDocument.ActiveView = view;
            }
        }
    }
}
