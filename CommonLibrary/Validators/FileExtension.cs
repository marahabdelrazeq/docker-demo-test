using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;

namespace CommonLibrary.Validators
{
    /// <summary>
    /// Custom validation attribute that ensures the uploaded file has an allowed extension and size limit.
    /// </summary>
    public class FileExtension : ValidationAttribute
    {
        private readonly List<string> _allowedExtensions;

        /// <summary>
        /// Initializes a new instance of the FileExtension validation attribute with a list of allowed extensions.
        /// </summary>
        /// <param name="allowedExtensions">An array of allowed file extensions (e.g., ".jpg", ".png").</param>
        public FileExtension(string[] allowedExtensions)
        {
            _allowedExtensions = new List<string>(allowedExtensions);
        }

        /// <summary>
        /// Performs the validation to check if the uploaded file has an allowed extension and is within the size limit (5 MB).
        /// </summary>
        /// <param name="value">The value being validated (must be of type IFormFile).</param>
        /// <param name="validationContext">Contextual information about the validation operation.</param>
        /// <returns>A ValidationResult indicating success or failure.</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Check if the value is an uploaded file (IFormFile)
            if (value is IFormFile file)
            {
                // Check the file size limit (5 MB)
                long fileSizeInBytes = file.Length;
                double fileSizeInKilobytes = fileSizeInBytes / 1024.0;
                double fileSizeInMegabytes = fileSizeInKilobytes / 1024.0;

                if (fileSizeInMegabytes > 5)
                {
                    return new ValidationResult($"INVALID_FILE_SIZE");
                }

                // Get the file extension and ensure it is allowed
                var extension = Path.GetExtension(file.FileName).ToUpper();

                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult("INVALID_FILE_EXTENSION");
                }
            }
            else if (value != null)
            {
                // If the value is not a file, return an error
                return new ValidationResult($"INVALID_FILE");
            }

            // If validation succeeds, return success
            return ValidationResult.Success!;
        }
    }
}
