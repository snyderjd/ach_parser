# ACH Raw File Generator

1. Purpose

- Build a standalone console application that generates valid, syntatically correct ACH .txt files for testing the AchParser API.

- The generator will:
    - Produce structurally valid NACHA-formatted files
    - Output raw fixed-width text files
    - Support configurable record counts
    - Generate realistic but synthetic financial data
    - Compute correct control totals

- This tool is intended for:
    - Testing file upload endpoints
    - Load testing
    - Parser development
    - Control total validation testing

- This is NOT intended to simulate returns, NOCs, or reversals in phase 1

2. High-Level Architecture

- The generator shall:
    - Be a separate console application project
    - Live in the same solution as AchParser.Api
    - Output .txt files to disk

- Solution Structure:
    ach_parser/
        AchParser.sln
        src/
            AchParser.Api/
            AchParser.Generator/   <-- new console app
        docs/
        tests/

3. Scope (Phase 1)

### Included
- File Header Record (Type 1)
- Batch Header Record (Type 5)
- Entry Detail Records (Type 6)
- Batch Control Record (Type 8)
- File Control Record (Type 9)
- File padding to block size (multiple of 10 records)

### Excluded (future phases)
- Addenda Records (Type 7)
- Return files
- NOC files
- Same-day ACH flags
- Reversals
- Multiple batches per file (Phase 1 may optionally support 1 batch only)

4. Functional Requirements

### 4.1 Command Line Interface
- The generator shall support:
```
dotnet run -- 
  --entries 250 
  --output sample1.ach
```

- Required parameters:
    - `--entries`: Number of Entry Detail records to generate (e.g. 250)
    - `--output`: Output file name (e.g. sample1.ach)


5. ACH File Rules (Must be correct)

- The generator must:

### 5.1 Fixed Width Records
- Each record must be exactly 94 characters
- No tabs
- No trailing spaces beyond defined width
- Newline separated

### 5.2 File Header (Type 1)
- Must include:
    - Immediate Destination (fake routing number)
    - Immediate Origin
    - File creation date/time
    - File ID modifier
    - Record size (094)
    - Blocking factor (10)
    - Format code (1)

### 5.3 Batch Header (Type 5)
- Service Class Code: 220 (credits only)
- SEC Code: PPD
- Company Name: "TEST COMPANY" or generate a random company name
- Company ID: fake EIN-like number
- Effective Entry Date = today

### 5.4 Entry Detail (Type 6)
- Each entry must include:
    - Transaction code (22 - checking credit)
    - RDFI routing number (8 digits)
    - Check digit (computed correctly)
    - Account number (random 8-17 digits)
    - Amount (in cents, zero-padded to 10 digits)
    - Individual name (fake but realistic)
    - Trace number (unique per entry)

- Routing numbers must pass check digit validation:
```
(3*d1 + 7*d2 + 1*d3 + 3*d4 + 7*d5 + 1*d6 + 3*d7 + 7*d8 + 1*d9) % 10 == 0
```

### 5.5 Batch Control (Type 8)
- Must compute correctly:
    - Entry count
    - Entry hash (sum of routing numbers' first 8 digits)
    - Total debit amount
    - Total credit amount

- Since phase 1 only generates credits:
    - Total debit amount = 0
    - Total credit amount = sum of all entry amounts

### 5.6 File Control (Type 9)
- Must compute:
    - Batch count
    - Block count
    - Entry count
    - Entry hash
    - Debit total
    - Credit total

### 5.7 Block Padding
- ACH files must:
    - Be divisible into blocks of 10 records
    - Pad with "9" records until total record count % 10 == 0

6. Non-Functional Requirements

- Deterministic option (optional future feature)
- Clean separation of formatting logic
- No database usage
- No external services
- No third-party ACH libraries

7. Design Guidelines

8. Validation Criteria

9. Success Criteria

10. Future Phases (Do not implement yet)
