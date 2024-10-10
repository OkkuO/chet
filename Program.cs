using chet.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using chet.Repositories;
using chet.Services;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<GunDbContext>(
    connections => 
    {
        connections.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

builder.Services.AddScoped<GunService, GunService>();
builder.Services.AddScoped<GunRepository, GunRepository>();

var app = builder.Build();

app.Run();


var cts = new CancellationTokenSource();
var bot = new TelegramBotClient("7671323724:AAEUlMgFmiZ3q9LpBSD50oNFTmWmQix_JZw", cancellationToken: cts.Token); //, 
var me = await bot.GetMeAsync();
bot.OnError += OnError;

async Task OnError(Exception exception, HandleErrorSource source)
{

    await bot.SendTextMessageAsync(exception.Message, "Error");
    Console.WriteLine(exception); // just dump the exception to the console
}

bot.OnMessage += OnMessage;
// Получаем сообщение от пользователя

int random (int a, int b) {
    Random rnd = new();
    return rnd.Next(a, b);
}


// method that handle messages received by the bot:
async Task OnMessage(Message msg, UpdateType type)
{
  
    
    Console.WriteLine($"{msg.From} | {msg.Text}"); 

    
    
    if(msg?.Text != null) 
    {
        
        var commandParams = msg.Text.Split(' ');
          
        int sumMess = random(0, 100);
        if(sumMess == 10) 
        {      
            int result = random(0, 120);
            //для отправки стикера
            var stickerId = await bot.GetStickerSetAsync("NegevBestGirl_by_fStikBot");
        
            await bot.SendStickerAsync(msg.Chat.Id, stickerId.Stickers[result].FileId);
            sumMess = 0;
        }
 
        switch (commandParams[0])
        {   
            case "/random": 
            {
                int a = int.Parse(commandParams[1]);
                int b = int.Parse(commandParams[2]);
                await bot.SendTextMessageAsync(msg.Chat, $"Рандом число: {random(a, b)}");
                break;
            }


            case "/convert":
            {
                if(commandParams[2]=="km") 
                {
                    double result = Math.Round(int.Parse(commandParams[1]) * 0.62137);
                    await bot.SendTextMessageAsync(msg.Chat, $"{commandParams[1]} km = {result} mi");
                }
                else 
                {
                    await bot.SendTextMessageAsync(msg.Chat, $"Я пока недостаточно умный, чтобы вычесть это((((");
                }

                break;
            }
            case "/sb":
            {
                if(msg?.ReplyToMessage!=null) {
                await bot.SendTextMessageAsync(msg.Chat.Id, 
                    "🍓",
                    replyParameters:msg.ReplyToMessage.MessageId
                    );

                await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);
                }
                break;        
            }

            case "/eblan":
            {
                if(msg?.ReplyToMessage!=null) {
                int result = random(0, 40);
                            //для отправки стикера
                var stickerId = await bot.GetStickerSetAsync("decommunization");

            await bot.SendStickerAsync(msg.Chat.Id, 
                    stickerId.Stickers[result].FileId,
                    replyParameters:msg.ReplyToMessage.MessageId
                    );
                }
                await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);
                break;
            } 
            
            case "/gun":
            {
                await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a> решил сыграть в русскую рулетку..", parseMode: ParseMode.Html);
        
                await Task.Delay(500);
                int result = random(0, 2);
                int points = random(1, 11);
                if (result == 0) 
                {
                    
                    await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a> Застрелился ⚰️", parseMode: ParseMode.Html);
                } else 
                {           
                    await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a> выжил! 😇 Получено {points} очков!", parseMode: ParseMode.Html);             
                }

                break;
            }
        }
    }

}

Console.ReadLine();
