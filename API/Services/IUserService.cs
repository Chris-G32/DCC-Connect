using MongoDB.Driver;
using MongoDB.Bson;
using API.Errors;
using API.Constants;
using API.Models.Users;

namespace API.Services
{
    public interface IUserService
    {
        //get user byid
        //get usre by email
        //Task<User> GetUserByEmailAsync(string email);
        User? GetUserByEmail(string email);
        public User GetUserById(ObjectId id);
        //public string GetUserRole(ObjectId id);

        ////Task<User> CreateUserAsync(User user);
        //public string GetUserRole(string email);
        //Task<User> UpdateUserAsync(string email, User updatedUser);
        //Task<User> GetUserByJWTTokenAsync(string jwtToken); // New method to get user by JWT token
        //Task<bool> DeleteUserAsync(string email);
    }

    public class UserService : IUserService
    {
        private readonly ICollectionsProvider _collectionsProvider;

        // Constructor to inject MongoDB collection for users
        public UserService(ICollectionsProvider cp)
        {
            _collectionsProvider = cp;
        }

        // Get a user by email
        public User? GetUserByEmail(string email)
        {
            return _collectionsProvider.Users.Find(u => u.Email == email).FirstOrDefault();
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _collectionsProvider.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }
        public User GetUserById(ObjectId id)
        {

            return _collectionsProvider.Users.Find(u => u.Id == id).FirstOrDefault();
        }

        // Create a new user
        public async Task<User> CreateUserAsync(User user)
        {
            // Check if user with this email already exists
            var existingUser = await _collectionsProvider.Users.Find(u => u.Email == user.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new Exception("User with this email already exists.");
            }

            await _collectionsProvider.Users.InsertOneAsync(user);
            return user;
        }

        // Update an existing user by email
        public async Task<User> UpdateUserAsync(string email, User updatedUser)
        {
            var result = await _collectionsProvider.Users.ReplaceOneAsync(
                u => u.Email == email,
                updatedUser,
                new ReplaceOptions { IsUpsert = true });

            return result.IsAcknowledged ? updatedUser : null;
        }

        // Delete a user by email
        public async Task<bool> DeleteUserAsync(string email)
        {
            var result = await _collectionsProvider.Users.DeleteOneAsync(u => u.Email == email);
            return result.DeletedCount > 0;
        }

        //public string GetUserRole(ObjectId id)
        //{
        //    var user = GetUserById(id);
        //    return user.EmployeeRole;
        //}

        //public string GetUserRole(string email)
        //{
        //    var employeeId = GetUserByEmail(email);
        //    return _collectionsProvider.Users.Find(e => e.Id == id).FirstOrDefault().EmployeeRole;
        //}
    }
}
