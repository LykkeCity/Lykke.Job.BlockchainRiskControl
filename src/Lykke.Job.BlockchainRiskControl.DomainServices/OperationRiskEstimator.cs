using System.Text;
using System.Threading.Tasks;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.Services;

namespace Lykke.Job.BlockchainRiskControl.DomainServices
{
    public sealed class OperationRiskEstimator : IOperationRiskEstimator
    {
        private readonly IRiskConstraintsRegistry _constraintsRegistry;

        public OperationRiskEstimator(IRiskConstraintsRegistry constraintsRegistry)
        {
            _constraintsRegistry = constraintsRegistry;
        }

        public async Task<OperationRisk> ValidateAsync(Operation operation)
        {
            var constraints = _constraintsRegistry.GetConstraints(operation.BlockchainType, operation.BlockchainAssetId, operation.Type);
            var violationsBuilder = new StringBuilder();

            foreach (var constraint in constraints)
            {
                var validationResult = await constraint.ApplyAsync(operation);

                if (validationResult.IsViolated)
                {
                    violationsBuilder.AppendLine($"{constraint.GetConstraintName()}: {validationResult.Violation}");
                }
            }

            return violationsBuilder.Length > 0
                ? OperationRisk.ResolutionRequired(violationsBuilder.ToString())
                : OperationRisk.Reliable;
        }
    }
}
