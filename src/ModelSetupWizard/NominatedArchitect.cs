using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCaddins.ModelSetupWizard
{
    class NominatedArchitect
    {
        public NominatedArchitect(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Id {
            get; set;
        }

        public string Name {
            get; set;
        }

        public override string ToString()
        {
            return string.Format("{0};{1}", Name, Id);
        }


    }
}
