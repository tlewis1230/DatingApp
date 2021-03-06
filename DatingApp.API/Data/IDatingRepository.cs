using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
         // Add - we can use this for both Our user class and our photo class 
         void Add<T>(T entity) where T: class;
         
         // Delete
        void Delete<T>(T entity) where T: class;
        
        // Save
        // false - no changes or there was a problem saving changes to the database
        Task<bool> SaveAll();
        
        // Get users
        Task<PagedList<User>> GetUsers(UserParams userParams);

        // Get Individual User
        Task<User> GetUser(int id, bool isCurrentUser);

        Task<Photo> GetPhoto(int id);
        Task<Photo> GetMainPhotoForUser(int userId);

        Task <Like> GetLike (int userId, int recipientId);
        Task <Message> GetMessage(int id);
        Task <PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId);
        
    }
}