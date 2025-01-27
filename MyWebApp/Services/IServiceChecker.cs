using System.Threading.Tasks;
using MyWebApp.Models;

namespace MyWebApp.Services
{
    public interface IServiceChecker
    {
        Task<ServiceStatus> CheckStatus(ServiceConfig config);
    }
}
