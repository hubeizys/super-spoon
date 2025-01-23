using System;

namespace MyWebApp.Models
{
    public class ServiceStatus
    {
        public string ServiceName { get; set; }
        public string Version { get; set; }
        public bool IsRunning { get; set; }
        public string Status { get; set; }
        public DateTime LastChecked { get; set; }
    }
}