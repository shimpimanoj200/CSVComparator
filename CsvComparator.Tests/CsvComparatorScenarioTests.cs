using NUnit.Framework;
using CsvComparator;
using System;
using System.IO;

namespace CsvComparator.Tests
{
    [TestFixture]
    public class CsvComparatorScenarioTests
    {
        private string GetPath(string scenario, bool expected)
        {
            string fileName = scenario + (expected ? "_expected.csv" : "_actual.csv");
            return Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", fileName);
        }

        [TestCase("missing")]
        public void Should_Find_Missing_Records(string scenario)
        {
            var result = CsvComparer.Compare(GetPath(scenario, true), GetPath(scenario, false), new int[] { 0 });
            Assert.That(result.MissingInActual.Count, Is.GreaterThan(0));
        }

        [TestCase("extra")]
        public void Should_Find_Extra_Records(string scenario)
        {
            var result = CsvComparer.Compare(GetPath(scenario, true), GetPath(scenario, false), new int[] { 0 });
            Assert.That(result.ExtraInActual.Count, Is.GreaterThan(0));
        }

        [TestCase("mismatch")]
        public void Should_Report_FieldLevel_Mismatches(string scenario)
        {
            var result = CsvComparer.Compare(GetPath(scenario, true), GetPath(scenario, false), new int[] { 0 });
            Assert.That(result.FieldDifferences.Count, Is.GreaterThan(0));
        }

        [TestCase("empty")]
        public void Should_Throw_On_EmptyFile(string scenario)
        {
            Assert.Throws<EmptyCsvFileException>(() =>
                CsvComparer.Compare(GetPath(scenario, true), GetPath(scenario, false), new int[] { 0 }));
        }

        [TestCase("header")]
        public void Should_Throw_On_HeaderMismatch(string scenario)
        {
            Assert.Throws<CsvHeaderMismatchException>(() =>
                CsvComparer.Compare(GetPath(scenario, true), GetPath(scenario, false), new int[] { 0 }));
        }

        [TestCase("duplicate")]
        public void Should_Throw_On_DuplicateKeys(string scenario)
        {
            Assert.Throws<DuplicateKeyException>(() =>
                CsvComparer.Compare(GetPath(scenario, true), GetPath(scenario, false), new int[] { 0 }));
        }

        [TestCase("malformed")]
        public void Should_Throw_On_MalformedCsv(string scenario)
        {
            Assert.Throws<CsvMalformedLineException>(() =>
                CsvParser.Parse(GetPath(scenario, true)));
        }

        [TestCase("composite")]
        public void Should_Handle_CompositeKeys(string scenario)
        {
            var result = CsvComparer.Compare(GetPath(scenario, true), GetPath(scenario, false), new int[] { 0, 1 });
            Assert.That(result.FieldDifferences.Count, Is.GreaterThan(0));
        }

        [TestCase("large")]
        public void Should_Handle_LargeFiles(string scenario)
        {
            var result = CsvComparer.Compare(GetPath(scenario, true), GetPath(scenario, false), new int[] { 0 });
            Assert.That(result.FieldDifferences.Count, Is.EqualTo(1)); // one mismatch introduced
        }
    }
}
