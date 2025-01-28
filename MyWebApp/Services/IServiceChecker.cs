using System.Threading.Tasks;
using MyWebApp.Models;

namespace MyWebApp.Services
{
    public interface IServiceChecker
    {
        string ServiceType { get; }
        Task<ServiceStatus> CheckStatus(ServiceConfig config);
    }
}
