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

    public class IUserService : IUserService
    {
        private readonly IMongoCollection<User> _userCollection;

        // Constructor to inject MongoDB collection for users
        public IUserService(IMongoDatabase database)
        {
            _userCollection = database.GetCollection<User>("Users");

            // Ensure the collection for users is created and has indexes as needed
            EnsureUserCollectionExists();
        }

        // Ensure that the "Users" collection is created and enforce unique email index
        private void EnsureUserCollectionExists()
        {
            // Create unique index for email to prevent duplicate users
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexKeys = Builders<User>.IndexKeys.Ascending(u => u.Email);
            _userCollection.Indexes.CreateOne(new CreateIndexModel<User>(indexKeys, indexOptions));
        }

        // Get a user by email
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userCollection.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        // Create a new user
        public async Task<User> CreateUserAsync(User user)
        {
            // Check if user with this email already exists
            var existingUser = await _userCollection.Find(u => u.Email == user.Email).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                throw new Exception("User with this email already exists.");
            }

            await _userCollection.InsertOneAsync(user);
            return user;
        }

        // Update an existing user by email
        public async Task<User> UpdateUserAsync(string email, User updatedUser)
        {
            var result = await _userCollection.ReplaceOneAsync(
                u => u.Email == email,
                updatedUser,
                new ReplaceOptions { IsUpsert = true });

            return result.IsAcknowledged ? updatedUser : null;
        }

        // Delete a user by email
        public async Task<bool> DeleteUserAsync(string email)
        {
            var result = await _userCollection.DeleteOneAsync(u => u.Email == email);
            return result.DeletedCount > 0;
        }
    }
}
