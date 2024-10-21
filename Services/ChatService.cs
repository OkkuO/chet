using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chet.Models;
using chet.Repositories;

namespace chet.Services
{
    public class ChatService
    {
        private readonly ChatRepository _chatRepository;

        public ChatService(ChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<List<Chats>> GetAllChats(long chatId) 
        {
            return await _chatRepository.GetAllChats(chatId);
        }

        public async void AddChatMsg(long chatId, string msg) 
        {
            await _chatRepository.AddChatMsg(chatId, msg);
        }

        public void DeleteChats(long chatId) 
        {
            _chatRepository.DeleteChats(chatId);
        }
    }
}