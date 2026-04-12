namespace SCaddins.ExportSchedules
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autodesk.Revit.DB;
    using Autodesk.Revit.DB.ExtensibleStorage;

    public class ProjectExportSettings
    {
        public string ExportDir { get; set; }
        public string DecimalSeparator { get; set; }
        public bool NoKeynote { get; set; }
        public bool IncludeStructureLayers { get; set; }
        public List<string> SelectedSchedules { get; set; } = new List<string>();
    }

    public static class ProjectSettingsStorage
    {
        private static readonly Guid SchemaGuid = new Guid("6d06fa0d-6c6b-4f87-8b3b-1dd4a1a2b1f7");
        private const string SchemaName = "NullCarbonExportSettings";

        public static ProjectExportSettings Load(Document document)
        {
            if (document == null)
            {
                return null;
            }

            var schema = GetSchema();
            var storage = FindStorage(document, schema);
            if (storage == null)
            {
                return null;
            }

            var entity = storage.GetEntity(schema);
            if (!entity.IsValid())
            {
                return null;
            }

            var settings = new ProjectExportSettings
            {
                ExportDir = entity.Get<string>(schema.GetField(nameof(ProjectExportSettings.ExportDir))),
                DecimalSeparator = entity.Get<string>(schema.GetField(nameof(ProjectExportSettings.DecimalSeparator))),
                NoKeynote = entity.Get<bool>(schema.GetField(nameof(ProjectExportSettings.NoKeynote))),
                IncludeStructureLayers = entity.Get<bool>(schema.GetField(nameof(ProjectExportSettings.IncludeStructureLayers)))
            };

            var schedules = entity.Get<IList<string>>(schema.GetField(nameof(ProjectExportSettings.SelectedSchedules)));
            if (schedules != null)
            {
                settings.SelectedSchedules = schedules.ToList();
            }

            return settings;
        }

        public static void Save(Document document, ProjectExportSettings settings)
        {
            if (document == null || settings == null || document.IsReadOnly)
            {
                return;
            }

            var schema = GetSchema();
            var storage = FindStorage(document, schema);
            using (var transaction = new Transaction(document, "Save nullCarbon export settings"))
            {
                transaction.Start();

                if (storage == null)
                {
                    storage = DataStorage.Create(document);
                }

                var entity = new Entity(schema);
                entity.Set(schema.GetField(nameof(ProjectExportSettings.ExportDir)), settings.ExportDir ?? string.Empty);
                entity.Set(schema.GetField(nameof(ProjectExportSettings.DecimalSeparator)), settings.DecimalSeparator ?? string.Empty);
                entity.Set(schema.GetField(nameof(ProjectExportSettings.NoKeynote)), settings.NoKeynote);
                entity.Set(schema.GetField(nameof(ProjectExportSettings.IncludeStructureLayers)), settings.IncludeStructureLayers);
                entity.Set(schema.GetField(nameof(ProjectExportSettings.SelectedSchedules)), settings.SelectedSchedules ?? new List<string>());
                storage.SetEntity(entity);

                transaction.Commit();
            }
        }

        private static Schema GetSchema()
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
            builder.AddSimpleField(nameof(ProjectExportSettings.ExportDir), typeof(string));
            builder.AddSimpleField(nameof(ProjectExportSettings.DecimalSeparator), typeof(string));
            builder.AddSimpleField(nameof(ProjectExportSettings.NoKeynote), typeof(bool));
            builder.AddSimpleField(nameof(ProjectExportSettings.IncludeStructureLayers), typeof(bool));
            builder.AddArrayField(nameof(ProjectExportSettings.SelectedSchedules), typeof(string));

            return builder.Finish();
        }

        private static DataStorage FindStorage(Document document, Schema schema)
        {
            return new FilteredElementCollector(document)
                .OfClass(typeof(DataStorage))
                .Cast<DataStorage>()
                .FirstOrDefault(storage => storage.GetEntity(schema).IsValid());
        }
    }
}
