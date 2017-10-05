namespace SCaddins.ExportManager
{
    using System;
    using Autodesk.Revit.UI;
        
    public class OpenableView
    {
        public OpenableView(string name, Autodesk.Revit.DB.View view)
        {
            View = view;
            Name = name;
        }
        
        public string Name
        {
            get; set;
        }
        
        public Autodesk.Revit.DB.View View
        {
            get; set;
        }
        
        public void Open()
        {
             UIApplication uiapp = new UIApplication(View.Document.Application);
             if(View != null) {
                    uiapp.ActiveUIDocument.ActiveView = View;
             }
        }
    }
}
