using System;
using System.Collections.Generic;

namespace CsvComparator
{
    /// <summary>
    /// Represents a CSV row with dictionary of fieldname->value and composite key.
    /// </summary>
    public class CsvRecord
    {
        public Dictionary<string,string> Fields { get; }
        public string CompositeKey { get; }

        public CsvRecord(string[] headers, string[] values, int[] keyIndexes)
        {
            Fields = new Dictionary<string,string>();
            for (int i = 0; i < headers.Length; i++)
            {
                string header = headers[i];
                string val = i < values.Length ? values[i] : "";
                Fields[header] = val;
            }
            var keyParts = new List<string>();
            foreach (int idx in keyIndexes)
            {
                if (idx < values.Length) keyParts.Add(values[idx]);
            }
            CompositeKey = string.Join("||", keyParts);
        }
    }
}
