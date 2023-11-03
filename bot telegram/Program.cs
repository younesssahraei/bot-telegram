//youness sahraei/////
// tamrin aval/////

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static TelegramBotClient botClient;
    private static readonly HttpClient httpClient = new HttpClient();
    private static readonly string coinGeckoApiUrl = "https://api.coingecko.com/api/v3/simple/price?ids=bitcoin&vs_currencies=usd";

    static async Task Main()
    {
        botClient = new TelegramBotClient("your_bot_token");
        botClient.OnMessage += Bot_OnMessage;
        botClient.OnCallbackQuery += Bot_OnCallbackQuery;
        botClient.StartReceiving();
        Console.ReadLine();
        botClient.StopReceiving();
    }

    private static async void Bot_OnMessage(object sender, MessageEventArgs e)
    {
        if (e.Message.Type == MessageType.Text && e.Message.Text == "/btc")
        {
            await botClient.SendChatActionAsync(e.Message.Chat.Id, ChatAction.Typing);

            decimal bitcoinPrice = await GetBitcoinPrice();

            var button = InlineKeyboardButton.WithCallbackData("نمایش قیمت بیتکوین");
            var inlineKeyboard = new InlineKeyboardMarkup(new[] { new[] { button } });

            await botClient.SendTextMessageAsync(e.Message.Chat.Id, $"قیمت بیتکوین: {bitcoinPrice}", replyMarkup: inlineKeyboard);
        }
    }

    private static async Task<decimal> GetBitcoinPrice()
    {
        HttpResponseMessage response = await httpClient.GetAsync(coinGeckoApiUrl);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        JObject json = JObject.Parse(responseBody);
        decimal price = (decimal)json["bitcoin"]["usd"];
        return price;
    }

    private static async void Bot_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
    {
        if (e.CallbackQuery.Data == "نمایش قیمت بیتکوین")
        {
            await botClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id, $"در حال بروزرسانی قیمت بیتکوین...");

            decimal bitcoinPrice = await GetBitcoinPrice();

            await botClient.SendTextMessageAsync(e.CallbackQuery.Message.Chat.Id, $"قیمت بیتکوین: {bitcoinPrice}");
        }
    }
}
















