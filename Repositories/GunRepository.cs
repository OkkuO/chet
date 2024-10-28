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

        public async Task<List<long>> GetAllUserId() 
        
        {
            var guns = await _dbContext.guns
            .AsNoTracking()
            .Select(c => c.UserId)
            .ToListAsync();
           
            return guns;
        }

        public async Task<Gun> GetId(long UserId) 
        {   
            var gun = await _dbContext.guns.FirstOrDefaultAsync(c => c.UserId == UserId);
            if (gun == null)
            {
                return null;
                throw new ArgumentException($"4004 : Пользователь с ID {UserId} не найден.");
            }
            return gun;

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

        public async Task UpdatePoints(long winId, long loseId, int points)
        {
            var gunWin = await _dbContext.guns.FirstOrDefaultAsync(c => c.UserId == winId)
                ?? throw new Exception("Пользователь не найден.");
            var gunLose = await _dbContext.guns.FirstOrDefaultAsync(c => c.UserId == loseId)
                ?? throw new Exception("Пользователь не найден.");
                
            gunWin.points += points;
            gunLose.points -= points;
            
            await _dbContext.SaveChangesAsync(); 
        }
        public async Task UpdateFindPoints(long UserId, int points)
        {
            var gun = await _dbContext.guns.FirstOrDefaultAsync(c => c.UserId == UserId)
                ?? throw new Exception("Пользователь не найден.");
                
            gun.points += points;
            
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