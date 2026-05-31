using GameBoost.Core.Interfaces;
using GameBoost.Shared.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.Features.Modules.Windows.Devices.Mouse
{
    public sealed class MouseSpeedModule : IInputActionModule<double>
    {
        public string Name => "Mouse Speed";

        public Task<object> RefreshStatusAsync(double input, CancellationToken token)
        {
            return Task.FromResult<object>(3);
        }

        public Task<ModuleResult> ExecuteAsync(double input, CancellationToken token)
        {
            return Task.FromResult(
                ModuleResult.Successful($"Mouse speed set to {input:0}"));
        }
    }
}