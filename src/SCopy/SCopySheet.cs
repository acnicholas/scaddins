using System;
using System.ComponentModel;
using Autodesk.Revit.DB;

namespace SCaddins.SCopy
{

/// <summary>
/// Description of SCopySheet.
/// </summary>
public class SCopySheet  : INotifyPropertyChanged
{
    private SCopy scopy;
    private string title;
    private string number;
    private ViewSheet destSheet;
    public event PropertyChangedEventHandler PropertyChanged;
    private BindingList<SCopyViewOnSheet> viewsOnSheet;

    public ViewSheet DestSheet
    {
        get {
            return destSheet;
        } set {
            destSheet = value;
        }
    }

    public string Number
    {
        get {
            return number;
        } set {
            if(value != number && scopy.CheckSheetNumberAvailability(value)) {
                number = value;
                if (PropertyChanged != null) {
                    PropertyChanged(
                        this, new PropertyChangedEventArgs("Number"));
                }
            } else {
                Autodesk.Revit.UI.TaskDialog.Show(
                    "SCopy - WARNING", value + " exists, you can't use it!.");
            }
        }
    }

    public string Title
    {
        get {
            return title;
        } set {
            title = value;
            if (PropertyChanged != null) {
                PropertyChanged( this, new PropertyChangedEventArgs("Title"));
            }
        }
    }

    public string GetNewViewName(ElementId id)
    {
        foreach (SCopyViewOnSheet v in viewsOnSheet) {
            if (id == v.OldId) {
                return v.Title;
            }
        }
        return null;
    }

    public BindingList<SCopyViewOnSheet> ViewsOnSheet
    {
        get {
            return viewsOnSheet;
        }
    }

    public SCopySheet(string number, string title, SCopy scopy)
    {
        this.scopy = scopy;
        this.number = number;
        this.title= title;
        this.destSheet = null;
        viewsOnSheet = new BindingList<SCopyViewOnSheet>();
        foreach(View v in scopy.SourceSheet.Views) {
            viewsOnSheet.Add(new SCopyViewOnSheet(v.Name, v, scopy));
        }
    }

}

}
/* vim: set ts=4 sw=4 nu expandtab: */
