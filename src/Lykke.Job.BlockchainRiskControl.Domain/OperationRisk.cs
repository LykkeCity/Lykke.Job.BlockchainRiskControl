namespace Lykke.Job.BlockchainRiskControl.Domain
{
    public sealed class OperationRisk
    {
        public static readonly OperationRisk Reliable = new OperationRisk(null, OperationRiskLevel.Reliable);

        public string Violations { get; }

        public OperationRiskLevel Level { get; }

        public bool IsReliable => Level == OperationRiskLevel.Reliable;
        public bool IsResolutionRequired => Level == OperationRiskLevel.ResolutionRequired;

        private OperationRisk(string violations, OperationRiskLevel level)
        {
            Violations = violations;
            Level = level;
        }

        public static OperationRisk ResolutionRequired(string violations)
        {
            return new OperationRisk(violations, OperationRiskLevel.ResolutionRequired);
        }
    }
}
