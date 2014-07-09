using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.DB;

namespace SCaddins.SCulcase
{
    public partial class SCulcaseMainForm : System.Windows.Forms.Form
    {
        private Document doc;
        private SCulcase.ConversionTypes conversionTypes;
        private SCulcase.ConversionMode conversionMode;

        public SCulcaseMainForm(Document doc)
        {
            this.doc = doc;    
            conversionTypes = SCulcase.ConversionTypes.None;
            conversionMode = SCulcase.ConversionMode.UPPER_CASE;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            tagCheckBoxes();
            addIcon();
            this.ShowDialog();
        }
        
        private void addIcon()
        {
        	System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream st = a.GetManifestResourceStream("SCulcase.Resources.sculcase.ico");
            System.Drawing.Icon icnTask = new System.Drawing.Icon(st);
            this.Icon = icnTask;  	
        }
        
        private void tagCheckBoxes()
        {
        	chkAnnotation.Tag = SCulcase.ConversionTypes.TEXT;
            chkRooms.Tag = SCulcase.ConversionTypes.ROOM_NAMES;
            chkSheets.Tag = SCulcase.ConversionTypes.SHEET_NAMES;
            chkViews.Tag = SCulcase.ConversionTypes.VIEW_NAMES;
            chkViewTitleOnSheets.Tag = SCulcase.ConversionTypes.TITLES_ON_SHEETS;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            SCulcase.ConvertAllDryRun(conversionMode, conversionTypes, ref doc);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) conversionMode = SCulcase.ConversionMode.UPPER_CASE;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked) conversionMode = SCulcase.ConversionMode.LOWER_CASE;
        }
        
        private void RadioButton3CheckedChanged(object sender, EventArgs e)
        {
        	if (radioButton3.Checked) conversionMode = SCulcase.ConversionMode.TITLE_CASE;	
        }

        private void toggleConversionFlag(CheckBox box, SCulcase.ConversionTypes val)
        {
            if(box.Checked == true){
                conversionTypes |= val;
            }else{
                conversionTypes = conversionTypes & ~val;  
            }
        }
        
        void toggleCheckBoxValue(object sender, EventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            SCulcase.ConversionTypes t = (SCulcase.ConversionTypes)c.Tag;
            toggleConversionFlag(c, t);
        }
      
        void BtnOKAYClick(object sender, EventArgs e)
        {
            SCulcase.ConvertAll(conversionMode, conversionTypes,ref doc);
        }
        

    }
}
