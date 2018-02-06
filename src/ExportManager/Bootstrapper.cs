using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace SCaddins.ExportManager
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper() : base(false)
        {
            Initialize();
        }
    }
}
