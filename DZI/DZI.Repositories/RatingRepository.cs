using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampRating.Data.Models;
using CampRating.Data;
using CampRating.Repostories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CampRating.Repostories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public RatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Rating>> GetAllAsync()
        {
            return await _context.Ratings.Include(r => r.User)
                                         .Include(r => r.Camp)
                                          .ToListAsync();
        }

        public async Task<IEnumerable<Rating>> GetAllByCampAsync(int campId)
        {
            return await _context.Ratings.Where(r => r.CampID == campId)
                                               .ToListAsync();
        }

        public async Task<IEnumerable<Rating>> GetAllByUserAsync(string userID)
        {
            return await _context.Ratings.Where(r => r.UserID == userID)
                                         .Include(r => r.Camp) //Makes sure that the camp can be visualized and used when the method is envoked
                                         .ToListAsync();
        }

        public async Task<Rating> GetByIdAsync(int id)
        {
            return await _context.Ratings.FirstOrDefaultAsync(rating => rating.Id == id);
        }
        public async Task<bool> AddAsync(Rating rating)
        {
            _context.Add(rating);
            return await SaveAsync();
        }
        public async Task<bool> UpdateAsync(Rating rating)
        {
            _context.Update(rating);
            return await SaveAsync();
        }
        public async Task<bool> DeleteAsync(Rating rating)
        {
            _context.Remove(rating);
            return await SaveAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<int> CounRatingsAsync()
        {
            return await _context.Ratings.CountAsync();
        }
    }
}
