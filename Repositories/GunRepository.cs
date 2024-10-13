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
            var guns = await _dbContext.guns.ToListAsync();
            System.Console.WriteLine(guns);
            //.Select(s => s.ToGunDto());
            return guns;
        }
        
        // public async Task<IActionResult> Create(CreateGunRequestDto gunDto)
        // { 
        //     var gunModel = gunDto.ToGunFromCreateDto();
        //     _dbContext.guns.Add(gunModel);
        //     _dbContext.SaveChanges();
        //     return CreatedAtAction(nameof(GetById), new {id=gunModel.Id}, gunModel.ToGunDto());
        // }
        public async Task Add( int points, long UserId, DateTime dateTime)
        {
            var gun = new Gun()
            {   
                points = points,
                UserId = UserId,
                dateTime = dateTime
            };

            await _dbContext.AddAsync(gun);
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