using System;

namespace Lykke.Job.BlockchainRiskControl.Mapping
{
    public static class OperationTypeMappingExtensions
    {
        public static Domain.OperationType ToDomain(this Contract.OperationType value)
        {
            switch (value)
            {
                case Contract.OperationType.Deposit:
                    return Domain.OperationType.Deposit;
                
                case Contract.OperationType.Withdrawal:
                    return Domain.OperationType.Withdrawal;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}
