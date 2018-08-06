using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Caliburn.Micro;


namespace SCaddins.ParameterUtilities.ViewModels 
{
    class SCincrementViewModel : Screen
    {

        public SCincrementViewModel()
        {

        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new System.Dynamic.ExpandoObject();
                settings.Height = 400;
                settings.Title = "SCincrement Settigns";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.WidthAndHeight;
                settings.ResizeMode = System.Windows.ResizeMode.CanResize;
                return settings;
            }
        }

        public void Quit()
        {
            TryClose(false);
        }


    }
}


//public SCincrementSettingsForm()
//{
//    this.InitializeComponent();
//    this.LoadSettings();
//}

//private void LoadSettings()
//{
//    this.offsetTextBox.Text = SCincrementSettings.Default.OffsetValue.ToString(CultureInfo.CurrentCulture);
//    this.incrementTextBox.Text = SCincrementSettings.Default.IncrementValue.ToString(CultureInfo.CurrentCulture);
//    this.replacementTextBox.Text = SCincrementSettings.Default.SourceReplacePattern;
//    this.searchTextBox.Text = SCincrementSettings.Default.SourceSearchPattern;
//    this.destReplacementTextBox.Text = SCincrementSettings.Default.DestinationReplacePattern;
//    this.destSearchTextBox.Text = SCincrementSettings.Default.DestinationSearchPattern;
//    this.customParamTextBox.Text = SCincrementSettings.Default.CustomParameterName;
//    this.CustomParamCheckBox.Checked = SCincrementSettings.Default.UseCustomParameterName;
//}

//private void Button1Click(object sender, EventArgs e)
//{
//    SCincrementSettings.Default.Reset();
//    this.LoadSettings();
//}

//private void Button2Click(object sender, EventArgs e)
//{
//    SCincrementSettings.Default.OffsetValue = int.Parse(this.offsetTextBox.Text, CultureInfo.InvariantCulture);
//    SCincrementSettings.Default.IncrementValue = int.Parse(this.incrementTextBox.Text, CultureInfo.InvariantCulture);
//    SCincrementSettings.Default.SourceSearchPattern = this.searchTextBox.Text;
//    SCincrementSettings.Default.SourceReplacePattern = this.replacementTextBox.Text;
//    SCincrementSettings.Default.DestinationReplacePattern = this.destReplacementTextBox.Text;
//    SCincrementSettings.Default.DestinationSearchPattern = this.destSearchTextBox.Text;
//    SCincrementSettings.Default.CustomParameterName = this.customParamTextBox.Text;
//    SCincrementSettings.Default.UseCustomParameterName = this.CustomParamCheckBox.Checked;
//    SCincrementSettings.Default.Save();
//}

//private void Button3Click(object sender, EventArgs e)
//{
//    // do nothing...
//}
