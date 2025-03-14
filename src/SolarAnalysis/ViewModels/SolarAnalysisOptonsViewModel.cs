using Caliburn.Micro;
using System.IO;

namespace SCaddins.SolarAnalysis.ViewModels
{
    public class SolarAnalysisOptonsViewModel : PropertyChangedBase, Common.ViewModels.ISettingPanel
    {
        public SolarAnalysisOptonsViewModel()
        {
            Reset();
        }

        public string PixCountGUIBinaryLocation { get; set; }

        public string RasterAnalysisExportDirectory { get; set; }

        public string RasterAnalysisDefaultExportName { get; set; }

        public int RasterAnalysisPixelSize { get; set; }

        public string RasterAnalysisTemporaryFilePrefix { get; set; }


        public void Apply()
        {
            SolarAnalysisSettings.Default.RasterAnalysisExportDirectory = RasterAnalysisExportDirectory;
            SolarAnalysisSettings.Default.RasterAnalysisDefaultExportName = RasterAnalysisDefaultExportName;
            SolarAnalysisSettings.Default.RasterAnalysisTemporaryFilePrefix = RasterAnalysisTemporaryFilePrefix;
            SolarAnalysisSettings.Default.PixCountGUIBinaryLocation = PixCountGUIBinaryLocation;
            SolarAnalysisSettings.Default.RasterAnalysisPixelSize = RasterAnalysisPixelSize;
            SolarAnalysisSettings.Default.Save();
        }

        public void Reset()
        {
            RasterAnalysisExportDirectory = SolarAnalysisSettings.Default.RasterAnalysisExportDirectory;
            RasterAnalysisDefaultExportName = SolarAnalysisSettings.Default.RasterAnalysisDefaultExportName;
            RasterAnalysisTemporaryFilePrefix = SolarAnalysisSettings.Default.RasterAnalysisTemporaryFilePrefix;
            PixCountGUIBinaryLocation = SolarAnalysisSettings.Default.PixCountGUIBinaryLocation;
            RasterAnalysisPixelSize = SolarAnalysisSettings.Default.RasterAnalysisPixelSize;
            NotifyOfPropertyChange(() => RasterAnalysisExportDirectory);
            NotifyOfPropertyChange(() => RasterAnalysisDefaultExportName);
            NotifyOfPropertyChange(() => RasterAnalysisTemporaryFilePrefix);
            NotifyOfPropertyChange(() => PixCountGUIBinaryLocation);
            NotifyOfPropertyChange(() => RasterAnalysisPixelSize);
        }

        public void ResetToDefault()
        {
            SolarAnalysisSettings.Default.Reset();
            Reset();
        }

        public void SelectPixCountGUIBinaryLocation()
        {
            string newPath = string.Empty;
            bool? result = SCaddinsApp.WindowManager.ShowFileSelectionDialog(
                SolarAnalysisSettings.Default.PixCountGUIBinaryLocation,
                out newPath);
            if(result.HasValue && result.Value == true)
            {
                if (File.Exists(newPath))
                {
                    PixCountGUIBinaryLocation = newPath;
                    NotifyOfPropertyChange(() => PixCountGUIBinaryLocation);
                }
            }
        }

        public void SelectRasterAnalysisExportDirectory()
        {
            string newPath = string.Empty;
            bool? result = SCaddinsApp.WindowManager.ShowDirectorySelectionDialog(
                SolarAnalysisSettings.Default.RasterAnalysisExportDirectory,
                out newPath);
            if (result.HasValue && result.Value == true)
            {
                if (Directory.Exists(newPath))
                {
                    RasterAnalysisExportDirectory = newPath;
                    NotifyOfPropertyChange(() => RasterAnalysisExportDirectory);
                }
            }
        }

    }
}
