using GameBoost.Application.Diagnostics;
using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Application.Startup
{
    public class StartupService
    {
        private readonly List<IStartupStep> _steps;

        public StartupService()
        {
            _steps =
            [
                new LoadSystemInfoStartupStep(),
                new CheckForUpdatesStartupStep(),
                new CheckRestorePointStartupStep(),
                new LoadMicrosoftAppxPackageInfoStep(),
            ];
        }

        public async Task<ModuleResult> InitialiseAsync(IProgress<ProgressResult> progress)
        {
            try
            {
                foreach (var step in _steps)
                {
                    var result = await GameBoostContext.Diagnostic.TrackAsync(
                        category: "Startup",
                        operationType: DiagnosticOperationType.StartupStep,
                        name: step.Name,
                        source: step.GetType().Name,
                        operation: _ => step.ExecuteAsync(progress),
                        metadata: new Dictionary<string, string?>
                        {
                            ["StepType"] = step.GetType().FullName
                        });

                    if (!result.Success)
                        return result;
                }

                progress.Report(new ProgressResult("Initialisation complete", 100));

                return ModuleResult.Successful("Initialisation complete");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in InitialiseAsync: {ex.Message}");
#endif

                progress.Report(new ProgressResult(ex.Message, 100));

                return ModuleResult.Failed(ex.Message);
            }
        }
    }
}
