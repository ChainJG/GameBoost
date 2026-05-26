namespace GameBoost.Infrastructure.Services
{
    public class ServiceEditInfo
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool RequiresAdmin { get; set; }
    }
}
