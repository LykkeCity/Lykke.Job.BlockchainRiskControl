using System;
using System.ComponentModel;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Lykke.Job.BlockchainRiskControl.Domain.RiskConstraints
{
    [DisplayName("MaxAmount")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class MaxAmountRiskConstraint : IRiskConstraint
    {
        private readonly decimal _maxAmount;

        public MaxAmountRiskConstraint(decimal maxAmount)
        {
            if (maxAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAmount), maxAmount, "Max amount should be positive number");
            }

            _maxAmount = maxAmount;
        }

        public Task<RiskConstraintResolution> ApplyAsync(Operation operation)
        {
            var resolution = operation.Amount > _maxAmount 
                ? RiskConstraintResolution.Violated($"Operation amount {operation.Amount} > {_maxAmount}") 
                : RiskConstraintResolution.Passed;

            return Task.FromResult(resolution);
        }
    }
}
