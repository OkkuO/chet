
using chet.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using chet.Repositories;
using chet.Services;
using chet.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

//using Telegram.BotAPI;



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

//app.Run();

var cts = new CancellationTokenSource();

GunService commands = app.Services.GetService<GunService>();

var bot = new TelegramBotClient("7671323724:AAEUlMgFmiZ3q9LpBSD50oNFTmWmQix_JZw", cancellationToken: cts.Token); //, 

var me = await bot.GetMeAsync();
bot.OnError += OnError;

async Task OnError(Exception exception, HandleErrorSource source)
{
    await bot.SendTextMessageAsync(exception.Message, "Error");
    Console.WriteLine(exception); // just dump the exception to the console
}


// Получаем сообщение от пользователя

int random (int a, int b) {
    Random rnd = new();
    return rnd.Next(a, b);
}


bot.OnMessage += OnMessage;
async Task OnMessage(Message msg, UpdateType type)
{
    Console.WriteLine($"{msg.From} | {msg.Text}"); 
    if(msg?.Text != null) {
        string[] commandParams = msg.Text.Split(' ');
        int sumMess = random(0, 200);

        if (msg.From.Id!=1830105695 && msg.From.Id!=6417034191 && msg.From.Id!=7301559119) {

       
        try {
            switch(sumMess) {
                case 1: 
                {
                    int result = random(0, 120);
                    //для отправки стикера
                    var stickerId = await bot.GetStickerSetAsync("NegevBestGirl_by_fStikBot");
                
                    await bot.SendStickerAsync(msg.Chat.Id, stickerId.Stickers[result].FileId);
                    sumMess = 0;
                    break;
                }
                case 2: 
                {
                    //для отправки эмодзи на сообщение
                    await bot.SetMessageReactionAsync(msg.Chat.Id, msg.MessageId, reaction: ["🎃"]);
                    break;
                }
                case 3: 
                {
                    await bot.SetMessageReactionAsync(msg.Chat.Id, msg.MessageId, reaction: ["👻"]);
                    break;
                }

                default: 
                //System.Console.WriteLine(sumMess);
                break;
            }

            switch (commandParams[0]) {   
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
                    
                   // await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a> решил сыграть в русскую рулетку..", parseMode: ParseMode.Html);
            
                    await Task.Delay(500);

                    int result = random(0, 2);
                    int points = random(1, 11);
                    
                         
                    //смотрим есть ли у меня такой ид в бд
                    Gun UserData = commands?.GetUserId(msg.From.Id).Result;

                    switch (result) 
                    {
                        case 0: 
                        {
                            if (UserData! == null)
                            {
                                if (UserData?.points > 1) 
                                {
                                    if (UserData?.dateTime.AddHours(12) >= DateTime.Now) 
                                    commands?.AddGun(1,msg.From.Id, DateTime.UtcNow);
                                    await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a> Застрелился ⚰️ Получено очко в знак утешения!", 
                                    parseMode: ParseMode.Html);
                                }
                                else 
                                {
                                    //если игрок первый раз пишет команду
                                    commands?.AddGun(1,msg.From.Id, DateTime.UtcNow);
                                    await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a> Застрелился ⚰️ Получено очко в знак утешения!", 
                                    parseMode: ParseMode.Html); 
                                }
                            }
                            else 
                            {
                                await bot.SendTextMessageAsync(msg.Chat, $"<a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a>, вы уже играли в русскую рулетку!" +
                                $" Попробуйте через {UserData?.dateTime.AddHours(16).Subtract(DateTime.Now).Hours} часов!",
                                parseMode: ParseMode.Html);
                            }

                            break;
                        }
                        case 1: 
                        {
                            if (UserData! == null) 
                            {
                                //если игрок ранее играл
                                if (UserData?.points > 1) 
                                {
                                    if (UserData?.dateTime.AddHours(12) >= DateTime.Now)
                                    commands?.AddGun(points,msg.From.Id, DateTime.UtcNow);
                                    await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a> выжил! 😇 Получено {points} очков!", 
                                    parseMode: ParseMode.Html);  
                                    commands?.UpdatePoints(msg.From.Id, points);
                                    commands?.UpdateData(msg.From.Id, UserData.dateTime);

                                } else 
                                {
                                    //если игрок первый раз пишет команду
                                    commands?.AddGun(points,msg.From.Id, DateTime.UtcNow);
                                    await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a> выжил! 😇 Получено {points} очков!", 
                                    parseMode: ParseMode.Html);  
                                }
                            }
                            else
                            {

                                await bot.SendTextMessageAsync(msg.Chat, $"<a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a>, вы уже играли в русскую рулетку!" +
                                $" Попробуйте через {UserData?.dateTime.AddHours(16).Subtract(DateTime.Now).Hours} часов!",
                                parseMode: ParseMode.Html);
                            }
                            break;
                        }
                    }
                    break;
                }
                case "/select":
                {          
                    if (msg.From.Id == 7186499641) 
                    {
                        var result = "";
                        commands?.GetAllGuns().Result.ForEach(g => result += JsonConvert.SerializeObject(g));
                        await bot.SendTextMessageAsync(msg.Chat, $" {result}");
                    } else 
                    { 
                        await bot.SendTextMessageAsync(msg.Chat, "У вас нет права использовать эту команду!");
                    }          
                    
                    break;
                }
                case "/delete":
                {
                    if (msg.From.Id == 7186499641) 
                    {
                        commands?.DeleteAll();
                    }
                    else
                    {
                        await bot.SendTextMessageAsync(msg.Chat, "У вас нет права использовать эту команду!");
                    }
                    break;
                }
            }
        }
        catch (Exception ex) {
            Console.WriteLine(ex.Message);
        }
        }
    }
}

// catch (DivideByZeroException)
// {
//     Console.WriteLine("Возникло исключение DivideByZeroException");
// }


Console.ReadLine();