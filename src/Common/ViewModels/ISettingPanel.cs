namespace SCaddins.Common.ViewModels
{
    using Caliburn.Micro;

    public interface ISettingPanel
    {
        void Apply();

        void Reset();

        void ResetToDefault();
    }
}
