using System;
using System.Threading.Tasks;
using Lykke.Job.BlockchainRiskControl.Domain.Repositories;
using Lykke.Job.BlockchainRiskControl.Domain.Services;

namespace Lykke.Job.BlockchainRiskControl.DomainServices
{
    public sealed class StatisticsService : IStatisticsService
    {
        private readonly IOperationValidationRepository _validationRepository;
        private readonly IStatisticsRepository _statisticsRepository;

        public StatisticsService(
            IOperationValidationRepository validationRepository,
            IStatisticsRepository statisticsRepository)
        {
            _validationRepository = validationRepository;
            _statisticsRepository = statisticsRepository;
        }

        public async Task RegisterStatisticsAsync(Guid operationId)
        {
            var validation = await _validationRepository.TryGetAsync(operationId);

            if (validation == null)
            {
                throw new InvalidOperationException($"Operation validation {operationId} to get statistics has not been found.");
            }

            await _statisticsRepository.RegisterOperationAsync(validation.Operation);
        }
    }
}
