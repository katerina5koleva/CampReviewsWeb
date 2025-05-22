using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CampRating.Data.Models
{
    public class User : IdentityUser
    {
        [Required]
        public override string UserName { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [NotMapped]
        public override string Email { get; set; }
        [NotMapped]
        public override bool EmailConfirmed { get; set; }

        /// <summary>
        /// Gets or sets the list of camp reviews made by the user.
        /// </summary>
        public virtual List<Rating> RatingsByUser { get; set; } = new List<Rating>();
    }
}
