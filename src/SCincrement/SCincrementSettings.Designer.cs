﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18063
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SCaddins.SCincrement {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("ICSharpCode.SettingsEditor.SettingsCodeGeneratorTool", "5.0.0.4218")]
    internal sealed partial class SCincrementSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static SCincrementSettings defaultInstance = ((SCincrementSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new SCincrementSettings())));
        
        public static SCincrementSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("#VAL#")]
        public string DestinationReplacePattern {
            get {
                return ((string)(this["DestinationReplacePattern"]));
            }
            set {
                this["DestinationReplacePattern"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("^.*$")]
        public string DestinationSearchPattern {
            get {
                return ((string)(this["DestinationSearchPattern"]));
            }
            set {
                this["DestinationSearchPattern"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public int IncrementValue {
            get {
                return ((int)(this["IncrementValue"]));
            }
            set {
                this["IncrementValue"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public int OffsetValue {
            get {
                return ((int)(this["OffsetValue"]));
            }
            set {
                this["OffsetValue"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("$1")]
        public string SourceReplacePattern {
            get {
                return ((string)(this["SourceReplacePattern"]));
            }
            set {
                this["SourceReplacePattern"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("(^.*$)")]
        public string SourceSearchPattern {
            get {
                return ((string)(this["SourceSearchPattern"]));
            }
            set {
                this["SourceSearchPattern"] = value;
            }
        }
    }
}