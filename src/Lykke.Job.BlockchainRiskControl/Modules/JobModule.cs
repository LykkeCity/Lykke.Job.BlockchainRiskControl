using Autofac;
using JetBrains.Annotations;
using Lykke.Job.BlockchainRiskControl.Services;
using Lykke.Job.BlockchainRiskControl.Settings;
using Lykke.Job.BlockchainRiskControl.Settings.JobSettings;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainRiskControl.Modules
{
    [UsedImplicitly]
    public class JobModule : Module
    {
        private readonly BlockchainRiskControlJobSettings _settings;

        public JobModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings.CurrentValue.BlockchainRiskControlJob;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .WithParameter(TypedParameter.From(_settings.ConstraintsGroups))
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .SingleInstance();

            builder.RegisterType<RiskConstrainsInitializer>()
                .AsSelf();

            // TODO: register services and repositories
        }
    }
}
