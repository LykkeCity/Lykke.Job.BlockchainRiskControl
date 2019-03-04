using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.Job.BlockchainRiskControl.Domain;

namespace Lykke.Job.BlockchainRiskControl.AzureRepositories.Validation
{
    internal class OperationValidationEntity : AzureTableEntity
    {
        public static string Partition(Guid operationId) => operationId.ToString();
        public static string Row() => string.Empty;

        public string Version { get; set; }

        [JsonValueSerializer]
        public OperationEntity Operation { get; set; }

        [JsonValueSerializer]
        public OperationRiskEntity Risk { get; set; }

        public DateTime ValidationMoment { get; set; }

        public DateTime? ResolutionMoment { get; set; }

        public OperationValidationResolution? Resolution { get; set; }

        public OperationValidationEntity()
        {
            // default ctor for deserialization
        }

        public OperationValidationEntity(OperationValidation operationValidation)
        {
            PartitionKey = Partition(operationValidation.Operation.Id);
            RowKey = Row();
            Version = operationValidation.Version;
            Operation = operationValidation.Operation == null ? null : new OperationEntity(operationValidation.Operation);
            Risk = operationValidation.Risk == null ? null : new OperationRiskEntity(operationValidation.Risk);
            ValidationMoment = operationValidation.ValidationMoment;
            ResolutionMoment = operationValidation.ResolutionMoment;
            Resolution = operationValidation.Resolution;
        }

        public OperationValidation ToOperationValidation()
        {
            var operation = Operation?.ToOperation();
            var risk = Risk?.ToRisk();

            return Resolution.HasValue && ResolutionMoment.HasValue
                ? OperationValidation.CreateResolved(operation, risk, ValidationMoment, ResolutionMoment.Value, Resolution.Value)
                : OperationValidation.Create(operation, risk, ValidationMoment);
        }
    }
}