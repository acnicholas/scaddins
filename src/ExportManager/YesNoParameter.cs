namespace SCaddins.ExportManager
{
    public class YesNoParameter
    {
        public YesNoParameter(string name, bool? value)
        {
            Name = name;
            Value = value;
        }

        public bool? Value { get; set; }

        public string Name { get; private set; }

        public override string ToString() => Name;
    }
}
