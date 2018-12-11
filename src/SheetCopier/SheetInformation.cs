namespace SCaddins.SheetCopier
{
    using Autodesk.Revit.DB;

    internal class SheetInformation
    {
        public SheetInformation(ViewSheet sheet)
        {
            IndexType = 0;
            IndexTypeDescription = "Selected Sheet";
            ParameterName = "Sheet Name";
            ParameterValue = sheet.SheetNumber + " - " + sheet.Name;
        }

        public SheetInformation(Element element)
        {
            IndexType = 1;
            IndexTypeDescription = "Views on Selected Sheet";
            ParameterName = element.GetType().Name;
            ParameterValue = element.Name;
        }

        public SheetInformation(Parameter param)
        {
            ParameterName = param.Definition.Name;
            IndexTypeDescription = "Parameters of Selected Sheet";
            IndexType = 2;
            switch (param.StorageType)
            {
                case StorageType.Double:
                    ParameterValue = param.AsString();
                    break;

                case StorageType.ElementId:
                    ParameterValue = param.AsString();
                    break;

                case StorageType.Integer:
                    ParameterValue = param.AsInteger().ToString(System.Globalization.CultureInfo.CurrentCulture);
                    break;

                case StorageType.None:
                    ParameterValue = string.Empty;
                    break;

                case StorageType.String:
                    ParameterValue = param.AsString();
                    break;

                default:
                    ParameterValue = param.AsString();
                    break;
            }
        }

        public int IndexType
        {
            get; private set;
        }

        public string IndexTypeDescription
        {
            get; set;
        }

        public string ParameterName
        {
            get; private set;
        }

        public string ParameterValue
        {
            get; private set;
        }
    }
}