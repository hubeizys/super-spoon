using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models.LogAnalysis;

namespace MyWebApp.Controllers
{
    public class LogAnalysisController : Controller
    {
        private readonly ILogger<LogAnalysisController> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public LogAnalysisController(
            ILogger<LogAnalysisController> logger, 
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewLogs()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeRequestLog(string logPath)
        {
            try
            {
                var analyzer = new LogAnalyzer(logPath, _loggerFactory.CreateLogger<LogAnalyzer>());
                var result = await analyzer.AnalyzeRequestLogAsync();
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing request log");
                return Json(new { error = "Error analyzing request log" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeErrorLog(string logPath)
        {
            try
            {
                var analyzer = new LogAnalyzer(logPath, _loggerFactory.CreateLogger<LogAnalyzer>());
                var result = await analyzer.AnalyzeErrorLogAsync();
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error analyzing error log");
                return Json(new { error = "Error analyzing error log" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> StreamLog(string logPath, int lastPosition = 0)
        {
            try
            {
                if (!System.IO.File.Exists(logPath))
                {
                    return Json(new { error = "Log file not found" });
                }

                using var fileStream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                if (lastPosition > fileStream.Length) lastPosition = 0;
                
                fileStream.Position = lastPosition;
                var buffer = new byte[4096];
                var bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length);
                
                var content = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                
                return Json(new { 
                    content = content,
                    position = fileStream.Position,
                    hasMore = fileStream.Position < fileStream.Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error streaming log");
                return Json(new { error = "Error streaming log" });
            }
        }
    }
}
