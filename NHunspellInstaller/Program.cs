using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHunspellInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            var HunspellLib = "Hunspellx64.dll";
            var Revit2016dir = @"C:\Program Files\Autodesk\Revit 2019";
            var Revit2017dir = @"C:\Program Files\Autodesk\Revit 2019";
            var Revit2018dir = @"C:\Program Files\Autodesk\Revit 2019";
            var Revit2019dir = @"C:\Program Files\Autodesk\Revit 2019";
            var Revit2020dir = @"C:\Program Files\Autodesk\Revit 2019";
            var HunspellLibDir = @"C:\Program Files\SCaddins\SCaddins\lib\Hunspellx64.dll";

            if (!File.Exists(HunspellLibDir))
            {
                return;
            }

            if (Directory.Exists(Revit2016dir) && !File.Exists(Path.Combine(Revit2016dir, HunspellLib)))
            {
                File.Copy(HunspellLibDir, Path.Combine(Revit2016dir, HunspellLib));
            }

            if (Directory.Exists(Revit2017dir) && !File.Exists(Path.Combine(Revit2017dir, HunspellLib)))
            {
                File.Copy(HunspellLibDir, Path.Combine(Revit2017dir, HunspellLib));
            }

            if (Directory.Exists(Revit2018dir) && !File.Exists(Path.Combine(Revit2018dir, HunspellLib)))
            {
                File.Copy(HunspellLibDir, Path.Combine(Revit2018dir, HunspellLib));
            }

            if (Directory.Exists(Revit2019dir) && !File.Exists(Path.Combine(Revit2019dir, HunspellLib)))
            {
                File.Copy(HunspellLibDir, Path.Combine(Revit2019dir, HunspellLib));
            }

            if (Directory.Exists(Revit2020dir) && !File.Exists(Path.Combine(Revit2020dir, HunspellLib)))
            {
                File.Copy(HunspellLibDir, Path.Combine(Revit2020dir, HunspellLib));
            }
        }
    }
}
