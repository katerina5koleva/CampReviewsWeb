using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CampRating.Data.Models;

namespace CampRating.Repostories.Interfaces
{
    public interface ICampRepository
    {
        Task<IEnumerable<Camp>> GetAllAsync();
        Task<Camp> GetByIdAsync(int id);
        Task<bool> AddAsync(Camp camp);
        Task<bool> UpdateAsync(Camp camp);
        Task<bool> DeleteAsync(Camp camp);
        Task<bool> SaveAsync();
        Task<int> CountCampsAsync();
    }
}
