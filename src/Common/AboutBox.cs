// (C) Copyright 2012-2014 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.Common
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Forms;

    public partial class AboutBox1 : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutBox1" /> class.
        /// </summary>
        public AboutBox1()
        {
            this.InitializeComponent();
            this.Text = string.Format("About {0}", this.AssemblyTitle);
            this.labelProductName.Text = this.AssemblyTitle;
            this.labelVersion.Text = string.Format(
                "Version {0}", this.AssemblyVersion);
            this.labelCopyright.Text = this.AssemblyCopyright;
            this.textBoxDescription.Text = Constants.License;
        }

        /// <summary>
        /// Gets the assembly title.
        /// </summary>
        /// <value> Gets the assembly of this program.</value>
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0) {
                    var titleAttribute =
                        (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != string.Empty) {
                        return titleAttribute.Title;
                    }
                }

                return System.IO.Path.GetFileNameWithoutExtension(Assembly
                        .GetExecutingAssembly().CodeBase);
            }
        }

        /// <summary>
        /// Gets the version of this program.
        /// </summary>
        /// <value> The version of this program.</value>
        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName()
                    .Version.ToString();
            }
        }

        /// <summary>
        /// Gets a description of the program.
        /// </summary>
        /// <value> Gets the description of the program.</value>
        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        /// <summary>
        /// Gets the Assembly Product.
        /// </summary>
        /// <value> The Assembly Product.</value>
        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        /// <summary>
        /// Gets The copyright.
        /// </summary>
        /// <value> The copyright.</value>
        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        /// <summary>
        /// Gets The company.
        /// </summary>
        /// <value> Gets the company name.</value>
        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0) {
                    return string.Empty;
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        private void LinkLabel2_LinkClicked(
            object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(SCaddins.Constants.SourceLink);
        }

        private void LinkLabel1_LinkClicked(
            object sender,
            LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(SCaddins.Constants.HelpLink);    
        }
        
        private void Button1Click(object sender, EventArgs e)
        {
            SCaddins.SCaddinsApp.CheckForUpdates();
        }
    }
}

/* vim: set ts=4 sw=4 nu expandtab: */
