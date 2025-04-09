using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;
using System.Linq;

public partial class StoredProceduresMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        var baseDirectory = AppContext.BaseDirectory;
        var projectRoot = Path.Combine(baseDirectory, "..", "..", "..");
        var scriptsDirectory = Path.Combine(projectRoot, "wwwroot", "Query");

        scriptsDirectory = Path.GetFullPath(scriptsDirectory);

        if (!Directory.Exists(scriptsDirectory))
        {
            throw new DirectoryNotFoundException($"The scripts directory was not found: {scriptsDirectory}");
        }

        var scriptFiles = Directory.GetFiles(scriptsDirectory, "*.sql");

        foreach (var scriptPath in scriptFiles)
        {
            var scriptName = Path.GetFileName(scriptPath);

            // Check if the script has already been executed
            var scriptExecuted = migrationBuilder.Sql($"SELECT COUNT(1) FROM ScriptExecutionHistory WHERE ScriptName = '{scriptName}'").ToString() == "1";

            if (!scriptExecuted)
            {
                var script = File.ReadAllText(scriptPath);
                var scriptBatches = script.Split(new[] { "GO", "go", "Go" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var batch in scriptBatches)
                {
                    var trimmedBatch = batch.Trim();
                    if (!string.IsNullOrWhiteSpace(trimmedBatch))
                    {
                        migrationBuilder.Sql(trimmedBatch);
                    }
                }

                // Log the execution
                migrationBuilder.Sql($"INSERT INTO ScriptExecutionHistory (ScriptName, ExecutionTime) VALUES ('{scriptName}', GETDATE());");
            }
        }
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Optionally implement down logic
        migrationBuilder.Sql($"DELETE FROM __EFMigrationsHistory WHERE MigrationId = '20240922102655_adding_storedProccedures'");
    }
}
