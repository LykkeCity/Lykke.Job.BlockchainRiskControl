namespace Lykke.Job.BlockchainRiskControl.Domain
{
    public sealed class RiskConstraintResolution
    {
        public static readonly RiskConstraintResolution Passed = new RiskConstraintResolution(null);

        public string Violation { get; }
        
        public bool IsViolated => !string.IsNullOrWhiteSpace(Violation);

        private RiskConstraintResolution(string violation)
        {
            Violation = violation;
        }
        
        public static RiskConstraintResolution Violated(string violation)
        {
            return new RiskConstraintResolution(violation);
        }
    }
}
