using System;
using System.Collections.Generic;
using System.IO;

namespace CsvComparator
{
    /// <summary>
    /// Compares two CSV files based on composite keys and field values.
    /// </summary>
    public static class CsvComparer
    {
        public static ComparisonResult Compare(string expectedPath, string actualPath, int[] keyIndexes)
        {
            var expectedRows = CsvParser.Parse(expectedPath);
            var actualRows = CsvParser.Parse(actualPath);
            if (expectedRows.Count == 0 || actualRows.Count == 0)
                throw new Exception("CSV files are empty");
            var headers = expectedRows[0];
            if (headers.Length != actualRows[0].Length)
                throw new Exception("Header count mismatch");
            var result = new ComparisonResult();
            var expectedDict = new Dictionary<string,CsvRecord>();
            var actualDict = new Dictionary<string,CsvRecord>();
            for (int i=1; i<expectedRows.Count; i++)
            {
                var rec = new CsvRecord(headers, expectedRows[i], keyIndexes);
                expectedDict[rec.CompositeKey] = rec;
            }
            for (int i=1; i<actualRows.Count; i++)
            {
                var rec = new CsvRecord(headers, actualRows[i], keyIndexes);
                actualDict[rec.CompositeKey] = rec;
            }
            foreach (var kv in expectedDict)
            {
                if (!actualDict.ContainsKey(kv.Key))
                    result.MissingInActual.Add(kv.Key);
            }
            foreach (var kv in actualDict)
            {
                if (!expectedDict.ContainsKey(kv.Key))
                    result.ExtraInActual.Add(kv.Key);
            }
            foreach (var kv in expectedDict)
            {
                if (actualDict.ContainsKey(kv.Key))
                {
                    var eRec = kv.Value;
                    var aRec = actualDict[kv.Key];
                    foreach (var field in eRec.Fields.Keys)
                    {
                        if (!aRec.Fields.ContainsKey(field)) continue;
                        string eVal = eRec.Fields[field];
                        string aVal = aRec.Fields[field];
                        if (eVal != aVal)
                        {
                            result.FieldDifferences.Add($"Failed Fieldname: {field} | Expected Input Value: {eVal} | Actual Value: {aVal} | for record having unique key: {kv.Key}");
                        }
                    }
                }
            }
            return result;
        }
    }
}
