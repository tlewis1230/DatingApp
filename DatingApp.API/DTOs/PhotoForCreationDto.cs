using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.DTOs
{
    public class PhotoForCreationDto
    {
        public string Url { get; set; }
        
        // Represents a file sent with the HttpRequest.
        public IFormFile File { get; set; }

        public string Description { get; set; }
        public DateTime DateAdded { get; set; }

        // we'll get this from Cloudinary
        public string PublicId { get; set; }
        public PhotoForCreationDto()
        {
            DateAdded = DateTime.Now;
        }
    }
}