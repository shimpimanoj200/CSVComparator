using NUnit.Framework;
using System.IO;

namespace CsvComparator.Tests
{
    public class CsvComparatorTests
    {
        private string expectedPath;
        private string actualPath;

        [SetUp]
        public void Setup()
        {
            expectedPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "expected.csv");
            actualPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "actual.csv");
            File.WriteAllText(expectedPath, "Name,ID,Category,Value\nBAII,001625,TypeA,10\nMissing,002,TypeB,20");
            File.WriteAllText(actualPath, "Name,ID,Category,Value\nBAII,001625,TypeA,11\nExtra,003,TypeC,30");
        }

        [Test]
        public void TestComparison()
        {
            var result = CsvComparator.CsvComparer.Compare(expectedPath, actualPath, new int[]{0,1});
            Assert.That(result.MissingInActual, Has.Count.EqualTo(0));
            Assert.That(result.ExtraInActual, Has.Count.EqualTo(0));
            Assert.That(result.FieldDifferences, Has.Count.EqualTo(0));
        }
    }
}
