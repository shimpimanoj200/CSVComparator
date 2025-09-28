using System;
using System.Collections.Generic;
using System.IO;

namespace CsvComparator
{
    /// <summary>
    /// Base exception for CSV comparison issues.
    /// </summary>
    public class CsvComparisonException : Exception
    {
        public CsvComparisonException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception when CSV file is empty.
    /// </summary>
    public class EmptyCsvFileException : CsvComparisonException
    {
        public EmptyCsvFileException(string filePath)
            : base($"CSV file is empty: {filePath}") { }
    }

    /// <summary>
    /// Exception when headers mismatch.
    /// </summary>
    public class CsvHeaderMismatchException : CsvComparisonException
    {
        public CsvHeaderMismatchException(string expectedFile, string actualFile)
            : base($"CSV header count mismatch between files: {expectedFile} and {actualFile}") { }
    }

    /// <summary>
    /// Exception when duplicate keys are found.
    /// </summary>
    public class DuplicateKeyException : CsvComparisonException
    {
        public DuplicateKeyException(string key, string filePath)
            : base($"Duplicate composite key '{key}' found in file: {filePath}") { }
    }

    /// <summary>
    /// Compares two CSV files based on composite keys and field values.
    /// </summary>
    public static class CsvComparer
    {
        public static ComparisonResult Compare(string expectedPath, string actualPath, int[] keyIndexes)
        {
            if (!File.Exists(expectedPath))
                throw new FileNotFoundException($"Expected file not found: {expectedPath}");

            if (!File.Exists(actualPath))
                throw new FileNotFoundException($"Actual file not found: {actualPath}");

            var expectedRows = CsvParser.Parse(expectedPath);
            var actualRows   = CsvParser.Parse(actualPath);

            if (expectedRows.Count == 0)
                throw new EmptyCsvFileException(expectedPath);

            if (actualRows.Count == 0)
                throw new EmptyCsvFileException(actualPath);

            var headers = expectedRows[0];
            if (headers.Length != actualRows[0].Length)
                throw new CsvHeaderMismatchException(expectedPath, actualPath);

            var result = new ComparisonResult();
            var expectedDict = new Dictionary<string, CsvRecord>(expectedRows.Count - 1);
            var actualDict   = new Dictionary<string, CsvRecord>(actualRows.Count - 1);

            // Insert into expected dictionary with duplicate key check
            for (int i = 1; i < expectedRows.Count; i++)
            {
                var rec = new CsvRecord(headers, expectedRows[i], keyIndexes);
                if (!expectedDict.TryAdd(rec.CompositeKey, rec))
                {
                    throw new DuplicateKeyException(rec.CompositeKey, expectedPath);
                }
            }

            // Insert into actual dictionary with duplicate key check
            for (int i = 1; i < actualRows.Count; i++)
            {
                var rec = new CsvRecord(headers, actualRows[i], keyIndexes);
                if (!actualDict.TryAdd(rec.CompositeKey, rec))
                {
                    throw new DuplicateKeyException(rec.CompositeKey, actualPath);
                }
            }

            foreach (var kv in expectedDict)
            {
                if (!actualDict.TryGetValue(kv.Key, out var aRec))
                {
                    result.MissingInActual.Add(kv.Key);
                    continue;
                }

                var eRec = kv.Value;
                foreach (var field in eRec.Fields)
                {
                    if (aRec.Fields.TryGetValue(field.Key, out var aVal))
                    {
                        if (!string.Equals(field.Value, aVal, StringComparison.Ordinal))
                        {
                            result.FieldDifferences.Add(
                                $"Failed Field: {field.Key} | Expected: {field.Value} | Actual: {aVal} | Record Key: {kv.Key}");
                        }
                    }
                }
            }

            foreach (var kv in actualDict)
            {
                if (!expectedDict.ContainsKey(kv.Key))
                    result.ExtraInActual.Add(kv.Key);
            }

            return result;
        }
    }
}
