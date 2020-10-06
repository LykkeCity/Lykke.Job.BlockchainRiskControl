using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Grpc.Core;
using Lykke.Common.Log;
using Lykke.Cqrs;
using Lykke.Job.BlockchainRiskControl.AzureRepositories.Statistics;
using Lykke.Sdk;

namespace Lykke.Job.BlockchainRiskControl.Services
{
    public class StartupManager : IStartupManager
    {
        private readonly ILog _log;
        private readonly RiskConstrainsInitializer _riskConstrainsInitializer;
        private readonly ICqrsEngine _cqrsEngine;
        private readonly StatisticsRepository _statisticsRepository;
        private readonly Server _grpcServer;

        public StartupManager(
            ILogFactory logFactory,
            RiskConstrainsInitializer riskConstrainsInitializer,
            ICqrsEngine cqrsEngine,
            StatisticsRepository statisticsRepository,
            Server grpcServer)
        {
            _log = logFactory.CreateLog(this);

            _riskConstrainsInitializer = riskConstrainsInitializer;
            _cqrsEngine = cqrsEngine;
            _statisticsRepository = statisticsRepository;
            _grpcServer = grpcServer;
        }

        public async Task StartAsync()
        {
            _log.Info(nameof(StartAsync), "Initialize risk constraints...");

            _riskConstrainsInitializer.Initialize();

            _log.Info(nameof(StartAsync), "Checking statistics indexes...");

            await _statisticsRepository.CheckIndexesAsync();

            _log.Info(nameof(StartAsync), "Starting CQRS engine publishers...");

            _cqrsEngine.StartPublishers();

            _log.Info(nameof(StartAsync), "Starting CQRS engine subscribers...");

            _cqrsEngine.StartSubscribers();

            _grpcServer.Start();

            _log.Info($"Grpc server listening on: {_grpcServer.Ports.First().Host}:{_grpcServer.Ports.First().Port}");
        }
    }
}
