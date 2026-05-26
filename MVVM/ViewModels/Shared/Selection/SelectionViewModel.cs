using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using System.Collections.ObjectModel;

namespace GameBoost.MVVM.ViewModels.Shared
{
    public class SelectionViewModel : ObservableObject
    {
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

        private SelectionFeatureViewModel _selectedFeature;

        public SelectionFeatureViewModel SelectedFeatureCard
        {
            get => _selectedFeature;
            set => Set(ref _selectedFeature, value);
        }

        #region View State
        public string PageTitle { get; set; }

        #endregion
    }
}
