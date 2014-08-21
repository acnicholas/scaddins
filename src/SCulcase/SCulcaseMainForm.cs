namespace SCaddins.SCulcase
{
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using Autodesk.Revit.DB;
    
    public partial class SCulcaseMainForm : System.Windows.Forms.Form
    {
        private Document doc;
        private SCulcase.ConversionTypes conversionTypes;
        private SCulcase.ConversionMode conversionMode;

        public SCulcaseMainForm(Document doc)
        {
            this.doc = doc;    
            this.conversionTypes = SCulcase.ConversionTypes.None;
            this.conversionMode = SCulcase.ConversionMode.UPPER_CASE;
            this.InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.TagCheckBoxes();
            this.ShowDialog();
        }
               
        private void TagCheckBoxes()
        {
            chkAnnotation.Tag = SCulcase.ConversionTypes.TEXT;
            chkRooms.Tag = SCulcase.ConversionTypes.ROOM_NAMES;
            chkSheets.Tag = SCulcase.ConversionTypes.SHEET_NAMES;
            chkViews.Tag = SCulcase.ConversionTypes.VIEW_NAMES;
            chkViewTitleOnSheets.Tag = SCulcase.ConversionTypes.TITLES_ON_SHEETS;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            SCulcase.ConvertAllDryRun(this.conversionMode, this.conversionTypes, ref this.doc);
        }

        private void RadioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) {
                this.conversionMode = SCulcase.ConversionMode.UPPER_CASE;
            }
        }

        private void RadioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked) {
                this.conversionMode = SCulcase.ConversionMode.LOWER_CASE;
            }
        }
        
        private void RadioButton3CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked) {
                this.conversionMode = SCulcase.ConversionMode.TITLE_CASE;
            }
        }

        private void ToggleConversionFlag(CheckBox box, SCulcase.ConversionTypes val)
        {
            if (box.Checked == true) {
                this.conversionTypes |= val;
            } else {
                this.conversionTypes = this.conversionTypes & ~val;  
            }
        }
        
        private void ToggleCheckBoxValue(object sender, EventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            SCulcase.ConversionTypes t = (SCulcase.ConversionTypes)c.Tag;
            this.ToggleConversionFlag(c, t);
        }
      
        private void BtnOKAYClick(object sender, EventArgs e)
        {
            SCulcase.ConvertAll(this.conversionMode, this.conversionTypes, ref this.doc);
        }
    }
}
