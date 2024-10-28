
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
//ChatService chatsCommands = app.Services.GetService<ChatService>();

var bot = new TelegramBotClient("", cancellationToken: cts.Token); //, 

var me = await bot.GetMeAsync();
bot.OnError += OnError;
  int msgFind = 0;

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


        Dictionary<int, long> randomUserPlusPoints = [];
        
        int counter2 = 0;

        int k = 0;

        foreach (var item in commands!.GetAllUserId().Result)
        {
            randomUserPlusPoints.Add(k, item);
            k++;
            counter2++;
        }
        
bot.OnMessage += OnMessage;
bot.OnUpdate += OnUpdate;

async Task OnMessage(Message msg, UpdateType type)
{
    try 
    {
        Console.WriteLine($"{msg.From} | {msg.Text}"); 
        //смотрим есть ли у меня такой ид в бд
        Gun UserData = commands.GetUserId(msg.From.Id).Result;
        
        string userName = msg.From.FirstName;

        int userPoints = 0;

        if (UserData!=null)
        {
            userPoints = UserData.points | 0;
        }
        
        //решение проблем с экранированием < >  
        userName.Replace("<", "&lt;");
        userName.Replace(">", "&lt;");



        
        
        if(msg?.Text != null) 
        {
           
        msgFind++;

        if (msgFind == 150)
        {
            msgFind = 0;
            int randomFindPoints = random(2, 20);
            int randomCounter = random(0, counter2);

            await bot.SendTextMessageAsync(msg.Chat, $"<a href=\"tg://user?id={randomUserPlusPoints[randomCounter]}\"> {commands.GetUserId(randomUserPlusPoints[randomCounter]).Result.userName}</a>"
            + $" находит {randomFindPoints} очков у параши! Мои поздравления!",
            parseMode : ParseMode.Html);

            commands.UpdateFindPoints(randomUserPlusPoints[randomCounter], randomFindPoints);
        }
        System.Console.WriteLine(msgFind);
            string[] commandParams = msg.Text.Split(' ');

                     //начисление рандомно очков каждый час

            async void gunCommand()
            {
                //выстрел
                int gun = random(0, 2);        
                switch (gun) 
                {
                    case 0: 
                    {
                        int points = random(1, 6); 

                        if (UserData!= null)
                        {                        
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
                            await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id=<{msg.From.Id}\">{userName} </a>Застрелился ⚰️ Потеряно {points} очков!",
                                parseMode : ParseMode.Html); 
                                commands?.AddGun(-points, msg.From.Id, DateTime.UtcNow, userName);
                        }
                        break;
                    }
                    case 1: 
                    {
                        int points = random(2, 11);

                        if (UserData!= null) 
                        {
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
                        //если игрок первый раз пишет команду
                            await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id={msg.From.Id}\">{userName} </a>выжил! 😇 Получено {points} очков!", 
                                parseMode : ParseMode.Html);    

                            commands?.AddGun(points, msg.From.Id, DateTime.UtcNow, userName);                          
                        }
                        break;          
                    }
                }
            }
            async void gunplayCommand()
            {
                Dictionary<string, int> userAndPoints = [];

                foreach (var item in commands!.GetAllGuns().Result)
                {  
                    userAndPoints.Add( $"{item.userName}", item.points);

                }

                if (userAndPoints.ContainsKey(msg.From.FirstName))
                {
                    if (userPoints >= 4)
                    {
                        var gunplay = new InlineKeyboardMarkup(
                            new[]{
                                    InlineKeyboardButton.WithCallbackData(text: "🔫", callbackData: $"{msg.From.Id}:gunplay"),
                                }
                                );                                
                        await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id=<{msg.From.Id}\">{userName} </a>вызвал_а на перестрелку!",
                            replyMarkup: gunplay,
                            parseMode : ParseMode.Html); 
                    }
                    else 
                    {
                        await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id=<{msg.From.Id}\">{userName} </a>, у вас не хватает очков для выхода на перестрелку!",
                            parseMode : ParseMode.Html);
                    }
                } else 
                {
                    await bot.SendTextMessageAsync(msg.Chat, $" <a href=\"tg://user?id=<{msg.From.Id}\">{msg.From.Username}</a>,"
                        + " Вы не можете учавствовать в перестрелке! Нажмите /gun, чтобы набрать очков.", 
                        parseMode : ParseMode.Html);                            
                }
            }
            async void UserPlayed ()
        {
            await bot.SendTextMessageAsync(msg.Chat, $"<a href=\"tg://user?id={msg.From.Id}\"> {userName} </a>, вы уже делали выстрел!" +
                $" Попробуйте через {UserData?.dateTime.AddHours(2).Subtract(DateTime.UtcNow).Minutes} минут!",
                parseMode : ParseMode.Html);
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
                    case "/gift@bulya2024_bot":
                    {   
                        var replyUser = msg.ReplyToMessage;  
                        
                        if(replyUser?.Chat != null) 
                        {
                            if (commands.GetUserId(replyUser.From.Id).Result!=null)
                            {
                                int giftPoints = int.Parse(commandParams[1]);

                                if (giftPoints <= userPoints && giftPoints > 0)
                                {
                                    if (giftPoints==1)
                                    {
                                        await bot.SendTextMessageAsync(msg.Chat, $"<a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a> подарил_а очко для <a href=\"tg://user?id={replyUser.From.Id}\">{replyUser.From.FirstName}</a>!",
                                        parseMode : ParseMode.Html);
                                        commands.UpdatePoints(replyUser.From.Id, msg.From.Id, giftPoints);                                        
                                    }
                                    else
                                    {
                                        await bot.SendTextMessageAsync(msg.Chat, $"<a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a> подарил_а {commandParams[1]} очков для <a href=\"tg://user?id={replyUser.From.Id}\">{replyUser.From.FirstName}</a>!",
                                        parseMode : ParseMode.Html);
                                        commands.UpdatePoints(replyUser.From.Id, msg.From.Id, giftPoints);                                       
                                    }

                                }
                                else
                                {
                                    await bot.SendTextMessageAsync(msg.Chat, $"<a href=\"tg://user?id={msg.From.Id}\">{msg.From.FirstName}</a> я рот ебала твой не твори хуйню пж, конча!",
                                        parseMode : ParseMode.Html);
                                }                              
                            }
                            else
                            {
                                await bot.SendTextMessageAsync(msg.Chat, $"<a href=\"tg://user?id={replyUser.From.Id}\">{replyUser.From.FirstName}</a> не является скорострелом!",
                                parseMode : ParseMode.Html);                                  
                            }
                        }
                                       
                        break;
                    }            
                    case "/gun@bulya2024_bot":
                    {
                        gunCommand();
                        break;
                    }
                    case "/topgun@bulya2024_bot":
                    {          
                        
                        

                        //строка для записи юзера и вывода ее ботом
                        string topUser = "";    
                        //счетчик юзеров (записывается в topUser)                                    
                        int i = 1;
                       
                        
                        //кнопка в тексте
                        string callbackQueryData = 'a' + new Random().Next(5_000).ToString();
                        // Массив кнопок прикрепляемых к сообщению
                        var pagination_back = new InlineKeyboardMarkup(
                            new[]{
                                InlineKeyboardButton.WithCallbackData(text: "⬅️", callbackData: $"{msg.From.Id}:pagination_back"),
                            }
                        ); 
                        var pagination_next = new InlineKeyboardMarkup(
                            new[]{
                                InlineKeyboardButton.WithCallbackData(text: "➡️", callbackData: $"{msg.From.Id}:pagination_next"),
                            }
                        ); 
                        var bottons = new InlineKeyboardMarkup(
                            new[]{
                                InlineKeyboardButton.WithCallbackData(text: "⬅️", callbackData: $"{msg.From.Id}:pagination_back"),
                                InlineKeyboardButton.WithCallbackData(text: "➡️", callbackData: $"{msg.From.Id}:pagination_next"),
                            }
                        ); 

                        
                        Dictionary<string, int> userAndPoints = [];


                        //количество записей
                        int counter = 0;
                                        
                        //добавление в словарь отсортированному по поинту
                        foreach (var item in commands!.GetAllGuns().Result)
                        {  
                            userAndPoints.Add( $"{item.userName}", item.points);
                            counter++;
                        }
                        //перебор словаря и отредактирование его, а также подсчет
                        foreach (var item in userAndPoints)
                        {                         
                            topUser += $"{i++}. {item.ToString().ToString().Replace(",", ":").Replace("[", " ").Replace("]", " ").Replace("&lt;", "")} очков\n";                           
                        }

                        // Message sentMessage = await bot.SendTextMessageAsync(msg.Chat, $"😎 Топ скорострелов 😎\n" + "\n" + topUser, replyMarkup: pagination_next);
                    
                        // int messageId = msg.MessageId;
                        // int sentMessageId = sentMessage.MessageId;

                        if(counter < 10)
                        {
                            Message sentMessage = await bot.SendTextMessageAsync(msg.Chat, $"😎 Топ скорострелов 😎\n" + "\n" + topUser);
                        }
                        else
                        {
                            Message sentMessage = await bot.SendTextMessageAsync(msg.Chat, $"😎 Топ скорострелов 😎\n" + "\n" + topUser, replyMarkup: pagination_next);
                        }

                        await Task.Delay(5000);
                        
                        break;
                    }
                    case "/gunplay@bulya2024_bot":
                    {
                        gunplayCommand();
                        break;
                    }
                    case "/me@bulya2024_bot":
                    {                       
                         await bot.SendTextMessageAsync(msg.Chat, $"<a href=\"tg://user?id={msg.From.Id}\"> {userName}</a>," +
                            $" количество ваших очков: {userPoints}.",
                            parseMode : ParseMode.Html);
                        break;
                    }

            }
        }
    }
    catch (Exception exc) 
    {
        Console.WriteLine(exc.Message);
    }
}

async Task OnUpdate(Update update) 
{
    try
        {
        switch (update.Type) 
        {
            case  UpdateType.CallbackQuery: 
            {
                
                var query = update?.CallbackQuery;
                  
                long actionCreatorId = long.Parse(query?.Data.Split(':')[0]); //id создателя
                string action = query?.Data?.Split(':')[1]; //кнопка

                if(action == "pagination_next") {
                    await bot.AnswerCallbackQueryAsync(query.Id, $"You picked {query.Data}"); //выводит всплывающее сообщение
                    await bot.SendTextMessageAsync(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
                    //  await bot.EditMessageTextAsync(msg.Chat, sentMessageId,  $"😎 Топ скорострелов 😎\n" + "\n" + topUser, replyMarkup: bottons);
                }             
                if(action == "pagination_back") {
                    await bot.AnswerCallbackQueryAsync(query.Id, $"You picked {query.Data}"); //выводит всплывающее сообщение
                    await bot.SendTextMessageAsync(query.Message!.Chat, $"User {query.From} clicked on {query.Data}");
                }

                if(action == "gunplay") {

                    Gun UserSend = commands.GetUserId(actionCreatorId).Result;
                    Gun UserGet = commands.GetUserId(query!.From.Id).Result;

                    if (UserGet == null)
                    {
                        await bot.SendTextMessageAsync(query.Message!.Chat, $" <a href=\"tg://user?id=<{query!.From.Id}\">{query!.From.Username}</a>,"
                            + " Вы не можете участвовать в перестрелке! Нажмите /gun, чтобы набрать очков.", 
                            parseMode : ParseMode.Html);
                                           
                    }
                    else
                    {
                        if (UserGet.UserId == UserSend.UserId)   
                        {
                            await bot.AnswerCallbackQueryAsync(query.Id, $"Вы не можете бросить себе вызов!");
                        }
                        else
                        {
                            if (UserGet.points >= 4)
                            {
                                await bot.EditMessageTextAsync(chatId: query.Message!.Chat,
                                    messageId: query.Message!.MessageId, 
                                    text: $"{UserGet.userName} принял вызов!");

                                await Task.Delay(3000);
                                int win = random(0, 2);

                                int winPoints = 0;
                                

                                if (UserGet.points > UserSend.points)
                                {
                                    winPoints = UserGet.points/2;

                                    if (UserSend.points-winPoints < 0)
                                    {
                                        winPoints = UserSend.points/2;
                                    }
                                    

                                } else if (UserGet.points < UserSend.points)
                                {
                                    winPoints = UserSend.points/2;

                                    if (UserGet.points-winPoints < 0)
                                    {
                                        winPoints = UserGet.points/2;
                                    }                                   
                                }
                                else
                                {
                                    winPoints = UserSend.points / 2;
                                }
                                    
                                long loseId, winId;
                                
                                if (win == 1)
                                {
                                    var msg1 = await bot.SendTextMessageAsync(query.Message!.Chat, $"{UserGet.userName} застрелил "
                                        + $"{UserSend.userName}!\n\n {UserGet.userName} получает {winPoints} очков. 😎\n {UserSend.userName} теряет {winPoints} очков. 🥺", 
                                        parseMode : ParseMode.Html);

                                    loseId = UserSend.UserId;
                                    winId = UserGet.UserId;
                                    commands.UpdatePoints(winId, loseId, winPoints); 

                                    await Task.Delay(3000);

                                    await bot.EditMessageTextAsync(chatId: query.Message!.Chat,
                                    messageId: msg1.MessageId, 
                                    text: $"{UserGet.userName} застрелил "
                                        + $"{UserSend.userName}!\n\n {UserGet.userName} получает {winPoints} очков. 😎\n{UserSend.userName} теряет {winPoints} очков. 🥺\n\n"
                                        + $"{UserGet.userName}: {UserGet.points}\n{UserSend.userName}: {UserSend.points}", 
                                        parseMode : ParseMode.Html);

                                } else
                                {
                                    var msg1 = await bot.SendTextMessageAsync(query.Message!.Chat, $"{UserSend.userName} застрелил {UserGet.userName}!\n\n"
                                        + $"{UserSend.userName} получает {winPoints} очков.😎\n {UserGet.userName} теряет {winPoints} очков. 🥺\n\n", 
                                        parseMode : ParseMode.Html); 

                                    winId = UserSend.UserId;
                                    loseId = UserGet.UserId;  
                                    commands.UpdatePoints(winId, loseId, winPoints);     

                                    
                                    await Task.Delay(3000);

                                    await bot.EditMessageTextAsync(chatId: query.Message!.Chat,
                                    messageId: msg1.MessageId, 
                                    text: $"{UserSend.userName} застрелил {UserGet.userName}!\n\n"
                                        + $"{UserSend.userName} получает {winPoints} очков. 😎\n {UserGet.userName} теряет {winPoints} очков. 🥺\n\n"
                                        + $"{UserGet.userName}: {UserGet.points}\n{UserSend.userName}: {UserSend.points}",
                                        parseMode : ParseMode.Html);
                                                             
                                }                                   
                            } else
                            {
                                await bot.SendTextMessageAsync(query.Message!.Chat, $" <a href=\"tg://user?id=<{query!.From.Id}\">{query!.From.Username}</a>,"
                                    + " Вы не можете участвовать в перестрелке! Нажмите /gun, чтобы набрать очков.", 
                                    parseMode : ParseMode.Html);
                            }
                        }                       
                    }         
                }                            
            break; 
            }
        }
    }
    catch (Exception exc)
    {   
        try {
            int errorCode = int.Parse(exc?.InnerException?.Message?.Split(':')[0]);
            string eror = exc?.InnerException?.Message?.Split(':')[1]; //ошибка

            if (errorCode == 4004)
            {
                await bot.SendTextMessageAsync(update?.CallbackQuery.Message!.Chat, " Вы не можете учавствовать в перестрелке! Нажмите /gun, чтобы набрать очков.", 
                                    parseMode : ParseMode.Html); 
            }       
        } catch (Exception ex) {Console.WriteLine(ex);}
        
    }
}

Console.ReadLine();