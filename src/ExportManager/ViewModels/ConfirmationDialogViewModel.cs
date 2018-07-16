
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;
using Caliburn.Micro;

namespace SCaddins.ExportManager.ViewModels
{

    public class ConfirmationDialogViewModel : Screen
    {
        
        public ConfirmationDialogViewModel()
        {
            Value = null;
        }

        public static dynamic DefaultWindowSettings
        {
            get
            {
                dynamic settings = new ExpandoObject();
                settings.Height = 320;
                settings.Icon = new System.Windows.Media.Imaging.BitmapImage(
                    new System.Uri("pack://application:,,,/SCaddins;component/Assets/scexport.png")
                    );
                settings.Width = 640;
                settings.Title = "Confirm file overwrite?";
                settings.ShowInTaskbar = false;
                settings.SizeToContent = System.Windows.SizeToContent.Manual;
                return settings;
            }
        }

        public bool? Value
        {
            get; set;
        }

        public bool ValueAsBool
        {
            get
            {
                return Value.HasValue ? Value.Value : false;
            }
        }

        public string Message
        {
            get; set;
        }

        public void Accept()
        {
            TryClose(true);
        }

        public void Cancel()
        {
            TryClose(false);
        }
    }
}
