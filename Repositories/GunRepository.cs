using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using chet.Data;
using chet.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;

namespace chet.Repositories
{
    public class GunRepository
    {
        
        private readonly GunDbContext _dbContext;
    
        public GunRepository(GunDbContext dbContext)
        {
            _dbContext = dbContext;
        }
 
        
        public async Task<List<Gun>> Get () 
        {
            return await _dbContext.guns
                .AsNoTracking()
                .OrderBy(c => c.dateTime)
                .ToListAsync();
        }

        public async Task<Gun?> GetById(long UserId) 
        {
            return await _dbContext.guns
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == UserId);
        }

        public async Task<List<Gun>> GetByFilter (long id) 
        {
            var qvery = _dbContext.guns.AsNoTracking();

            if(id > 0)
            {
                qvery = qvery.Where(c => c.Id > id);
            }

            return await qvery.ToListAsync();
        }

        public async Task<List<Gun>> GetByPage (int page, int pageSize) 
        {
            return await _dbContext.guns
                .AsNoTracking()
                .Skip((page - 1)*pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task Add( int points, long UserId, DateTime dateTime)
        {
            var gun = new Gun()
            {   
                // Id = id,
                points = points,
                UserId = UserId,
                dateTime = dateTime
            };

            await _dbContext.AddAsync(gun);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(int id, int points, long UserId, DateTime dateTime)
        {
            var gun = await _dbContext.guns.FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new Exception();
            
            gun.points = points;
            gun.UserId = UserId;
            gun.dateTime = dateTime;

            await _dbContext.SaveChangesAsync();
        }

        
        public async Task UpdatePoints(long UserId, int points, DateTime dateTime)
        {
            var gun = await _dbContext.guns.FirstOrDefaultAsync(c => c.UserId == UserId)
                ?? throw new Exception();

            gun.points += points;
            gun.dateTime = dateTime;
        }
    }
}