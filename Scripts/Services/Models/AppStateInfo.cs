namespace GameBoost.Scripts.Services.Models
{
    public class AppStateInfo
    {
        public RestorePointState RestorePoint { get; set; } = new();
    }

    public class RestorePointState
    {
        public DateTime? LastCreated { get; set; }
        public ResultType LastStatus { get; set; } = ResultType.Unknown;
    }
}
