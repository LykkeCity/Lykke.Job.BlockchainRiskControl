using System.Collections.Generic;

namespace Lykke.Job.BlockchainRiskControl.Domain.RiskConstraintsConfiguration
{
    public sealed class RiskConstraintConfiguration
    {
        public string ConstraintName { get; }
        public IReadOnlyDictionary<string, string> Parameters { get; }

        public RiskConstraintConfiguration(
            string constraintName, 
            IReadOnlyDictionary<string, string> parameters)
        {
            ConstraintName = constraintName;
            Parameters = parameters;
        }
    }
}
