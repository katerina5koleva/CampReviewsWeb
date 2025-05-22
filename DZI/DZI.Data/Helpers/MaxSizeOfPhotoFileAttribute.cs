using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CampRating.Data.Helpers
{
    public class MaxSizeOfPhotoFileAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeInBytes;

        //An attribute to validate the maximum size of a photo file.
        public MaxSizeOfPhotoFileAttribute(int maxFileSizeInMB)
        {
            _maxFileSizeInBytes = maxFileSizeInMB * 1024 * 1024;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null && file.Length > _maxFileSizeInBytes)
            {
                return new ValidationResult($"Maximum allowed file size is {_maxFileSizeInBytes / (1024 * 1024)} MB.");
            }

            return ValidationResult.Success;
        }
    }
}
