using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using VoiceTexterBot.Services;

namespace VoiceTexterBot.Controllers
{
    public class InlineKeyboardController
    {
        private readonly IStorage _memoryStorage;
        private readonly ITelegramBotClient _telegramClient;

        public InlineKeyboardController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(CallbackQuery? callbackQuery, CancellationToken ct)
        {
            if (callbackQuery?.Data == null)
                return;

            if (callbackQuery.Data == "ru" || callbackQuery.Data == "en")
            {
                _memoryStorage.GetSession(callbackQuery.From.Id).LanguageCode = callbackQuery.Data;

                // Генерим информационное сообщение
                string languageText = callbackQuery.Data switch
                {
                    "ru" => " Русский",
                    "en" => " Английский",
                    _ => String.Empty
                };

                // Отправляем в ответ уведомление о выборе
                await _telegramClient.SendMessage(callbackQuery.From.Id,
                    $"<b>Язык аудио - {languageText}.{Environment.NewLine}</b>" +
                    $"{Environment.NewLine}Можно поменять в главном меню.", cancellationToken: ct, parseMode: ParseMode.Html);
            } else if (callbackQuery.Data == "count")
            {
                _memoryStorage.GetSession(callbackQuery.From.Id).Status = "count";
                await _telegramClient.SendMessage(callbackQuery.From.Id,
                   $"Режим изменен на количество символов", cancellationToken: ct, parseMode: ParseMode.Html);
            }
            else if (callbackQuery.Data == "sum")
            {
                _memoryStorage.GetSession(callbackQuery.From.Id).Status = "sum";
                await _telegramClient.SendMessage(callbackQuery.From.Id,
                   $"Режим изменен на суммирование", cancellationToken: ct, parseMode: ParseMode.Html);
            }
        }
    }
}