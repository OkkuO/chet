
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

        public async void AddGun(int points, long UserId, DateTime dateTime) 
        {
            await _gunRepository.Add(points, UserId, dateTime);
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