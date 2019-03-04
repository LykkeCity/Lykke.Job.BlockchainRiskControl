using System.Threading.Tasks;

namespace Lykke.Job.BlockchainRiskControl.Domain
{
    public interface IRiskConstraint
    {
        Task<RiskConstraintResolution> ApplyAsync(
            string blockchainType,
            string blockchainAssetId,
            OperationType? operationType,
            Operation operation);
    }
}
