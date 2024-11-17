using Carter;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Models;
using API.Constants;

namespace API.Routes
{
    public class UserRoutes : CarterModule
    {
        private readonly IUserService _userService;

        public UserRoutes(IUserService userService)
        {
            _userService = userService;
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            // User CRUD routes
            app.MapPost(RouteConstants.RegisterUserRoute, RegisterUser);
            app.MapGet(RouteConstants.GetUserRoute, GetUser);
            app.MapPut(RouteConstants.UpdateUserRoute, UpdateUser);
            app.MapDelete(RouteConstants.DeleteUserRoute, DeleteUser);
            app.MapPost(RouteConstants.LoginUserRoute, LoginUser); // Added login route
        }

        // Register new user
        public async Task<IResult> RegisterUser([FromBody] User user)
        {
            try
            {
                String truePassworduser = user.PasswordHash;
                user.SetPassword(truePassworduser);

                var createdUser = await _userService.CreateUserAsync(user);
                return Results.Ok(createdUser);
            }
            catch (Exception e)
            {
                return Results.Problem("Error registering user: " + e.Message);
            }
        }

        // Get user by email
        public async Task<IResult> GetUser([FromQuery] string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                return user != null ? Results.Ok(user) : Results.NotFound("User not found.");
            }
            catch (Exception e)
            {
                return Results.Problem("Error retrieving user: " + e.Message);
            }
        }

        // Update user information
        public async Task<IResult> UpdateUser([FromQuery] string email, [FromBody] User updatedUser)
        {
            try
            {
                var user = await _userService.UpdateUserAsync(email, updatedUser);
                return user != null ? Results.Ok(user) : Results.NotFound("User not found.");
            }
            catch (Exception e)
            {
                return Results.Problem("Error updating user: " + e.Message);
            }
        }

        // Delete user by email
        public async Task<IResult> DeleteUser([FromQuery] string email)
        {
            try
            {
                var success = await _userService.DeleteUserAsync(email);
                return success ? Results.Ok("User deleted successfully") : Results.NotFound("User not found.");
            }
            catch (Exception e)
            {
                return Results.Problem("Error deleting user: " + e.Message);
            }
        }

        // Login user
        public async Task<IResult> LoginUser(string email, string attemptedPassword)
        {
            try
            {
                // Retrieve user by email
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return Results.NotFound("User not found.");
                }

                // Validate the attempted password using VerifyPassword method
                bool isPasswordValid = user.VerifyPassword(attemptedPassword);

                if (!isPasswordValid)
                {
                    return Results.NotFound("Invalid password.");
                }

                return Results.Ok(true); // Return true if login is successful
            }
            catch (Exception e)
            {
                return Results.Problem("Error logging in: " + e.Message);
            }
        }
    }
}
