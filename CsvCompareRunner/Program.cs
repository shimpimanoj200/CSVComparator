using System;
using System.IO;

namespace CsvComparator.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage: CsvCompareRunner <expected.csv> <actual.csv> <outputFolder>");
                return;
            }
            string solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));

            string expected = Path.Combine(solutionRoot, "TestData", args[0]);
            string actual   = Path.Combine(solutionRoot, "TestData", args[1]);
            string outDir   = Path.Combine(solutionRoot, args[2]);

            Directory.CreateDirectory(outDir);
            var result = CsvComparator.CsvComparer.Compare(expected, actual, new int[] { 0, 1 });
            File.WriteAllLines(Path.Combine(outDir, "ComparisonReport.txt"), result.FieldDifferences);
            Console.WriteLine("Comparison complete. See ComparisonReport.txt");
        }
    }
}
