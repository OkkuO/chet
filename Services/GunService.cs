
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


        public async void AddGun(int points, long UserId, DateTime dateTime) 
        {
            await _gunRepository.Add(points, UserId, dateTime);
        }

        
        public async void UpdatePoints(long UserId, int points) 
        {
            await _gunRepository.UpdatePoints(UserId, points);
        }
        

        public async void UpdateData(long UserId, DateTime dateTime) 
        {
            await _gunRepository.UpdateData(UserId, dateTime);
        }

        public void DeleteAll() 
        {
            _gunRepository.DeleteAll();
        }


        // public async Task<Gun> GetByFilter(Gun gun) 
        // {
        //     return await _gunRepository.Create(gun);
        // }

    }
}