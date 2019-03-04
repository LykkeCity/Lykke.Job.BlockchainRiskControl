using System;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainRiskControl.Domain.Repositories
{
    public interface IStatisticsRepository
    {
        Task<long> GetOperationsCountForTheLastPeriodAsync(
            string blockchainType,
            string blockchainAssetId,
            OperationType? operationType,
            TimeSpan period);

        Task<long> GetOperationsCountForTheLastPeriodAsync(
            string blockchainType,
            string blockchainAssetId,
            OperationType? operationType,
            TimeSpan period,
            Guid userId);

        Task<decimal> GetAggregatedAmountForTheLastPeriodAsync(
            string blockchainType,
            string blockchainAssetId,
            OperationType? operationType,
            TimeSpan period);

        Task<decimal> GetAggregatedAmountForTheLastPeriodAsync(
            string blockchainType,
            string blockchainAssetId,
            OperationType? operationType,
            TimeSpan period,
            Guid userId);
            
        Task RegisterOperationAsync(Operation operation);
    }
}
