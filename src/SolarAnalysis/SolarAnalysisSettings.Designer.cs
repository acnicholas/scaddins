﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCaddins.SolarAnalysis {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.12.0.0")]
    internal sealed partial class SolarAnalysisSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static SolarAnalysisSettings defaultInstance = ((SolarAnalysisSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new SolarAnalysisSettings())));
        
        public static SolarAnalysisSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Temp")]
        public string RasterAnalysisExportDirectory {
            get {
                return ((string)(this["RasterAnalysisExportDirectory"]));
            }
            set {
                this["RasterAnalysisExportDirectory"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("HoursOfDirectSun.png")]
        public string RasterAnalysisDefaultExportName {
            get {
                return ((string)(this["RasterAnalysisDefaultExportName"]));
            }
            set {
                this["RasterAnalysisDefaultExportName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("solarAnalysis")]
        public string RasterAnalysisTemporaryFilePrefix {
            get {
                return ((string)(this["RasterAnalysisTemporaryFilePrefix"]));
            }
            set {
                this["RasterAnalysisTemporaryFilePrefix"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("C:\\Program Files\\Studio.SC\\SCaddins\\bin\\PixCountGUI\\PixCountGUI.exe")]
        public string PixCountGUIBinaryLocation {
            get {
                return ((string)(this["PixCountGUIBinaryLocation"]));
            }
            set {
                this["PixCountGUIBinaryLocation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1024")]
        public int RasterAnalysisPixelSize {
            get {
                return ((int)(this["RasterAnalysisPixelSize"]));
            }
            set {
                this["RasterAnalysisPixelSize"] = value;
            }
        }
    }
}
