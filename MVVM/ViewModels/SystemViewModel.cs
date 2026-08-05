using GameBoost.Application.Operations;
using GameBoost.Application.Selection.Builders;
using GameBoost.Application.Selection.Registries;
using GameBoost.Application.Selection.Services;
using GameBoost.MVVM.ViewModels.Shared.Selection;

namespace GameBoost.MVVM.ViewModels
{
    public class SystemViewModel : SelectionViewModel
    {
        public SystemViewModel(
            GlobalOperationService globalOperations,
            SelectionActionRefreshService refreshService,
            SelectionExecutionRequirementService requirementService,
            SelectionScanNotificationService scanNotifications)
            : base(globalOperations, refreshService, requirementService, scanNotifications)
        {
            PageTitle = "System Optimisation";

            FeatureCards =
            [
                ..SelectionFeatureBuilder.BuildMany(
                    SystemFeatureRegistry.GetFeatures())
            ];
        }
    }
}
