using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Domain.Repositories;
using Lykke.Job.BlockchainRiskControl.Domain.RiskConstraints;
using Lykke.Job.BlockchainRiskControl.Domain.RiskConstraintsConfiguration;
using Lykke.Job.BlockchainRiskControl.DomainServices;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Lykke.Job.BlockchainRiskControl.Tests.Unit
{
    public class RiskConstrainsFactoryTests
    {
        [Fact]
        public async Task Test_that_constraint_without_dependencies_with_parameters_can_be_created()
        {
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var factory = new RiskConstraintsFactory(serviceProvider);

            var constraint = factory.Create(new RiskConstraintConfiguration("MaxAmount", new Dictionary<string, string>
            {
                {"maxAmount", "10"}
            }));

            Assert.IsType<MaxAmountRiskConstraint>(constraint);

            var resolution1 = await constraint.ApplyAsync(
                string.Empty,
                string.Empty,
                OperationType.Deposit,
                new Operation(Guid.Empty, OperationType.Deposit, Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 10));

            var resolution2 = await constraint.ApplyAsync(
                string.Empty,
                string.Empty,
                OperationType.Deposit,
                new Operation(Guid.Empty, OperationType.Deposit, Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 11));

            Assert.False(resolution1.IsViolated);
            Assert.True(resolution2.IsViolated);
        }

        [Fact]
        public async Task Test_that_constraint_with_dependencies_with_parameters_can_be_created()
        {
            var statisticsRepositoryMock = new Mock<IStatisticsRepository>();

            statisticsRepositoryMock
                .Setup(x => x.GetOperationsCountForTheLastPeriodAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<OperationType?>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync<string, string, OperationType?, TimeSpan, IStatisticsRepository, long>((b, a, t, period) => 5);

            var services = new ServiceCollection();

            services.AddSingleton(statisticsRepositoryMock.Object);

            var serviceProvider = services.BuildServiceProvider();
            var factory = new RiskConstraintsFactory(serviceProvider);

            var constraint1 = factory.Create(new RiskConstraintConfiguration("MaxFrequency", new Dictionary<string, string>
            {
                {"period", "00:10:00"},
                {"maxOperationsCount", "5"}
            }));

            var constraint2 = factory.Create(new RiskConstraintConfiguration("MaxFrequency", new Dictionary<string, string>
            {
                {"period", "00:15:00"},
                {"maxOperationsCount", "6"}
            }));

            var resolution1 = await constraint1.ApplyAsync(
                string.Empty,
                string.Empty,
                OperationType.Deposit,
                new Operation(Guid.Empty, OperationType.Deposit, Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 1));

            var resolution2 = await constraint2.ApplyAsync(
                string.Empty,
                string.Empty,
                OperationType.Deposit,
                new Operation(Guid.Empty, OperationType.Deposit, Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 1));

            Assert.IsType<MaxFrequencyRiskConstraint>(constraint1);
            Assert.IsType<MaxFrequencyRiskConstraint>(constraint2);

            Assert.True(resolution1.IsViolated);
            Assert.False(resolution2.IsViolated);

            statisticsRepositoryMock.Verify(
                x => x.GetOperationsCountForTheLastPeriodAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<OperationType?>(), It.Is<TimeSpan>(v => v == TimeSpan.FromMinutes(10))),
                Times.Once);
            statisticsRepositoryMock.Verify(
                x => x.GetOperationsCountForTheLastPeriodAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<OperationType?>(), It.Is<TimeSpan>(v => v == TimeSpan.FromMinutes(15))),
                Times.Once);
        }
    }
}
