using System.Threading.Tasks;

namespace MyWebApp.Services
{
    public interface IServiceChecker
    {
        Task<ServiceStatus> CheckStatus(ServiceConfig config);
    }
}
