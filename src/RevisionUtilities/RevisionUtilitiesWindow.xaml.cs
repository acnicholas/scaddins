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
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace SCaddins.RevisionUtilities
{
    /// <summary>
    /// Interaction logic for RevisionUtilitiesWindow.xaml
    /// </summary>
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
            //if (radioButtonRevisions.Checked) {  
            RevisionsList.ItemsSource = RevisionUtilities.GetRevisions(doc);                  
            //} //else {
            //    dataGridView1.DataSource = RevisionUtilities.GetRevisionClouds(doc); 
            //    AdjustColumnOrder();
            //}
            //this.dataGridView1.Refresh();
        }
        
        void RevisionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var row in RevisionsList.SelectedItems as DataGridRow) {
                
            }
        }
        
    }
}