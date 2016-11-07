
using System;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.UI;

namespace SCaddins.RoomConvertor
{
    /// <summary>
    /// Description of RoomToSheetWizard.
    /// </summary>
    public partial class RoomToSheetWizard : System.Windows.Forms.Form
    {
        RoomConversionManager roomConversionManager;
        
        public RoomToSheetWizard(RoomConversionManager roomConversionManager)
        {
            this.roomConversionManager = roomConversionManager;
            InitializeComponent();
            this.PopulateTitleblockCombo();
             
            //set titleblock combo to last index which should be none.
            this.comboBoxTitles.SelectedIndex = this.comboBoxTitles.Items.Count - 1;
            this.textBox1.Text = roomConversionManager.Scale.ToString();
        }
        
        private void PopulateTitleblockCombo()
        {
            foreach (var key in roomConversionManager.TitleBlocks.Keys){
                comboBoxTitles.Items.Add(key);
            }
            comboBoxTitles.SelectedIndex = 0;
        }
             
        private void ButtonOKClicked(object sender, EventArgs e)
        {
            //roomConversionManager.ViewTemplateId = ElementId.InvalidElementId;
            int i = 0;
            if (int.TryParse(textBox1.Text, out i)) {
                roomConversionManager.Scale = i;
            } else {
                TaskDialog td = new TaskDialog("Scale Input Error");
                td.MainInstruction = "Invalid Scale.";
                td.MainContent = "Scale input: " + textBox1.Text + " cannot be used." + System.Environment.NewLine +
                    "Last good value (" + roomConversionManager.Scale + ") will be used";
                td.MainIcon = TaskDialogIcon.TaskDialogIconWarning;
                td.Show();   
            }
            roomConversionManager.TitleBlockId = 
                roomConversionManager.GetTitleBlockByName(this.comboBoxTitles.Text);
        }
    }
}
