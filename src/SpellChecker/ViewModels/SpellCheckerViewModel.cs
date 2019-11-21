using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace SCaddins.SpellChecker.ViewModels
{
    internal class SpellCheckerViewModel : Screen
    {
        private SpellChecker manager;

        public SpellCheckerViewModel(SpellChecker manager)
        {
            this.manager = manager;
        }

        public BindableCollection<CorrectionCandiate> RenameCandidates => manager.RenameCandidates;

    }
}
