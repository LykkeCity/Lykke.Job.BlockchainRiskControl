using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainRiskControl.Domain.Repositories
{
    public interface IStatisticsRepository
    {
        Task<int> GetOperationsCountForTheLastPeriodAsync(TimeSpan period);
        Task<int> GetOperationsCountForTheLastPeriodAsync(TimeSpan period, Guid userId);
        Task<decimal> GetAggregatedAmountForTheLastPeriodAsync(TimeSpan period);
        Task<decimal> GetAggregatedAmountForTheLastPeriodAsync(TimeSpan period, Guid userId);
        Task RegisterOperationAsync(Operation operation);
    }
}
