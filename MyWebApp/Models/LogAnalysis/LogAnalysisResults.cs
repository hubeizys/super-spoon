namespace MyWebApp.Models.LogAnalysis
{
    public class LogAnalysisResult
    {
        public int TotalRequests { get; set; }
        public int GetRequests { get; set; }
        public int PostRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int ClientErrors { get; set; }
        public int ServerErrors { get; set; }
    }

    public class ErrorLogAnalysisResult
    {
        public int TotalErrors { get; set; }
        public int NullReferenceErrors { get; set; }
        public int ArgumentErrors { get; set; }
        public int DatabaseErrors { get; set; }
    }

    public class MySqlLogAnalysisResult
    {
        public int TotalQueries { get; set; }
        public int SelectQueries { get; set; }
        public int InsertQueries { get; set; }
        public int UpdateQueries { get; set; }
        public int DeleteQueries { get; set; }
        public int QueryErrors { get; set; }
    }
}
