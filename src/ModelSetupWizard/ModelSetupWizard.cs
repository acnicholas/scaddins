// (C) Copyright 2019-2020 by Andrew Nicholas
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

namespace SCaddins.ModelSetupWizard
{
    using System.Collections.Generic;
    using Autodesk.Revit.DB;

    public static class ModelSetupWizardUtilities
    {
        public static void ApplyWorksetModifications(Document doc, List<WorksetParameter> worksets, ref TransactionLog log)
        {
            using (Transaction t = new Transaction(doc))
            {
                if (t.Start("Add Worksets to Model.") == TransactionStatus.Started)
                {
                    foreach (var w in worksets)
                    {
                        if (w.IsModified)
                        {
                            if (w.IsExisting && w.Id >= 0)
                            {
                                if (WorksetTable.IsWorksetNameUnique(doc, w.Name))
                                {
                                    WorksetTable.RenameWorkset(doc, new WorksetId(w.Id), w.Name);
                                }
                                var defaultVisibilitySettings = WorksetDefaultVisibilitySettings.GetWorksetDefaultVisibilitySettings(doc);
                                defaultVisibilitySettings.SetWorksetVisibility(new WorksetId(w.Id), w.VisibleInAllViews);
                                log.AddSuccess(w.ToString());
                            }
                            else
                            {
                                Workset newWorkset = null;
                                if (WorksetTable.IsWorksetNameUnique(doc, w.Name))
                                {
                                    newWorkset = Workset.Create(doc, w.Name);
                                }
                                else
                                {
                                    log.AddFailure(w.ToString());
                                    continue;
                                }
                                if (newWorkset != null)
                                {
                                    var defaultVisibilitySettings = WorksetDefaultVisibilitySettings.GetWorksetDefaultVisibilitySettings(doc);
                                    defaultVisibilitySettings.SetWorksetVisibility(newWorkset.Id, w.VisibleInAllViews);
                                    log.AddSuccess(w.ToString());
                                }
                            }
                        }
                    }
                    t.Commit();
                }
            }
        }

        public static void ApplyProjectInfoModifications(Document doc, List<ProjectInformationParameter> projectInformationParameters, ref TransactionLog log)
        {
            using (Transaction t = new Transaction(doc))
            {
                if (t.Start("Change Project Information Parameter Values") == TransactionStatus.Started)
                {
                    foreach (var p in projectInformationParameters)
                    {
                        if (p.IsModified)
                        {
                            if (p.IsEditable)
                            {
                                SetParameterValue(p.GetParameter(), p.Value);
                                log.AddSuccess(p.ToString());
                            }
                            else
                            {
                                log.AddFailure(p.ToString());
                            }
                        }
                    }
                    t.Commit();
                }
            }
        }

        public static void SetParameterValue(Parameter param, string value)
        {
            if (param.StorageType == StorageType.Double)
            {
                double d = 0;
                if (double.TryParse(value, out d))
                {
                    param.Set(d);
                }
            }
            else if (param.StorageType == StorageType.Integer)
            {
                int i = 0;
                if (int.TryParse(value, out i))
                {
                    param.Set(i);
                }
            }
            else
            {
                param.Set(value);
            }
        }
    }
}
