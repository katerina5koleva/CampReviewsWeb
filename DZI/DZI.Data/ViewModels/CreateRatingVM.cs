using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampRating.Data.ViewModels
{
    public class CreateRatingVM
    {
        [Required(ErrorMessage = "Review is required")]
        public string Review { get; set; }
        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be an integer between 1 and 5")]
        public int CampRating { get; set; }
        public int CampID { get; set; }
        public string UserID { get; set; }
    }
}
