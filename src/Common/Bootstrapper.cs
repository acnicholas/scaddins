namespace SCaddins.Common
{
    using Caliburn.Micro;

    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper() : base(false)
        {
            Initialize();
            ConventionManager.IncludeStaticProperties = true;
        }
    }
}