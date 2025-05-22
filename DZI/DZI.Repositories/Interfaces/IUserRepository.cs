using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampRating.Data.Models;

namespace CampRating.Repostories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(string id);
        Task<bool> AddAsync(User user, string password);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(User user);
        Task<bool> SaveAsync();
        Task<int> CountUsersAsync();
    }
}
