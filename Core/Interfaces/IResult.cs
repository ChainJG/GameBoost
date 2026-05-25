namespace GameBoost.Core.Interfaces
{
    public interface IResult
    {
        bool Success { get; }
        string Message { get; }
        ResultType Status { get; }
    }
}
