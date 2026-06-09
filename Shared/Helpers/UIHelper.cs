using GameBoost.Application;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GameBoost.Shared.Helpers
{
    public static class UIHelper
    {
        public static string VersionText
        {
            get
            {
                var version = Assembly
                    .GetEntryAssembly()?
                    .GetName()
                    .Version?
                    .ToString(3);

                if (string.IsNullOrWhiteSpace(version))
                    return "Unknown Version";

                return GameBoostContext.IsBeta
                    ? $"Beta"
                    : $"v{version}";
            }
        }
    }
}
