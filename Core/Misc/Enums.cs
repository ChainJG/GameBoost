namespace GameBoost
{
    public enum ToggleType
    {
        Enabled,
        Disabled,
        Unknown,
        None
    }

    public enum DockState
    {
        Full,
        Compact
    }

    public enum WorkflowView
    {
        Selection,
        Execution,
        Result
    }

    public enum RecommendationPriority
    {
        None = 0,
        Low = 1,
        Medium = 2,
        High = 3,
    }

    public enum ResultType
    {
        Successful,
        Failed,
        AdministratorProtection,
        Unknown
    }

    public enum ShellType
    {
        Cmd,
        PowerShell,
    }
    public enum ServiceAction
    {
        Start,
        Stop,
        Enable,
        Disable
    }

    public enum SelectionType
    {
        Single,
        Multiple
    }

    public enum SelectionScreenType
    {
        Selection,
        Execution,
        Result
    }

    public enum RegistryValueAction
    {
        Set,
        Delete,
        Ignore
    }

    public enum InfoCardState
    {
        Info,
        Recommended,
        Success,
        Warning,
        Error,
        Performance,
        Disabled,
        Unknown
    }
}
