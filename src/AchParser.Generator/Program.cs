using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using AchParser.Generator.Builders;

namespace AchParser.Generator;

class Program
{
    static int Main(string[] args)
    {
        var entriesOption = new Option<int>("--entries", "Number of Entry Detail records to generate") { IsRequired = true };
        var outputOption = new Option<string>("--output", "Output file name") { IsRequired = true };

        var rootCommand = new RootCommand
        {
            entriesOption,
            outputOption
        };

        rootCommand.Description = "ACH File Generator";

        rootCommand.SetHandler((int entries, string output) =>
        {
            var builder = new AchFileBuilder();
            var achFile = builder.Build(entries);
            
            var outputDir = System.IO.Path.Combine(AppContext.BaseDirectory, "GeneratedFiles");
            
            if (!System.IO.Directory.Exists(outputDir))
                System.IO.Directory.CreateDirectory(outputDir);
            
            var fileName = System.IO.Path.GetFileName(output);
            var fullPath = System.IO.Path.Combine(outputDir, fileName);
            System.IO.File.WriteAllText(fullPath, achFile);

            Console.WriteLine($"ACH file generated: {fullPath}");
        }, entriesOption, outputOption);

        return rootCommand.Invoke(args);
    }
}
