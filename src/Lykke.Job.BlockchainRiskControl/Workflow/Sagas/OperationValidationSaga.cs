using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Cqrs;
using Lykke.Job.BlockchainRiskControl.Contract;
using Lykke.Job.BlockchainRiskControl.Contract.Events;
using Lykke.Job.BlockchainRiskControl.Domain;
using Lykke.Job.BlockchainRiskControl.Workflow.Commands;
using Lykke.Job.BlockchainRiskControl.Workflow.Events;
using Telegram.Bot;

namespace Lykke.Job.BlockchainRiskControl.Workflow.Sagas
{
    public class OperationValidationSaga
    {
        private readonly ITelegramBotClient _telegramBot;
        private readonly string _chatId;

        public OperationValidationSaga(
            ITelegramBotClient telegramBot,
            string chatId)
        {
            _telegramBot = telegramBot;
            _chatId = chatId;
        }

        [UsedImplicitly]
        public async Task Handle(RiskyOperationDetectedEvent evt, ICommandSender sender)
        {
            switch (evt.RiskLevel)
            {
                case OperationRiskLevel.ResolutionRequired:
                    if (!string.IsNullOrEmpty(_chatId))
                        await _telegramBot.SendTextMessageAsync(_chatId, $"Operation requires for manual confirmation (operationId = {evt.OperationId})");

                    sender.SendCommand
                    (
                        new WaitForOperationResolutionCommand
                        {
                            OperationId = evt.OperationId
                        },
                        BlockchainRiskControlBoundedContext.Name
                    );

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(evt.RiskLevel), evt.RiskLevel, null);
            }
        }

        [UsedImplicitly]
        public Task Handle(OperationAcceptedEvent evt, ICommandSender sender)
        {
            sender.SendCommand
            (
                new RegisterOperationStatisticsCommand
                {
                    OperationId = evt.OperationId
                },
                BlockchainRiskControlBoundedContext.Name
            );

            return Task.CompletedTask;
        }
    }
}
