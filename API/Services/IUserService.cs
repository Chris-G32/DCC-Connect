using MongoDB.Driver;
using API.Models;

namespace API.Services
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(string email, User updatedUser);
        Task<bool> DeleteUserAsync(string email);
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
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _collectionsProvider.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
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
    }
}
