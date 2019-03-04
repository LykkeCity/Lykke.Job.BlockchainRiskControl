using Lykke.Job.BlockchainRiskControl.Domain;

namespace Lykke.Job.BlockchainRiskControl.AzureRepositories.Validation
{
    internal class OperationRiskEntity
    {
        public string Violations { get; set; }
        public OperationRiskLevel Level { get; set; }

        public OperationRiskEntity()
        {
            // default ctor for deserialization
        }

        public OperationRiskEntity(OperationRisk risk)
        {
            Violations = risk.Violations;
            Level = risk.Level;
        }

        public OperationRisk ToRisk()
        {
            return Level == OperationRiskLevel.Reliable
                ? OperationRisk.Reliable
                : OperationRisk.ResolutionRequired(Violations);
        }
    }
}