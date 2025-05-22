using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampRating.Data.Models;
using CampRating.Data;
using CampRating.Repostories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CampRating.Repostories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(
            ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.RatingsByUser)
                .ToListAsync();
        }
        public async Task<User> GetByIdAsync(string id)
        {
            return await _context.Users
                .Include(u => u.RatingsByUser)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<bool> AddAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password); //Hashes the passwords automatically

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "BasicUser");
            }
            return result.Succeeded;
        }
        public async Task<bool> UpdateAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
        public async Task<bool> DeleteAsync(User user)
        {
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
        public async Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0;
        }

        public async Task<int> CountUsersAsync()
        {
            return await _context.Users.CountAsync();
        }
    }
}
