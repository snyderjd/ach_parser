# ACH Parser

ACH file parsing application built with .NET, PostgreSQL, and Docker.

## AchParser.Generator

The `AchParser.Generator` project allows you to generate sample ACH files for testing and development. It produces properly formatted ACH files with a specified number of entry records.

### Usage

To generate a sample ACH file, run the following command from your project root:

```
dotnet run --project ./src/AchParser.Generator -- --entries <number_of_entries> --output <output_filename>
```

For example, to generate an ACH file with 10 entries and save it as `sample.ach`:

```
dotnet run --project ./src/AchParser.Generator -- --entries 10 --output sample.ach
```

The generated file will be saved in the `GeneratedFiles` directory under your project.

## Local Development
```
docker-compose up -d postgres
dotnet run --project ./src/AchParser.Api
```