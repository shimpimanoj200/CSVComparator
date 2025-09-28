// using NUnit.Framework;
// using System.IO;
// using System;

// namespace CsvComparator.Tests
// {
//     public class CsvComparatorTests
//     {
//         private string expectedPath;
//         private string actualPath;

//         [SetUp]
//         public void Setup()
//         {
//          var config = new ConfigurationSetting();

//             expectedPath = config.GetExpectedPath();
//             actualPath   = config.GetActualPath();

//             if (string.IsNullOrWhiteSpace(expectedPath) || string.IsNullOrWhiteSpace(actualPath))
//             throw new Exception("ExpectedPath or ActualPath is not configured properly in appsettings.json");

//              // Resolve absolute paths if needed
//             expectedPath = Path.Combine(TestContext.CurrentContext.TestDirectory, expectedPath);
//             actualPath   = Path.Combine(TestContext.CurrentContext.TestDirectory, actualPath);
//         }

//         [Test]
//         public void TestComparison()
//         {
//             var result = CsvComparator.CsvComparer.Compare(expectedPath, actualPath, new int[]{0,1});
//             Assert.That(result.MissingInActual, Has.Count.EqualTo(0));
//             Assert.That(result.ExtraInActual, Has.Count.EqualTo(0));
//             Assert.That(result.FieldDifferences, Has.Count.EqualTo(0));
//         }
//     }
// }
