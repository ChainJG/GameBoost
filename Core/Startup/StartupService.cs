using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Core.Startup
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
                new CheckRestorePointStartupStep()
            ];
        }

        public async Task<ModuleResult> InitialiseAsync(IProgress<ProgressResult> progress)
        {
            try
            {
                foreach (var step in _steps)
                {
                    var result = await step.ExecuteAsync(progress);

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
