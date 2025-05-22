using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampRating.Data.Helpers;
using Microsoft.AspNetCore.Http;

namespace CampRating.Data.ViewModels
{
    public class CreateCampVM
    {
            [Required(ErrorMessage = "Camp's name is required")]
            [StringLength(64, ErrorMessage = "Camp's name cannot exceed 64 characters.")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Camp's description is required")]
            [StringLength(255, ErrorMessage = "Camp's description cannot exceed 255 characters.")]
            public string Description { get; set; }
            [Required]
            [MaxSizeOfPhotoFile(2)]
            public IFormFile Image { get; set; }
            [Required]
            [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90.")]
            public double Latitude { get; set; }
            [Required]
            [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180.")]    
            public double Longitude { get; set; }
    }
}
