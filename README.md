# CSV Compare Solution

## Overview
The **CSV Compare Solution** is a .NET console application designed to compare two CSV files (`Expected.csv` and `Actual.csv`) and generate detailed comparison reports.  

It highlights:
- Extra rows in either file  
- Missing rows in either file  
- Field-level differences (when a row exists in both files but with different values)  

This makes it useful for validating test results, verifying data migrations, and ensuring consistency between environments.

---

## Features
- Compares **Expected.csv** and **Actual.csv**
- Identifies **extra rows**, **missing rows**, and **field-level mismatches**
- Generates reports:
  - `ComparisonReport.txt` – summary of differences
  - `FieldDifferences.csv` – detailed field-by-field mismatches
- Uses the **first column as a unique key** to match rows
- Clean and simple console application

---

## Project Structure
```
CsvCompareSolution/
│
├── CsvCompare/              # Console app source code
│   ├── CsvComparer.cs       # Handles CSV comparison logic
│   ├── Program.cs           # Entry point
│   └── CsvCompare.csproj    # Project file
│
├── TestData/                # Sample input data
│   ├── Expected.csv
│   └── Actual.csv
│
├── Outputs/                 # Generated reports
│   ├── ComparisonReport.txt
│   └── FieldDifferences.csv
│
├── CsvCompareSolution.sln   # Solution file
└── README.md                # Project documentation
```

---

## How to Run

### 1. Open Solution
Open the solution in **Visual Studio** or run commands from the terminal.

### 2. Build Project
```sh
dotnet build
```

### 3. Run Application
```sh
dotnet run --project CsvCompare
```

### 4. Check Results
- Reports will be created in the `Outputs` folder:
  - `ComparisonReport.txt`
  - `FieldDifferences.csv`

---

## Example Output

**ComparisonReport.txt**
```
Extra rows in Actual:
Id=4, Name=David, Age=29

Missing rows in Actual:
Id=2, Name=Bob, Age=25

Field differences:
Id=3 => Column 'Age': Expected=40, Actual=41
```

**FieldDifferences.csv**
```csv
Key,Column,Expected,Actual
3,Age,40,41
```

---

## Requirements
- .NET 6.0 or later
- Visual Studio Code

---

## Future Enhancements
- Support configurable key columns  
- Option to ignore column order  
- Generate Excel output for easier analysis  

---

## Author
Developed as part of a case study for CSV data comparison and automation.
