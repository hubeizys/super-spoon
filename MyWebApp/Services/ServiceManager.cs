using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using MyWebApp.Models;

namespace MyWebApp.Services
{
    public class ServiceManager
    {
        private readonly ILogger<ServiceManager> _logger;
        private readonly Dictionary<string, IServiceChecker> _checkers;
        private readonly List<ServiceConfig> _services;

        public ServiceManager(ILogger<ServiceManager> logger, IEnumerable<IServiceChecker> checkers)
        {
            _logger = logger;
            _checkers = checkers.ToDictionary(c => c.ServiceType);
            _services = LoadServiceConfigs();
        }

        public async Task<List<ServiceStatus>> CheckAllServices()
        {
            var results = new List<ServiceStatus>();
            foreach (var service in _services)
            {
                if (_checkers.TryGetValue(service.Type, out var checker))
                {
                    var status = await checker.CheckStatus(service);
                    results.Add(status);
                }
                else
                {
                    _logger.LogWarning($"未找到服务类型 {service.Type} 的检查器");
                }
            }
            return results;
        }

        private List<ServiceConfig> LoadServiceConfigs()
        {
            try
            {
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Config", "services.json");
                var jsonContent = File.ReadAllText(jsonPath);
                var config = JsonSerializer.Deserialize<ServicesConfig>(jsonContent);
                return config?.Services ?? new List<ServiceConfig>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "加载服务配置文件失败");
                return new List<ServiceConfig>();
            }
        }
    }

    public class ServicesConfig
    {
        public required List<ServiceConfig> Services { get; set; }
    }
}
