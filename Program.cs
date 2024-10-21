
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
using chet.Models;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using Telegram.Bot.Extensions;
using Microsoft.VisualBasic;
using Telegram.Bot.Types.ReplyMarkups;
using Newtonsoft.Json;

//using Telegram.BotAPI;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<GunDbContext>(
    connections => 
    {
        connections.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    
builder.Services.AddDbContext<ChatsDbContext>(
    connections => 
    {
        connections.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

builder.Services.AddScoped<GunService, GunService>();
builder.Services.AddScoped<GunRepository, GunRepository>();

builder.Services.AddScoped<ChatService, ChatService>();
builder.Services.AddScoped<ChatRepository, ChatRepository>();

var app = builder.Build();

//app.Run();

var cts = new CancellationTokenSource();

GunService commands = app.Services.GetService<GunService>();
ChatService chatsCommands = app.Services.GetService<ChatService>();

var bot = new TelegramBotClient("", cancellationToken: cts.Token); //, 

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
bot.OnUpdate += OnUpdate;



async Task OnMessage(Message msg, UpdateType type)
{
    Console.WriteLine($"{msg.From} | {msg.Text}"); 

    //смотрим есть ли у меня такой ид в бд
    Gun UserData = commands.GetUserId(msg.From.Id).Result;

    
    //решение проблем с экранированием < >
    
    string userName = msg.From.FirstName;


    if  (userName!=null) 
    {
        userName = userName.Replace("<", "&lt;");
        userName = userName.Replace(">", "&lt;");
    }
    else 
    {
        userName = "null";
    }

    //функция если пользователь уже делал выстрел
    async void UserPlayed ()
    {
        await bot.SendTextMessageAsync(msg.Chat, $"<a href=\"tg://user?id={msg.From.Id}\"> {userName} </a>, вы уже делали выстрел!" +
            $" Попробуйте через {UserData?.dateTime.AddHours(2).Subtract(DateTime.UtcNow).Minutes} минут!",
            parseMode : ParseMode.Html);
    }
        
    if(msg?.Text != null) {

        string[] commandParams = msg.Text.Split(' ');
        
        //рандом для отправки стикеров или эмодзи на сообщение
   


        int msgRandom = random(0, 10);

        if (msgRandom == 1)
        {
            if(msg.Text.Length > 10)
            {
                chatsCommands?.AddChatMsg(msg.Chat.Id, $"{msg.Text}");
            }   
        }
        int sumMess = random(0, 200);

        try {
            

            switch(sumMess) {
                case 1: 
                {
                    int result = random(0, 45);
                    //для отправки стикера
                    var stickerId = await bot.GetStickerSetAsync("CutePumpkinAnim");
                
                    await bot.SendStickerAsync(msg.Chat.Id, stickerId.Stickers[result].FileId);
                    
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
                case 4: 
                {
                    await bot.SetMessageReactionAsync(msg.Chat.Id, msg.MessageId, reaction: ["🍓"]);
                    break;
                }
                case 5:
                {

                    if (chatsCommands?.GetAllChats(msg.Chat.Id).Result!=null)
                    {
                        var x = chatsCommands!.GetAllChats(msg.Chat.Id).Result;

                        int lastId = x.Count - 1;
                        int randomMsg = random(1, lastId);

                        System.Console.WriteLine(x[randomMsg].msg);
                        await bot.SendTextMessageAsync(msg.Chat, $" {x[randomMsg].msg}");
                    }
                    else 
                    {
                        System.Console.WriteLine("null");
                    }
                    break;
                }
                case 6:
                {
                    int result = random(0, 20);
                    //для отправки стикера

                    var stickerId = await bot.GetStickerSetAsync("GhostHamster");
                
                    await bot.SendStickerAsync(msg.Chat.Id, stickerId.Stickers[result].FileId);
                    break;
                }
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
                    //выстрел
                    int gun = random(0, 2);        
                    switch (gun) 
                    {
                        case 0: 
                        {
                            int points = random(1, 8);  
                            if (UserData!= null)
                            {
                                
                            System.Console.WriteLine($"Пользователь: {UserData.Id}");

                                if (UserData?.dateTime.AddHours(2) < DateTime.UtcNow) 
                                {   
                                                                       
                                    await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id=<{msg.From.Id}\">{userName} </a>Застрелился ⚰️ Потеряно {points} очков!"
                                    + $"\n Всего очков: {UserData.points - points}", 
                                    parseMode : ParseMode.Html);

                                    commands?.UpdateData(userName, msg.From.Id, UserData.dateTime, -points);
                                }
                                else 
                                {
                                    UserPlayed();
                                }       
                            } 
                            else
                            {
                                //если игрок первый раз пишет команду

                                    System.Console.WriteLine($"Пользователь: |{msg.From.Id}| {userName}");

                                await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id=<{msg.From.Id}\">{userName} </a>Застрелился ⚰️ Потеряно {points} очков!",
                                parseMode : ParseMode.Html); 
                                    System.Console.WriteLine(1);
                                commands?.AddGun(-points, msg.From.Id, DateTime.UtcNow, userName);
                                    System.Console.WriteLine(2);
                            }
                            break;
                        }
                        case 1: 
                        {
                            int points = random(2, 11);
                            if (UserData!= null) 
                            {
                                System.Console.WriteLine($"Пользователь: {UserData.Id}");

                                if (UserData?.dateTime.AddHours(2) < DateTime.UtcNow) 
                                {
                                    await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id={msg.From.Id}\">{userName} </a>выжил! 😇 Получено {points} очков!"
                                    + $"\n Всего очков: {UserData.points + points}",
                                    parseMode : ParseMode.Html);  
                                    commands?.UpdateData(userName, msg.From.Id, UserData.dateTime, points);
                                }
                                else
                                {
                                    UserPlayed();
                                }                                                                  
                            }
                            else
                            {
                                    System.Console.WriteLine($"Пользователь: |{msg.From.Id}| {userName}");

                                //если игрок первый раз пишет команду
                                await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id={msg.From.Id}\">{userName} </a>выжил! 😇 Получено {points} очков!", 
                               parseMode : ParseMode.Html);    

                                commands?.AddGun(points, msg.From.Id, DateTime.UtcNow, userName);                          
                            }
                            break;          
                        }
                    }
                await Task.Delay(2000);
                await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);
                break;
                }

                case "/topGun":
                {          
                       
                    Dictionary<string, int> userAndPoints = [];

                    //строка для записи юзера и вывода ее ботом
                    string topUser = "";    
                    //счетчик юзеров (записывается в topUser)                                    
                    int i = 1;
                    var pageSize = 5;
                    

                    //добавление в словарь отсортированному по поинту
                    foreach (var item in commands?.GetAllGuns().Result)
                    {
                        
                        userAndPoints.Add( $"{item.userName}", item.points);
                        
                    }
                    
                    //кнопка в тексте
                    string callbackQueryData = 'a' + new Random().Next(5_000).ToString();
                    // Массив кнопок прикрепляемых к сообщению
                    var pagination_back = new InlineKeyboardMarkup(
                        new[]{
                            InlineKeyboardButton.WithCallbackData(text: "⬅️", callbackData: "pagination_back"),
                        }
                    ); 
                    var pagination_next = new InlineKeyboardMarkup(
                        new[]{
                            InlineKeyboardButton.WithCallbackData(text: "➡️", callbackData: "pagination_next"),
                        }
                    ); 
                    var bottons = new InlineKeyboardMarkup(
                        new[]{
                            InlineKeyboardButton.WithCallbackData(text: "⬅️", callbackData: "pagination_back"),
                            InlineKeyboardButton.WithCallbackData(text: "➡️", callbackData: "pagination_next"),
                        }
                    ); 

                    //перебор словаря и отредактирование его, а также подсчет
                    foreach (var item in userAndPoints)
                    {                         
                        topUser += $"{i++}. {item.ToString().ToString().Replace(",", ":").Replace("[", " ").Replace("]", " ").Replace("&lt;", "")} очков\n";                           
                    }

                    Message sentMessage = await bot.SendTextMessageAsync(msg.Chat, $"😎 Топ скорострелов 😎\n" + "\n" + topUser, replyMarkup: pagination_next);
                   
                    int messageId = msg.MessageId;
                    int sentMessageId = sentMessage.MessageId;
                    await Task.Delay(5000);
                    
                    break;
                }
                case "/topMsg":
                {
                    // var result = "";
                    // chatsCommands?.GetAllChats(msg.Chat.Id).Result.ForEach(g => result += JsonConvert.SerializeObject(g));
                    // await bot.SendTextMessageAsync(msg.Chat, $" {result}");

                    if (chatsCommands?.GetAllChats(msg.Chat.Id).Result!=null)
                    {
                        var x = chatsCommands!.GetAllChats(msg.Chat.Id).Result;

                        int lastId = x.Count - 1;
                        int randomMsg = random(1, lastId);

                        System.Console.WriteLine(x[randomMsg].msg);
                        await bot.SendTextMessageAsync(msg.Chat, $" {x[randomMsg].msg}");
                    }
                    else 
                    {
                        System.Console.WriteLine("null");
                    }
                    await bot.DeleteMessageAsync(msg.Chat.Id, msg.MessageId);

                    break;
                }
                case "/delete":
                {
                    chatsCommands?.DeleteChats(msg.Chat.Id);
                    break;
                }

            }
        }
        catch (Exception exc) 
        {
           // Console.WriteLine(exc.Message);
        }
        
    }
}

async Task OnUpdate(Update update) {
Message msg = new Message();
 
    switch (update.Type) {
        case  UpdateType.CallbackQuery: {
            var query = update?.CallbackQuery;

            System.Console.WriteLine(update);
            if(query?.Data == "pagination_next") {
                await bot.AnswerCallbackQueryAsync(query.Id, $"You picked {query.Data}"); //выводит всплывающее сообщение
                await bot.SendTextMessageAsync(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
                //  await bot.EditMessageTextAsync(msg.Chat, sentMessageId,  $"😎 Топ скорострелов 😎\n" + "\n" + topUser, replyMarkup: bottons);
            }
            
            if(query?.Data == "pagination_back") {
                await bot.AnswerCallbackQueryAsync(query.Id, $"You picked {query.Data}"); //выводит всплывающее сообщение
                await bot.SendTextMessageAsync(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
            }
           

           break; 
        }
        default:
        break;

    }
   
}

Console.ReadLine();