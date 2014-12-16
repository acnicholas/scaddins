using System;
using System.Drawing;
using System.Windows.Forms;

namespace SCaddins.SCincrement
{
    public partial class SCincrementSettingsForm : Form
    {
        public SCincrementSettingsForm()
        {
            InitializeComponent();
            loadSettings();     
        }
        
        void loadSettings()
        {
            this.offsetTextBox.Text = SCincrementSettings.Default.OffsetValue.ToString();
            this.incrementTextBox.Text = SCincrementSettings.Default.IncrementValue.ToString();
            this.replacementTextBox.Text = SCincrementSettings.Default.SourceReplacePattern;
            this.searchTextBox.Text = SCincrementSettings.Default.SourceSearchPattern;
            this.destReplacementTextBox.Text = SCincrementSettings.Default.DestinationReplacePattern;
            this.destSearchTextBox.Text = SCincrementSettings.Default.DestinationSearchPattern;
        }
        
        void Button1Click(object sender, EventArgs e)
        {
            SCincrementSettings.Default.Reset();
            loadSettings();
        }
        
        void Button2Click(object sender, EventArgs e)
        {
            SCincrementSettings.Default.OffsetValue = int.Parse(this.offsetTextBox.Text);
            SCincrementSettings.Default.IncrementValue = int.Parse(this.incrementTextBox.Text);
            SCincrementSettings.Default.SourceSearchPattern = this.searchTextBox.Text;
            SCincrementSettings.Default.SourceReplacePattern = this.replacementTextBox.Text;
            SCincrementSettings.Default.DestinationReplacePattern = this.destReplacementTextBox.Text;
            SCincrementSettings.Default.DestinationSearchPattern = this.destSearchTextBox.Text;
            SCincrementSettings.Default.Save();
        }
        void Button3Click(object sender, EventArgs e)
        {
          //do nothing...
        }
    }
}
