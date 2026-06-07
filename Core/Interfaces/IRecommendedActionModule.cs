namespace GameBoost.Core.Interfaces
{
    public interface IRecommendedActionModule
    {
        RecommendationPriority RecommendationPriority { get; }

        object? RecommendedValue { get; }
        string RecommendationReason { get; }

        bool IsRecommendedValue(object? currentValue);
    }
}
