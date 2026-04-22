using JCC.Debug;
using JCC.Enums;
using System;
using System.Collections.Generic;

namespace JCC.Localization
{
    public class LocalizationProcessorCSV : ILocalizationProcessor
    {
        private const char SEPARATOR = ';';

        private Dictionary<string, string> _localizedData = new Dictionary<string, string>();

        public void Clear() => _localizedData.Clear();

        public bool IsReady() => _localizedData.Count > 0;

        public string GetValue(string key) => _localizedData.TryGetValue(key, out string value) ? value : $"[{key}]";

        public void LoadFromCSV(string csvContent, ELanguageID langID, string fileName)
        {
            string[] lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length == 0)
            {
                DebugManager.LogError($"[LocalizationProcessorCSV] lines is zero");
                return;
            }

            int langIndex = GetLangIndex(lines[0].Split(SEPARATOR), langID.ToString());
            if (langIndex == -1)
            {
                DebugManager.LogError($"[LocalizationProcessorCSV] Language '{langID.ToString()}' not found in header of {fileName}");
                return;
            }

            FillData(lines, langIndex, fileName);
        }

        private int GetLangIndex(string[] headers, string langString) 
        {
            int langIndex = -1;
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Trim().Equals(langString, StringComparison.OrdinalIgnoreCase))
                {
                    langIndex = i;
                    break;
                }
            }
            return langIndex;
        }

        private void FillData(string[] lines, int langIndex, string fileName) 
        {
            for (int i = 1; i < lines.Length; i++)
            {
                string[] columns = lines[i].Split(SEPARATOR);
                if (columns.Length <= 0) 
                {
                    continue;
                }

                if (columns.Length > langIndex)
                {
                    string key = columns[0].Trim();
                    string value = columns[langIndex].Trim();

                    value = value.Trim('"');

                    if (string.IsNullOrEmpty(key)) continue;

                    if (_localizedData.ContainsKey(key))
                    {
                        DebugManager.LogError($"[LocalizationProcessorCSV] Duplicate key found: '{key}' in file '{fileName}'. " +
                                             $"Original: '{_localizedData[key]}', Ignored: '{value}'");
                        continue;
                    }

                    _localizedData[key] = value.Replace("\\n", "\n");
                }
            }
        }
    }
}