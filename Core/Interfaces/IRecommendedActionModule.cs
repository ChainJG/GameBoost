namespace GameBoost.Core.Interfaces
{
    public interface IRecommendedActionModule
    {
        object? RecommendedValue { get; }

        string RecommendedText { get; }

        string RecommendationReason { get; }

        bool IsRecommendedValue(object? currentValue);
    }
}
