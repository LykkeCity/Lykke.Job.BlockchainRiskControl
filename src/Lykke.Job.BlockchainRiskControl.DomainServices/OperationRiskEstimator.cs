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

            foreach (var item in constraints)
            {
                var validationResult = await item.constraint.ApplyAsync(
                    item.blockchainType,
                    item.blockchainAssetId,
                    item.operationType,
                    operation);

                if (validationResult.IsViolated)
                {
                    violationsBuilder.AppendLine($"{item.constraint.GetConstraintName()}: {validationResult.Violation}");
                }
            }

            return violationsBuilder.Length > 0
                ? OperationRisk.ResolutionRequired(violationsBuilder.ToString())
                : OperationRisk.Reliable;
        }
    }
}
