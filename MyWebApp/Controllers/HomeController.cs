using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using MyWebApp.Models;
using MyWebApp.Services;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace MyWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ServiceManager _serviceManager;

    public HomeController(ILogger<HomeController> logger, ServiceManager serviceManager)
    {
        _logger = logger;
        _serviceManager = serviceManager;
    }

    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        if (string.IsNullOrEmpty(culture))
        {
            culture = "en-US";
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
        );

        return LocalRedirect(!string.IsNullOrEmpty(returnUrl) ? returnUrl : "~/");
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetServicesStatus()
    {
        var services = await _serviceManager.CheckAllServices();
        return Json(services);
    }

    private void CheckMySQLStatus(List<ServiceStatus> services)
    {
        var version = "未知";
        var isRunning = false;

        try
        {
            var mysqlCommand = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "mysql.exe" : "mysql";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var fullPath = "/usr/local/bin/mysql";
                if (System.IO.File.Exists(fullPath))
                {
                    mysqlCommand = fullPath;
                }
                else if (System.IO.File.Exists("/usr/bin/mysql"))
                {
                    mysqlCommand = "/usr/bin/mysql";
                }
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = mysqlCommand,
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd() ?? process?.StandardError.ReadToEnd();
            process?.WaitForExit(3000); // 添加3秒超时

            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                var match = Regex.Match(output, @"Ver \d+\.\d+\.\d+");
                version = match.Success ? match.Value : output.Trim();
                isRunning = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查MySQL状态时出错");
        }

        services.Add(new ServiceStatus
        {
            ServiceName = "MySQL",
            Version = version,
            IsRunning = isRunning,
            Status = isRunning ? "运行中" : "未安装",
            LastChecked = DateTime.Now
        });
    }

    private void CheckRedisStatus(List<ServiceStatus> services)
    {
        var version = "未知";
        var isRunning = false;

        try
        {
            var redisCommand = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "redis-cli.exe" : "redis-cli";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var fullPath = "/usr/local/bin/redis-cli";
                if (System.IO.File.Exists(fullPath))
                {
                    redisCommand = fullPath;
                }
                else if (System.IO.File.Exists("/usr/bin/redis-cli"))
                {
                    redisCommand = "/usr/bin/redis-cli";
                }
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = redisCommand,
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd() ?? process?.StandardError.ReadToEnd();
            process?.WaitForExit(3000);

            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                version = output.Trim();
                isRunning = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查Redis状态时出错");
        }

        services.Add(new ServiceStatus
        {
            ServiceName = "Redis",
            Version = version,
            IsRunning = isRunning,
            Status = isRunning ? "运行中" : "未安装",
            LastChecked = DateTime.Now
        });
    }

    private void CheckDockerStatus(List<ServiceStatus> services)
    {
        var version = "未知";
        var isRunning = false;

        try
        {
            var dockerCommand = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "docker.exe" : "docker";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var fullPath = "/usr/local/bin/docker";
                if (System.IO.File.Exists(fullPath))
                {
                    dockerCommand = fullPath;
                }
                else if (System.IO.File.Exists("/usr/bin/docker"))
                {
                    dockerCommand = "/usr/bin/docker";
                }
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = dockerCommand,
                Arguments = "--version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd() ?? process?.StandardError.ReadToEnd();
            process?.WaitForExit(3000);

            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                var match = Regex.Match(output, @"version \d+\.\d+\.\d+");
                version = match.Success ? match.Value : output.Trim();
                isRunning = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查Docker状态时出错");
        }

        services.Add(new ServiceStatus
        {
            ServiceName = "Docker",
            Version = version,
            IsRunning = isRunning,
            Status = isRunning ? "运行中" : "未安装",
            LastChecked = DateTime.Now
        });
    }

    private void CheckJavaStatus(List<ServiceStatus> services)
    {
        var version = "未知";
        var isRunning = false;

        try
        {
            var javaCommand = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "java.exe" : "java";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var fullPath = "/usr/bin/java";
                if (System.IO.File.Exists(fullPath))
                {
                    javaCommand = fullPath;
                }
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = javaCommand,
                Arguments = "-version",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardError.ReadToEnd() ?? process?.StandardOutput.ReadToEnd();
            process?.WaitForExit(3000);

            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                var match = Regex.Match(output, @"version ""\d+\.\d+\.\d+");
                version = match.Success ? match.Value : output.Split('\n')[0].Trim();
                isRunning = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查Java状态时出错");
        }

        services.Add(new ServiceStatus
        {
            ServiceName = "Java",
            Version = version,
            IsRunning = isRunning,
            Status = isRunning ? "已安装" : "未安装",
            LastChecked = DateTime.Now
        });
    }

    private void CheckPythonStatus(List<ServiceStatus> services)
    {
        var version = "未知";
        var isRunning = false;
        var pythonCommands = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
            ? new[] { "python.exe", "python3.exe", "py.exe" }
            : new[] { "python3", "python" };

        foreach (var cmd in pythonCommands)
        {
            try
            {
                var pythonCommand = cmd;
                if ((RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) 
                    && System.IO.File.Exists($"/usr/bin/{cmd}"))
                {
                    pythonCommand = $"/usr/bin/{cmd}";
                }

                var startInfo = new ProcessStartInfo
                {
                    FileName = pythonCommand,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                using var process = Process.Start(startInfo);
                var output = process?.StandardOutput.ReadToEnd() ?? process?.StandardError.ReadToEnd();
                process?.WaitForExit(3000);

                if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
                {
                    version = output.Trim();
                    isRunning = true;
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"使用命令 {cmd} 检查Python状态时出错");
                continue;
            }
        }

        services.Add(new ServiceStatus
        {
            ServiceName = "Python",
            Version = version,
            IsRunning = isRunning,
            Status = isRunning ? "已安装" : "未安装",
            LastChecked = DateTime.Now
        });
    }

    private void CheckApacheStatus(List<ServiceStatus> services)
    {
        var version = "未知";
        var isRunning = false;

        try
        {
            var apacheCommand = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "httpd.exe" : "httpd";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (System.IO.File.Exists("/usr/sbin/httpd"))
                {
                    apacheCommand = "/usr/sbin/httpd";
                }
                else if (System.IO.File.Exists("/usr/sbin/apache2"))
                {
                    apacheCommand = "/usr/sbin/apache2";
                }
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = apacheCommand,
                Arguments = "-v",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd() ?? process?.StandardError.ReadToEnd();
            process?.WaitForExit(3000);

            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                var match = Regex.Match(output, @"version: Apache/\d+\.\d+\.\d+");
                version = match.Success ? match.Value : output.Split('\n')[0].Trim();
                isRunning = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查Apache状态时出错");
        }

        services.Add(new ServiceStatus
        {
            ServiceName = "Apache",
            Version = version,
            IsRunning = isRunning,
            Status = isRunning ? "运行中" : "未安装",
            LastChecked = DateTime.Now
        });
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult ApiTest()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
