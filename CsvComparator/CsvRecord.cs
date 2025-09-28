using System;
using System.Collections.Generic;

namespace CsvComparator
{
    /// <summary>
    /// Base exception for CSV record issues.
    /// </summary>
    public class CsvRecordException : Exception
    {
        public CsvRecordException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception when headers or values are invalid.
    /// </summary>
    public class CsvInvalidDataException : CsvRecordException
    {
        public CsvInvalidDataException(string details)
            : base($"Invalid CSV data: {details}") { }
    }

    /// <summary>
    /// Exception when composite key cannot be built.
    /// </summary>
    public class CsvKeyBuildException : CsvRecordException
    {
        public CsvKeyBuildException(string details)
            : base($"Failed to build composite key: {details}") { }
    }

    /// <summary>
    /// Represents a CSV row with dictionary of fieldname->value and composite key.
    /// </summary>
    public class CsvRecord
    {
        public Dictionary<string, string> Fields { get; }
        public string CompositeKey { get; }

        public CsvRecord(string[] headers, string[] values, int[] keyIndexes)
        {
            if (headers == null || headers.Length == 0)
                throw new CsvInvalidDataException("Headers are null or empty.");

            if (values == null)
                throw new CsvInvalidDataException("Values array is null.");

            if (values.Length > headers.Length)
                throw new CsvInvalidDataException(
                    $"Row has {values.Length} values, but only {headers.Length} headers.");

            if (keyIndexes == null || keyIndexes.Length == 0)
                throw new CsvKeyBuildException("No key indexes provided for composite key.");

            Fields = new Dictionary<string, string>(headers.Length, StringComparer.Ordinal);

            for (int i = 0; i < headers.Length; i++)
            {
                string header = headers[i];
                if (string.IsNullOrWhiteSpace(header))
                    throw new CsvInvalidDataException($"Header at index {i} is null or empty.");

                string val = i < values.Length ? values[i] : string.Empty;
                Fields[header] = val;
            }

            var keyParts = new List<string>();
            foreach (int idx in keyIndexes)
            {
                if (idx < 0 || idx >= values.Length)
                    throw new CsvKeyBuildException(
                        $"Key index {idx} is out of range for row with {values.Length} values.");

                keyParts.Add(values[idx]);
            }

            if (keyParts.Count == 0)
                throw new CsvKeyBuildException("Composite key could not be built (no valid key parts).");

            CompositeKey = string.Join("||", keyParts);
        }
    }
}
