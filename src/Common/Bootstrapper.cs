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