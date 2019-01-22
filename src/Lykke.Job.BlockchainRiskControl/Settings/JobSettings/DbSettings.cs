using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainRiskControl.Settings.JobSettings
{
    [UsedImplicitly]
    public class DbSettings
    {
        [AzureTableCheck]
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public string LogsConnString { get; set; }

        [AzureTableCheck]
        public string AzureDataConnString { get; set; }

        [MongoCheck]
        public string MongoDataConnString { get; set; }
    }
}
