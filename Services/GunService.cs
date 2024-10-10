
using chet.Models;
using chet.Repositories;

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

        public async void UpdateGun(int id, int points, long UserId, DateTime dateTime)
        {
             await _gunRepository.Update(id, points, UserId, dateTime);
        }
    }
}