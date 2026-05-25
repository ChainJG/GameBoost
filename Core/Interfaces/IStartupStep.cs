using GameBoost.Shared.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.Core.Interfaces
{
    public interface IStartupStep
    {
        Task<ModuleResult> ExecuteAsync(IProgress<ProgressResult> progress);
    }
}
