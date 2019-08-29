// (C) Copyright 2018 by Andrew Nicholas
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

namespace SCaddins.Common.ViewModels
{
    using System.Diagnostics;
    using System.Reflection;
    using Caliburn.Micro;

    public class AboutViewModel : Screen
    {
        public AboutViewModel()
        {
        }

        public static string AssemblyBuildDate
        {
            get
            {
                return Properties.Resources.BuildDate.TrimEnd(System.Environment.NewLine.ToCharArray());
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyCopyrightAttribute)attributes[0]).Copyright.Trim();
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                return attributes.Length == 0 ? string.Empty : ((AssemblyDescriptionAttribute)attributes[0]).Description.Trim();
            }
        }

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    var titleAttribute =
                        (AssemblyTitleAttribute)attributes[0];
                    if (!string.IsNullOrEmpty(titleAttribute.Title))
                    {
                        return titleAttribute.Title.Trim();
                    }
                }

                return System.IO.Path.GetFileNameWithoutExtension(Assembly
                        .GetExecutingAssembly().CodeBase.Trim());
            }
        }

        public static string AssemblyVersion => Assembly.GetExecutingAssembly().GetName()
                .Version.ToString().Trim();

        public static string License
        {
            get { return Constants.License; }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly()
                    .GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0) {
                    return string.Empty;
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company.Trim();
            }
        }

        public static string AssemblyInformationalVersion
        {
            get
            {
                return GetInformationalVersion(Assembly.GetExecutingAssembly());
            }
        }

        public static string AssemblyVersionExtended
        {
            get
            {
                return AssemblyVersion + @"(" + AssemblyInformationalVersion + @") - " + AssemblyBuildDate;
            }
        }

        public static string AssemblyProduct()
        {
            object[] attributes = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            return attributes.Length == 0 ? string.Empty : ((AssemblyProductAttribute)attributes[0]).Product.Trim();
        }

        public static void CheckForUpgrades()
        {
            SCaddinsApp.CheckForUpdates(false);
        }

        public static string GetInformationalVersion(Assembly assembly) => FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;

        public static void NavigateTo(System.Uri url)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url.AbsoluteUri));
        }
    }
}
