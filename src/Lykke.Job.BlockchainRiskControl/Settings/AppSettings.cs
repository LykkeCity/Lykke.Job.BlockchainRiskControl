using JetBrains.Annotations;
using Lykke.Job.BlockchainRiskControl.Settings.JobSettings;
using Lykke.Sdk.Settings;

namespace Lykke.Job.BlockchainRiskControl.Settings
{
    [UsedImplicitly]
    public class AppSettings : BaseAppSettings
    {
        [UsedImplicitly(ImplicitUseKindFlags.Assign)]
        public BlockchainRiskControlJobSettings BlockchainRiskControlJob { get; set; }
    }
}
