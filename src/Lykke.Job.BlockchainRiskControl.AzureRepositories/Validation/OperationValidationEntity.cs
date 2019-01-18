using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.Job.BlockchainRiskControl.Domain;

namespace Lykke.Job.BlockchainRiskControl.AzureRepositories.Validation
{
    public class OperationValidationEntity : AzureTableEntity
    {
        public static string Partition(Guid operationId) => operationId.ToString();
        public static string Row() => string.Empty;

        public OperationValidationEntity()
        {
        }

        public OperationValidationEntity(OperationValidation operationValidation)
        {
            PartitionKey = Partition(operationValidation.Operation.Id);
            RowKey = Row();
            Version = operationValidation.Version;
            Operation = operationValidation.Operation;
            Risk = operationValidation.Risk;
            ValidationMoment = operationValidation.ValidationMoment;
            ResolutionMoment = operationValidation.ResolutionMoment;
            Resolution = operationValidation.Resolution;
        }

        public string Version { get; set; }

        [JsonValueSerializer]
        public Operation Operation { get; set; }
        
        [JsonValueSerializer]
        public OperationRisk Risk { get; set; }

        public DateTime ValidationMoment { get; set; }
        
        public DateTime? ResolutionMoment { get; set; }
        
        public OperationValidationResolution? Resolution { get; set; }

        public OperationValidation ToOperationValidation()
        {
            return Resolution.HasValue && ResolutionMoment.HasValue
                ? OperationValidation.CreateResolved(Operation, Risk, ValidationMoment, ResolutionMoment.Value, Resolution.Value)
                : OperationValidation.Create(Operation, Risk, ValidationMoment);
        }
    }
}