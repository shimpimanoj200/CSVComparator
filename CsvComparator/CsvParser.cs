using System;
using System.Collections.Generic;
using System.IO;

namespace CsvComparator
{
    /// <summary>
    /// Simple CSV parser without external libraries.
    /// Handles quoted values and commas inside quotes.
    /// </summary>
    public static class CsvParser
    {
        public static List<string[]> Parse(string filePath)
        {
            var result = new List<string[]>();
            using (var reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    result.Add(ParseLine(line));
                }
            }
            return result;
        }

        private static string[] ParseLine(string line)
        {
            var values = new List<string>();
            bool inQuotes = false;
            var current = new System.Text.StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
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
            values.Add(current.ToString());
            return values.ToArray();
        }
    }
}
