using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Lykke.Job.BlockchainRiskControl.Domain.Repositories;

namespace Lykke.Job.BlockchainRiskControl.Domain.RiskConstraints
{
    [DisplayName("MaxFrequency")]
    public sealed class MaxFrequencyRiskConstraint : IRiskConstraint
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly TimeSpan _period;
        private readonly int _maxOperationsCount;

        public MaxFrequencyRiskConstraint(
            IStatisticsRepository statisticsRepository,
            TimeSpan period, 
            int maxOperationsCount)
        {
            _statisticsRepository = statisticsRepository ?? throw new ArgumentNullException(nameof(statisticsRepository));

            if (period <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(period), period, "Period should be positive time interval");
            }

            if (maxOperationsCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxOperationsCount), maxOperationsCount, "Max operations count should be positive number");
            }

            _period = period;
            _maxOperationsCount = maxOperationsCount;
        }

        public async Task<RiskConstraintResolution> ApplyAsync(Operation operation)
        {
            var operationsCount = await _statisticsRepository.GetOperationsCountForTheLastPeriodAsync(_period);

            return operationsCount + 1 > _maxOperationsCount 
                ? RiskConstraintResolution.Violated($"Operations count {operationsCount} + 1 > {_maxOperationsCount} for the last {_period}") 
                : RiskConstraintResolution.Passed;
        }
    }
}
