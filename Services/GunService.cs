
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
            return await _gunRepository.Get();
            
        }

        // public async Task<Gun> GetByFilter(Gun gun) 
        // {
        //     return await _gunRepository.Create(gun);
        // }

        public async void Update(int id, int points, long UserId, DateTime dateTime)
        {
             await _gunRepository.Update(id, points, UserId, dateTime);
        }

        public async void UpdatePoints(long UserId, int points, DateTime dateTime)
        {
             await _gunRepository.UpdatePoints(UserId, points, dateTime);
        }

        public async void GetById(long UserId)
        {
             await _gunRepository.GetById(UserId);
        }

        public async void Add( int points, long UserId, DateTime dateTime)
        {
             await _gunRepository.Add( points, UserId, dateTime);
        }

    }
}