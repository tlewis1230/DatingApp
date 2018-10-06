using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        // Don't need async as we're not querying the db
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        // Don't need async as we're not querying the db
        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p=> p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
              var users = await _context.Users.Include(p=> p.Photos).ToListAsync();
              return users;
        }

        public async Task<bool> SaveAll()
        {
            // if 0 - nothing has been saved, so we return false
            return await _context.SaveChangesAsync() > 0 ;
        }
    }
}