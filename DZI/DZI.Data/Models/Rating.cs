using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CampRating.Data.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Review { get; set; }
        [Range(1, 5)]
        public int CampRating { get; set; }
        [Required]
        public DateTime DateOfRequest { get; set; }

        [ForeignKey("UserID")]
        public string UserID { get; set; }
        public User User { get; set; }
        [ForeignKey("CampID")]
        public int CampID { get; set; }
        public Camp Camp{ get; set; }

    }
}
