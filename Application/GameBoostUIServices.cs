using GameBoost.Application.Operations;
using GameBoost.Application.Selection.Services;
using GameBoost.Application.Startup;
using GameBoost.Application.Titlebar;
using GameBoost.Core.EventArguments;

namespace GameBoost.Application
{
    public sealed class GameBoostUIServices
    {
        public event Action<SelectionScanCompletedEventArgs>? ScanComplete;

        public GlobalOperationService GlobalOperations { get; }

        public TitleBarActionService TitleBarActions { get; }

        public StartupNotificationService StartupNotifications { get; }

        public SelectionExecutionRequirementService SelectionRequirements { get; }

        public SelectionScanNotificationService SelectionScanNotifications { get; }

        private GameBoostUIServices(
            GlobalOperationService globalOperations,
            TitleBarActionService titleBarActions,
            StartupNotificationService startupNotifications,
            SelectionExecutionRequirementService selectionRequirements,
            SelectionScanNotificationService selectionScanNotifications)
        {
            GlobalOperations = globalOperations;
            TitleBarActions = titleBarActions;
            StartupNotifications = startupNotifications;
            SelectionRequirements = selectionRequirements;
            SelectionScanNotifications = selectionScanNotifications;
        }

        public static GameBoostUIServices Create()
        {
            var globalOperations = new GlobalOperationService();

            var titleBarActions = new TitleBarActionService(
                globalOperations);

            var startupNotifications = new StartupNotificationService(
                titleBarActions);

            var selectionRequirements = new SelectionExecutionRequirementService(
                titleBarActions);

            var selectionScanNotifications = new SelectionScanNotificationService();

            return new GameBoostUIServices(
                globalOperations,
                titleBarActions,
                startupNotifications,
                selectionRequirements,
                selectionScanNotifications);
        }
    }
}