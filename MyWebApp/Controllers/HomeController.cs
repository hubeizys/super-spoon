using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;
using MyWebApp.Models;
using System.Text.RegularExpressions;

namespace MyWebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
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
    public IActionResult GetServicesStatus()
    {
        var services = new List<ServiceStatus>();
        
        // 检查 MySQL
        CheckMySQLStatus(services);
        
        // 检查 Redis
        CheckRedisStatus(services);
        
        // 检查 Docker
        CheckDockerStatus(services);
        
        // 检查 Java
        CheckJavaStatus(services);
        
        // 检查 Python
        CheckPythonStatus(services);
        
        // 检查 Apache
        CheckApacheStatus(services);
        
        return Json(services);
    }

    private void CheckMySQLStatus(List<ServiceStatus> services)
    {
        var version = "未知";
        var isRunning = false;

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "mysql",
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd();
            process?.WaitForExit();

            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                var match = Regex.Match(output, @"Ver \d+\.\d+\.\d+");
                version = match.Success ? match.Value : output.Trim();
                isRunning = true;
            }
        }
        catch
        {
            // 命令不存在或执行失败
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
            var startInfo = new ProcessStartInfo
            {
                FileName = "redis-cli",
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd();
            process?.WaitForExit();

            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                version = output.Trim();
                isRunning = true;
            }
        }
        catch
        {
            // 命令不存在或执行失败
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
            var startInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "--version",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd();
            process?.WaitForExit();

            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                var match = Regex.Match(output, @"version \d+\.\d+\.\d+");
                version = match.Success ? match.Value : output.Trim();
                isRunning = true;
            }
        }
        catch
        {
            // 命令不存在或执行失败
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
            var startInfo = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = "-version",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardError.ReadToEnd();
            process?.WaitForExit();

            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                var match = Regex.Match(output, @"version ""\d+\.\d+\.\d+");
                version = match.Success ? match.Value : output.Split('\n')[0].Trim();
                isRunning = true;
            }
        }
        catch
        {
            // 命令不存在或执行失败
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
        var pythonCommands = new[] { "python", "python3", "py" };

        foreach (var cmd in pythonCommands)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = cmd,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                var output = process?.StandardOutput.ReadToEnd() ?? process?.StandardError.ReadToEnd();
                process?.WaitForExit();

                if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
                {
                    version = output.Trim();
                    isRunning = true;
                    break;
                }
            }
            catch
            {
                // 继续尝试下一个命令
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
            var startInfo = new ProcessStartInfo
            {
                FileName = "httpd",
                Arguments = "-v",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            var output = process?.StandardOutput.ReadToEnd() ?? process?.StandardError.ReadToEnd();
            process?.WaitForExit();

            if (process?.ExitCode == 0 && !string.IsNullOrEmpty(output))
            {
                var match = Regex.Match(output, @"version: Apache/\d+\.\d+\.\d+");
                version = match.Success ? match.Value : output.Split('\n')[0].Trim();
                isRunning = true;
            }
        }
        catch
        {
            // 命令不存在或执行失败
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
