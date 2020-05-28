// (C) Copyright 2018-2020 by Andrew Nicholas
//
// This file is part of SCaddins.
//
// SCaddins is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SCaddins is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with SCaddins.  If not, see <http://www.gnu.org/licenses/>.

namespace SCaddins.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Threading;
    using Caliburn.Micro;

    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper() : base(false)
        {
            AssemblySource.Instance.Clear();
            Initialize();
            ConventionManager.IncludeStaticProperties = true;
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return base.SelectAssemblies().Where(x => !AssemblySource.Instance.Contains(x));
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            base.OnUnhandledException(sender, e);
            AssemblySource.Instance.Clear();
        }
    }
}