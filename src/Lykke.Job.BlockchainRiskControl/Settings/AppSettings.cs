using Lykke.Job.BlockchainRiskControl.Settings.JobSettings;
using Lykke.Sdk.Settings;

namespace Lykke.Job.BlockchainRiskControl.Settings
{
    public class AppSettings : BaseAppSettings
    {
        public BlockchainRiskControlJobSettings BlockchainRiskControlJob { get; set; }
    }
}
