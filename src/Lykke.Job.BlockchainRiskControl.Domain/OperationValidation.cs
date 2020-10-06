using System;

namespace Lykke.Job.BlockchainRiskControl.Domain
{
    public sealed class OperationValidation
    {
        public string Version { get; }
        public Operation Operation { get; }
        public OperationRisk Risk { get; }
        public DateTime ValidationMoment { get; }

        public DateTime? ResolutionMoment { get; private set; }
        public OperationValidationResolution? Resolution { get; private set; }

        public bool IsResolved => Risk.IsResolutionRequired &&
                                  Resolution != OperationValidationResolution.Unconfirmed;

        private OperationValidation(
            string version,
            Operation operation,
            OperationRisk risk,
            DateTime validationMoment,
            DateTime? resolutionMoment,
            OperationValidationResolution? resolution)
        {
            Version = version;
            Operation = operation;
            Risk = risk;
            ValidationMoment = validationMoment;
            Resolution = resolution;
        }

        public static OperationValidation Create(Operation operation, OperationRisk risk, DateTime? validationMoment = null)
        {
            return new OperationValidation
            (
                version: null,
                operation: operation,
                risk: risk,
                validationMoment: validationMoment ?? DateTime.UtcNow,
                resolutionMoment: null,
                resolution: risk.IsResolutionRequired
                    ? (OperationValidationResolution?)OperationValidationResolution.Unconfirmed
                    : null
            );
        }

        public static OperationValidation CreateResolved(
            Operation operation,
            OperationRisk risk,
            DateTime validationMoment,
            DateTime resolutionMoment,
            OperationValidationResolution resolution)
        {
            return new OperationValidation
            (
                version: null,
                operation: operation,
                risk: risk,
                validationMoment: validationMoment,
                resolutionMoment: resolutionMoment,
                resolution: resolution
            );
        }

        public void Resolve(OperationValidationResolution resolution)
        {
            if (!Risk.IsResolutionRequired)
            {
                throw new InvalidOperationException($"Resolution is not required, since operation risk level is {Risk.Level}");
            }

            if (IsResolved)
            {
                if (Resolution != resolution)
                {
                    throw new InvalidOperationException($"Validation already was resolved as {Resolution} and can't be changed to {resolution}");
                }
            }
            else
            {
                Resolution = resolution;
                ResolutionMoment = DateTime.UtcNow;
            }
        }
    }
}
