using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampRating.Data.Models;

namespace CampRating.Data.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string Role { get; set; }
        public virtual List<Rating>? Ratings { get; set; }
    }
}
