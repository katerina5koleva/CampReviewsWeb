using CampRating.Data;
using CampRating.Data.Models;
using CampRating.Repostories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace CampRating.Repositories
{
    public class CampRepository : ICampRepository
    {
        private readonly ApplicationDbContext _context;

        public CampRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Camp>> GetAllAsync()
        {
            return await _context.Camps.Include(b => b.CampRating).ToListAsync();
        }

        public async Task<Camp> GetByIdAsync(int id)
        {
            return await _context.Camps.Include(b => b.CampRating)
                                         .ThenInclude(r => r.User)
                                         .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> AddAsync(Camp camp)
        {
            _context.Add(camp);
            return await SaveAsync();
        }

        public async Task<bool> UpdateAsync(Camp camp)
        {
            _context.Update(camp);
            return await SaveAsync();
        }
        public async Task<bool> DeleteAsync(Camp camp)
        {
            _context.Remove(camp);
            return await SaveAsync();
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<int> CountCampsAsync()
        {
            return await _context.Camps.CountAsync();
        }
    }
}
