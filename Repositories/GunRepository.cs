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


namespace chet.Repositories
{
    public class GunRepository
    {
        
        private readonly GunDbContext _dbContext;

    
        public GunRepository(GunDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<List<Gun>> GetAll() 
        
        {
            var guns = await _dbContext.guns
            .AsNoTracking()
            .OrderByDescending(c => c.points)
            .ToListAsync();
           
            return guns;
        }

        public async Task<Gun> GetId(long UserId) 
        {
            return await _dbContext.guns.AsNoTracking().FirstOrDefaultAsync(c => c.UserId == UserId);
        }

        public async Task<List<Gun>> GetPagedData (int pageNumber, int pageSize)
        {

            var data = _dbContext.guns

                .Skip((pageNumber - 1) * pageSize)

                .Take(pageSize)

                .ToList();

           return data;
        }
        public async Task Add(int points, long UserId, DateTime dateTime, string userName)
        {
            var gun = new Gun()
            {   
                points = points,
                UserId = UserId,
                dateTime = dateTime,
                userName = userName
            };

            await _dbContext.AddAsync(gun);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateData(string userName, long UserId, DateTime dateTime, int points)
        {
            var gun = await _dbContext.guns.FirstOrDefaultAsync(c => c.UserId == UserId)
                ?? throw new Exception("Пользователь не найден.");

            gun.dateTime = DateTime.UtcNow;
            gun.points += points;
            gun.userName = userName;
            await _dbContext.SaveChangesAsync(); 
        }


        public async void DeleteAll()
        {
            var gunModel = await _dbContext.guns.ToListAsync();
            _dbContext.guns.RemoveRange(gunModel);
            await _dbContext.SaveChangesAsync();
        }
    }
}