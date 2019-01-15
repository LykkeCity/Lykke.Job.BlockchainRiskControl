using Lykke.Job.BlockchainRiskControl.Domain.RiskConstraintsConfiguration;
using Lykke.Job.BlockchainRiskControl.Domain.Services;

namespace Lykke.Job.BlockchainRiskControl.DomainServices
{
    public sealed class RiskConstraintsRegistryConfigurator : IRiskConstraintsRegistryConfigurator
    {
        private readonly IRiskConstraintsRegistry _constraintsRegistry;
        private readonly IRiskConstraintsFactory _riskConstraintsFactory;
        
        public RiskConstraintsRegistryConfigurator(
            IRiskConstraintsRegistry constraintsRegistry,
            IRiskConstraintsFactory constraintsFactory)
        {
            _constraintsRegistry = constraintsRegistry;
            _riskConstraintsFactory = constraintsFactory;  
        }

        public void Configure(RiskConstraintsGroupConfiguration groupConfiguration)
        {
            foreach (var constraintConfiguration in groupConfiguration.Constraints)
            {
                ConfigureConstraint(groupConfiguration, constraintConfiguration);
            }
        }

        private void ConfigureConstraint(
            RiskConstraintsGroupConfiguration groupConfiguration, 
            RiskConstraintConfiguration constraintConfiguration)
        {
            var r = _constraintsRegistry;
            var g = groupConfiguration;
            var constraint = _riskConstraintsFactory.Create(constraintConfiguration);

            if (g.OperationType.HasValue &&
                !string.IsNullOrWhiteSpace(g.BlockchainType) &&
                !string.IsNullOrWhiteSpace(g.BlockchainAssetId))
            {
                r.Add(g.BlockchainType, g.BlockchainAssetId, g.OperationType.Value, constraint);
            }
            else if (g.OperationType.HasValue &&
                     !string.IsNullOrWhiteSpace(g.BlockchainType))
            {
                _constraintsRegistry.Add(g.BlockchainType, g.OperationType.Value, constraint);
            }
            else if (g.OperationType.HasValue)
            {
                _constraintsRegistry.Add(g.OperationType.Value, constraint);
            }
            else if (!string.IsNullOrWhiteSpace(g.BlockchainType) &&
                     !string.IsNullOrWhiteSpace(g.BlockchainAssetId))
            {
                _constraintsRegistry.Add(g.BlockchainType, g.BlockchainAssetId, constraint);
            }
            else if (!string.IsNullOrWhiteSpace(g.BlockchainType))
            {
                _constraintsRegistry.Add(g.BlockchainType, constraint);
            }
            else
            {
                _constraintsRegistry.Add(constraint);
            }
        }
    }
}
