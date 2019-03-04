using System;
using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainRiskControl.Settings.JobSettings
{
    [UsedImplicitly]
    public class CqrsSettings
    {
        [AmqpCheck]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string RabbitConnectionString { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public TimeSpan RetryDelay { get; set; }

        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public TimeSpan WaitForOperationResolutionRetryDelay { get; set; }
    }
}
