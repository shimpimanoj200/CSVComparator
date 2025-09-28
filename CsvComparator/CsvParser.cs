using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CsvComparator
{
    /// <summary>
    /// Base exception for CSV parsing errors.
    /// </summary>
    public class CsvParseException : Exception
    {
        public CsvParseException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception when the CSV file is not found or cannot be opened.
    /// </summary>
    public class CsvFileNotFoundException : CsvParseException
    {
        public CsvFileNotFoundException(string filePath)
            : base($"CSV file not found or cannot be opened: {filePath}") { }
    }

    /// <summary>
    /// Exception when a line in CSV is malformed (e.g., unbalanced quotes).
    /// </summary>
    public class CsvMalformedLineException : CsvParseException
    {
        public int LineNumber { get; }
        public string LineContent { get; }

        public CsvMalformedLineException(int lineNumber, string lineContent)
            : base($"Malformed CSV line at {lineNumber}: {lineContent}")
        {
            LineNumber = lineNumber;
            LineContent = lineContent;
        }
    }

    /// <summary>
    /// Simple CSV parser without external libraries.
    /// Handles quoted values and commas inside quotes.
    /// </summary>
    public static class CsvParser
    {
        public static List<string[]> Parse(string filePath)
        {
            if (!File.Exists(filePath))
                throw new CsvFileNotFoundException(filePath);

            var result = new List<string[]>();
            using (var reader = new StreamReader(filePath))
            {
                string line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    if (string.IsNullOrWhiteSpace(line))
                        continue; // skip blank lines, or throw if you want strict validation

                    try
                    {
                        result.Add(ParseLine(line, lineNumber));
                    }
                    catch (CsvMalformedLineException)
                    {
                        throw; // rethrow to keep line context
                    }
                    catch (Exception ex)
                    {
                        throw new CsvParseException($"Unexpected error parsing line {lineNumber}: {ex.Message}");
                    }
                }
            }
            return result;
        }

        private static string[] ParseLine(string line, int lineNumber)
        {
            var values = new List<string>();
            bool inQuotes = false;
            var current = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // Escaped double-quote ("")
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes; // toggle in/out of quotes
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    values.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            if (inQuotes)
                throw new CsvMalformedLineException(lineNumber, line);

            values.Add(current.ToString());
            return values.ToArray();
        }
    }
}
