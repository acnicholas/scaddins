namespace SCaddins.ModelSetupWizard
{
    using System.Collections.Generic;

    public class ColourScheme
    {
        public ColourScheme(string config)
        {
            // FIXME add some error checking here
            var s = config.Split(';');
            Name = s[0];
            Colors = new List<System.Windows.Media.Color>(16);
            for (int i = 1; i < 17; i++) {
                Colors.Insert(i - 1, IniIO.ConvertStringToColor(s[i]));
            }
        }

        public string Name {
            get; set;
        }

        public List<System.Windows.Media.Color> Colors {
            get; private set;
        }
    }
}