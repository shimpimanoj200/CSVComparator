# Technical Design — CsvCompareSolution

## 1. Overview

CsvCompareSolution is designed to compare two CSV files and report differences at row and field levels. It is implemented as a small .NET solution with a reusable library (`CsvComparator`), a console runner (`CsvCompareRunner`), and unit tests (`CsvComparator.Tests`).

The comparator relies on the concept of a **composite key** — a set of column indexes that together uniquely identify a record. Rows with the same composite key are compared field-by-field to surface differences.

> **Important:** Some code files in the provided extraction contain placeholder ellipses (`...`). The design below is derived from available file names, comments, and common patterns for CSV comparison utilities. Please cross-check with the actual complete implementation in your repository.

---

## 2. High-level Architecture

- `CsvComparator` (class library)
  - `CsvParser` — Reads CSV files into an in-memory representation (list of `CsvRecord`).
  - `CsvRecord` — Represents a single CSV row (array of string fields + helper methods).
  - `CsvComparer` — Compares two lists of `CsvRecord` using composite key index(es) and produces a `ComparisonResult`.
  - `ComparisonResult` — DTO containing lists of missing/extra rows and field-level differences.

- `CsvCompareRunner` (console app)
  - `Program` — CLI entry that parses args, calls `CsvComparer.Compare`, and outputs results (console / file).

- `CsvComparator.Tests` (unit tests)
  - Tests for parser behavior and comparison scenarios (matching, missing/extra rows, field diffs).

---

## 3. Data Model

### CsvRecord
- Internal storage: `string[] Fields`.
- Helper properties/methods:
  - `GetKey(int[] keyIndexes)` — returns a composite key string (e.g., concatenation with `|`) for lookup.
  - Indexer to access fields by column index.

### ComparisonResult
- `List<string> MissingInActual` — composite keys present in expected but not in actual.
- `List<string> ExtraInActual` — composite keys present in actual but not in expected.
- `List<FieldDifference> FieldDifferences` — records differences with shape `{ Key, ColumnIndex, ExpectedValue, ActualValue }`.

---

## 4. Parsing & Normalization

`CsvParser.Parse(string filePath)` reads a CSV and returns `List<string[]>` or `List<CsvRecord>`.

Key behaviors expected from the parser:
- Handle quoted values (`"value, with comma"`).
- Trim optional whitespace depending on config.
- Optionally support header row (current code appears to parse all rows — confirm behavior).

Potential improvements:
- Support for different encodings and newline styles.
- Ability to ignore/normalize certain columns (e.g., timestamps) via configuration.

---

## 5. Comparison Algorithm (recommended approach)

1. Parse expected and actual CSVs into `List<CsvRecord>`.
2. Build dictionaries keyed by composite key:
   - `expectedMap: Dictionary<string, CsvRecord>`
   - `actualMap: Dictionary<string, CsvRecord>`
3. For each key in `expectedMap`:
   - If key not in `actualMap`, add to `MissingInActual`.
   - Else, compare fields: for each column index `i`, if `expected.Fields[i] != actual.Fields[i]` then record a `FieldDifference`.
4. For each key in `actualMap` not in `expectedMap`, add to `ExtraInActual`.

This approach is O(n) in the number of rows (assuming dictionary operations are O(1)).

---

## 6. Error Handling & Logging

- The runner should catch IO exceptions (file not found, permission denied) and parse exceptions and return non-zero exit codes.
- The library (`CsvComparator`) should throw descriptive exceptions (`FormatException`, `ArgumentException`) for invalid inputs and leave high-level handling to the caller.
- Add logging (ILogger) optional dependency to the runner for detailed diagnostics.

---

## 7. Tests

Current tests (NUnit) validate:
- Basic comparison where expected and actual match.
- Edge-cases such as quoted fields or comma-in-quote handling (verify parser behavior).

Recommended additional tests:
- Multi-column composite keys.
- Rows with missing fields or different lengths.
- Large files / performance heuristics.
- Unicode and encoding tests.

---

## 8. Extensibility & Improvements

- **Named column support**: Allow passing column names (from header row) instead of numeric indexes.
- **Tolerance rules**: Allow configuring tolerances (e.g., numeric tolerance for floating values).
- **Output formats**: JSON, CSV, or HTML reports.
- **Streaming comparison**: For very large files, consider streaming/partial comparison to reduce memory usage.
- **Configuration file**: YAML/JSON to describe keys, ignored columns, normalization rules.

---

## 9. Suggested CLI UX

```
csv-compare --expected expected.csv --actual actual.csv --keys 0,1 --header --out report.json
```

Flags:
- `--keys` (required): comma-separated column indexes or names.
- `--header`: indicates files contain header row.
- `--ignore-columns`: comma-separated list.
- `--tolerance`: per-column numeric tolerance.

---

## 10. Appendix — Sample Pseudocode for `Compare`

```
ComparisonResult Compare(expectedPath, actualPath, int[] keyIndexes)
{
    var expectedRows = CsvParser.Parse(expectedPath);
    var actualRows = CsvParser.Parse(actualPath);

    var expectedMap = BuildMap(expectedRows, keyIndexes);
    var actualMap = BuildMap(actualRows, keyIndexes);

    var result = new ComparisonResult();

    foreach (var kvp in expectedMap)
    {
        var key = kvp.Key;
        var expected = kvp.Value;
        if (!actualMap.ContainsKey(key))
            result.MissingInActual.Add(key);
        else
        {
            var actual = actualMap[key];
            CompareFields(expected, actual, key, result);
        }
    }

    foreach (var key in actualMap.Keys)
        if (!expectedMap.ContainsKey(key))
            result.ExtraInActual.Add(key);

    return result;
}
```

---

## 11. Final Notes

I generated this technical document from the repository layout and available source comments. I noticed some files in the extracted copy contain `...` placeholders. Please review the actual `.cs` files in your working repository to confirm implementation details and share any additional requirements if you'd like the docs expanded (e.g., API reference, sequence diagrams, or sample reports).
