using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.Shared.Results;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace GameBoost.MVVM.ViewModels.Shared
{
    public class SelectionViewModel : ObservableObject
    {
        private bool _hasInitilised = false;

        private CancellationTokenSource? _executionCancellation;

        private int _completedExecutions;
        public int CompletedExecutions
        {
            get => _completedExecutions;
            set
            {
                if (!Set(ref _completedExecutions, value)) return;

                OnPropertyChanged(nameof(ExecutionProgressText));
            }
        }
        private int FailedExecutions { get; set; } = 0;
        private int SuccessExecutions { get; set; } = 0;
        private int TotalExecutions => // Gets the global (IsChecked) Actions count in (IsChecked) FeatureCards
            FeatureCards.Where(feature => feature.IsRunnable)
            .SelectMany(feature => feature.Actions)
            .Count(action => action.IsChecked);
            
        #region Selection ObserbableCollection
        private ObservableCollection<SelectionFeatureViewModel> _featuresCards;

        public ObservableCollection<SelectionFeatureViewModel> FeatureCards
        {
            get => _featuresCards;
            protected set
            {
                if (Set(ref _featuresCards, value))
                    SelectedFeatureCard = _featuresCards.FirstOrDefault();
            }
        }

        private SelectionFeatureViewModel? _selectedFeatureCard;

        public SelectionFeatureViewModel? SelectedFeatureCard
        { 
            get => _selectedFeatureCard;
            set => Set(ref _selectedFeatureCard, value);
        }
        #endregion

        #region Result ObservableCollection
        public string ResultTitleText;

        private ObservableCollection<SelectionResultViewModel> _resultCards;

        public ObservableCollection<SelectionResultViewModel> ResultCards
        {
            get => _resultCards;
            protected set => Set(ref _resultCards, value);
        }
        #endregion

        #region Execution ObservableCollection
        public string ExecutionTitleText => $"Processing {PageTitle} Changes...";
        public string ExecutionProgressText => $"{CompletedExecutions}/{TotalExecutions}";
        #endregion

        #region Selected Execution Card
        private SelectionResultViewModel? _selectedExecutionCard;
        public SelectionResultViewModel? SelectedExecutionCard
        {
            get => _selectedExecutionCard;
            set
            {
                if (!Set(ref _selectedExecutionCard, value)) return;

            }
        }

        public string SelectedResultSummary => SelectedExecutionCard?.Result?.Success ?? false ? "Great" : "Failed";
        public string SelectedResultOutput => SelectedExecutionCard?.Result?.Message ?? $"{SelectedExecutionCard?.Title} doesn't have active modules";
        #endregion

        #region View State
        public string? PageTitle { get; set; }

        private SelectionScreenType _selectionType = SelectionScreenType.Selection;
        public SelectionScreenType DisplayScreenType
        {
            get => _selectionType;
            set
            {
                if (!Set(ref _selectionType, value)) return;

                StateChanged?.Invoke(value);
            }
        }

        public event Action<SelectionScreenType>? StateChanged;
        #endregion

        public async Task InitialiseAsync()
        {
            if (_hasInitilised)
                return;

            _hasInitilised = true;

            await RefreshAllStatusesAsync();
        }

        public async Task RefreshAllStatusesAsync()
        {
            try
            {
                foreach (var feature in FeatureCards)
                    await feature.RefreshStatusesAsync();

            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in RefreshAllStatusesAsync: {ex.Message}");
#endif
            }
        }

        private CancellationToken CreateExecutionSession()
        {
            _executionCancellation = new CancellationTokenSource();

            CompletedExecutions = 0;
            FailedExecutions = 0;
            SuccessExecutions = 0;
            ResultCards.Clear();
            DisplayScreenType = SelectionScreenType.Execution;

            return _executionCancellation.Token;
        }
        public async Task ExecuteSelectedActionsAsync()
        {
            var token = CreateExecutionSession();

            try
            {
                await ExecuteSelectedFeatureCardsAsync(token);

                DisplayScreenType = SelectionScreenType.Result;
            }
            catch (OperationCanceledException ex)
            {
                HandleExecutionCancellation(ex.Message);
            }
        }
        private async Task ExecuteSelectedFeatureCardsAsync(CancellationToken token)
        {
            var runnableFeatureCards = FeatureCards
                .Where(feature => feature.IsRunnable)
                .ToList();

            foreach(var featureCard in runnableFeatureCards)
            {
                token.ThrowIfCancellationRequested();

                await ExecuteSelectedActionCardsAsync(featureCard, token);
            }
        }
        private async Task ExecuteSelectedActionCardsAsync(SelectionFeatureViewModel featureCard, CancellationToken token)
        {
            var runnableActionCards = featureCard.Actions
                .Where(action => action.IsChecked)
                .ToList();

            foreach (var actionCard in runnableActionCards)
            {
                token.ThrowIfCancellationRequested();

                await ExecuteActionCardAsync(actionCard, token);

                await Task.Delay(300, token);
            }
        }

        private async Task ExecuteActionCardAsync(SelectionActionViewModel actionCard, CancellationToken token)
        {
            var execution = new SelectionResultViewModel(actionCard)
            {
                State = ResultButtonState.Running
            };

            ResultCards.Insert(0, execution);
            SelectedExecutionCard = execution;

            var stopWatch  = Stopwatch.StartNew();

            try
            {
                execution.Result = await actionCard.ExecuteAsync();
            }       
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error {actionCard.Title}: {ex.Message}");
#endif
                execution.Result = ModuleResult.Failed(ex.Message);

            }
            finally
            {
                FinaliseActionExecution(execution, stopWatch);
                CompletedExecutions++;
            }
        }

        private void FinaliseActionExecution(SelectionResultViewModel execution, Stopwatch stopWatch)
        {
            stopWatch.Stop();
            var duration = stopWatch.Elapsed.TotalSeconds;

            execution.Status = duration >= 1
                ? $"{execution.Result?.Status} {duration:f1}s"
                : $"{execution.Result?.Status}";

            if (execution.Result?.Success ?? false)
                SuccessExecutions++;
            else
                FailedExecutions++;

            execution.State = ResultButtonState.Result;
        }

        private static void HandleExecutionCancellation(string error)
        {
#if DEBUG
            Debug.WriteLine($"Execution Cancellation: {error}");
#endif
        }
    }
}
