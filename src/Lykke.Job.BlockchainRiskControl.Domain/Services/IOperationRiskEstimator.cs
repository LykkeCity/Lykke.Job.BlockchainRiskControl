using System.Threading.Tasks;

namespace Lykke.Job.BlockchainRiskControl.Domain.Services
{
    public interface IOperationRiskEstimator
    {
        Task<OperationRisk> ValidateAsync(Operation operation);
    }
}
