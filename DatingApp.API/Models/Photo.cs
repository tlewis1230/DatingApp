using System;

namespace DatingApp.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public Boolean IsMain { get; set; }

        // creates cascade delete in photo table if we delete a user
        public User User { get; set; }
        public int UserId { get; set; }
    }
}