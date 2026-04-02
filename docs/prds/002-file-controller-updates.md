# 002-file-controller-updates.md

## Overview

This document outlines the requirements for updating the FileController in AchParser.Api to support database persistence of uploaded ACH files, use Data Transfer Objects (DTOs) for API responses, and provide corresponding unit tests.

## Goals
- Enable uploading and storing unparsed ACH files in the database.
- Expose endpoints to retrieve all files or a single file by ID.
- Use DTOs for all API input/output.
- Ensure unit test coverage for new behaviors.

## Scope
- Update FileController methods: UploadFile, GetFiles, GetFile.
- Create DTOs for file upload and response.
- Update/add unit tests in tests/AchParser.Api.UnitTests/Controllers/FileControllerTests.cs.

## Requirements

### 1. FileController Updates
- Inject AchParserDbContext via constructor.
- **UploadFile**: Accept an unparsed ACH file (IFormFile), read content as string, create AchFile entity, persist to ach_files table, return DTO.
- **GetFiles**: Return all AchFile records as a list of DTOs.
- **GetFile**: Return a single AchFile as a DTO by ID.
- All methods should use async/await for database operations.

### 2. DTO Design
- **AchFileResponseDto**: Represents AchFile for API responses. Includes: Id, FileName, FileContent, OriginalFileName, UploadedAt.
- **AchFileUploadDto**: Represents upload input (if needed for future extensibility).
- DTOs should be placed in a DTOs folder or namespace.

### 3. Unit Tests
- Update or add tests in FileControllerTests.cs to cover:
  - UploadFile: Persists file, returns correct DTO.
  - GetFiles: Returns all files as DTOs.
  - GetFile: Returns correct file by ID as DTO, returns NotFound for missing ID.
- Use in-memory database for testing.

## Out of Scope
- File parsing logic.
- Error handling (to be added in future phase).
- Integration tests.

## Rationale
- Storing file content as string simplifies initial persistence and retrieval.
- DTOs decouple internal models from API contracts and allow for future changes without breaking clients.

## Future Considerations
- Add file parsing and validation.
- Implement error handling and logging.
- Add integration and edge-case tests.
