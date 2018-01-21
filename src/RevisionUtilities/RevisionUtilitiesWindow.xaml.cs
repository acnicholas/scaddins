/*
 * Created by SharpDevelop.
 * User: AndrewN
 * Date: 12/19/2017
 * Time: 15:19
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SCaddins.RevisionUtilities
{
    public partial class RevisionUtilitiesWindow : Window
    {
       Autodesk.Revit.DB.Document doc;
        
        public RevisionUtilitiesWindow(Autodesk.Revit.DB.Document doc)
        {
            this.doc = doc;
            InitializeComponent();
            RefreshDataGrid();
        }
        
        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            RevisionsList.SelectAll();
        }
        
        private void SelectNoneButton_Click(object sender, RoutedEventArgs e)
        {
            RevisionsList.UnselectAll();
        }
        
        private void RefreshDataGrid()
        {
            if(RevisionsList == null) return;
            if (RadioRevisions.IsChecked == true) {  
            RevisionsList.ItemsSource = RevisionUtilities.GetRevisions(doc);          
            } else {
            RevisionsList.ItemsSource = RevisionUtilities.GetRevisionClouds(doc); 
            //    AdjustColumnOrder();
            }
        }
              
        private void RadioRevisions_Checked(object sender, RoutedEventArgs e)
        {
            RefreshDataGrid();
            DeleteCloudsButton.IsEnabled = false;
            AssignRevisionsButton.IsEnabled = false;
        }
        
        private void RadioClouds_Checked(object sender, RoutedEventArgs e)
        {
            RefreshDataGrid();
             DeleteCloudsButton.IsEnabled = true;
            AssignRevisionsButton.IsEnabled = true;
        }
        
        private void DeleteCloudsButton_Click(object sender, RoutedEventArgs e)
        {
            RevisionUtilities.DeleteRevisionClouds(doc, SelectedRevisionCloudItems());
        }
        
        private Collection<RevisionCloudItem> SelectedRevisionCloudItems()
        {
           Collection<RevisionCloudItem> cloudSelection = new Collection<RevisionCloudItem>();
           foreach (var row in RevisionsList.SelectedItems) {
                RevisionCloudItem rev = row as RevisionCloudItem;
                    cloudSelection.Add(rev); 
                } 
           return cloudSelection;
        }
        
        private Collection<RevisionItem> SelectedRevisionItems()
        {
           Collection<RevisionItem> selection = new Collection<RevisionItem>();
           foreach (var row in RevisionsList.SelectedItems) {
                RevisionItem rev = row as RevisionItem;
                    selection.Add(rev);
            } 
           return selection;
        }
        
        private void AssignRevisionsButton_Click(object sender, RoutedEventArgs e)
        {
            RevisionUtilities.AssignRevisionToClouds(doc, SelectedRevisionCloudItems());
        }
        
        private void ScheduleCloudsButton_Click(object sender, RoutedEventArgs e)
        {
            var dictionary = new Dictionary<string, RevisionItem>();
            foreach (RevisionItem rev in this.SelectedRevisionItems()) {
                if (rev != null) {
                    string s = rev.Date + rev.Description;
                    if (!dictionary.ContainsKey(s)) {
                        dictionary.Add(s, rev);
                    }
                }
            }
 
            var saveFileName = string.Empty;
            
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".xls"; // Default file extension
            dlg.Filter = "Excel documents (.xls)|*.xls"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true) {
                saveFileName = dlg.FileName;
            }

        RevisionUtilities.ExportCloudInfo(this.doc, dictionary, saveFileName);
        }
               
    }
}