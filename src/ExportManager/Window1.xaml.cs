/*
 * Created by SharpDevelop.
 * User: AndrewN
 * Date: 18/12/2017
 * Time: 1:44 PM
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
using SCaddins.Common;

namespace SCaddins.ExportManager
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        ExportManager scx;
        
        public Window1(ExportManager scx)
        {
            this.scx = scx;
            InitializeComponent();
            PopulateViewSheetSetCombo();
            PopulateList();
        }
        
        private void PopulateList()
        {
            this.SheetList.ItemsSource = this.scx.AllSheets;
        }
        
        private void PopulateViewSheetSetCombo()
        {
            var allViews = new ViewSheetSetCombo("<All views>");
            this.ViewsetCombo.Items.Add(allViews);
            foreach (ViewSheetSetCombo s in this.scx.AllViewSheetSets) {
                 this.ViewsetCombo.Items.Add(s);
            }
        }
        
        private void NoViewFilter_Clicked(object sender, RoutedEventArgs e)
        {
            scx.ForceRevisionToDateString = true;
        }

    }
}