using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampRating.Data.Models;

namespace CampRating.Repostories.Interfaces
{
    public interface IRatingRepository
    {
        Task<IEnumerable<Rating>> GetAllAsync();
        Task<IEnumerable<Rating>> GetAllByCampAsync(int campID);
        Task<IEnumerable<Rating>> GetAllByUserAsync(string userID);
        Task<Rating> GetByIdAsync(int id);
        Task<bool> AddAsync(Rating rating);
        Task<bool> UpdateAsync(Rating rating);
        Task<bool> DeleteAsync(Rating rating);
        Task<bool> SaveAsync();
        Task<int> CounRatingsAsync();
    }
}
