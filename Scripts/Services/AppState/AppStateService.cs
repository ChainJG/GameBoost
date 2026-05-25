using GameBoost.Scripts.Services.Models;
using System.IO;
using System.Text.Json;

namespace GameBoost.Scripts.Services.AppState
{
    public static class AppStateService
    {
        private static readonly string Path =
            System.IO.Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.ApplicationData),
               "GameBoost",
               "state.json");

        public static AppStateInfo Load()
        {
            if (!File.Exists(Path))
                return new AppStateInfo();

            return JsonSerializer.Deserialize<AppStateInfo>(File.ReadAllText(Path)) ?? new AppStateInfo();
        }

        public static void Save(AppStateInfo state)
        {
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path)!);

            File.WriteAllText(
             Path,
             JsonSerializer.Serialize(state,
                 options: new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
