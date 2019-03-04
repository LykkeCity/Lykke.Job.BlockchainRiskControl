using System.Threading.Tasks;
using Common.Log;
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

        public StartupManager(
            ILogFactory logFactory,
            RiskConstrainsInitializer riskConstrainsInitializer,
            ICqrsEngine cqrsEngine,
            StatisticsRepository statisticsRepository)
        {
            _log = logFactory.CreateLog(this);

            _riskConstrainsInitializer = riskConstrainsInitializer;
            _cqrsEngine = cqrsEngine;
            _statisticsRepository = statisticsRepository;
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
        }
    }
}
