using System;

namespace MyWebApp.Models
{
    public class ServiceStatus
    {
        public required string ServiceName { get; set; }
        public required string Version { get; set; }
        public bool IsRunning { get; set; }
        public required string Status { get; set; }
        public DateTime LastChecked { get; set; }
    }
}