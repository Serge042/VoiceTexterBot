//using Telegram.Bot;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.ReplyMarkups;
//using Telegram.Bot.Types.Enums;
//using VoiceTexterBot.Services;

//namespace VoiceTexterBot.Controllers
//{
//    public class TextMessageController
//    {
//        private readonly ITelegramBotClient _telegramClient;

//        public TextMessageController(ITelegramBotClient telegramBotClient)
//        {
//            _telegramClient = telegramBotClient;
//        }

//        public async Task Handle(Message message, CancellationToken ct)
//        {
//            switch (message.Text)
//            {
//                case "/start":

//                    // Объект, представляющий кнопки
//                    var buttons = new List<InlineKeyboardButton[]>();
//                    buttons.Add(new[]
//                    {
//                        InlineKeyboardButton.WithCallbackData($" Русский" , $"ru"),
//                        InlineKeyboardButton.WithCallbackData($" English" , $"en")
//                    });

//                    // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
//                    await _telegramClient.SendMessage(message.Chat.Id, $"<b>  Наш бот превращает аудио в текст, а также считает количество введённых знаков или сумму введённых чисел..</b> {Environment.NewLine}" +
//                        $"{Environment.NewLine}Можно записать сообщение и переслать другу, если лень печатать.{Environment.NewLine}", cancellationToken: ct, parseMode: ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(buttons));

//                    break;
//                default:
//                    await _telegramClient.SendMessage(message.Chat.Id, "Отправьте аудио для превращения в текст.", cancellationToken: ct);
//                    break;
//            }
//        }
//    }
//}


using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using VoiceTexterBot.Services;
using Telegram.Bots.Types;
using static System.Net.Mime.MediaTypeNames;
using Telegram.Bots.Http;

namespace VoiceTexterBot.Controllers
{
    public class TextMessageController
    {
        private readonly ITelegramBotClient _telegramClient;
        private readonly IStorage _memoryStorage;

        public TextMessageController(ITelegramBotClient telegramBotClient, IStorage memoryStorage)
        {
            _telegramClient = telegramBotClient;
            _memoryStorage = memoryStorage;
        }

        public async Task Handle(Telegram.Bot.Types.Message message, CancellationToken ct)
        {
            if (message.Text == "/start")
            {

                // Объект, представляющий кнопки
                var buttons = new List<InlineKeyboardButton[]>();
                buttons.Add(new[]
                {
                        InlineKeyboardButton.WithCallbackData($" Русский" , $"ru"),
                        InlineKeyboardButton.WithCallbackData($" English" , $"en"),
                        InlineKeyboardButton.WithCallbackData($" Сумма чисел" , $"sum"),
                        InlineKeyboardButton.WithCallbackData($" Количество символов" , $"count")

                });

                // передаем кнопки вместе с сообщением (параметр ReplyMarkup)
                await _telegramClient.SendMessage(message.Chat.Id, $"<b>  Наш бот превращает аудио в текст, а также считает количество введённых знаков или сумму введённых чисел.</b> {Environment.NewLine}" +
                    $"{Environment.NewLine}Можно записать аудиосообщение и переслать другу, если лень печатать, или посчитать количество знаков в тексте.{Environment.NewLine}", cancellationToken: ct, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(buttons));
            }
            else if (_memoryStorage.GetSession(message.Chat.Id).Status == "count")
            {
                await _telegramClient.SendMessage(message.Chat.Id, $"Длина вашего текста {message.Text.Length} ", cancellationToken: ct);

            } 
            else if (_memoryStorage.GetSession(message.Chat.Id).Status == "sum")
            {
                var parts = message.Text.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var numbers = new List<double>();

                foreach (var part in parts)
                {
                    if (double.TryParse(part, out var number))
                        numbers.Add(number);
                }

                double sum = numbers.Sum();
                await _telegramClient.SendMessage(
                    message.Chat.Id, $"Сумма чисел: {sum}");

            }
            else
            {
                await _telegramClient.SendMessage(message.Chat.Id, "Отправьте аудио для превращения в текст.", cancellationToken: ct);
            }
        }
    }
}