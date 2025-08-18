using Caliburn.Micro;

namespace SCaddins.RunScript.ViewModels
{
    public class RunScriptSettingsViewModel : PropertyChangedBase, Common.ViewModels.ISettingPanel
    {

        public RunScriptSettingsViewModel()
        {
            Reset();
        }

        public string ExternalEditor { get; set; }

        public void Apply()
        {
            RunScriptSettings.Default.ExternalEditor = ExternalEditor;
            RunScriptSettings.Default.Save();
        }

        public void Reset()
        {
            ExternalEditor = RunScriptSettings.Default.ExternalEditor;
            NotifyOfPropertyChange(() => ExternalEditor);
        }

        public void ResetToDefault()
        {
            RunScriptSettings.Default.Reset();
            RunScriptSettings.Default.Save();
        }

    }
}
