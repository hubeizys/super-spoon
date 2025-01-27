using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyWebApp.Models;

namespace MyWebApp.Services
{
    public class ProcessServiceChecker : IServiceChecker
    {
        private readonly ILogger<ProcessServiceChecker> _logger;

        public ProcessServiceChecker(ILogger<ProcessServiceChecker> logger)
        {
            _logger = logger;
        }

        public async Task<ServiceStatus> CheckStatus(ServiceConfig config)
        {
            var version = "未知";
            var isRunning = false;

            try
            {
                var command = GetCommandPath(config.Command);
                var startInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = config.VersionArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                using var process = Process.Start(startInfo);
                var output = await process.StandardOutput.ReadToEndAsync();
                if (string.IsNullOrEmpty(output))
                {
                    output = await process.StandardError.ReadToEndAsync();
                }
                await process.WaitForExitAsync(TimeSpan.FromSeconds(3));

                if (process.ExitCode == 0 && !string.IsNullOrEmpty(output))
                {
                    version = ParseVersion(output, config.VersionPattern);
                    isRunning = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"检查{config.Name}状态时出错");
            }

            return new ServiceStatus
            {
                ServiceName = config.Name,
                Version = version,
                IsRunning = isRunning,
                Status = isRunning ? "运行中" : "未安装",
                LastChecked = DateTime.Now
            };
        }

        private string GetCommandPath(CommandConfig command)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return command.Windows;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return command.Linux;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return command.OSX;
            
            throw new PlatformNotSupportedException();
        }

        private string ParseVersion(string output, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return output.Trim();

            var match = Regex.Match(output, pattern);
            return match.Success ? match.Value : output.Trim();
        }
    }
}
