using Autofac;
using JetBrains.Annotations;
using Lykke.Job.BlockchainRiskControl.AzureRepositories.Statistics;
using Lykke.Job.BlockchainRiskControl.AzureRepositories.Validation;
using Lykke.Job.BlockchainRiskControl.Domain.Repositories;
using Lykke.Job.BlockchainRiskControl.Domain.Services;
using Lykke.Job.BlockchainRiskControl.DomainServices;
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
        private readonly IReloadingManager<BlockchainRiskControlJobSettings> _settings;

        public JobModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings.Nested(x => x.BlockchainRiskControlJob);
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.ConstraintsGroups))
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .SingleInstance();

            builder.RegisterType<RiskConstrainsInitializer>()
                .AsSelf();

            builder.RegisterType<OperationValidationRepository>()
                .As<IOperationValidationRepository>()
                .WithParameter(TypedParameter.From(_settings.ConnectionString(x => x.Db.AzureDataConnString)))
                .SingleInstance();

            builder.RegisterType<StatisticsRepository>()
                .As<IStatisticsRepository>()
                .AsSelf()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.Db.MongoDataConnString))
                .SingleInstance();

            builder.RegisterType<OperationValidationService>()
                .As<IOperationValidationService>()
                .SingleInstance();

            builder.RegisterType<StatisticsService>()
                .As<IStatisticsService>()
                .SingleInstance();

            builder.RegisterType<OperationRiskEstimator>()
                .As<IOperationRiskEstimator>()
                .SingleInstance();

            builder.RegisterType<RiskConstraintsRegistry>()
                .As<IRiskConstraintsRegistry>()
                .SingleInstance();

            builder.RegisterType<RiskConstraintsRegistryConfigurator>()
                .As<IRiskConstraintsRegistryConfigurator>()
                .SingleInstance();

            builder.RegisterType<RiskConstraintsFactory>()
                .As<IRiskConstraintsFactory>()
                .SingleInstance();
        }
    }
}
