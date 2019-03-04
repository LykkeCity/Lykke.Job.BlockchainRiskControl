using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainRiskControl.Domain.Services
{
    public interface IStatisticsService
    {
        Task RegisterStatisticsAsync(Guid operationId);
    }
}
