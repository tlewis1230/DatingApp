using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;
        }
        // one-time use - don't need to do it as async
        public void SeedUsers()
        {
            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            //serialize data into objects
            var users = JsonConvert.DeserializeObject<List<User>>(userData);
            foreach (var user in users)
            {
               byte[] passwordHash, passwordSalt;
               // this is our value in the json data
               CreatePasswordHash("password", out passwordHash, out passwordSalt); 
               user.PasswordHash = passwordHash;
               user.PasswordSalt = passwordSalt;
               user.Username = user.Username.ToLower();
               _context.Users.Add(user);
            } 
            _context.SaveChanges();
        }

        // this is a class for development purposes, so we can swipe this from AuthRepository.cs
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {   
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }          

        }

    }
}