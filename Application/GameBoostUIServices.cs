using GameBoost.Application.Operations;
using GameBoost.Application.Selection.Services;
using GameBoost.Application.Startup;
using GameBoost.Application.Titlebar;
using GameBoost.Core.EventArguments;
using System.Printing;

namespace GameBoost.Application
{
    public sealed class GameBoostUIServices
    {
        public event Action? StartupCompleted;

        public GlobalOperationService GlobalOperations { get; }

        public TitleBarActionService TitleBarActions { get; }

        public StartupNotificationService StartupNotifications { get; }

        public SelectionExecutionRequirementService SelectionRequirements { get; }

        public SelectionScanNotificationService SelectionScanNotifications { get; }

        public SelectionActionRefreshService SelectionRefresh { get; }

        public RecommendedActionService RecommendedActions { get; }

        private GameBoostUIServices(
            GlobalOperationService globalOperations,
            TitleBarActionService titleBarActions,
            StartupNotificationService startupNotifications,
            SelectionExecutionRequirementService selectionRequirements,
            SelectionScanNotificationService selectionScanNotifications,
            SelectionActionRefreshService selectionRefresh)
        {
            GlobalOperations = globalOperations;
            TitleBarActions = titleBarActions;
            StartupNotifications = startupNotifications;
            SelectionRequirements = selectionRequirements;
            SelectionScanNotifications = selectionScanNotifications;
            SelectionRefresh = selectionRefresh;
            RecommendedActions = new RecommendedActionService(this);
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

            var selectionRefresh = new SelectionActionRefreshService(
                maxConcurrentRefreshes: 4);

            return new GameBoostUIServices(
                globalOperations,
                titleBarActions,
                startupNotifications,
                selectionRequirements,
                selectionScanNotifications,
                selectionRefresh);
        }
        public void NotifyStartupCompleted()
        {
            StartupCompleted?.Invoke();
        }
    }
}