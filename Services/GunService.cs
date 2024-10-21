
using chet.Models;
using chet.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace chet.Services
{
    public class GunService
    {
        private readonly GunRepository _gunRepository;

        public GunService(GunRepository gunRepository)
        {
            _gunRepository = gunRepository;
        }
        public async Task<List<Gun>> GetAllGuns() 
        {
            return await _gunRepository.GetAll();
        }

        public async Task<Gun> GetUserId(long UserId) 
        {
            return await _gunRepository.GetId(UserId);
        }

        public async Task<List<Gun>> GetPagedData() 
        {
            return await _gunRepository.GetPagedData(1, 5);
        }

        public async void AddGun(int points, long UserId, DateTime dateTime, string userName) 
        {
            await _gunRepository.Add(points, UserId, dateTime, userName);
        }



        public async void UpdateData(string userName, long UserId, DateTime dateTime, int points) 
        {
            await _gunRepository.UpdateData(userName, UserId, dateTime, points);
        }

        public void DeleteAll() 
        {
            _gunRepository.DeleteAll();
        }

        internal void GetAllChats()
        {
            throw new NotImplementedException();
        }
    }
}