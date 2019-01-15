using Lykke.Job.BlockchainRiskControl.Domain.RiskConstraintsConfiguration;

namespace Lykke.Job.BlockchainRiskControl.Domain.Services
{
    public interface IRiskConstraintsRegistryConfigurator
    {
        void Configure(RiskConstraintsGroupConfiguration groupConfiguration);
    }
}
