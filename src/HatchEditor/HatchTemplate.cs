namespace SCaddins.HatchEditor
{
    using System;
    using System.Collections.Generic;
    using Caliburn.Micro;
    using NCalc;

    internal class HatchTemplate
    {
        public static string[] GetHatchTemplateLineArray(string fileName)
        {
            return System.IO.File.Exists(fileName) ? System.IO.File.ReadAllLines(fileName) : null;
        }

        public static BindableCollection<TemplateParameter> GetHatchParameters(string fileName)
        {
            var dictionary = GetHatchParameterDictionary(fileName);
            if (dictionary == null)
            {
                return null;
            }
            var result = new BindableCollection<TemplateParameter>();
            foreach (var item in dictionary)
            {
                result.Add(new TemplateParameter(item.Key, item.Value));
            }
            return result;
        }

        public static string GetPatternString(
            string fileName, 
            BindableCollection<TemplateParameter> templateParameters)
        {
            var lineArray = GetHatchTemplateLineArray(fileName);
            if (lineArray == null)
            {
                return string.Empty;
            }

            var dictionary = new Dictionary<string, double>();
            foreach (var item in templateParameters)
            {
                dictionary.Add(item.Name, item.Value);
            }
            var stringArray = ApplyParameterValuesToTemplate(lineArray, dictionary);
            var stringArrayToString = string.Empty;
            for (int i = 0; i < stringArray.Length; i++)
            {
                stringArrayToString += stringArray[i] + System.Environment.NewLine;
            }
            return stringArrayToString;
        }

        public static Dictionary<string, double> GetHatchParameterDictionary(string fileName)
        {
            var result = new Dictionary<string, double>();
            var fileLines = GetHatchTemplateLineArray(fileName);
            if (fileLines == null)
            {
                return null;
            }

            foreach (var line in fileLines)
            {
                if (line.StartsWith(";"))
                {
                    continue;
                }

                var paramCatcher = string.Empty;
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    if (c == '$' && i != line.Length - 1)
                    {
                        i++;
                        while (char.IsLetter(line[i]))
                        {
                            paramCatcher += line[i];
                            if (i != line.Length - 1)
                            {
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (!result.ContainsKey(paramCatcher))
                        {
                            result.Add(paramCatcher, 0.0);
                        }
                        paramCatcher = string.Empty;
                    }
                }
            }
            return result;
        }
  
        public static string[] ApplyParameterValuesToTemplate(
            string[] templateLineArray,
            Dictionary<string, double> hatchParameterDictionary)
        {
            var lines = ReplaceParameterValuesInTemplate(templateLineArray, hatchParameterDictionary);
            var result = new List<string>();
            for (int i = 0; i < lines.Length; i++)
            {
                var newLine = string.Empty;
                if (lines[i].Trim().StartsWith(";")) { 
                    continue;
                }
                if (string.IsNullOrEmpty(lines[i].Trim())) {
                    continue;
                }
                var splitedLine = lines[i].Split(',');
                foreach (var segment in splitedLine)
                {
                    if (string.IsNullOrEmpty(segment))
                    {
                        continue;
                    }
                    var expressionResult = new Expression(segment).Evaluate();
                    newLine += expressionResult;
                    newLine += ",";
                }
                var newLineFixed = newLine.Substring(0, newLine.Length - 1);
                result.Add(newLineFixed);
            }

            return result.ToArray();
        }

        private static string[] ReplaceParameterValuesInTemplate(
            string[] templateLineArray,
            Dictionary<string, double> hatchParameterDictionary)
        {
            var result = new List<string>();
            for (int i = 0; i < templateLineArray.Length; i++)
            {
                if (templateLineArray[i].Trim().StartsWith(";"))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(templateLineArray[i].Trim()))
                {
                    continue;
                }
                var ns = templateLineArray[i];
                foreach (var hatchParam in hatchParameterDictionary)
                {
                    ns = ns.Replace(@"$" + hatchParam.Key, hatchParam.Value.ToString());
                }
                result.Add(ns);
            }
            return result.ToArray();
        }
    }
}
