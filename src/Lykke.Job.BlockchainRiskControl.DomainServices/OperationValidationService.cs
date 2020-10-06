using System;
using System.Threading.Tasks;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.Repositories;
using Lykke.Job.BlockchainRiskControl.Domain.Services;

namespace Lykke.Job.BlockchainRiskControl.DomainServices
{
    public sealed class OperationValidationService : IOperationValidationService
    {
        private readonly IOperationValidationRepository _validationRepository;
        private readonly IOperationRiskEstimator _operationRiskEstimator;

        public OperationValidationService(
            IOperationValidationRepository validationRepository,
            IOperationRiskEstimator operationRiskEstimator)
        {
            _validationRepository = validationRepository;
            _operationRiskEstimator = operationRiskEstimator;
        }

        public async Task<OperationValidation> ValidateAsync(Operation operation)
        {
            var validation = await _validationRepository.TryGetAsync(operation.Id);

            if (validation != null)
            {
                return validation;
            }

            var risk = await _operationRiskEstimator.ValidateAsync(operation);

            validation = OperationValidation.Create(operation, risk);

            await _validationRepository.AddAsync(validation);

            return validation;
        }

        public async Task<OperationValidationResolution> GetResolutionAsync(Guid operationId)
        {
            var validation = await _validationRepository.TryGetAsync(operationId);

            if (validation == null)
            {
                throw new InvalidOperationException($"Validation of operation {operationId} is not found");
            }

            if (!validation.Risk.IsResolutionRequired)
            {
                throw new InvalidOperationException($"Resolution is not required, since operation risk level is {validation.Risk.Level}");
            }

            // ReSharper disable PossibleInvalidOperationException
            return validation.Resolution.Value;
            // ReSharper restore PossibleInvalidOperationException
        }
    }
}
