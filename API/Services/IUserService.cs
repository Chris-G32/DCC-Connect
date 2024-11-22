using MongoDB.Driver;
using API.Models;
using MongoDB.Bson;
using API.Errors;
using API.Constants;

namespace API.Services
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);
        User GetUserByEmail(string email);

        Task<User> CreateUserAsync(User user);
        public User GetUserById(ObjectId id);
        public string GetUserRole(ObjectId id);
        public string GetUserRole(string email);
        Task<User> UpdateUserAsync(string email, User updatedUser);
        Task<User> UpdateJWTTokenAsync(string email, string jwtToken);  // Method to update JWT token
        Task<User> GetUserByJWTTokenAsync(string jwtToken); // New method to get user by JWT token
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
        public User GetUserByEmail(string email)
        {
            return _collectionsProvider.Users.Find(u => u.Email == email).FirstOrDefault();
        }
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _collectionsProvider.Users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }
        public User GetUserById(ObjectId id)
        {
            var result = _collectionsProvider.Users.Find(u => u.Id == id).FirstOrDefault() ?? throw new EntityDoesNotExistException(id.ToString(), CollectionConstants.UsersCollection);
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

        // Update the JWT token for a user
        public async Task<User> UpdateJWTTokenAsync(string email, string jwtToken)
        {
            var updateDefinition = Builders<User>.Update.Set(u => u.JWTToken, jwtToken);

            var result = await _collectionsProvider.Users.UpdateOneAsync(
                u => u.Email == email,
                updateDefinition);

            return result.MatchedCount > 0 ? await GetUserByEmailAsync(email) : null;
        }

        // Get the user by JWT token
        public async Task<User> GetUserByJWTTokenAsync(string jwtToken)
        {
            return await _collectionsProvider.Users.Find(u => u.JWTToken == jwtToken).FirstOrDefaultAsync();
        }

        // Delete a user by email
        public async Task<bool> DeleteUserAsync(string email)
        {
            var result = await _collectionsProvider.Users.DeleteOneAsync(u => u.Email == email);
            return result.DeletedCount > 0;
        }

        public string GetUserRole(ObjectId id)
        {
            var employeeId = GetUserById(id).EmployeeID;
            return _collectionsProvider.Employees.Find(e=>e.Id== employeeId).FirstOrDefault().EmployeeRole;
        }

        public string GetUserRole(string email)
        {
            var employeeId = GetUserByEmail(email).EmployeeID;
            return _collectionsProvider.Employees.Find(e => e.Id == employeeId).FirstOrDefault().EmployeeRole;
        }
    }
}
