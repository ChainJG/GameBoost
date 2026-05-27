using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using GameBoost.Shared.Helpers;
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

                OnPropertyChanged(nameof(ResultTitleText));
                OnPropertyChanged(nameof(ExecutionProgressText));
                OnPropertyChanged(nameof(ExecutionProgressPercentage));
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
        public string ResultTitleText
        {
            get
            {
                return FailedExecutions switch
                {
                    0 => "All selected optimisations were applied",
                    1 => "1 Optimisation could not be completed",
                    _ => $"{FailedExecutions} Optimisations could not be completed"
                };
            }
        }

        private ObservableCollection<SelectionResultViewModel> _resultCards = [];

        public ObservableCollection<SelectionResultViewModel> ResultCards
        {
            get => _resultCards;
            protected set => Set(ref _resultCards, value);
        }
        #endregion

        #region Execution ObservableCollection
        public string ExecutionTitleText => $"Processing {PageTitle} Changes...";
        public string ExecutionProgressText => $"{CompletedExecutions}/{TotalExecutions}";
        public int ExecutionProgressPercentage => MathHelper.ToPercentageInt(CompletedExecutions, TotalExecutions);
        #endregion

        #region Selected Execution Card
        private SelectionResultViewModel? _selectedExecutionCard;
        public SelectionResultViewModel? SelectedExecutionCard
        {
            get => _selectedExecutionCard;
            set
            {
                if (!Set(ref _selectedExecutionCard, value)) return;

                OnPropertyChanged(nameof(SelectedResultSummary));
                OnPropertyChanged(nameof(SelectedResultOutput));
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
            _executionCancellation = new CancellationTokenSource();

            try
            {
                foreach (var feature in FeatureCards)
                    await feature.RefreshStatusesAsync(_executionCancellation.Token);

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
                // Execute the selected feature cards
                await ExecuteSelectedFeatureCardsAsync(token);

                // Set the result screen
                DisplayScreenType = SelectionScreenType.Result;
            }
            catch (OperationCanceledException ex)
            {
                // Execution Cancellation
                HandleExecutionCancellation(ex.Message);
            }
            finally
            {
                // Initialise the first selected execution card
                OnPropertyChanged(nameof(SelectedResultSummary));
                OnPropertyChanged(nameof(SelectedResultOutput));
            }
        }
        private async Task ExecuteSelectedFeatureCardsAsync(CancellationToken token)
        {
            // Execute the selected feature cards
            var runnableFeatureCards = FeatureCards
                .Where(feature => feature.IsRunnable)
                .ToList();

            foreach(var featureCard in runnableFeatureCards)
            {
                token.ThrowIfCancellationRequested();

                SelectedFeatureCard = featureCard;

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

                await Task.Delay(1000, token);
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
                await Task.Delay(1000, token);
                execution.Result = await actionCard.ExecuteAsync(token);
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
