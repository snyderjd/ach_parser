# PRD 005 — File Controller Updates (Full ACH parsing, conditional persistence, full retrieval)

## Summary
- Enhance the FileController to fully parse uploaded NACHA ACH files using the existing parser, always persist the raw AchFile record, and conditionally persist parsed child records (FileHeader, FileControl, BatchHeader, BatchControl, EntryDetail, Addenda) only when parsing succeeds without fatal errors. Expand GetFiles and GetFile to return the fully parsed AchFile object graph (raw file + associated parsed entities).

## Motivation
- Preserve the original uploaded file as the canonical source-of-truth while enabling downstream consumers and UI to read structured NACHA contents when parsing succeeds. Avoid partial persistence when parsing encounters fatal errors.

## Scope
- Upload API: persist raw file always; run parser; if parser returns a valid parsed model (no fatal errors) persist parsed children and relationships; otherwise, return 400 with parser issues and do not persist parsed children.
- Retrieval API: modify GetFiles and GetFile to include parsed associations in responses when they exist.
- Tests: update and add unit tests to cover both success and failure flows, and updated retrieval behavior.

## Out of scope
- Persisting parse issues to the DB (this may be added in a future PR).
- Adding new database tables or migrations beyond the existing models.

## Goals & Acceptance Criteria
1. Every upload results in a persisted AchFile record (FileName, Hash, UnparsedFile, CreatedAt, Id).
2. Parsed child records are persisted only when parser returns a non-null parsed AchFile and there are no fatal parse errors (no ParseSeverity.Error issues). If parsing fails fatally, the raw AchFile remains in DB but child records are not persisted.
3. GetFiles and GetFile return AchFile objects with nested parsed associations when present.
4. Unit tests updated and passing that demonstrate both success and failure behavior and the expanded retrieval responses.

## High-Level Approach
- Use the existing IAchFileParser (AchFileParser) to parse uploaded content.
- Inject IAchFileParser and ILogger<FileController> into FileController via DI.
- On upload: compute file hash (existing behavior), persist raw AchFile, then call parser.Parse. Decide child persistence based on parser result. Persist parsed children in the same transaction if allowed.
- For persistence: assign new GUIDs for all entities (AchFile and nested entities) and set FK properties consistently before calling SaveChanges.
- For retrieval: create a rich DTO (AchFileDetailDto) representing the AchFile and nested objects, and return this from GetFile(s). Maintain existing AchFileResponseDto for compatibility where appropriate.

## Data Mapping & Persistence Rules
- Parser returns an AchFile instance with nested collections but without Ids or CreatedAt values. Before persistence:
  - Set AchFile.Id = Guid.NewGuid() if empty and AchFile.CreatedAt = DateTime.UtcNow.
  - For each FileHeader / FileControl: set Id = Guid.NewGuid(); AchFileId = AchFile.Id.
  - For each BatchHeader: set Id = Guid.NewGuid(); AchFileId = AchFile.Id.
  - For each BatchControl: set Id = Guid.NewGuid(); BatchHeaderId = parent.Id.
  - For each EntryDetail: set Id = Guid.NewGuid(); BatchHeaderId = parent.Id.
  - For each Addenda: set Id = Guid.NewGuid(); EntryDetailId = parent.Id.
- Use navigation properties or explicit FK fields but ensure both are consistent prior to Add/SaveChanges.
- Persist everything inside a database transaction to prevent partial commits on failure.

## API Behavior
- POST /api/files (multipart/form-data file)
  - Always store the raw AchFile record.
  - Call parser.Parse(content, filename).
  - If ParseResult.File == null OR ParseResult.Issues contains any ParseSeverity.Error:
    - Do not persist parsed children.
    - Return HTTP 400 BadRequest with body containing parser issues and the persisted raw file id (so the file can be reprocessed later).
  - Else (no fatal errors): persist parsed children, return 201 Created with AchFileResponseDto (Id, FileName, FileContent, OriginalFileName, UploadedAt).

- GET /api/files
  - Return a list of AchFileDetailDto containing the raw file plus nested parsed associations where present. If a file has no parsed children, the collections should be empty.

- GET /api/files/{id}
  - Return AchFileDetailDto for the specified id including parsed associations if present; 404 if not found.

## DTOs (new)
- AchFileDetailDto
  - Guid Id
  - string FileName
  - string FileContent
  - string OriginalFileName
  - DateTime UploadedAt
  - List<FileHeaderDto> FileHeaders
  - List<FileControlDto> FileControls
  - List<BatchHeaderDto> BatchHeaders

- FileHeaderDto
  - Guid Id
  - string ImmediateDestination
  - string ImmediateOrigin
  - DateTime FileCreationDate
  - TimeSpan FileCreationTime
  - string ImmediateDestinationName
  - string ImmediateOriginName
  - int LineNumber
  - string UnparsedRecord

- FileControlDto
  - Guid Id
  - int BatchCount
  - int BlockCount
  - int EntryAddendaCount
  - decimal TotalDebit
  - decimal TotalCredit
  - int LineNumber
  - string UnparsedRecord

- BatchHeaderDto
  - Guid Id
  - string ServiceClassCode
  - string CompanyName
  - string CompanyIdentification
  - int LineNumber
  - string UnparsedRecord
  - List<BatchControlDto> BatchControls
  - List<EntryDetailDto> EntryDetails

- BatchControlDto
  - Guid Id
  - int EntryAddendaCount
  - decimal TotalDebit
  - decimal TotalCredit
  - int LineNumber
  - string UnparsedRecord

- EntryDetailDto
  - Guid Id
  - string RoutingNumber
  - string AccountNumber
  - int TransactionCode
  - decimal Amount
  - string IndividualName
  - int LineNumber
  - string UnparsedRecord
  - List<AddendaDto> Addendas

- AddendaDto
  - Guid Id
  - string Information
  - int LineNumber
  - string UnparsedRecord

- ParseIssueDto (used in error responses)
  - string Severity
  - string Message
  - int? LineNumber
  - string? Code

## Error Responses
- Parsing fatal error (400 BadRequest):
  - Body: { "fileId": "<guid>", "issues": [ ParseIssueDto, ... ] }

## Implementation Tasklist (detailed, small steps)
NOTE: follow the project's Guided Implementation Mode (AGENTS.md). Each step should be small and verifiable. After completing each step, run tests or a build and report back.

### Phase A — Preparations
1. [ ] (A1) Add parser & logger dependencies to FileController
   - Inject IAchFileParser and ILogger<FileController> into FileController constructor.
   - Add using statements as needed.
   - Register AchFileParser in DI (Program.cs): services.AddScoped<IAchFileParser, AchFileParser>();

2. [ ] (A2) Update unit test setup
   - Update existing FileController unit tests to supply an IAchFileParser (mock or real) and ILogger to the controller constructor.

### Phase B — Upload flow changes
3. [ ] (B1) Persist raw AchFile early
   - In UploadFile, compute hash (existing) and create AchFile domain object with Id (Guid.NewGuid()), Filename, Hash, UnparsedFile, CreatedAt.
   - Save AchFile to DB immediately or within the transaction before child persistence.

4. [ ] (B2) Parse uploaded content
   - Invoke _parser.Parse(content, fileName) and collect ParseResult.

5. [ ] (B3) Handle parse failures (no parsed file or errors)
   - If ParseResult.File == null or ParseResult.Issues contains ParseSeverity.Error:
     - Log parse issues.
     - Return 400 BadRequest with { fileId, issues } body.
     - Do not persist parsed children.

6. [ ] (B4) Prepare parsed model for persistence
   - If parse succeeds (ParseResult.File != null and no Error issues):
     - Call a local helper to assign GUIDs and set FK values for the AchFile and all nested parsed entities.
     - Ensure navigation properties align with FK fields.

7. [ ] (B5) Persist parsed children in a single transaction
   - Start a DB transaction (if using relational DB). Add or attach the AchFile (if not already added), then add all parsed child entities (or Add the top-level AchFile which includes navigations) and SaveChanges.
   - Commit transaction.

8. [ ] (B6) Finalize response
   - On successful persistence return 201 Created with AchFileResponseDto.

### Phase C — Retrieval endpoints
9. [ ] (C1) Create DTOs
   - Add the AchFileDetailDto and nested DTOs (see DTOs section) under src/AchParser.Api/DTOs.

10. [ ] (C2) Implement mapping to DTOs
    - Add private mapping helpers to map domain models to DTOs (file -> detail dto, batch -> batch dto, etc.). Keep helpers local to controller or a small internal mapper class.

11. [ ] (C3) Update GetFiles to return AchFileDetailDto list
    - Query AchFiles using .Include to eager load related associations (FileHeaders, FileControls, BatchHeaders -> include BatchControls and EntryDetails -> include Addendas) and project to DTOs.

12. [ ] (C4) Update GetFile(id) to return AchFileDetailDto
    - Similar to GetFiles but filter by id and return NotFound if missing.

### Phase D — Tests
13. [ ] (D1) Update existing tests
    - Update FileControllerTests to construct FileController with parser and logger dependencies.
    - Update expectations to reflect DTOs returned by GetFile/GetFiles.

14. [ ] (D2) Add unit tests for upload success
    - UploadFile_PersistsParsedChildren_WhenParserSucceeds
    - Use a real AchFileParser with sample ACH content or mock IAchFileParser to return a ParseResult with File and no fatal issues.
    - Assert database rows for parsed children are present and linked.

15. [ ] (D3) Add unit tests for parsing failure
    - UploadFile_PersistsRawOnly_AndReturnsBadRequest_WhenParserFails
    - Mock parser to return ParseResult.File == null or include an Error issue.
    - Assert raw AchFile persisted, child tables empty for that file id, and controller returns BadRequest with issues.

16. [ ] (D4) Add tests for retrieval
    - GetFiles_ReturnsParsedAssociations_WhenPresent
    - GetFile_ReturnsParsedAssociations_WhenPresent

### Phase E — Manual verification & polish
17. [ ] (E1) Manual verification steps (local)
    - Start DB: docker-compose up -d postgres
    - Run API: dotnet run --project ./src/AchParser.Api
    - POST a valid ACH file to POST /api/files as multipart/form-data. Expect 201 Created and AchFileResponseDto. Verify DB rows for parsed children.
    - POST an invalid ACH file (missing header/control). Expect 400 BadRequest with parse issues and raw file persisted; child tables should have no rows for that file id.
    - GET /api/files and GET /api/files/{id} should return AchFileDetailDto with nested parsed data when available.

18. [ ] (E2) Logging & diagnostics
    - Ensure parser issues are logged at appropriate levels (Error for fatal, Warning/Info for non-fatal), and persistence exceptions are logged.

19. [ ] (E3) Code style & formatting
    - Run dotnet format and run unit tests (dotnet test). Fix any analyzer warnings if needed.

## Non-functional considerations
- Use a transaction to avoid partial writes.
- Keep changes minimal and local to the controller where possible; prefer small helper methods over new public types unless reuse is anticipated.
- Maintain backward compatibility for existing consumers using AchFileResponseDto for upload success responses; introduce AchFileDetailDto for retrieval.

## Guided Implementation Mode
- Follow the Guided Implementation Mode detailed in AGENTS.md.

## Security & Privacy
- Do not log file contents or sensitive account numbers in plain text in production logs. For development / test logging, be careful not to leak production data.

## Open Questions (to be decided before implementation)
1. Persist parse issues to DB? (Recommended: postpone to a follow-up PR.)
2. Should the upload endpoint return AchFileDetailDto on successful parse instead of the lighter AchFileResponseDto? (Recommended: keep current upload response unchanged to reduce breaking changes. Consumers can call GET to receive detailed info.)
3. Do we want to expose endpoints for reprocessing a file (re-run parser) later? (Future enhancement.)

## Appendix: Example parse-failure response shape (recommended)
HTTP 400 BadRequest
{
  "fileId": "<guid>",
  "issues": [
    { "severity": "Error", "message": "Missing file header", "lineNumber": 1 },
    { "severity": "Error", "message": "Missing file control", "lineNumber": null }
  ]
}

---
This PRD contains the full list of steps to implement the feature in small increments under the Guided Implementation Mode. Proceed step-by-step and run tests between steps.
