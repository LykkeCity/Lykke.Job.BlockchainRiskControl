using Lykke.Job.BlockchainRiskControl.Domain.RiskConstraintsConfiguration;

namespace Lykke.Job.BlockchainRiskControl.Domain.Services
{
    public interface IRiskConstraintsFactory
    {
        IRiskConstraint Create(RiskConstraintConfiguration configuration);
    }
}
