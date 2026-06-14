using GameBoost.Infrastructure.MicrosoftApps.Models;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Infrastructure.MicrosoftApps.Services
{
    public static class MicrosoftAppCatalog
    {
        public static IReadOnlyList<MicrosoftAppDefinition> CommonApps { get; } =
        [
            new()
            {
                DisplayName = "Xbox Game Bar",
                PackageName = "Microsoft.XboxGamingOverlay",
                Icon = PackIconKind.XboxLive,
                Description = "Xbox Game Bar overlay used for capture, widgets, and gaming overlay features."
            },

            new()
            {
                DisplayName = "Xbox App",
                PackageName = "Microsoft.GamingApp",
                Icon = PackIconKind.XboxLive,
                Description = "Xbox app used for Game Pass, Xbox services, and Xbox game management."
            },

            new()
            {
                DisplayName = "Calculator",
                PackageName = "Microsoft.WindowsCalculator",
                Icon = PackIconKind.Calculator,
                Description = "Windows Calculator app."
            },

            new()
            {
                DisplayName = "Paint",
                PackageName = "Microsoft.Paint",
                Icon = PackIconKind.Palette,
                Description = "Microsoft Paint app."
            },

            new()
            {
                DisplayName = "Snipping Tool",
                PackageName = "Microsoft.ScreenSketch",
                Icon = PackIconKind.ContentCut,
                Description = "Windows screenshot and snipping app."
            },

            new()
            {
                DisplayName = "Photos",
                PackageName = "Microsoft.Windows.Photos",
                Icon = PackIconKind.Image,
                Description = "Microsoft Photos app."
            },

            new()
            {
                DisplayName = "Notepad",
                PackageName = "Microsoft.WindowsNotepad",
                Icon = PackIconKind.NoteText,
                Description = "Windows Notepad app."
            },

            new()
            {
                DisplayName = "Windows Terminal",
                PackageName = "Microsoft.WindowsTerminal",
                Icon = PackIconKind.Console,
                Description = "Windows Terminal app."
            },

            new()
            {
                DisplayName = "Sticky Notes",
                PackageName = "Microsoft.MicrosoftStickyNotes",
                Icon = PackIconKind.Note,
                Description = "Microsoft Sticky Notes app."
            },

            new()
            {
                DisplayName = "Get Help",
                PackageName = "Microsoft.GetHelp",
                Icon = PackIconKind.HelpCircle,
                Description = "Microsoft Get Help support app."
            },

            new()
            {
                DisplayName = "Feedback Hub",
                PackageName = "Microsoft.WindowsFeedbackHub",
                Icon = PackIconKind.CommentQuote,
                Description = "Microsoft Feedback Hub app."
            },

            new()
            {
                DisplayName = "HEIF Image Extension",
                PackageName = "Microsoft.HEIFImageExtension",
                Icon = PackIconKind.FileImage,
                Description = "HEIF image support extension."
            },

            new()
            {
                DisplayName = "WebP Image Extension",
                PackageName = "Microsoft.WebpImageExtension",
                Icon = PackIconKind.FileImage,
                Description = "WebP image support extension."
            },

            new()
            {
                DisplayName = "Web Media Extensions",
                PackageName = "Microsoft.WebMediaExtensions",
                Icon = PackIconKind.Extension,
                Description = "Additional media format support for Windows apps."
            },

            new()
            {
                DisplayName = "Movies & TV",
                PackageName = "Microsoft.ZuneVideo",
                Icon = PackIconKind.Movie,
                Description = "Microsoft Movies & TV media app."
            },

            new()
            {
                DisplayName = "Media Player",
                PackageName = "Microsoft.ZuneMusic",
                Icon = PackIconKind.Music,
                Description = "Microsoft Media Player app."
            },

            new()
            {
                DisplayName = "Weather",
                PackageName = "Microsoft.BingWeather",
                Icon = PackIconKind.WeatherPartlyCloudy,
                Description = "Microsoft Weather app."
            },

            new()
            {
                DisplayName = "News",
                PackageName = "Microsoft.BingNews",
                Icon = PackIconKind.Newspaper,
                Description = "Microsoft News app."
            },

            new()
            {
                DisplayName = "Phone Link",
                PackageName = "Microsoft.YourPhone",
                Icon = PackIconKind.CellphoneLink,
                Description = "Phone Link app for connecting Android/iPhone with Windows."
            }
        ];

        public static MicrosoftAppDefinition? FindByPackageName(string packageName) =>
             CommonApps.FirstOrDefault(app =>
                string.Equals(app.PackageName, packageName, StringComparison.OrdinalIgnoreCase));

        public static MicrosoftAppDefinition? FindMatchingInstalledPackage(string installedPackageName) =>
            CommonApps.FirstOrDefault(app =>
                installedPackageName.Contains(app.PackageName, StringComparison.OrdinalIgnoreCase));
    }
}