using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampRating.Data.Models
{
    public class Camp
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public string Description { get; set; }
        [Required]
        public string Image { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        /// <summary>
        /// Gets or sets the list of reviews on the camp.
        /// </summary>
        public virtual List<Rating> CampRating { get; set; } = new List<Rating>();
    }
}
