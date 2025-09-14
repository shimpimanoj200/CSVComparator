using System.Collections.Generic;

namespace CsvComparator
{
    /// <summary>
    /// Holds results of comparison: missing, extra, and field diffs.
    /// </summary>
    public class ComparisonResult
    {
        public List<string> MissingInActual { get; } = new List<string>();
        public List<string> ExtraInActual { get; } = new List<string>();
        public List<string> FieldDifferences { get; } = new List<string>();
    }
}
