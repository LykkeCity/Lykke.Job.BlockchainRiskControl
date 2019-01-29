using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Job.BlockchainRiskControl.AzureRepositories.Statistics;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.Repositories;
using Lykke.Logs;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace Lykke.Job.BlockchainRiskControl.Tests.Integration
{
    public class StatisticsRepositoryTests
    {
        private const string SKIP = "Should not be executed by TeamCity";
        private readonly Guid _user1 = Guid.NewGuid();
        private readonly Guid _user2 = Guid.NewGuid();
        private readonly IStatisticsRepository _statistics;
        private readonly List<Operation> _operations = new List<Operation>();
        private readonly DateTime _startedAt = DateTime.UtcNow;
        private readonly ITestOutputHelper _output;

        public StatisticsRepositoryTests(ITestOutputHelper output)
        {
            dynamic settings = JsonConvert.DeserializeObject(File.ReadAllText("../../../appsettings.json"));
            _statistics = new StatisticsRepository((string)settings.BlockchainRiskControlJob.Db.MongoDataConnString,  LogFactory.Create());
            _output = output;
        }

        private async Task Arrange()
        {
            // wait a bit to ensure we fall into period
            Thread.Sleep(500);

            for (var i = 1; i <= 10; i++)
            {
                var op = new Operation(Guid.NewGuid(), OperationType.Deposit, i < 4 ? _user1 : _user2, "TEST", "TEST", "test1", "test2", i/100M);
                var stopwatch = Stopwatch.StartNew();
                await _statistics.RegisterOperationAsync(op);
                stopwatch.Stop();
                _output.WriteLine($"{op.Id}\t{stopwatch.Elapsed}");
                _operations.Add(op);
            };
        }

        [Fact(Skip = SKIP)]
        public async Task ShouldCalculateAggregatedAmountForTheLastPeriod()
        {
            // Arrange

            await Arrange();

            // Act

            var stopwatch = Stopwatch.StartNew();
            var amount = await _statistics.GetAggregatedAmountForTheLastPeriodAsync(DateTime.UtcNow - _startedAt);
            stopwatch.Stop();
            _output.WriteLine($"{nameof(IStatisticsRepository.GetAggregatedAmountForTheLastPeriodAsync)}\t{stopwatch.Elapsed}");

            // Assert

            Assert.Equal(_operations.Sum(op => op.Amount), amount, 2);
        }

        [Fact(Skip = SKIP)]
        public async Task ShouldCalculateAggregatedAmountForTheLastPeriodAndUser()
        {
            // Arrange

            await Arrange();

            // Act

            var stopwatch = Stopwatch.StartNew();
            var amount = await _statistics.GetAggregatedAmountForTheLastPeriodAsync(DateTime.UtcNow - _startedAt, _user2);
            stopwatch.Stop();
            _output.WriteLine($"{nameof(IStatisticsRepository.GetAggregatedAmountForTheLastPeriodAsync)}(userId)\t{stopwatch.Elapsed}");

            // Assert

            Assert.Equal(_operations.Where(op => op.UserId == _user2).Sum(op => op.Amount), amount, 2);
        }

        [Fact(Skip = SKIP)]
        public async Task ShouldCalculateOperationsCountForTheLastPeriod()
        {
            // Arrange

            await Arrange();

            // Act

            var stopwatch = Stopwatch.StartNew();
            var count = await _statistics.GetOperationsCountForTheLastPeriodAsync(DateTime.UtcNow - _startedAt);
            stopwatch.Stop();
            _output.WriteLine($"{nameof(IStatisticsRepository.GetOperationsCountForTheLastPeriodAsync)}\t{stopwatch.Elapsed}");

            // Assert

            Assert.Equal(_operations.Count(), count);
        }

        [Fact(Skip = SKIP)]
        public async Task ShouldCalculateOperationsCountForTheLastPeriodAndUser()
        {
            // Arrange

            await Arrange();

            // Act

            var stopwatch = Stopwatch.StartNew();
            var count = await _statistics.GetOperationsCountForTheLastPeriodAsync(DateTime.UtcNow - _startedAt, _user1);
            stopwatch.Stop();
            _output.WriteLine($"{nameof(IStatisticsRepository.GetOperationsCountForTheLastPeriodAsync)}(userId)\t{stopwatch.Elapsed}");

            // Assert

            Assert.Equal(_operations.Count(op => op.UserId == _user1), count);
        }
    }
}