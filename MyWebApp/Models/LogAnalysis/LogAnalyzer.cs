using System.Text.RegularExpressions;

namespace MyWebApp.Models.LogAnalysis
{
    public class LogAnalyzer
    {
        private readonly string _logPath;
        private readonly ILogger<LogAnalyzer> _logger;

        public LogAnalyzer(string logPath, ILogger<LogAnalyzer> logger)
        {
            _logPath = logPath;
            _logger = logger;
        }

        public IEnumerable<string> ReadLogFile()
        {
            if (!File.Exists(_logPath))
            {
                _logger.LogError($"Log file not found at path: {_logPath}");
                throw new FileNotFoundException($"Log file not found at path: {_logPath}");
            }
            return File.ReadLines(_logPath);
        }

        public async Task<LogAnalysisResult> AnalyzeRequestLogAsync()
        {
            var result = new LogAnalysisResult();
            try
            {
                var lines = await File.ReadAllLinesAsync(_logPath);
                foreach (var line in lines)
                {
                    if (line.Contains("HTTP"))
                    {
                        result.TotalRequests++;
                        if (line.Contains("GET")) result.GetRequests++;
                        if (line.Contains("POST")) result.PostRequests++;
                        if (line.Contains("200")) result.SuccessfulRequests++;
                        if (Regex.IsMatch(line, @"4\d{2}")) result.ClientErrors++;
                        if (Regex.IsMatch(line, @"5\d{2}")) result.ServerErrors++;
                    }
                }
                _logger.LogInformation($"Successfully analyzed request log at {_logPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error analyzing request log at {_logPath}");
                throw;
            }
            return result;
        }

        public async Task<ErrorLogAnalysisResult> AnalyzeErrorLogAsync()
        {
            var result = new ErrorLogAnalysisResult();
            try
            {
                var lines = await File.ReadAllLinesAsync(_logPath);
                foreach (var line in lines)
                {
                    if (line.Contains("ERROR", StringComparison.OrdinalIgnoreCase))
                    {
                        result.TotalErrors++;
                        if (line.Contains("NullReference")) result.NullReferenceErrors++;
                        if (line.Contains("ArgumentException")) result.ArgumentErrors++;
                        if (line.Contains("Database")) result.DatabaseErrors++;
                    }
                }
                _logger.LogInformation($"Successfully analyzed error log at {_logPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error analyzing error log at {_logPath}");
                throw;
            }
            return result;
        }

        public async Task<MySqlLogAnalysisResult> AnalyzeMySqlLogAsync()
        {
            var result = new MySqlLogAnalysisResult();
            try
            {
                var lines = await File.ReadAllLinesAsync(_logPath);
                foreach (var line in lines)
                {
                    if (line.Contains("Query", StringComparison.OrdinalIgnoreCase))
                    {
                        result.TotalQueries++;
                        if (line.Contains("SELECT")) result.SelectQueries++;
                        if (line.Contains("INSERT")) result.InsertQueries++;
                        if (line.Contains("UPDATE")) result.UpdateQueries++;
                        if (line.Contains("DELETE")) result.DeleteQueries++;
                    }

                    if (line.Contains("Error", StringComparison.OrdinalIgnoreCase))
                    {
                        result.QueryErrors++;
                    }
                }
                _logger.LogInformation($"Successfully analyzed MySQL log at {_logPath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error analyzing MySQL log at {_logPath}");
                throw;
            }
            return result;
        }
    }
}
