using System.IO;

namespace GameBoost.Application.Diagnostics
{
    public sealed class DiagnosticOptions
    {
        public bool Enabled { get; init; }

        public bool IncludeSuccessfulOperations { get; init; } = true;

        public string RootDirectory { get; init; } =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "GameBoost",
                "Diagnostics");
    }
}
