using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using ProjectExportSettingsModel = SCaddins.ExportSchedules.Models.ProjectExportSettings;

namespace SCaddins.ExportSchedules.Services
{
    public static class ProjectSettingsStore
    {
        private static readonly Guid SchemaGuid = new Guid("3C3E7F45-3868-4CC8-A2B9-EC2C87E93BD5");
        private const string SchemaName = "NullCarbonExportSettings";
        private const string StorageName = "nullCarbon.ExportSettings";
        private const string ExportDirectoryField = "ExportDirectory";
        private const string DecimalSeparatorField = "DecimalSeparator";
        private const string IncludeWithoutKeynoteField = "IncludeWithoutKeynote";
        private const string IncludeStructureLayersField = "IncludeStructureLayers";
        private const string FillEmptyKeynoteField = "FillEmptyKeynote";
        private const string SelectedScheduleIdsField = "SelectedScheduleIds";

        public static ProjectExportSettingsModel Load(Document document)
        {
            var settings = new ProjectExportSettingsModel();
            if (document == null)
            {
                return settings;
            }

            var schema = Schema.Lookup(SchemaGuid);
            if (schema == null)
            {
                return settings;
            }

            var storage = FindStorage(document);
            if (storage == null)
            {
                return settings;
            }

            var entity = storage.GetEntity(schema);
            if (!entity.IsValid())
            {
                return settings;
            }

            settings.ExportDirectory = entity.Get<string>(schema.GetField(ExportDirectoryField));
            settings.DecimalSeparator = entity.Get<string>(schema.GetField(DecimalSeparatorField));
            settings.IncludeWithoutKeynote = entity.Get<bool>(schema.GetField(IncludeWithoutKeynoteField));
            settings.IncludeStructureLayers = entity.Get<bool>(schema.GetField(IncludeStructureLayersField));
            settings.FillEmptyKeynote = entity.Get<bool>(schema.GetField(FillEmptyKeynoteField));

            var idsField = schema.GetField(SelectedScheduleIdsField);
            var ids = entity.Get<IList<string>>(idsField);
            if (ids != null)
            {
                settings.SelectedScheduleIds = ids.ToList();
            }

            return settings;
        }

        public static void Save(Document document, ProjectExportSettingsModel settings)
        {
            if (document == null || settings == null)
            {
                return;
            }

            void SaveSettings()
            {
                var schema = GetOrCreateSchema();
                var storage = FindStorage(document) ?? DataStorage.Create(document);
                storage.Name = StorageName;

                var entity = new Entity(schema);
                entity.Set(schema.GetField(ExportDirectoryField), settings.ExportDirectory ?? string.Empty);
                entity.Set(schema.GetField(DecimalSeparatorField), settings.DecimalSeparator ?? string.Empty);
                entity.Set(schema.GetField(IncludeWithoutKeynoteField), settings.IncludeWithoutKeynote);
                entity.Set(schema.GetField(IncludeStructureLayersField), settings.IncludeStructureLayers);
                entity.Set(schema.GetField(FillEmptyKeynoteField), settings.FillEmptyKeynote);
                entity.Set(schema.GetField(SelectedScheduleIdsField), (IList<string>)settings.SelectedScheduleIds ?? new List<string>());
                storage.SetEntity(entity);
            }

            if (document.IsModifiable)
            {
                SaveSettings();
                return;
            }

            using (var transaction = new Transaction(document, "Save nullCarbon export settings"))
            {
                transaction.Start();
                SaveSettings();
                transaction.Commit();
            }
        }

        private static DataStorage FindStorage(Document document)
        {
            return new FilteredElementCollector(document)
                .OfClass(typeof(DataStorage))
                .Cast<DataStorage>()
                .FirstOrDefault(storage => string.Equals(storage.Name, StorageName, StringComparison.OrdinalIgnoreCase));
        }

        private static Schema GetOrCreateSchema()
        {
            var schema = Schema.Lookup(SchemaGuid);
            if (schema != null)
            {
                return schema;
            }

            var builder = new SchemaBuilder(SchemaGuid);
            builder.SetSchemaName(SchemaName);
            builder.SetReadAccessLevel(AccessLevel.Public);
            builder.SetWriteAccessLevel(AccessLevel.Public);
            builder.AddSimpleField(ExportDirectoryField, typeof(string));
            builder.AddSimpleField(DecimalSeparatorField, typeof(string));
            builder.AddSimpleField(IncludeWithoutKeynoteField, typeof(bool));
            builder.AddSimpleField(IncludeStructureLayersField, typeof(bool));
            builder.AddSimpleField(FillEmptyKeynoteField, typeof(bool));
            builder.AddArrayField(SelectedScheduleIdsField, typeof(string));
            return builder.Finish();
        }
    }
}
