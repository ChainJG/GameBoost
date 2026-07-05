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
                new CheckForUpdatesStartupStep(),
                new LoadSystemInfoStartupStep(),
                new CheckRestorePointStartupStep(),
                new LoadMicrosoftAppxPackageInfoStep(),
            ];
        }

        public async Task<ModuleResult> InitialiseAsync(IProgress<ProgressResult> progress,
            Func<IProgress<ProgressResult>, CancellationToken, Task>? initialiseMainViewModel = null,
            CancellationToken token = default)
        {
            try
            {
                var hasModuleRefresh = initialiseMainViewModel is not null;
                var totalUnits = _steps.Count + (hasModuleRefresh ? 1 : 0);

                if (totalUnits <= 0)
                {
                    progress.Report(new ProgressResult("Initialisation Complete", 100));
                    return ModuleResult.Successful("Initialisation Complete");
                }


                for (var index = 0; index < _steps.Count; index++)
                {
                    token.ThrowIfCancellationRequested();

                    var step = _steps[index];

                    var range = StartupProgressMapper.GetRange(index, totalUnits);

                    progress.Report(new ProgressResult(step.Name, range.StartPercent));

                    var stepProgress = step is CheckForUpdatesStartupStep
                        ? progress
                        : StartupProgressMapper.CreateRange(
                            progress,
                            range.StartPercent,
                            range.EndPercent,
                            step.Name);

                    var result = await GameBoostContext.Diagnostic.TrackAsync(
                        category: "Startup",
                        operationType: DiagnosticOperationType.StartupStep,
                        name: step.Name,
                        source: step.GetType().Name,
                        operation: _ => step.ExecuteAsync(stepProgress, token),
                        metadata: new Dictionary<string, string?>
                        {
                            ["StepType"] = step.GetType().FullName,
                            ["ProgressStart"] = range.StartPercent.ToString(),
                            ["ProgressEnd"] = range.EndPercent.ToString()
                        });

                    progress.Report(new ProgressResult($"{step.Name} Complete", range.EndPercent));
                }

                if (initialiseMainViewModel is not null)
                {
                    token.ThrowIfCancellationRequested();

                    var ModuleRefreshIndex = _steps.Count;

                    var range = StartupProgressMapper.GetRange(ModuleRefreshIndex, totalUnits);

                    progress.Report(new ProgressResult("Initialisating Modules", range.StartPercent));

                    var moduleProgress = StartupProgressMapper.CreateRange(progress, range.StartPercent, range.EndPercent, "Initialisating Modules");

                    await initialiseMainViewModel(moduleProgress, token);
                }

                progress.Report(new ProgressResult("Initialisation Complete", 100));

                return ModuleResult.Successful("Initialisation Complete");
            }
            catch (OperationCanceledException)
            {
                progress.Report(new ProgressResult("Initialisation Cancelled", 100));
                return ModuleResult.Failed("Initialisation Cancelled");
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
