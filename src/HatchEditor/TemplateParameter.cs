namespace SCaddins.HatchEditor
{
    public class TemplateParameter
    {
        public TemplateParameter(string name, double value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; private set; }

        public double Value { get; set; }
    }
}
