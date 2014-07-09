using System;
using System.ComponentModel;
using Autodesk.Revit.DB;

namespace SCaddins.SCopy
{

/// <summary>
/// Description of SCopySheetView.
/// </summary>
public class SCopyViewOnSheet  : INotifyPropertyChanged
{
    private SCopy scopy;
    private string originalTitle;
    private string newTitle;
    private ElementId oldId;
    private string associatedLevelName;
    private View oldView;
    private string viewTemplateName;
    public event PropertyChangedEventHandler PropertyChanged;
    private bool duplicateWithDetailing;
    private SCopy.ViewCreationMode creationMode;
   

    public SCopyViewOnSheet(string title, View v, SCopy scopy)
    {
        this.scopy = scopy;
        this.oldView = v;
        this.oldId = v.Id;
        this.originalTitle = title;
        SetDefualtCreationMode();
        this.newTitle =
            title + @"(" + ((DateTime.Now.TimeOfDay.Ticks)/100000).ToString() + @")";
        this.associatedLevelName = SCopyConstants.MenuItemCopy;
        this.viewTemplateName = SCopyConstants.MenuItemCopy;
        this.duplicateWithDetailing = true;
    }
    
    private void SetDefualtCreationMode()
    {
        if( oldView.ViewType == ViewType.Legend ){
            this.creationMode = SCopy.ViewCreationMode.Place;   
        } else {
            this.creationMode = SCopy.ViewCreationMode.Copy;
        }
    }
    
    public SCopy.ViewCreationMode CreationMode
    {
        get {
            return creationMode;
        }
    }

    public ElementId OldId
    {
        get {
            return oldId;
        }
    }

    public View OldView
    {
        get {
            return oldView;
        }
    }

    public ViewType RevitViewType
    {
        get {
            return oldView.ViewType;
        }
    }

    public bool DuplicateWithDetailing
    {
        get {
            return duplicateWithDetailing;
        } set {
            duplicateWithDetailing = value;
            if (PropertyChanged != null) {
                PropertyChanged(
                    this, new PropertyChangedEventArgs("DuplicateWithDetailing"));
            }
        }
    }

    public string ViewTemplateName
    {
        get {
            return viewTemplateName;
        } set {
            viewTemplateName = value;
            if (PropertyChanged != null) {
                PropertyChanged(
                    this, new PropertyChangedEventArgs("ViewTemplateName"));
            }
        }
    }

    public string AssociatedLevelName
    {
        get {
            return associatedLevelName;
        } set {
            associatedLevelName = value;
            if (value != SCopyConstants.MenuItemCopy) {
                DuplicateWithDetailing = false;
                creationMode = SCopy.ViewCreationMode.CopyAndModify;
            } else {
                creationMode = SCopy.ViewCreationMode.Copy;   
            }
            if (PropertyChanged != null) {
                PropertyChanged(
                    this, new PropertyChangedEventArgs("AssociatedLevelName"));
            }
        }
    }

    public string Title
    {
        get {
            return newTitle;
        } set {
            if(value != newTitle && scopy.CheckViewNameAvailability(value)) {
                newTitle = value;
                if (PropertyChanged != null) {
                    PropertyChanged(
                        this, new PropertyChangedEventArgs("Title"));
                }
            } else {
                Autodesk.Revit.UI.TaskDialog.Show(
                    "SCopy - WARNING", value + " exists, you can't use it!.");
            }
        }
    }

    public string OriginalTitle
    {
        get {
            return originalTitle;
        }
    }
    
    public static bool PlanEnough(ViewType vt)
    {
         switch (vt) {
            case ViewType.FloorPlan:
            case ViewType.CeilingPlan:
            case ViewType.AreaPlan:
                return true;
            default:
                return false;
         }
    }
    
    public bool PlanEnough()
    {
        return PlanEnough(this.RevitViewType);
    }
}

}
/* vim: set ts=4 sw=4 nu expandtab: */
