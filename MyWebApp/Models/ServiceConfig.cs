namespace MyWebApp.Models
{
    public class ServiceConfig
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public CommandConfig Command { get; set; }
        public string VersionArgs { get; set; }
        public string VersionPattern { get; set; }
    }

    public class CommandConfig
    {
        public string Windows { get; set; }
        public string Linux { get; set; }
        public string OSX { get; set; }
    }
}
