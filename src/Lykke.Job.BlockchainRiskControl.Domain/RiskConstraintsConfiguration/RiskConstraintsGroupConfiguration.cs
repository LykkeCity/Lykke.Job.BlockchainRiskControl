using System.Collections.Generic;

namespace Lykke.Job.BlockchainRiskControl.Domain.RiskConstraintsConfiguration
{
    public sealed class RiskConstraintsGroupConfiguration
    {
        public OperationType? OperationType { get; }
        public string BlockchainType { get; }
        public string BlockchainAssetId { get; }
        public IReadOnlyCollection<RiskConstraintConfiguration> Constraints { get; }

        public RiskConstraintsGroupConfiguration(
            OperationType? operationType, 
            string blockchainType, 
            string blockchainAssetId, 
            IReadOnlyCollection<RiskConstraintConfiguration> constraints)
        {
            OperationType = operationType;
            BlockchainType = blockchainType;
            BlockchainAssetId = blockchainAssetId;
            Constraints = constraints;
        }
    }
}
