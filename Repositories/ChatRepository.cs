using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chet.Data;

using chet.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using System.Threading.Tasks;
using NetTelegramBotApi.Types;


namespace chet.Repositories
{
    public class ChatRepository
    {
        private readonly ChatsDbContext _dbContext;

        public ChatRepository(ChatsDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Chats>> GetAllChats(long chatId) 
        
        {
            return await _dbContext.chats
                .AsNoTracking()
                .Where(c => c.chatId == chatId)
                .ToListAsync();
        }
        public async Task AddChatMsg(long chatId, string msg)
        {
            var chat = new Chats()
            {   
                chatId = chatId,
                msg = msg,
            };

            await _dbContext.AddAsync(chat);
            await _dbContext.SaveChangesAsync();
        }

        public async void DeleteChats(long chatId)
        {
            var chatsModel = await _dbContext.chats.FirstOrDefaultAsync(c => c.chatId == chatId);
            if (chatsModel!=null)
            {
                _dbContext.chats.RemoveRange(chatsModel);
                await _dbContext.SaveChangesAsync();
            }

        }
        public async void DeleteAllChats()
        {
            var chatsModel = await _dbContext.chats.ToListAsync();
            _dbContext.chats.RemoveRange(chatsModel);
            await _dbContext.SaveChangesAsync();
        }

    }
}