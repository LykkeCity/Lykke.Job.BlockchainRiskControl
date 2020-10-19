using System.Collections.Generic;
using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Job.BlockchainRiskControl.Contract;
using Lykke.Job.BlockchainRiskControl.Contract.Commands;
using Lykke.Job.BlockchainRiskControl.Contract.Events;
using Lykke.Job.BlockchainRiskControl.Settings;
using Lykke.Job.BlockchainRiskControl.Settings.JobSettings;
using Lykke.Job.BlockchainRiskControl.Workflow.CommandHandlers;
using Lykke.Job.BlockchainRiskControl.Workflow.Commands;
using Lykke.Job.BlockchainRiskControl.Workflow.Events;
using Lykke.Job.BlockchainRiskControl.Workflow.Sagas;
using Lykke.Messaging;
using Lykke.Messaging.RabbitMq;
using Lykke.Messaging.Serialization;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainRiskControl.Modules
{
    [UsedImplicitly]
    public class CqrsModule : Module
    {
        private readonly CqrsSettings _settings;
        private readonly IReloadingManager<AppSettings> _appSettings;

        public CqrsModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings.CurrentValue.BlockchainRiskControlJob.Cqrs;
            _appSettings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new AutofacDependencyResolver(context)).As<IDependencyResolver>().SingleInstance();

            RegisterMessageHandlers(builder);

            builder.Register(CreateEngine)
                .As<ICqrsEngine>()
                .SingleInstance()
                .AutoActivate();
        }

        private MessagingEngine RegisterMessagingEngine(IComponentContext ctx)
        {
            var logFactory = ctx.Resolve<ILogFactory>();
            var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory
            {
                Uri = _settings.RabbitConnectionString
            };
            var rabbitMqEndpoint = rabbitMqSettings.Endpoint.ToString();

            var messagingEngine = new MessagingEngine(logFactory,
                new TransportResolver(new Dictionary<string, TransportInfo>
                {
                    {
                        "RabbitMq",
                        new TransportInfo(
                            rabbitMqEndpoint,
                            rabbitMqSettings.UserName,
                            rabbitMqSettings.Password, "None", "RabbitMq")
                    }
                }),
                new RabbitMqTransportFactory(logFactory));

            return messagingEngine;
        }

        private static IEndpointResolver GetDefaultEndpointResolver()
        {
            return new RabbitMqConventionEndpointResolver(
                "RabbitMq",
                SerializationFormat.MessagePack,
                environment: "lykke");
        }

        private static IRegistration[] GetInterceptors()
        {
            return null;
        }

        private void RegisterMessageHandlers(ContainerBuilder builder)
        {
            // Sagas
            builder.RegisterType<OperationValidationSaga>()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.BlockchainRiskControlJob.Telegram?.ChatId));

            // Command handlers
            builder.RegisterType<ValidateOperationCommandsHandler>();
            builder.RegisterType<WaitForOperationResolutionCommandsHandler>()
                .WithParameter(TypedParameter.From(_settings.WaitForOperationResolutionRetryDelay));
            builder.RegisterType<RegisterOperationStatisticsCommandsHandler>();
        }

        private CqrsEngine CreateEngine(IComponentContext ctx)
        {
            var logFactory = ctx.Resolve<ILogFactory>();
            var messagingEngine = RegisterMessagingEngine(ctx);
            var defaultRetryDelay = (long)_settings.RetryDelay.TotalMilliseconds;

            const string defaultPipeline = "commands";
            const string defaultRoute = "self";
            const string eventsRoute = "events";

            var registration = new List<IRegistration>
            {
                Register.DefaultEndpointResolver(GetDefaultEndpointResolver()),
                Register.BoundedContext(BlockchainRiskControlBoundedContext.Name)
                    .FailedCommandRetryDelay(defaultRetryDelay)

                    .ListeningCommands(typeof(ValidateOperationCommand))
                    .On(defaultRoute)
                    .WithCommandsHandler<ValidateOperationCommandsHandler>()
                    .PublishingEvents(
                        typeof(OperationAcceptedEvent),
                        typeof(RiskyOperationDetectedEvent))
                    .With(defaultPipeline)

                    .ListeningCommands(typeof(WaitForOperationResolutionCommand))
                    .On(defaultRoute)
                    .WithCommandsHandler<WaitForOperationResolutionCommandsHandler>()
                    .PublishingEvents(
                        typeof(OperationAcceptedEvent),
                        typeof(OperationRejectedEvent))
                    .With(defaultPipeline)

                    .ListeningCommands(typeof(RegisterOperationStatisticsCommand))
                    .On(defaultRoute)
                    .WithCommandsHandler<RegisterOperationStatisticsCommandsHandler>()

                    .ProcessingOptions(defaultRoute).MultiThreaded(8).QueueCapacity(1024)
                    .ProcessingOptions(eventsRoute).MultiThreaded(8).QueueCapacity(1024),

                Register.Saga<OperationValidationSaga>($"{BlockchainRiskControlBoundedContext.Name}.saga")
                    .ListeningEvents(typeof(RiskyOperationDetectedEvent))
                    .From(BlockchainRiskControlBoundedContext.Name)
                    .On(defaultRoute)
                    .PublishingCommands(typeof(WaitForOperationResolutionCommand))
                    .To(BlockchainRiskControlBoundedContext.Name)
                    .With(defaultPipeline)

                    .ListeningEvents(typeof(OperationAcceptedEvent))
                    .From(BlockchainRiskControlBoundedContext.Name)
                    .On(defaultRoute)
                    .PublishingCommands(typeof(RegisterOperationStatisticsCommand))
                    .To(BlockchainRiskControlBoundedContext.Name)
                    .With(defaultPipeline)

                    .ProcessingOptions(defaultRoute).MultiThreaded(8).QueueCapacity(1024)
            };

            var interceptors = GetInterceptors();

            if (interceptors != null)
            {
                registration.AddRange(interceptors);
            }

            var cqrsEngine = new CqrsEngine(
                logFactory,
                new AutofacDependencyResolver(ctx.Resolve<IComponentContext>()),
                messagingEngine,
                new DefaultEndpointProvider(),
                true,
                registration.ToArray());

            return cqrsEngine;
        }
    }
}
