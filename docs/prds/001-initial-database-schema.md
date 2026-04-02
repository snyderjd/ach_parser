# PRD-001: Initial Database Schema

## Objective
Implement the database layer for the ACH Parser using:
- .NET (EF Core)
- PostgreSQL
- snake_case database naming convention

The implementation must:
1. Add EF Core entity models
2. Build AchParserDbContext
3. Generate the initial database migration
4. Create tables with correct constraints and relationships
5. Enforce ACH structural integrity at the database level

## 1. Naming Conventions

### Database
- Use snake_case for all database identifiers (tables, columns, constraints, etc.)
- Ensure EF Core is configured with: `.UseSnakeCaseNamingConvention()`

### C# Models
- Use PascalCase for class and property names

## 2. Core Entity Hierarchy (Must Match ACH Spec)

ACH structure:

```
ach_files
 ├── file_headers (1:1)
 ├── file_controls (1:1)
 └── batch_headers (1:N)
        ├── batch_controls (1:1)
        └── entry_details (1:N)
                └── addendas (0:N)

```
### Required relationships
- `file_headers.ach_file_id` -> FK -> `ach_files.id`
- `file_controls.ach_file_id` -> FK -> `ach_files.id`
- `batch_headers.ach_file_id` -> FK -> `ach_files.id`
- `batch_controls.batch_header_id` -> FK -> `batch_headers.id`
- `entry_details.batch_header_id` -> FK -> `batch_headers.id`
- `addendas.entry_detail_id` -> FK -> `entry_details.id`

## 3. Required Constraints

### Primary Keys
- All tables use `uuid` as primary key (e.g., `id` column)

### 1:1 Relationship Constraints (Unique Indexes)

Enforce database-level uniqueness for 1:1 relationships:
- `file_headers.ach_file_id` -> UNIQUE
- `file_controls.ach_file_id` -> UNIQUE
- `batch_controls.batch_header_id` -> UNIQUE

These enforce ACH spec compliance.

### NOT NULL Requirements

All of the following must be NOT NULL:
- All foreignt keys
- `line_number`
- `unparsed_record`
- Monetary fields
- Critical ACH spec fields (routing_number, service_class_code, etc.)

## 4. Raw vs Parsed Data

### ach_files Table

Must contain:
- `id`
- `filename`
- `hash` (unique recommended)
- `unparsed_file` (full raw content, NOT NULL)
- `create_at`

This is the system's canonical source of truth.

### Parsed Tables

Each parsed table must include:
- `line_number` (int, NOT NULL)
- `unparsed_record` (char(94), NOT NULL)

These provide debug traceability, audit capability, deterministic file reconstruction, and validation support.

## 5. Data Types (Important - define before migration)

### UUID

All IDs -> `uuid`

### Data and Time

In `file_headers`:
- `file_creation_date` -> `date`
- `file_creation_time` -> `time`

Do NOT store as strings.

### Monetary Fields

Use `numeric(12, 2)`

Applies to:
- `entry_details.amount`
- `file_controls.total_debit`
- `file_controls.total_credit`
- `batch_controls.total_debit`
- `batch_controls.total_credit`

Scale must be 2

### Fixed-Length ACH fields (recommended)
- `routing_number` -> `char(9)`
- `service_class_code` -> `char(3)`
- `unparsed_record` -> `char(94)`

If spec length is known, enforce it at the database level.

## 6. DbContext Requirements

Create or Update `AchParserDbContext` with:
- DbSet for each table
- Fluent API configurations for:
    - All foreign keys
    - Unique constraints (1:1 relationships)
    - Decimal precision
    - Fixed-length column definitions where required

Do NOT use `OnConfiguring` if using ASP.NET Core DI. Configure Npgsql + snake_case in `Program.cs` instead.

## 7. Migration Requirements

Initial migration must:
- Create all tables
- Create foreign keys
- Create required UNIQUE constraints
- Set correct numeric precision
- Enforce NOT NULL rules
- Reflect snake_case naming
- Create indexes on foreign keys

## 8. Architectural Principles to Preserve

- Database enforces ACH structural integrity
- Parsed tables store typed, validated fields
- Raw file is preserved in full in `ach_files`
- Relationships reflect ACH file hierarchy exactly
- No redundant "record type" tables
- No quoted PascalCase identifiers in PostgreSQL

## 9. Non-Goals
- No staging/unparsed record tables
- No partial ingestion states
- No polymorphic record tables
- No denormalization

## Deliverables

1. Create all EF entity models
2. Implement or update `AchParserDbContext`
3. Configure all relationships and constraints using Fluent API
4. Generate a correct initial migration
5. Produce a clean PostgreSQL schema matching the ERD and this PRD


