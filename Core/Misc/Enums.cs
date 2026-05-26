namespace GameBoost
{
    public enum ToggleType
    {
        Enabled,
        Disabled,
        Unknown
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
}
