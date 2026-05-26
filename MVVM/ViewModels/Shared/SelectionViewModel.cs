using GameBoost.MVVM.Core;
using GameBoost.MVVM.UserControls.Models;
using System.Collections.ObjectModel;

namespace GameBoost.MVVM.ViewModels.Shared
{
    public class SelectionViewModel : ObservableObject
    {
        private ObservableCollection<SelectionFeatureCard> _features;

        public ObservableCollection<SelectionFeatureCard> FeatureCards
        {
            get => _features;
            set => Set(ref _features, value);
        }

        private SelectionFeatureCard _selectedFeature;

        public SelectionFeatureCard SelectedFeatureCard
        {
            get => _selectedFeature;
            set => Set(ref _selectedFeature, value);
        }

        #region View State
        public string PageTitle { get; set; }

        #endregion
    }
}
