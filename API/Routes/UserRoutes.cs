using Carter;
using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Models;
using API.Constants;
using System.Security.Cryptography;
using MongoDB.Bson;

namespace API.Routes
{
    public class UserRoutes : CarterModule
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;
        public UserRoutes(IUserService userService, IEmailService emailService, IAuthService authService)
        {
            _userService = userService;
            _emailService = emailService;
            _authService= authService;
        }

        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            // User CRUD routes
            app.MapPost(RouteConstants.RegisterUserRoute, RegisterUser);
            app.MapGet(RouteConstants.GetUserRoute, GetUser);
            app.MapPut(RouteConstants.UpdateUserRoute, UpdateUser);
            app.MapDelete(RouteConstants.DeleteUserRoute, DeleteUser);
            app.MapPost(RouteConstants.LoginUserRoute, LoginUser); // Added login route
            app.MapPost(RouteConstants.Validate2FACodeRoute, VerifyMFA); // Added MFA route
        }

        // Register new user
        public async Task<IResult> RegisterUser(UserRegistrationInfo userInfo)
        {
            try
            {
                var user = new User();
                user.Email = userInfo.Email;
                ObjectId employeeId;
                user.EmployeeID = ObjectId.TryParse(userInfo.EmployeeID, out employeeId) ? employeeId : null;
                user.SetPassword(userInfo.Password);
                using (var rng = RandomNumberGenerator.Create())
                {
                    byte[] secretBytes = new byte[32];
                    rng.GetBytes(secretBytes);
                    user.JWTSecret = Convert.ToBase64String(secretBytes);
                }
                var createdUser = await _userService.CreateUserAsync(user);
                return Results.Ok("Successfully created user!");
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
                _=_emailService.SendTwoFactorCodeAsync(email);
                return Results.Ok("Awaiting MFA input"); // Return true if login is successful
            }
            catch (Exception e)
            {
                return Results.Problem("Error logging in: " + e.Message);
            }
        }
        public async Task<IResult> VerifyMFA(string email, string MFAcode)
        {
            try
            {
                var token=_authService.AuthenticateAndGenerateToken(email,MFAcode);
                var user =await _userService.GetUserByEmailAsync(email);
                return Results.Ok(new Tuple<string,string>(token,user.Id.ToString()));
            }
            catch (Exception e)
            {
                return Results.Problem("Error logging in: " + e.Message);
            }
        }

    }
}
