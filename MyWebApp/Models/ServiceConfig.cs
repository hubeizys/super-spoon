namespace MyWebApp.Models
{
    public class ServiceConfig
    {
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required CommandConfig Command { get; set; }
        public required string VersionArgs { get; set; }
        public string? VersionPattern { get; set; }
    }

    public class CommandConfig
    {
        public required string Windows { get; set; }
        public required string Linux { get; set; }
        public required string OSX { get; set; }
    }
}
