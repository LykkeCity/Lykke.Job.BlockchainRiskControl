using System;
using System.ComponentModel;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Job.BlockchainRiskControl.Domain.Repositories;

namespace Lykke.Job.BlockchainRiskControl.Domain.RiskConstraints
{
    [DisplayName("MaxAggregatedAmount")]
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    public sealed class MaxAggregatedAmountRiskConstraint : IRiskConstraint
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly TimeSpan _period;
        private readonly decimal _maxAmount;

        public MaxAggregatedAmountRiskConstraint(
            IStatisticsRepository statisticsRepository,
            TimeSpan period,
            decimal maxAmount)
        {
            _statisticsRepository = statisticsRepository ?? throw new ArgumentNullException(nameof(statisticsRepository));

            if (period <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(period), period, "Period should be positive time interval");
            }

            if (maxAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAmount), maxAmount, "Max amount should be positive number");
            }

            _period = period;
            _maxAmount = maxAmount;
        }

        public async Task<RiskConstraintResolution> ApplyAsync(Operation operation)
        {
            var aggregatedAmount = await _statisticsRepository.GetAggregatedAmountForTheLastPeriodAsync(_period);

            return aggregatedAmount + operation.Amount > _maxAmount 
                ? RiskConstraintResolution.Violated($"Aggregated operations amount {aggregatedAmount} + {operation.Amount} > {_maxAmount} for the last {_period}") 
                : RiskConstraintResolution.Passed;
        }
    }
}
