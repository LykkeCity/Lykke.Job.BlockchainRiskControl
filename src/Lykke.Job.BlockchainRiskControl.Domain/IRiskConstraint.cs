using System.Threading.Tasks;

namespace Lykke.Job.BlockchainRiskControl.Domain
{
    public interface IRiskConstraint
    {
        Task<RiskConstraintResolution> ApplyAsync(Operation operation);
    }
}
